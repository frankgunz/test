using System.ComponentModel.DataAnnotations;

namespace Hubbell.EHubb.BACnetAPI.Models
{
    public class AccessTokenModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        public int ExpiresAt { get; set; }
    }
}
