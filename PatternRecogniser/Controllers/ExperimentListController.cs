using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Messages.ExperimentList.Recive;
using PatternRecogniser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("{login}")]
    public class ExperimentListController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        public ExperimentListController(PatternRecogniserDBContext context)
        {
            _context = context;
        }

        [HttpPut("createExperimentList")]
        public async Task<IActionResult> Create([FromRoute] string login, string experimentListName, string experimentType)
        {
            if (IsExperimentListExsist(login, experimentListName))
                return BadRequest("Lista już istnieje");

            try
            {
                _context.experimentList.Add(new ExperimentList()
                {
                    userLogin = login,
                    name = experimentListName,
                    experimentType = experimentType
                });
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        // trzeba dokończyć
        [HttpPut("addModelTrainingExperiment")]
        public async Task<IActionResult> AddModelTrainingExperiment([FromRoute] string login, string experimentListName, int experimentId)
        {
            try
            {
                var list = _context.experimentList.Include(list => list.experiments). Where(list => list.name == experimentListName && list.userLogin == login && list.experimentType == "ModelTrainingExperiment").FirstOrDefault();
                var experiment = _context.modelTrainingExperiment.Include(e => e.extendedModel).Where(experiment => experiment.experimentId == experimentId && experiment.extendedModel.userLogin == login).FirstOrDefault();
                if (experiment == null || list == null)
                    return BadRequest("Lista lub eksperyment nie istnieje");
                else
                {
                    list.experiments.Add(experiment);
                    await _context.SaveChangesAsync();
                    return Ok(list.experiments);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut("addPatternRecognitionExperiment")]
        [Consumes("multipart/form-data")]
        public IActionResult AddPatternRecognitionExperiment([FromRoute] string login, string experimentListName, IFormFile pattern, PatternRecognitionExperimentInfo info)
        {
            if (!IsExperimentListExsist(login, experimentListName))
                return BadRequest("Lista nie istnieje");
            var list = _context.experimentList.Include(list => list.experiments).Where(list => list.name == experimentListName && list.userLogin == login && list.experimentType == "PatternRecognitionExperiment").FirstOrDefault();

            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Pobiera listy
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetLists")]
        public IActionResult GetLists([FromRoute]  string login)
        {
            var list = _context.experimentList.Where(a => a.userLogin == login);
            return Ok(list);
        }

        /// <summary>
        /// Pobiera eksperymenty z danej listy
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetExperiments")]
        public async Task<IActionResult>  GetExperiments([FromRoute] string login, string experimentListName)
        {
            var list = await _context.experimentList.Include(list => list.experiments).Where(a => a.name == experimentListName && a.userLogin == login ).FirstOrDefaultAsync();
            var experiments = list?.experiments;
            if(experiments == null)
                return NotFound();
            else
                return Ok(experiments);
        }


        private bool IsExperimentListExsist(string login, string experimentName)
        {
            return _context.experimentList.Where(list => list.name == experimentName && list.userLogin == login).Count() > 0;
        }
    }
}
