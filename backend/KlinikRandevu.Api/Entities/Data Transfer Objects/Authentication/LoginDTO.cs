using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Authentication
{
    public class LoginDTO
    {
        [Required(ErrorMessage ="Kullanıcı adı zorunludur")]
        public string username { get; set; }
        [Required(ErrorMessage = "Kullanıcı şifresi zorunludur")]
        public string password { get; set; }
    }
}
