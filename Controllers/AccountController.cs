using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ASP.NET_Identity.Models;

namespace ASP_NET_Identity.Controllers
{
    public  class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManger)
        {
            _signInManager = signInManager;
            _userManager = userManger;
        }

        //------------------ Start-login -----------------------------

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
       
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Handle successful login

                    // Check if the ReturnUrl is not null and is a local URL
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        // Redirect to default page
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    // Handle two-factor authentication case
                }
                if (result.IsLockedOut)
                {
                    // Handle lockout scenario
                }
                else
                {
                    // Handle failure
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Login(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
                return View();
        }

        //------------------ End-login -----------------------------
        
        
        //------------------ Start-logout -------------------------



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


        //------------------ End-logout -----------------------------

        
        //------------------ Start-Register -------------------------


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult>Register(RegisterViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser

                var user = new ApplicationUser()
                {
                    UserName=model.Email,
                    Email=model.Email,
                    FirstName=model.FirstName,
                    LastName=model.LastName
                };

                // Store user data in AspNetUsers database table
               var result =
                    await _userManager.CreateAsync(user, model.Password);

				// If the user is signed in and in the Admin role, then it is
				// the Admin user that is creating a new user. 
				// So redirect the Admin user to ListUsers action of Administration Controller

				if (result.Succeeded) {

                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {

                    return RedirectToAction("ListUsers", "Administration");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper

                foreach (var error in result.Errors) {
                
                    ModelState.AddModelError( string.Empty,error.Description );
                }

            }
            return View(model);
        }

        //------------------ End-Register ---------------------------

        //------------------ Start-Access Deined --------------------------

        [AllowAnonymous]
        [HttpGet]

        public async Task<IActionResult> AccessDenied()
        {

            return View();
        }

        //------------------ End-Access Deined --------------------------


        //------------------ start-validation --------------------------
        [AllowAnonymous]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> IsEmailAvailable(string Email)
        {
            //Check If the Email Id is Already in the Database
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {Email} is already in use.");
            }
        }
        //------------------ end-validation --------------------------

    }
}
