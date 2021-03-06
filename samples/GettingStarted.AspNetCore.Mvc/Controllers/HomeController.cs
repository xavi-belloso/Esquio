﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GettingStarted.AspNetCore.Mvc.Models;
using Esquio.AspNetCore.Endpoints.Metadata;

namespace GettingStarted.AspNetCore.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [FeatureFilter(Name= "HiddenGem")]
        public IActionResult Index()
        {
            return View();
        }

        [FeatureFilter(Name="PrivacyFeature")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ActionName("Privacy")]
        public IActionResult PrivacyWheFeatureIsNotEnabled()
        {
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
