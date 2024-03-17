using homework3_13.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using homework3_13Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace homework3_13.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=AdWebsite; Integrated Security=true;";
        public IActionResult Index()
        {
            AdsWebRepository repo = new AdsWebRepository(_connectionString);
            HomeViewModel vm = new HomeViewModel();
            vm.Ads = repo.GetAds();
            if(User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                vm.Account = repo.GetByEmail(email);
            }
            return View(vm);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(Account account, string password)
        {
            AdsWebRepository repo = new AdsWebRepository(_connectionString);
            repo.AddAccount(account, password);
            return Redirect("/home/login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new AdsWebRepository(_connectionString);
            var account = repo.Login(email, password);
            if (account == null)
            {
                TempData["Message"] = "Invalid Login!";
                return RedirectToAction("Login");
            }


            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email) 
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))
                ).Wait();

            return Redirect("/home/newad");
        }

        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(string phoneNumber, string description)
        {
         
            AdsWebRepository repo = new AdsWebRepository ( _connectionString);
            var email = User.Identity.Name;
            Account account = repo.GetByEmail(email);
            Ad ad = new()
            {
                Title = account.Name,
                PhoneNumber = phoneNumber,
                Description = description,
                AccountId = account.Id
            };
            repo.NewAd(ad);
            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            AdsWebRepository repo = new AdsWebRepository( _connectionString);
            repo.DeleteAd(id);
            return Redirect("/home/index");
        }
    }
}