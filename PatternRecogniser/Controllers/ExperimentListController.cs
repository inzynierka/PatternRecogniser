using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("{userId}")]
    public class ExperimentListController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        public ExperimentListController(PatternRecogniserDBContext context)
        {
            _context = context;
        }

        [HttpPut("createExperimentList")]
        public async Task<IActionResult> Create( [FromRoute] int userId,  string experimentListName,  string experimentType)
        {
            if (IsExperimentListExsist(userId, experimentListName))
                return BadRequest("Lista już istnieje");

            try
            {
                _context.experimentList.Add(new ExperimentList()
                {
                    userId = userId,
                    name = experimentListName,
                    experimentType = experimentType
                });
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e )
            {
                return BadRequest(e.Message);
            }
        }


        // trzeba dokończyć
        [HttpPut("addExperiment")]
        public IActionResult Add([FromRoute] int userId,  string experimentListName,  string experimentType, [FromBody] Experiment Experyment)
        {
            if (! IsExperimentListExsist(userId, experimentListName))
                return BadRequest("Lista nie istnieje");

            var list = _context.experimentList.Where(list => list.name == experimentListName && list.userId == userId).First();
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        private bool IsExperimentListExsist(int userId,  string experimentName)
        {
            return _context.experimentList.Where(list => list.name == experimentName && list.userId == userId).Count() > 0;
        }
    }
}
