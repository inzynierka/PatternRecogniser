using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public class Authentication // possibly not needed
    {
        [Key]
        public int userId { get; set; }
        public string hashedToken { get; set; }
        
        public virtual User user { get; set; }

        public void LogIn(string name, string password) { }

        public void LogOut(User user) { }

        public void SignIn(string email, string name, string password) { }
    }
}
