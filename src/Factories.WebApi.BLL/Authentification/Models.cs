using System.Security.Claims;

namespace Factories.WebApi.BLL.Models
{
    public class UpdatePasswordModel
    {
        public required string Login { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }

    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }


    public class RegisterModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }

        public IList<ClaimModel>? Claims { get; set; }
    }

    public class DeleteModel
    {
        public required string Username { get; set; }
    }

    public class ClaimModel
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
    }
}
