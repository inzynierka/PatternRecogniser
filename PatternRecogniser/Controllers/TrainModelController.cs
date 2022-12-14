using Lib.AspNetCore.ServerSentEvents;
using Lib.AspNetCore.ServerSentEvents.Internals;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Messages.TrainModel;
using PatternRecogniser.Models;
using PatternRecogniser.ThreadsComunication;
using System;
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
        private PatternRecogniserDBContext _context;
        public TrainModelStringMessages _messages = new TrainModelStringMessages();
        public TrainModelController(PatternRecogniserDBContext context, IBackgroundTaskQueue trainInfoQueue, ITrainingUpdate trainingUpdate)
        {
            _context = context;
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
            DistributionType distributionType, IFormFile trainingSet)
        {
            try
            {
                string login = User.Identity.Name;
                if (GetStatus(login, modelName) != ModelStatus.NotFound)
                    return BadRequest(_messages.modelAlreadyExist);

                if (!(trainingSet.FileName.EndsWith(".zip")))
                    throw new Exception(_messages.incorectFileFormat);


                _trainInfoQueue.Enqueue(new TrainingInfo(login, trainingSet, modelName, distributionType));
                var user = _context.user.Where(a => a.login == login).FirstOrDefault();
                user.lastTrainModelName = modelName;

                await _context.SaveChangesAsync();

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
            var statistics = _context.extendedModel.Include(model => model.modelTrainingExperiment ).Where(
                model => model.userLogin == login && model.name == modelName).FirstOrDefault()?.modelTrainingExperiment;
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
            var models = _context.extendedModel.Where(model => model.userLogin == login);
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
                var model = _context.extendedModel.FirstOrDefault(model => model.name == modelName && model.userLogin == login);
                if (model == null)
                    return Ok();

                _context.extendedModel.Remove(model);

                await _context.SaveChangesAsync();
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
            var user = _context.user.Where(user => user.login == login).FirstOrDefault();
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
            await _context.SaveChangesAsync();


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

            if (_context.extendedModel.Where(model => model.userLogin == login && model.name == modelName).Count() > 0)
                return ModelStatus.TrainingComplete;

            var user = _context.user.Where(a => a.login == login).FirstOrDefault();

            if (user != null && 
                user.lastTrainModelName != null  && 
                user.lastTrainModelName == modelName)
                return ModelStatus.TrainingFailed;

            return ModelStatus.NotFound;
        }

    }
}
