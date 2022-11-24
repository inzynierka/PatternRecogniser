using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Helpers;
using PatternRecogniser.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("{login}/[Controller]")]
    public class PatternRecognitionController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        public PatternRecognitionController(PatternRecogniserDBContext context)
        {
            _context = context;
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async  Task<IActionResult> Recognize([FromRoute] string login, string modelName, IFormFile pattern)
        {
            try
            {
                Bitmap picture = new Bitmap(pattern.OpenReadStream());
                var model = _context.extendedModel.Where(model => model.userLogin == login && model.name == modelName)
                    .FirstOrDefault();

                if(model == null)
                    return NotFound("Model którego chcesz użyć nie istnieje");

                var result = model.RecognisePattern(picture);

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
                user.ExsistUnsavePatternRecognitionExperiment = true;

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
