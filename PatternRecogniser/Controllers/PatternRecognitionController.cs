using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Helpers;
using PatternRecogniser.Models;
using System;
using System.Drawing;
using System.Linq;

namespace PatternRecogniser.Controllers
{
    [Route("[Controller]")]
    public class PatternRecognitionController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        public PatternRecognitionController(PatternRecogniserDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult Recognize([FromRoute] int userId, [FromRoute] string modelName, IFormFile pattern)
        {
            try
            {
                Bitmap picture = new Bitmap(pattern.OpenReadStream());
                var result = _context.extendedModel.Where(model => model.userId == userId && model.name == modelName)
                    .FirstOrDefault()?.RecognisePattern(picture);

                if (result == null)
                    return Ok(result);
                else
                    return NotFound();
            }
            catch(Exception e )
            {
                return BadRequest(e.Message);
            }

        }
    }
}
