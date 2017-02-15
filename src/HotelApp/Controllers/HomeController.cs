using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HotelApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<HotelUser> _signInManager;
        public HomeController(SignInManager<HotelUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            //await _signInManager.SignOutAsync();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
