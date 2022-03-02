using AspNetIdentitydemo.Identity;
using AspNetIdentitydemo.KontoInsert;
using AspNetIdentitydemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetIdentitydemo.Controllers
{
    public class KontoController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return RedirectToAction("Login", "External", new { provider = KontoInsertDefaults.AuthenticationScheme });
            //return RedirectToAction("Login", "External", new { provider = GoogleDefaults.AuthenticationScheme });
        }
    }
}