using Api.Models;

namespace api.Dtos.Auth
{
    public class LoginModelDto
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}