using System.ComponentModel.DataAnnotations;

namespace Entities.Data_Transfer_Objects.Authentication
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "Refresh token zorunludur")]
        public string RefreshToken { get; set; }
    }
}
