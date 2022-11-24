using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Models;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Linq;

namespace PatternRecogniser.Controllers
{
    public enum ModelStatus
    {
        InQueue,
        Training,
        TrainingComplite,
        NotFound
    }

    [Route("{login}")]
    public class TrainModelController : ControllerBase
    {

        private readonly IBackgroundTaskQueue _trainInfoQueue;
        private readonly ITrainingUpdate _traningUpdate;
        private PatternRecogniserDBContext _context;
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
        public IActionResult TrainModel([FromRoute] string login, string modelName,
            DistributionType distributionType, IFormFile trainingSet)
        {
            try
            {
                if (GetStatus(login, modelName) != ModelStatus.NotFound)
                    return BadRequest("Model już istnieje");

                if ( ! (trainingSet.FileName.EndsWith(".zip")) )
                    throw new Exception("Zły format pliku");


                _trainInfoQueue.Enqueue(new TrainingInfo(login, trainingSet, modelName, distributionType));

                return NumberInQueue(login);
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
        public IActionResult NumberInQueue([FromRoute] string login)
        {
            int numberInQueue = _trainInfoQueue.NumberInQueue(login);
            if (numberInQueue >= 0)
                return Ok(numberInQueue);
            else
                return NotFound("Nie ma cię w kolejce");
        }

        /// <summary>
        /// Usuwanie z kolejki
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpDelete("Cancel")]
        public IActionResult Cancel([FromRoute] string login)
        {
            bool deleted = _trainInfoQueue.Remove(login);
            if (deleted)
                return Ok("usunięto");
            else
                return NotFound("nie udało się usunąć");
        }

        /// <summary>
        /// Pobiera dane gdy model jest w trakcie trenowania. 
        /// </summary>
        /// <description></description>
        /// <returns>
        /// string
        /// </returns>
        [HttpGet("TrainUpdate")]
        public IActionResult TrainUpdate([FromRoute] string login, string modelName)
        {
            var info = _traningUpdate.ActualInfo(login, modelName);
            if (string.IsNullOrEmpty(info))
                return NotFound();
            else
                return Ok(info);
        }

        /// <summary>
        /// Pobiera Statystyki modelu
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetModelStatistics")]
        public IActionResult GetModelStatistics([FromRoute] string login, string modelName)
        {
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
        public IActionResult GetModels(string login)
        {
            var models = _context.extendedModel.Where(model => model.userLogin == login);

            if (models == null)
                return NotFound();
            else
                return Ok(models);
        }


        /// <summary>
        /// Pobiera aktualny status modelu
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetModelStatus")]
        public IActionResult GetModelStatus([FromRoute] string login, string modelName)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(modelName))
                return BadRequest("Nie podano nazwy lub loginu");

            var status = GetStatus( login,  modelName);

            if (status == ModelStatus.InQueue)
                return Ok("Model jest w kolejce");

            if (status == ModelStatus.Training)
                return Ok("Model jest trenowany");

            if (status == ModelStatus.TrainingComplite)
                return Ok("Model jest wytrenowany (znajduje się w zakładce \"Moje Modele\")");

            return NotFound();
        }


        private ModelStatus GetStatus(string login, string modelName)
        {

            if (_trainInfoQueue.IsUserModelInQueue(login, modelName))
                return ModelStatus.InQueue;

            if (_traningUpdate.IsUserModelInTraining(login, modelName))
                return ModelStatus.Training;

            if (_context.extendedModel.Where(model => model.userLogin == login && model.name == modelName).Count() > 0)
                return ModelStatus.TrainingComplite;

            return ModelStatus.NotFound;
        }

    }
}
