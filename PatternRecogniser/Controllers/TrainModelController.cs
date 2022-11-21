using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Helpers;
using PatternRecogniser.Models;
using PatternRecogniser.ThreadsComunication;
using System;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("{userId}")]
    public class TrainModelController : ControllerBase
    {

        private readonly IBackgroundTaskQueue _trainInfoQueue;
        private readonly ITrainingUpdate _traningUpdate;
        private PatternRecogniserDBContext _context;
        public TrainModelController(PatternRecogniserDBContext context, IBackgroundTaskQueue trainInfoQueue, ITrainingUpdate traningUpdate)
        {
            _context = context;
            _trainInfoQueue = trainInfoQueue;
            _traningUpdate = traningUpdate;
        }


        /// <summary>
        /// Dodaj rządanie trenowania do kolejki.
        /// Plik jest wysyłany za pomocą "multipart/form-data"
        /// </summary>
        /// <returns>
        /// 200:
        /// int
        /// 404:
        /// string
        /// </returns>
        [HttpPost("TrainModel")]
        [Consumes("multipart/form-data")]
        public IActionResult TrainModel([FromRoute] int userId, string modelName,
            DistributionType distributionType, IFormFile trainingSet)
        {
            try
            {
                if (!HttpExtraOperations.IsZip(trainingSet))
                    throw new Exception("zły format pliku");

                _trainInfoQueue.Enqueue(new TrainingInfo(userId, trainingSet, modelName, distributionType));

                return NumberInQueue(userId);
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
        public IActionResult NumberInQueue([FromRoute] int userId)
        {
            int numberInQueue = _trainInfoQueue.NumberInQueue(userId);
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
        public IActionResult Cancel([FromRoute] int userId)
        {
            bool deleted = _trainInfoQueue.Remove(userId);
            if (deleted)
                return Ok("usunięto");
            else
                return NotFound("nie udało się usunąć");
        }

        /// <summary>
        /// Pobiera dane gdy model jest w trakcie trenowania. 
        /// Czy powiniśmy zapisywać dane w bazie dla ostatniego trenowania ?
        /// </summary>
        /// <description></description>
        /// <returns>
        /// string
        /// </returns>
        [HttpGet("TrainUpdate")]
        public IActionResult TrainUpdate([FromRoute] int userId, string modelName)
        {
            var info = _traningUpdate.ActualInfo(userId, modelName);
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
        [HttpGet("ModelStatistics")]
        public IActionResult ModelStatistics([FromRoute] int userId, string modelName)
        {
            var statistics = _context.extendedModel.Where(
                model => model.userId == userId && model.name == modelName).FirstOrDefault()?.modelTrainingExperiment;
            if (statistics == null)
                return Ok(statistics);
            else
                return NotFound();
        }



    }
}
