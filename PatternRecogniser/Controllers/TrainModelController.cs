using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Helpers;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using System;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("")]
    public class TrainModelController : ControllerBase
    {

        private readonly IBackgroundTaskQueue _trainingQueue;
        private PatternRecogniserDBContext _context;
        public TrainModelController(PatternRecogniserDBContext context, IBackgroundTaskQueue backgroundJobs)
        {
            _context = context;
            _trainingQueue = backgroundJobs;
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        [Route("{userId}/TrainModel")]
        public IActionResult StartTrainingModel([FromRoute] int userId, [FromRoute] string modelName, [FromRoute] DistributionType distributionType)
        {
            try
            {
               // pobieranie zipa
                var trainingSet = Request.Form.Files.FirstOrDefault();
                if (!HttpExtraOperations.IsZip(trainingSet))
                    throw new Exception("zły format pliku"); 

                _trainingQueue.Enqueue(new TrainingInfo(userId, trainingSet, modelName));
                int numberInQueue = _trainingQueue.Count;

                if (numberInQueue >= 0)
                    return Ok(numberInQueue);
                else
                    return NotFound("Nie ma cię w kolejce");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            

        }

        [HttpGet]
        [Route("{userId}/TrainingModel/NumberInQueue")]
        public IActionResult NumberInQueue([FromRoute] int userId)
        {
            return Ok(_trainingQueue.NumberInQueue(userId));
        }

        [HttpDelete]
        [Route("{userId}/Cancel")]
        public IActionResult Cancel([FromRoute] int userId)
        {
            bool deleted = _trainingQueue.Remove(userId);
            if (deleted) 
                return Ok("usunięto");
            else
                return  NotFound("nie udało się usunąć");
        }
    }
}
