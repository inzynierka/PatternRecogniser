using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Messages.ExperimentList;
using PatternRecogniser.Models;
using PatternRecogniser.Services.Repos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{
    [Route("")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExperimentListController : ControllerBase
    {
        private IGenericRepository<ExperimentList> _experimentListRepo;
        private IGenericRepository<ExtendedModel> _extendedModelRepo;
        private IGenericRepository<User> _userRepo;
        private ExperimentListStringMesseges _messeges = new ExperimentListStringMesseges();
        public ExperimentListController(IGenericRepository<ExtendedModel> extendedModelRepo,
            IGenericRepository<ExperimentList> experimentListRepo,
            IGenericRepository<User> userRepo)
        {
            _experimentListRepo = experimentListRepo;
            _extendedModelRepo = extendedModelRepo;
            _userRepo = userRepo;
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

                _experimentListRepo.Insert(new ExperimentList()
                {
                    userLogin = login,
                    name = experimentListName,
                    experimentType = experimentType
                });
                await _experimentListRepo.SaveChangesAsync();
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
        public async Task<IActionResult> AddModelTrainingExperiment(string experimentListName, int modelId)
        {
            try
            {
                string login = User.Identity.Name;
                var list = _experimentListRepo.Get(list => list.name == experimentListName &&
                    list.userLogin == login &&
                    list.experimentType == "ModelTrainingExperiment",
                        "experiment")
                    .FirstOrDefault(); 
                var experiment = _extendedModelRepo.Get(model => model.extendedModelId == modelId && model.userLogin == login, "modelTrainingExperiment").FirstOrDefault()?.modelTrainingExperiment;

                if (experiment == null || list == null)
                    return BadRequest(_messeges.listOrExperimentDontExist);
                else
                {
                    list.experiments.Add(experiment);
                    await _experimentListRepo.SaveChangesAsync();
                    return Ok(_messeges.experimentHasBeenAdded);
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
        public async Task<IActionResult> AddPatternRecognitionExperiment(string experimentListName)
        {
            try
            {
                string login = User.Identity.Name; 
                var list = _experimentListRepo.Get(list => list.name == experimentListName && list.userLogin == login && list.experimentType == "PatternRecognitionExperiment",
                    "experiments").FirstOrDefault();

                if (list == null)
                    return BadRequest(_messeges.listDontExisit);
                //lastPatternRecognitionExperiment
                var user = _userRepo.Get(user => user.login == login, "lastPatternRecognitionExperiment").FirstOrDefault();

                if (user == null)
                    BadRequest();

                if (user.IsAbbleToAddPatternRecognitionExperiment())
                {
                    list.experiments.Add(user.lastPatternRecognitionExperiment);
                    user.exsistUnsavePatternRecognitionExperiment = false;
                    await _experimentListRepo.SaveChangesAsync();
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
            var list = _experimentListRepo.Get(a => a.userLogin == login);
            return Ok(list);
        }

        /// <summary>
        /// Pobiera eksperymenty z danej listy
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpGet("GetExperiments")]
        public IActionResult  GetExperiments(string experimentListName)
        {
            string login = User.Identity.Name; 
            var list = _experimentListRepo.Get(a => a.name == experimentListName && a.userLogin == login, "experiments").FirstOrDefault();
            var experiments = list?.experiments;
            if(experiments == null)
                return NotFound();
            else
                return Ok(experiments);
        }


        private bool IsExperimentListExsist(string login, string experimentName)
        {
            return _experimentListRepo.Get(list => list.name == experimentName && list.userLogin == login).Count() > 0;
        }
    }
}
