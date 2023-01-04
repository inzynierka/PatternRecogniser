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
using System.Linq.Expressions;
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
        private IGenericRepository<Experiment> _experimentRepo;
        private IGenericRepository<RecognisedPatterns> _recognisedPatternsRepo;
        private ExperimentListStringMesseges _messeges = new ExperimentListStringMesseges();
        public ExperimentListController(IGenericRepository<ExtendedModel> extendedModelRepo,
            IGenericRepository<ExperimentList> experimentListRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<Experiment> experimentRepo,
            IGenericRepository<RecognisedPatterns> recognisedPatternsRepo)
        {
            _experimentListRepo = experimentListRepo;
            _extendedModelRepo = extendedModelRepo;
            _userRepo = userRepo;
            _experimentRepo = experimentRepo;
            _recognisedPatternsRepo = recognisedPatternsRepo;
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
                var experimentsList = _experimentListRepo.Get(list => list.name == experimentListName &&
                    list.userLogin == login &&
                    list.experimentType == "ModelTrainingExperiment",
                        list => list.Include( l => l.experiments))
                    .FirstOrDefault();
                var experiment = _extendedModelRepo.Get(model => model.extendedModelId == modelId && model.userLogin == login, 
                    model => model.Include(m => m.experiments))
                    .FirstOrDefault()?.modelTrainingExperiment;

                if (experiment == null || experimentsList == null)
                    return BadRequest(_messeges.listOrExperimentDontExist);

                experimentsList.experiments.Add(experiment);
                await _experimentListRepo.SaveChangesAsync();
                return Ok(_messeges.experimentHasBeenAdded);

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
                    list => list.Include(l => l.experiments)).FirstOrDefault();

                if (list == null)
                    return BadRequest(_messeges.listDontExisit);
                //lastPatternRecognitionExperiment
                var user = _userRepo.Get(user => user.login == login,
                    user => user.
                        Include(u => u.lastPatternRecognitionExperiment))
                    .FirstOrDefault();

                if (user == null)
                    BadRequest();

                if (user.IsAbbleToAddPatternRecognitionExperiment())
                {
                    list.experiments.Add(user.lastPatternRecognitionExperiment);
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
            var list = _experimentListRepo.Get(
                a => a.userLogin == login, 
                include: a => a.Include(l => l.experiments), 
                selector: a => new
                {
                    a.experimentListId,
                    a.name,
                    a.userLogin,
                    a.experimentType,
                    experimentsCount = a.experiments.Count
                });
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

            var experiments = _experimentListRepo.Get(
                a => a.name == experimentListName && a.userLogin == login,
                list => list.Include(l => l.experiments)).FirstOrDefault()?.experiments;


            if (experiments == null)
                return NotFound();

            var experimentsWithModel = _experimentListRepo.GetSelectMany(
                a => a.experiments,
                (el, ex) => new
                {
                    ex.experimentId,
                    extendModel = new
                    {
                        ex.extendedModel.extendedModelId,
                        ex.extendedModel.name,
                        ex.extendedModel.userLogin,
                        ex.extendedModel.distribution,
                        ex.extendedModel.num_classes
                    }
                },
                a => a.name == experimentListName && a.userLogin == login,
                list => list.Include(l => l.experiments).ThenInclude(e => e.extendedModel));

            
            foreach(var ex in experiments)
            {
                var tmp = experimentsWithModel.Single(a => a.experimentId == ex.experimentId).extendModel;
                ex.extendedModel = new ExtendedModel();
                ex.extendedModel.extendedModelId = tmp.extendedModelId;
                ex.extendedModel.name = tmp.name;
                ex.extendedModel.userLogin = tmp.userLogin;
                ex.extendedModel.distribution = tmp.distribution;
                ex.extendedModel.num_classes = tmp.num_classes;
                ex.experimentLists = null;
            }

            return Ok(experiments);
        }

        /// <summary>
        /// Usuwanie listy
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpDelete("DeleteList")]
        public async Task<IActionResult> DeleteList(string experimentListName)
        {
            string login = User.Identity.Name;

            var listIdAndType = _experimentListRepo.Get(a => a.name == experimentListName && a.userLogin == login,
                include: list => list.Include(l => l.experiments), selector: a => new { a.experimentListId, a.experimentType }).FirstOrDefault();

            if (listIdAndType == null)
                return Ok(_messeges.susessfullyDeleted);

            var user = _userRepo.Get(a => a.login == login).FirstOrDefault();

            if (new PatternRecognitionExperiment().IsItMe(listIdAndType.experimentType))
            {
                DeletePatternRecognitionExperimentsWithoutList(listIdAndType.experimentListId, user);
            }

            _experimentListRepo.Delete(listIdAndType.experimentListId);
            await _experimentListRepo.SaveChangesAsync();

            return Ok(_messeges.susessfullyDeleted);
        }


        private bool IsExperimentListExsist(string login, string experimentName)
        {
            return _experimentListRepo.Get(list => list.name == experimentName && list.userLogin == login).Count() > 0;
        }

        private void DeletePatternRecognitionExperimentsWithoutList(int listId, User user)
        {
            var experimentsInList = _experimentListRepo.GetSelectMany(
                a => a.experiments,
                (el, ex) => new
                {
                    ex.experimentId,
                    numberOfConectedList = ex.experimentLists.Count
                },
                a => a.experimentListId == listId,
                include: list => list.Include(l => l.experiments));


            foreach (var ex in experimentsInList)
            {
                if (ex.numberOfConectedList == 1)
                {
                    if (user.lastPatternRecognitionExperimentexperimentId == ex.experimentId)
                        continue;

                    var rps = _recognisedPatternsRepo.Get(rp => rp.PatternRecognitionExperimentexperimentId == ex.experimentId);

                    foreach (var rp in rps)
                        _recognisedPatternsRepo.Delete(rp);

                    _experimentRepo.Delete(ex.experimentId);

                }
            }
        }

    }
}
