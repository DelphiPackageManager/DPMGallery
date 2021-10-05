using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace DPMGallery.Controllers
{
    public class ProfilesController : Controller
    {
        [Route("/profiles/{profileName}")]
        public IActionResult Index(string profileName)
        {
            return View();
        }
    }
}
