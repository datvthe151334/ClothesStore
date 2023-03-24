using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; }
                
        public string Password { get; set; }

        public string? RefreshToken { get; set; }
    }
}
