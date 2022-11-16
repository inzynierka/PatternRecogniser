using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PatternRecogniser.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class UserController : ControllerBase
    {
        PatternRecogniserDBContext _context;
    public UserController(PatternRecogniserDBContext context)
    {
        _context = context;
    }
        public IEnumerable<User> Get()
        {
            return _context.user.ToList();
        }

    }
}
