using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using PatternRecogniser.Services;
using System.Collections;

namespace PatternRecogniser.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class UserController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _trainingQueue;
        private PatternRecogniserDBContext _context;
        public UserController(PatternRecogniserDBContext context, IBackgroundTaskQueue backgroundJobs)
        {
            _context = context;
            _trainingQueue = backgroundJobs;
        }
        public IEnumerable<User> Get()
        {
            return _context.user.ToList();
        }

        [Route("{userId}/train-Model/modelName={modelName}&distributionType={distributionType}")]
        public async Task<ActionResult<int>> StartTrainingModel([FromRoute] int userId, [FromRoute] string modelName, [FromRoute] DistributionType distributionType /*, [FromBody] byte[] trainingSet*/)
        {
            await _trainingQueue.AddAsync(new TrainingInfo(userId, null));
            return _trainingQueue.Count;
            
        }

        [HttpGet]
        public ActionResult Cancel([FromRoute] int userId)
        {
            throw new NotImplementedException();
        }


        [HttpGet]
        [Route("{userId}/PlaceIntheQueue")]
        public ActionResult<int> PlaceIntheQueue([FromRoute] int userId)
        {
            return _trainingQueue.PlaceInQueue(userId);
        }

        
    }
}
