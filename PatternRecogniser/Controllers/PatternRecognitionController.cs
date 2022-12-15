using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Messages.PatternRecognition;
using PatternRecogniser.Models;
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
        private PatternRecogniserDBContext _context;
        private PatternRecognitionStringMessages _messages = new PatternRecognitionStringMessages();
        public PatternRecognitionController(PatternRecogniserDBContext context)
        {
            _context = context;
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
                var model = _context.extendedModel.Where(model => model.userLogin == login && model.name == modelName)
                    .FirstOrDefault();

                if(model == null)
                    return NotFound(_messages.modelNotFound);

                //var result = model.RecognisePattern(picture); \\ odkomentuj
       //////////////////////////   ////////////////////////   ///// usuń po prezentacji
                var result = new List<RecognisedPatterns>();
                //for (int i = 0; i < 3; i++)
                //{
                //    RecognisedPatterns recognisedPattern = new RecognisedPatterns();
                //    recognisedPattern.probability = new Random().NextDouble();
                //    result.Add(recognisedPattern);
                //}
   ////////////////////     ///////////////////// //////////  ///
                
                // zapisujemy rezultat
                PatternRecognitionExperiment pre = new PatternRecognitionExperiment()
                {
                    testedPattern = ReadAllByte(pattern.OpenReadStream()),
                    recognisedPatterns = result,
                    extendedModel = model

                };


                var user = _context.user.Where(user => user.login == login).First();
                if (user.IsAbbleToAddPatternRecognitionExperiment())
                    _context.patternRecognitionExperiment.Remove(user.lastPatternRecognitionExperiment);

                user.lastPatternRecognitionExperiment = pre;
                user.exsistUnsavePatternRecognitionExperiment = true;

                _context.patternRecognitionExperiment.Add(pre);
                await _context.SaveChangesAsync();


                if (pre == null)
                    return NotFound();
                else
                    return Ok(pre);
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
