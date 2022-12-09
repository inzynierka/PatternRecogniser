﻿using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using PatternRecogniser.ThreadsComunication;

namespace PatternRecogniser.Controllers
{
    [ApiController]
    [Route("DevelopingRequests")]
    public class DevelopingRequestsController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _trainingQueue;
        private PatternRecogniserDBContext _context;
        public DevelopingRequestsController(PatternRecogniserDBContext context, IBackgroundTaskQueue backgroundJobs)
        {
            _context = context;
            _trainingQueue = backgroundJobs;
        }

        /// <summary>
        /// Zwraca dane wszystkich urzytkowników. Zapytanie nie pojawi się w produkcji
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _context.user.ToList();
        }
        
    }
}
