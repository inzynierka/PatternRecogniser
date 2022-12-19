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
        private double defultTrainPercent = 0.8;
        private int defultStesNumber = 2;
        private IGenericRepository<ExtendedModel> _extendedModelRepo;
        private readonly IGenericRepository<User> _userRepo;

        public TrainModelController(
            IGenericRepository<ExtendedModel> extendedModelRepo,
            IGenericRepository<User> userRepo,
            IBackgroundTaskQueue trainInfoQueue,
            ITrainingUpdate trainingUpdate)
        {
            _extendedModelRepo = extendedModelRepo;
            _userRepo = userRepo;
            _trainInfoQueue = trainInfoQueue;
            _traningUpdate = trainingUpdate;
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
            DistributionType distributionType, IFormFile trainingSet, double trainingPercent, int setsNumber)
        {
            try
            {
                string login = User.Identity.Name;
                if (GetStatus(login, modelName) != ModelStatus.NotFound)
                    return BadRequest(_messages.modelAlreadyExist);

                if (!(trainingSet.FileName.EndsWith(".zip")))
                    throw new Exception(_messages.incorectFileFormat);

                if (distributionType == DistributionType.CrossValidation && setsNumber <= 1)
                    return BadRequest(_messages.incorectCrossValidationOption);

                if (distributionType == DistributionType.TrainTest && (trainingPercent >= 100 || trainingPercent < 0)   )
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
                return NotFound(_messages.youAreNotInQueue);
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
                return NotFound(_messages.modelIsTrained);
            else
                return Ok(info);
        }

        /// <summary>
        /// Pobiera Statystyki modelu
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetModelStatistics")]
        public IActionResult GetModelStatistics(string modelName)
        {
            string login = User.Identity.Name; 
            var statistics = _extendedModelRepo.Get(
                model => model.userLogin == login && model.name == modelName, 
                "modelTrainingExperiment")
                .FirstOrDefault()?.modelTrainingExperiment;
            if (statistics == null)
                return NotFound();
            else
                return Ok(statistics);
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
            var models = _extendedModelRepo.Get(model => model.userLogin == login);
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
                var model = _extendedModelRepo.Get(model => model.name == modelName && model.userLogin == login).FirstOrDefault();
                if (model == null)
                    return Ok();

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
                message = _messages.modelIsTrained;

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

            if (_trainInfoQueue.IsUserModelInQueue(login, modelName))
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
