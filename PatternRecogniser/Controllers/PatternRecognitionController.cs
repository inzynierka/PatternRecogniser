using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Messages.PatternRecognition;
using PatternRecogniser.Models;
using PatternRecogniser.Services.Repos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PatternRecognitionController : ControllerBase
    {
        private IGenericRepository<ExtendedModel> _extendModelRepo;
        private IGenericRepository<User> _userRepo;
        private IGenericRepository<PatternRecognitionExperiment> _patternRecognitionExperimentRepo;

        private PatternRecognitionStringMessages _messages = new PatternRecognitionStringMessages();

        public PatternRecognitionController(IGenericRepository<ExtendedModel > extendModelRepo,
            IGenericRepository<User> userRepo, 
            IGenericRepository<PatternRecognitionExperiment> patternRecognitionExperimentRepo)
        {
            _extendModelRepo = extendModelRepo;
            _userRepo = userRepo;
            _patternRecognitionExperimentRepo = patternRecognitionExperimentRepo;
        }

        /// <summary>
        /// Rozpoznawanie znaku
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async  Task<IActionResult> Recognize( string modelName, IFormFile pattern)
        {
            try
            {
                string login = User.Identity.Name;
                Bitmap picture = new Bitmap(pattern.OpenReadStream());
                var model = _extendModelRepo.Get(model => model.userLogin == login && model.name == modelName,
                    model => model.Include( m => m.patterns))
                    .FirstOrDefault();

                if(model == null)
                    return NotFound(_messages.modelNotFound);

                var result = model.RecognisePattern(picture); // odkomentuj
               
                // zapisujemy wynik
                PatternRecognitionExperiment pre = new PatternRecognitionExperiment()
                {
                    testedPattern = ReadAllByte(pattern.OpenReadStream()),
                    recognisedPatterns = result,
                    extendedModel = model

                };


                var user = _userRepo.Get(user => user.login == login).First();
                if (user.IsAbbleToAddPatternRecognitionExperiment())
                   _patternRecognitionExperimentRepo.Delete(user.lastPatternRecognitionExperiment);

                user.lastPatternRecognitionExperiment = pre;
                user.exsistUnsavePatternRecognitionExperiment = true;

                _patternRecognitionExperimentRepo.Insert(pre);
                await _patternRecognitionExperimentRepo.SaveChangesAsync();

                

                if (pre == null)
                    return NotFound();

                
                return Ok(new RecognizeRespond(pre));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        private byte[] ReadAllByte(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }

        }

    }



    
}
