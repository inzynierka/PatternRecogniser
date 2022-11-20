using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Models;
using System;
using System.Linq;

namespace PatternRecogniser.Controllers
{
    [Route("")]
    public class ExperimentListController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        public ExperimentListController(PatternRecogniserDBContext context)
        {
            _context = context;
        }

        [HttpPut]
        [Route("{userId}/createExperimentList")]
        public IActionResult Create([FromRoute] int userId, [FromRoute] string experimentName, [FromRoute] string experimentType)
        {
            if (IsExperimentListExsist(userId, experimentName))
                return BadRequest("Lista już istnieje");

            try
            {
                _context.experimentList.Add(new ExperimentList()
                {
                    userId = userId,
                    name = experimentName

                });
                return Ok();
            }
            catch (Exception e )
            {
                return BadRequest(e.Message);
            }
        }


        // trzeba dokończyć 
        [HttpPut]
        [Route("{userId}/addExperiment")]
        public IActionResult Add([FromRoute] int userId, [FromRoute] string experimentName, [FromRoute] string experimentType, [FromBody] dynamic jsonExperimentList)
        {
            if (! IsExperimentListExsist(userId, experimentName))
                return BadRequest("Lista nie istnieje");

            var list = _context.experimentList.Where(list => list.name == experimentName && list.userId == userId).First();
            try
            {
                _context.experimentList.Add(new ExperimentList()
                {
                    userId = userId,
                    name = experimentName

                });
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        private bool IsExperimentListExsist(int userId,  string experimentName)
        {
            return _context.experimentList.Where(list => list.name == experimentName && list.userId == userId).Count() == 0;
        }
    }
}
