using Factories.WebApi.BLL.Models;
using Factories.WebApi.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Factories.WebApi.BLL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(JwtService jwtService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
    {
        private readonly JwtService jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        private readonly UserManager<IdentityUser> userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));


        [HttpPost("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userClaims = await userManager.GetClaimsAsync(user);

                var userRoles = await userManager.GetRolesAsync(user);

                var token = jwtService.GenerateJwtToken(user, userClaims, userRoles);

                return Ok(new { Token = token });
            }

            return NotFound("User login or password incorrect");
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser == null)
                return NotFound();

            return Ok(new
            {
                currentUser.Id,
                currentUser.UserName,
                currentUser.Email,
            });
        }

        [HttpPost("password/update")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordModel model)
        {
            var targetUser = await userManager.FindByNameAsync(model.Login);

            if (targetUser is null)
                return NotFound("User not found");

            // Проверяем, является ли текущий пользователь администратором
            var isAdmin = User.IsInRole("Admin");

            // Если текущий пользователь не администратор и пытается изменить пароль другого пользователя, возвращаем ошибку
            if (!isAdmin && targetUser.UserName != User.Identity.Name)
                return StatusCode(403,"You do not have permission to change other users' passwords");

            if (targetUser.UserName == User.Identity.Name)
            {
                var result = await userManager.ChangePasswordAsync(targetUser, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                    return Ok("Password updated");

                return BadRequest(result.Errors);
            }

            // Если пользователь администратор, то он может изменять пароль любого пользователя

            var changeResult = await userManager.ChangePasswordAsync(targetUser, model.CurrentPassword, model.NewPassword);

            if (changeResult.Succeeded)
                return Ok($"Password for user {model.Login} updated");

            return BadRequest(changeResult.Errors);

        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            var user = new IdentityUser { UserName = model.Username, Email = model.Username };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, model.Role);

                foreach (var claim in model.Claims)
                    await userManager.AddClaimAsync(user, new Claim(claim.Type, claim.Value));

                return Ok("Successfully registered");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return BadRequest(ModelState);
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromBody] DeleteModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
                return NotFound();


            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
                return Ok("Deleted");

            return BadRequest("Failed to delete user");
        }
    }
}