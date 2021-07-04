using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthBasics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public IActionResult index()
        {
            return View();
        }

        [Authorize]
        public IActionResult secret()
        {
            return View();
        }

        [Authorize(Policy = "Claim.DoB")]
        [Authorize(Policy ="SecurityLevel.5")]
        public IActionResult SecretPolicy()
        {
            return View("secret");
        }

        [Authorize(Roles ="Admin")]
        public IActionResult SecretRole()
        {
            return View("secret");
        }

        //we throw in AllowAnonymous to lift the Global Filters in StartUp.cs line 58 to 64

        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var carolineClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "Bob@gmail.com"),
                new Claim("Caroline.Says", "Very nice boi" ),

            };

            var lisenceClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, "Bob K Foo"),
                new Claim("Driving Lisence", "A+" ),
            };

            var carolineIdentity = new ClaimsIdentity(carolineClaims, "Caroline Identity");
            var lisenceIdentity = new ClaimsIdentity(lisenceClaims, "Government"); 

            var userPrincipal = new ClaimsPrincipal(new[] { carolineIdentity, lisenceIdentity });

            HttpContext.SignInAsync(userPrincipal);
             return RedirectToAction("Index");
        }
        public async Task<IActionResult> DoStuff()
        {
            //we do stuff here
            var builder = new AuthorizationPolicyBuilder("Schema");
            var CustomPolicy = builder.RequireClaim("Hello").Build();

            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, "Claim.DoB");

            if(authResult.Succeeded)
            {

            }

            return View("Index");
        }
    }
}
