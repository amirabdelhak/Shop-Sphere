using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using graduated_project.Models;
using graduated_project.viewmodels;
using Microsoft.AspNetCore.Authorization;


namespace graduated_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(Registervm appuser)
        {
            if (!ModelState.IsValid)
                return View(appuser);

            try
            {
                AppUser user = new AppUser();
                user.UserName = appuser.username;
                user.FirstName = appuser.FirstName;
                user.LastName = appuser.LastName;
                user.Address = appuser.Address;

                var register = await userManager.CreateAsync(user, appuser.password);
                if (register.Succeeded)
                {
                    var addRoleResult = await userManager.AddToRoleAsync(user, "user");
                    // continue to sign in even if role assignment failed

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in register.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing your request.");
            }

            return View(appuser);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(loginvm login)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(login.username);
                if(user != null)
                {
                    var result =await signInManager.PasswordSignInAsync(user, login.password,false,false);
                    if (result.Succeeded) 
                    {
                        return RedirectToAction("getproducts", "Products");
                    }
                    else
                    {
                        ModelState.AddModelError("","invalid user name or password");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "invalid user name or password");

                }
            }
            return View(login);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("getproducts", "Products");
        }


        [Authorize(Roles = "super admin")]
        public IActionResult adminRegister()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> adminRegister(Registervm appuser)
        {
            if (!ModelState.IsValid)
                return View(appuser);

            try
            {
                AppUser user = new AppUser();
                user.UserName = appuser.username;
                user.FirstName = appuser.FirstName;
                user.LastName = appuser.LastName;
                user.Address = appuser.Address;
                var register = await userManager.CreateAsync(user, appuser.password);
                if (register.Succeeded)
                {
                    var addRoleResult = await userManager.AddToRoleAsync(user, "admin");

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("getproducts", "Products");
                }
                else
                {
                    foreach (var item in register.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing your request.");
            }

            return View(appuser);
        }



        [Authorize(Roles = "super admin")]
        public IActionResult superadminRegister()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> superadminRegister(Registervm appuser)
        {
            if (!ModelState.IsValid)
                return View(appuser);

            try
            {
                AppUser user = new AppUser();
                user.UserName = appuser.username;
                user.FirstName = appuser.FirstName;
                user.LastName = appuser.LastName;
                user.Address = appuser.Address;
                var register = await userManager.CreateAsync(user, appuser.password);
                if (register.Succeeded)
                {
                    var addRoleResult = await userManager.AddToRoleAsync(user, "super admin");

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("getproducts", "Products");
                }
                else
                {
                    foreach (var item in register.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing your request.");
            }

            return View(appuser);
        }

    }
}
