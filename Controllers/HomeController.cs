using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ASP.NET_Identity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.NET_Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(UserManager<ApplicationUser> userManagerService,
            SignInManager<ApplicationUser> signInManagerService)
        {
            _userManager = userManagerService;
            //_roleManager = roleManagerService;
            _signInManager = signInManagerService;
        }

        [AllowAnonymous]

        public ActionResult Index()
        {
            //Let’s create list department for dropdownlist
            List<Department> ListDepartments = new List<Department>()
            {
                new Department() {Id = 1, Name="IT" },
                new Department() {Id = 2, Name="HR" },
                new Department() {Id = 3, Name="Payroll" },
            };
            ViewBag.Departments = ListDepartments;
            //let’s create one employee
            Employee emp = new Employee()
            {
                EmployeeId = 1,
                EmployeeName = "Pranaya",
                Gender = "Male",
                SelectedDepartmentIDs = new List<int>{ 1, 2, 3 }
            };
            //Pass that employee to the view
            return View(emp);
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles="Admin")]
        public IActionResult SecureMethod()
        {

            return View();

        }


        [AllowAnonymous]
        public IActionResult NotSecureMethod()
        {

            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
