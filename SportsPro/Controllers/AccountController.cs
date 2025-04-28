using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SportsPro.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace SportsPro.Controllers
{
    public class AccountController : Controller
    {
        private Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private  Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager ;


        public AccountController(Microsoft.AspNetCore.Identity.UserManager<User> userMngr,
                                SignInManager<User> signInMngr, Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> RoleMngr )
            {
            userManager = userMngr;
            signInManager = signInMngr;
            roleManager = RoleMngr;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.Firstname,
                    LastName = model.Lastname
                };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult LogIn(string returnURL = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnURL };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password, isPersistent: model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.Username);

                    const string defaultRole = "User";
                    if (!await roleManager.RoleExistsAsync(defaultRole))
                        await roleManager.CreateAsync(new IdentityRole(defaultRole));

                    var roles = await userManager.GetRolesAsync(user);
                    if (!roles.Any() || (!roles.Contains("Admin") && !roles.Contains(defaultRole)))
                        await userManager.AddToRoleAsync(user, defaultRole);

                    await signInManager.RefreshSignInAsync(user);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid username/password.");
            return View(model);
        }

        public ViewResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel
            {
                Username = User.Identity?.Name ?? ""
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByNameAsync(model.Username);
                var result = await userManager.ChangePasswordAsync(user,
                    model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["message"] = "Password changed successfully";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

    }
}