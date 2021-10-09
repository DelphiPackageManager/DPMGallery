using DPMGallery.Types;
using DPMGallery.Extensions;
using DPMGallery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DPMGallery.Controllers
{
    public class HomeController : DPMController
    {
        private readonly ILogger<HomeController> _logger;
        private byte[] _cache;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Compiler = "11.0";
            var versions = new List<SelectListItem>();
            foreach (var ver in Enum.GetValues<CompilerVersion>())
            {
                if (ver == CompilerVersion.UnknownVersion)
                    continue;

                var item = new SelectListItem
                {
                    Value = ver.GetDescription(),
                    Text = "Delphi " + ver.GetDescription()
                };
                item.Selected = item.Value == "11.0";
                versions.Add(item);
            }
            ViewBag.CompilerVersions = versions;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // doesn't seem to get called by the browser - will look into it again when deployed on server
        [Route("/opensearch.xml")]
        public IActionResult OpenSearch()
        {
            if (_cache == null)
            {
                XNamespace nsOs = "http://a9.com/-/spec/opensearch/1.1/";

                XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(nsOs + "OpenSearchDescription",
                        new XElement(nsOs + "ShortName", "DPM Packages"),
                        new XElement(nsOs + "Description", "Search for DPM Packages"),
                        new XElement(nsOs + "InputEncoding", "UTF-8"),
                        new XElement(nsOs + "Image",
                            new XAttribute("width", 64),
                            new XAttribute("height", 64),
                            new XAttribute("type", "image/x-icon"),
                            $"{Url.Content("~/favicon.ico")}"
                        ),
                        new XElement(nsOs + "Url",
                            new XAttribute("type", "text/html"),
                            new XAttribute("method", "get"),
                            new XAttribute("template", "/packages?query={searchTerms}")
                        )                       
                    )
                );

                _cache = Encoding.UTF8.GetBytes(doc.ToString());

                
            }
            return File(_cache, "application/opensearchdescription+xml");
        }

    }
}
