﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
#if (OrganizationalAuth)
using Microsoft.AspNetCore.Authorization;
#endif
using Microsoft.AspNetCore.Mvc;
using Company.WebApplication1.Models;

namespace Company.WebApplication1.Controllers
{
#if (OrganizationalAuth)
    [Authorize]
#endif
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

#if (OrganizationalAuth)
        [AllowAnonymous]
#endif
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
