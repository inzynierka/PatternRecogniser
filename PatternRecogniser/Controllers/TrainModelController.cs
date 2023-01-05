using Lib.AspNetCore.ServerSentEvents;
using Lib.AspNetCore.ServerSentEvents.Internals;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Messages.TrainModel;
using PatternRecogniser.Models;
using PatternRecogniser.Services.Repos;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    

    [Route("")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TrainModelController : ControllerBase
    {

        private readonly IBackgroundTaskQueue _trainInfoQueue;
        private readonly ITrainingUpdate _traningUpdate;
        public TrainModelStringMessages _messages = new TrainModelStringMessages();
        private readonly int defultTrainPercent = 80;
        private readonly int defultStesNumber = 2;
        private readonly int minSetsNumber = 2;
        private readonly int maxTrainPercent = 99;
        private readonly int minTrainPercent = 1;
        private readonly IGenericRepository<ExtendedModel> _extendedModelRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<ModelTrainingExperiment> _modelTrainingExperimentRepo;
        private readonly IGenericRepository<Experiment> _experimentRepo;

        public TrainModelController(
            IGenericRepository<ExtendedModel> extendedModelRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<ModelTrainingExperiment> modelTrainingExperimentRepo,
            IGenericRepository<Experiment> experimentRepo,
            IBackgroundTaskQueue trainInfoQueue,
            ITrainingUpdate trainingUpdate)
        {
            _extendedModelRepo = extendedModelRepo;
            _userRepo = userRepo;
            _modelTrainingExperimentRepo = modelTrainingExperimentRepo;
            _trainInfoQueue = trainInfoQueue;
            _traningUpdate = trainingUpdate;
            _experimentRepo = experimentRepo;
        }


        /// <summary>
        /// Dodaj rządanie trenowania do kolejki.
        /// </summary>
        /// <returns>
        /// 200:
        /// int
        /// 404:
        /// string
        /// </returns>
        [HttpPost("TrainModel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> TrainModel( string modelName,
            DistributionType distributionType, IFormFile trainingSet, int trainingPercent, int setsNumber)
        {
            try
            {
                string login = User.Identity.Name;

                // sprawdzam czy status modela o podanej nazwie jest prawidłowy
                var modelStatus = GetStatus(login, modelName);
                if (modelStatus != ModelStatus.NotFound && modelStatus != ModelStatus.TrainingFailed)
                    return BadRequest(_messages.modelAlreadyExist);

                // sprawdzam czy czekamy na zakonczenie trenoania innego modelu użytkownika
                if(_trainInfoQueue.NumberInQueue(login)>=0)
                    return BadRequest(_messages.youAlreadyWaitInTheQueue);

                // sprawdzam czy inny model użytkownika jest trenowany
                if(_traningUpdate.IsUserTrainingModel(login))
                    return BadRequest(_messages.oneOfYourModelIsTraining);

                // sprawdzam czy zgadza się rozszerzenie pliku
                if (!(trainingSet.FileName.EndsWith(".zip")))
                    return BadRequest(_messages.incorectFileFormat);

                // sprawdzam czy podano właściwe ustawienia validacji
                if (distributionType == DistributionType.CrossValidation && setsNumber <= minSetsNumber)
                    return BadRequest(_messages.incorectCrossValidationOption);

                if (distributionType == DistributionType.TrainTest && (trainingPercent > maxTrainPercent || trainingPercent < minTrainPercent)   )
                    return BadRequest(_messages.incorectTrainTest);

                trainingPercent = trainingPercent == 0 ? defultTrainPercent : trainingPercent;
                setsNumber = setsNumber == 0 ? defultStesNumber : setsNumber;

                _trainInfoQueue.Enqueue(new TrainingInfo(login, trainingSet, modelName, distributionType, trainingPercent, setsNumber));
                var user = _userRepo.Get(a => a.login == login).FirstOrDefault();
                user.lastTrainModelName = modelName;

                await _userRepo.SaveChangesAsync();

                return NumberInQueue();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// Sprawdź miejsce w kolejce danego użytkownika
        /// </summary>
        /// <returns>
        /// 200:
        /// int
        /// 404:
        /// string
        /// </returns>
        [HttpGet("TrainingModel/NumberInQueue")]
        public IActionResult NumberInQueue()
        {
            string login = User.Identity.Name;
            int numberInQueue = _trainInfoQueue.NumberInQueue(login);
            if (numberInQueue >= 0)
                return Ok(numberInQueue);
            else
                return Ok(_messages.youAreNotInQueue);
        }

        /// <summary>
        /// Usuwanie z kolejki
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpDelete("Cancel")]
        public IActionResult Cancel()
        {
            string login = User.Identity.Name;
            bool deleted = _trainInfoQueue.Remove(login);
            if (deleted)
                return Ok(_messages.deletedFromQueue);
            else
                return NotFound(_messages.failedToDelete);
        }

        /// <summary>
        /// Pobiera dane gdy model jest w trakcie trenowania. 
        /// </summary>
        /// <description></description>
        /// <returns>
        /// string
        /// </returns>
        [HttpGet("TrainUpdate")]
        public IActionResult TrainUpdate(string modelName)
        {
            string login = User.Identity.Name;
            var info = _traningUpdate.ActualInfo(login, modelName);
            if (GetStatus(login, modelName) != ModelStatus.Training)
                return Ok(_messages.modelIsNotTraining);
            else
                return Ok(info);
        }

        /// <summary>
        /// Pobiera Statystyki modelu
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("ModelDetails")]
        public IActionResult GetModelDetails(string modelName)
        {
            string login = User.Identity.Name; 

            var extendedModelIdAndPatterns = _extendedModelRepo.Get(
                model => model.userLogin == login && model.name == modelName,
               model=> model.Include( a => a.patterns), selector: model => new { 
                   model.extendedModelId,
                   model.patterns
               })
                .FirstOrDefault();
            if (extendedModelIdAndPatterns == null)
                return NotFound();

            var statistics = _modelTrainingExperimentRepo.Get(s => s.extendedModelId == extendedModelIdAndPatterns.extendedModelId,
                s => s.Include( a => a.validationSet)).FirstOrDefault();
            if (statistics == null)
                return NotFound();

            return Ok(new ModelDetalisRespond(statistics, extendedModelIdAndPatterns.patterns));
        }

        /// <summary>
        /// Pobiera modele
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetModels")]
        public IActionResult GetModels()
        {
            string login = User.Identity.Name;
            var models = _extendedModelRepo.Get(model => model.userLogin == login, selector: model => new
            {
                model.extendedModelId,
                model.name,
                model.userLogin,
                model.distribution,
                model.num_classes
            });
            return Ok(models);
        }

        /// <summary>
        /// Pobiera aktualny status modelu
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpDelete("DeleteModel")]
        public async Task<IActionResult> DeleteModel(string modelName)
        {
            try
            {
                string login = User.Identity.Name;
                var model = _extendedModelRepo.Get(model => model.name == modelName && model.userLogin == login, model => model.Include(a => a.modelTrainingExperiment)).FirstOrDefault();
                if (model == null)
                    return Ok();
                _modelTrainingExperimentRepo.Delete(model.modelTrainingExperiment);
                _experimentRepo.Delete(model.modelTrainingExperiment);
                _extendedModelRepo.Delete(model);

                await _extendedModelRepo.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


            /// <summary>
            /// Pobiera aktualny status modelu
            /// </summary>
            /// <description></description>
            /// <returns></returns>
            [HttpGet("GetModelStatus")]
        public async Task<IActionResult> GetModelStatus(string modelName)
        {
            string login = User.Identity.Name;
            var user = _userRepo.Get(user => user.login == login).FirstOrDefault();
            if (user == null)
                return NotFound(_messages.userNotFound);

            if (string.IsNullOrEmpty(modelName))
                modelName = user.lastTrainModelName;

            var status = GetStatus( login,  modelName);

            if (user.lastModelStatus == status && user.lastCheckModel == modelName &&
                (status == ModelStatus.TrainingFailed || status == ModelStatus.TrainingComplete))
                return Ok(new GetModelStatusRespond()
                {
                    modelName = modelName,
                    messege = _messages.alreadyAsked
                });


            string message = null;

            if (status == ModelStatus.InQueue)
                message = _messages.modelIsInQueue;

            if (status == ModelStatus.Training)
                message = _messages.modelIsTraining;

            if (status == ModelStatus.TrainingComplete)
                message = _messages.modelTrainingComplete;

            if (status == ModelStatus.TrainingFailed)
                message = _messages.modelTrainingFailed;

            if (status == ModelStatus.NotFound)
                message = _messages.modelNotFound;

            user.lastModelStatus = status;
            user.lastCheckModel = modelName;
            await _userRepo.SaveChangesAsync();


            return Ok(new GetModelStatusRespond()
            {
                modelName = modelName,
                messege = message
            });
        }


        private ModelStatus GetStatus(string login, string modelName)
        {

            if (_trainInfoQueue.IsUsersModelInQueue(login, modelName))
                return ModelStatus.InQueue;

            if (_traningUpdate.IsUserModelInTraining(login, modelName))
                return ModelStatus.Training;

            if (_extendedModelRepo.Get(model => model.userLogin == login && model.name == modelName).Count() > 0)
                return ModelStatus.TrainingComplete;

            var user = _userRepo.Get(a => a.login == login).FirstOrDefault();

            if (user != null && 
                user.lastTrainModelName != null  && 
                user.lastTrainModelName == modelName)
                return ModelStatus.TrainingFailed;

            return ModelStatus.NotFound;
        }

    }
}
