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
        
    }
}
