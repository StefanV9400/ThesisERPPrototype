using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Diagnostics;
using System.Linq;
using VERPS.WebApp.Database;
using VERPS.WebApp.Database.Enums;
using VERPS.WebApp.Models;

namespace VERPS.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHtmlLocalizer<HomeController> _loc;
        private readonly VERPSDBContext _context;

        public HomeController(IHtmlLocalizer<HomeController> loc, VERPSDBContext context)
        {
            _loc = loc;
            _context = context;
        }

        public IActionResult Index()
        {
            // TEMPORARY
            return RedirectToAction("Index", "Dashboard", new { area = "ExactOnline" });
        }

        public IActionResult ProviderConfiguration()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}
