using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public class Authentication // possibly not needed
    {
        [Key]
        public string userLogin { get; set; }
        public string hashedToken { get; set; }
        public string lastSeed { get; set; } // chyba potrzebuejmy tego by dodać do hasła by sprawdzić czy zgadza się z ostatnim
        [ForeignKey("userLogin")]
        public virtual User user { get; set; }
        

        public void LogIn(string name, string password) { }

        public void LogOut(User user) { }

        public void SignIn(string email, string name, string password) { }

        
    }
}
