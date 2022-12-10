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
    public enum ModelStatus
    {
        InQueue,
        Training,
        TrainingComplete,
        TrainingFailed,
        NotFound,

    }

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
                //if (GetStatus(login, modelName) != ModelStatus.NotFound)
                //    return BadRequest(_messages.modelAlreadyExist);

                //if ( ! (trainingSet.FileName.EndsWith(".zip")) )
                //    throw new Exception(_messages.incorectFileFormat);


                //_trainInfoQueue.Enqueue(new TrainingInfo(login, trainingSet, modelName, distributionType));
                //var user = _context.user.Where(a => a.login == login).FirstOrDefault();
                //user.lastTrainModelName = modelName;

                //await _context.SaveChangesAsync();

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
        [HttpGet("GetModelStatus")]
        public IActionResult GetModelStatus(string modelName)
        {
            string login = User.Identity.Name;
            if (string.IsNullOrEmpty(modelName))
                modelName = _context.user.Where(user => user.login == login).FirstOrDefault()?.lastTrainModelName;

            var status = GetStatus( login,  modelName);

            if (status == ModelStatus.InQueue)
                return Ok(_messages.modelIsInQueue);

            if (status == ModelStatus.Training)
                return Ok(_messages.modelIsTrained);

            if (status == ModelStatus.TrainingComplete)
                return Ok(_messages.modelTrainingComplete);

            if (status == ModelStatus.TrainingFailed)
                return Ok(_messages.modelTrainingFailed);

            return NotFound(_messages.modelNotFound);
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

            if (user != null && user.lastTrainModelName == modelName)
                return ModelStatus.TrainingFailed;

            return ModelStatus.NotFound;
        }

    }
}
