using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Messages.ExperimentList;
using PatternRecogniser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExperimentListController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        private ExperimentListStringMesseges _messeges = new ExperimentListStringMesseges();
        public ExperimentListController(PatternRecogniserDBContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Tworzenie listy
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPut("createExperimentList")]
        public async Task<IActionResult> Create(string experimentListName, string experimentType)
        {
            

            try
            {
                string login = User.Identity.Name;
                if (IsExperimentListExsist(login, experimentListName))
                    return BadRequest(_messeges.listAlreadyExist);

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


        /// <summary>
        /// Dodawanie eksperymentu trenowania
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPut("addModelTrainingExperiment")]
        public async Task<IActionResult> AddModelTrainingExperiment(string experimentListName, int experimentId)
        {
            try
            {
                string login = User.Identity.Name;
                var list = _context.experimentList.Include(list => list.experiments). Where(list => list.name == experimentListName && list.userLogin == login && list.experimentType == "ModelTrainingExperiment").FirstOrDefault();
                var experiment = _context.modelTrainingExperiment.Include(e => e.extendedModel).Where(experiment => experiment.experimentId == experimentId && experiment.extendedModel.userLogin == login).FirstOrDefault();
                if (experiment == null || list == null)
                    return BadRequest(_messeges.listOrExperimentDontExist);
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

        /// <summary>
        /// Dodawania eksperymentu rozpoznawania znaku
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPut("addPatternRecognitionExperiment")]
        [Consumes("multipart/form-data")]
        public IActionResult AddPatternRecognitionExperiment(string experimentListName)
        {
            try
            {
                string login = User.Identity.Name;
                var list = _context.experimentList.Include(list => list.experiments).Where(list => list.name == experimentListName && list.userLogin == login && list.experimentType == "PatternRecognitionExperiment").FirstOrDefault();

                if (list == null)
                    return BadRequest(_messeges.listDontExisit);

                var user = _context.user.Include(user => user.lastPatternRecognitionExperiment).Where(user => user.login == login).FirstOrDefault();

                if (user == null)
                    BadRequest();

                if(user.IsAbbleToAddPatternRecognitionExperiment())
                {
                    list.experiments.Add(user.lastPatternRecognitionExperiment);
                    user.exsistUnsavePatternRecognitionExperiment = false;
                }
                else
                {
                    return NotFound(_messeges.notFoundExperimentsToAdd); 
                }



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
        public IActionResult GetLists()
        {
            string login = User.Identity.Name;
            var list = _context.experimentList.Where(a => a.userLogin == login);
            return Ok(list);
        }

        /// <summary>
        /// Pobiera eksperymenty z danej listy
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetExperiments")]
        public async Task<IActionResult>  GetExperiments(string experimentListName)
        {
            string login = User.Identity.Name;
            var list = await _context.experimentList.Include(list => list.experiments)
                .Where(a => a.name == experimentListName && a.userLogin == login ).FirstOrDefaultAsync();
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
