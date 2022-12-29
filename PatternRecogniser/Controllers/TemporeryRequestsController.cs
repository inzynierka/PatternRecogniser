using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using PatternRecogniser.ThreadsComunication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace PatternRecogniser.Controllers
{
    [ApiController]
    [Route("TemporeryRequests")]
    public class TemporeryRequestsController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _trainingQueue;
        private PatternRecogniserDBContext _context;
        public TemporeryRequestsController(PatternRecogniserDBContext context, IBackgroundTaskQueue backgroundJobs)
        {
            _context = context;
            _trainingQueue = backgroundJobs;
        }

        /// <summary>
        /// Zwraca dane wszystkich użytkowników. Zapytanie nie pojawi się w finalnej wersji.
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpGet]
        [Route("Users")]
        public IEnumerable<User> Get()
        {
            return _context.user.ToList();
        }


        /// <summary>
        ///Usuwa urzytkownika. Zapytanie nie pojawi się w finalnej wersji.
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpDelete]
        [Route("deleteUser")]
        public IActionResult DeleteUser(string login)
        {
            try
            {
                var user = _context.user.Where(a => login == a.login).FirstOrDefault();
                if (user != null)
                {
                    _context.user.Remove(user);
                    _context.SaveChanges();
                }

                return Ok("usnięto");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Wprowadza wzory do bazy. Zapytanie nie pojawi się w finalnej wersji.
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpPost("GeneratePatterns")]
        [Consumes("multipart/form-data")]
        public IEnumerable<Pattern> GeneratePatterns( int modelId, List<IFormFile> attachment)
        {
            var model = _context.extendedModel.First(model => model.extendedModelId == modelId);
            for(int i = 0; i<attachment.Count; i++)
            {
                var pattern = new Pattern();
                pattern.name = attachment[i].FileName;
                pattern.extendedModel = model;
                using (var ms = new MemoryStream())
                {
                    attachment[i].CopyTo(ms);
                    pattern.picture = ms.ToArray();
                }
                _context.pattern.Add(pattern);
            }

            _context.SaveChanges();

            return _context.pattern;
        }

        /// <summary>
        /// Wprowadza validationSet. Zapytanie nie pojawi się w finalnej wersji.
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpPost("GenerateValidationSet")]
        [Consumes("multipart/form-data")]
        public IEnumerable<ValidationSet> GenerateValidationSet(List<IFormFile> attachment, int experimentId)
        {
            var expe = _context.modelTrainingExperiment.First(expe => expe.experimentId == experimentId);
            var pattens = _context.pattern.ToList();
            var r = new Random();
            int max = pattens.Count();
            for (int i = 0; i < attachment.Count; i++)
            {
                var validationSet = new ValidationSet();
                validationSet.truePattern = pattens.ElementAt(  r.Next(max) );
                validationSet.recognisedPattern = pattens.ElementAt( r.Next(max) );
                validationSet.modelTrainingExperiment = expe;
                using (var ms = new MemoryStream())
                {
                    attachment[i].CopyTo(ms);
                    validationSet.testedPattern = ms.ToArray();
                }
                _context.validationSet.Add(validationSet);
            }

            _context.SaveChanges();

            return _context.validationSet;
        }


        /// <summary>
        /// GeneratePaternRecognizerExperiment. Zapytanie nie pojawi się w finalnej wersji.
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpPost("GeneratePaternRecognizerExperiment")]
        [Consumes("multipart/form-data")]
        public IEnumerable<PatternRecognitionExperiment> GeneratePaternRecognizerExperiment(IFormFile attachment, int modelId)
        {
            var expePattern = new PatternRecognitionExperiment();
            int probeblyPatternsNumber = 4;

            using (var ms = new MemoryStream())
            {
                attachment.CopyTo(ms);
                expePattern.testedPattern = ms.ToArray();
            }

            expePattern.extendedModelId = modelId;
            List<RecognisedPatterns> ps = new List<RecognisedPatterns>();

            Random r = new Random();
            var paterns = _context.pattern.Where(a => a.extendedModelId == modelId).ToList();
            int maxPaterns = paterns.Count();
            for(int i =0; i<probeblyPatternsNumber; i++)
            {
                RecognisedPatterns rp = new RecognisedPatterns()
                {
                    pattern = paterns.ElementAt(r.Next(maxPaterns)),
                    probability = (float) r.NextDouble()
                };
                ps.Add(rp);
            }

            expePattern.recognisedPatterns = ps;

            _context.patternRecognitionExperiment.Add(expePattern);
            _context.SaveChanges();
            return _context.patternRecognitionExperiment.Include(p => p.recognisedPatterns);

        }

        /// <summary>
        /// Save random pattern as Image. Zapytanie nie pojawi się w finalnej wersji.
        /// </summary>
        /// <returns> 
        /// string
        /// 
        /// </returns>
        [HttpGet("SaveRandomPatterAsImage")]
        public byte[] SaveRandomPatterAsImage(string fileName)
        {
            var pattern = _context.pattern.ToList();
            byte[] picture = pattern.ElementAt(new Random().Next(pattern.Count())).picture;

            using (var ms = new MemoryStream(picture))
            {
                var image = Image.FromStream(ms);
                image.Save(@".\obrazki\"+fileName+".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            return picture;
        }
    }
}
