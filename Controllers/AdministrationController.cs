using ASP.NET_Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Identity.Controllers
{

    [Authorize(Roles ="Admin,Moderator")]
    public class AdministrationController : Controller
    {

        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdministrationController(RoleManager<ApplicationRole>roleMangerService, 
            UserManager<ApplicationUser> userManagerService)
        {
            _roleManager = roleMangerService;
            _userManager = userManagerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //------------------ Start-Roles -------------------------

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the role already exists
                bool roleExists = await _roleManager.RoleExistsAsync(roleModel?.RoleName);
            
                if (roleExists)
                {
                    ModelState.AddModelError("","Role Already Exists");
                }
       
                else
                {
                    //Create the role
                    //We just need to specify a unique role name to create a new role
                    ApplicationRole identityRole = new ApplicationRole
                    {
                        Name = roleModel?.RoleName,
                        Description = roleModel?.Description,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };

                    // Saves the role in the underlying AspNetRoles table
                    IdentityResult result = await _roleManager.CreateAsync(identityRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(roleModel);
        }
        [HttpGet]
        [Authorize]
        public async Task< IActionResult >ListRoles()
        {



            List<ApplicationRole> lstRoles =await
            _roleManager.Roles.ToListAsync();

            return View(lstRoles);

        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            //First Get the role information from the database
            ApplicationRole role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Handle the scenario when the role is not found
                return View("Error");
            }

            //Populate the EditRoleViewModel from
            //the data retrived from the database
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                Description=role.Description,
                Users=new List<string>()

                // You can add other properties here if needed
            
            };
            //retrive all the users

            foreach (var user in _userManager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. 
                // This model object is then passed to the view for display
            
                if( await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }


            }
        
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    // Handle the scenario when the role is not found
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    
                    role.Name = model.RoleName;
                    role.Description = model.Description;
                    // Update other properties if needed

                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles"); // Redirect to the roles list
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Role not found, handle accordingly
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                // Role deletion successful
                return RedirectToAction("ListRoles"); // Redirect to the roles list page
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // If we reach here, something went wrong, return to the view
            return View("ListRoles", await _roleManager.Roles.ToListAsync());
        }

        //------------------ End-Roles ---------------------------

        //------------------ Start-Users-Roles -------------------------


        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {

            ViewBag.RoleId = roleId;
            
            //check if the role exists in database or not
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
         
            //return all users relate to this role

            ViewBag.RollName = role.Name; //To display purpose

            var model=new List<UserRoleViewModel>();
            
            foreach (ApplicationUser user in _userManager.Users.ToList())
            {

                var UserRoleViewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                //check if user in role or not 
                if(await _userManager.IsInRoleAsync(user, roleId))
                {
                    UserRoleViewModel.IsSelected = true;
                }
                else
                {
                    UserRoleViewModel.IsSelected = false;
                }
                model.Add(UserRoleViewModel);
            }
          
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,string roleId)
        {

            //First check wethear role is exist or not x
            if (model == null) {

                ViewBag.ErrorMessage = $"Role with id ={roleId} cannot be found";
                return View("NotFound");
            
            }

            IdentityRole role =await _roleManager.FindByIdAsync(roleId);
            int cnt = 1;

            foreach (var entity in model) {
                cnt++;
                var user = await _userManager.FindByIdAsync(entity.UserId);

                IdentityResult? result;

                if (entity.IsSelected &&! (await _userManager.IsInRoleAsync(user, role.Name))) 
                {
                
                    //if selected is true and user not already in this role ,then add it 
                    result =await _userManager.AddToRoleAsync(user, role.Name);

                }
                else if(!entity.IsSelected&&(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    //not selected and in role's users list then you wants to remove

                   result =await _userManager.RemoveFromRoleAsync(user, role.Name);

                }
                  else
                  {
                    //don't do anything simply continue loop
                    continue; 
                  }

                //If you add or remove any user, please check the Succeeded of the IdentityResult
                if (result.Succeeded)
                {
                    if (cnt < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { roleId = roleId });
                }

            }
            return RedirectToAction("EditRole", new { roleId = roleId });

        }


        //------------------ End-Users-Roles  ---------------------------


        //------------------ Start-Users -------------------------

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }
        //------------------ End-Users  ---------------------------






    }

}
