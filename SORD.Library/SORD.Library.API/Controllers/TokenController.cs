using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SORD.Library.API.Controllers
{
    public class TokenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
