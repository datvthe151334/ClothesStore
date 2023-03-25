using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
	public class SignUpDTO
	{
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; } = 2;
        public CustomerSignUpDTO? customer { get; set; }
    }
}
