using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ComputerVision.Core;
using ComputerVision.Core.ImageEvaluation;
using ComputerVision.Messages;
using ComputerVision.Mvc.Models;
using Enexure.MicroBus;
using Microsoft.MSR.CNTK.Extensibility.Managed;

namespace ComputerVision.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMicroBus _bus;

        public HomeController(IMicroBus bus)
        {
            _bus = bus;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            
            if (Request.Files.Count > 0)
            {
                var modelFilePath = Path.Combine(Server.MapPath("~/PreTrainedModel/"), "cifar10.ResNet.cmf");

                var file = Request.Files[0];
                var result = await _bus.QueryAsync(new EvaluateImageQuery(modelFilePath, file.InputStream, 32, 32));
                
                var target = new MemoryStream();
                file.InputStream.Position = 0;
                file.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                string label = "Other";
                switch (result.MatchingResultIndex)
                {
                    case 0:
                        label = "Horse";
                        break;
                    case 1:
                        label = "Lion";
                        break;
                }
                
            
                var viewModel = new ImageClassifierModel(label, "", $"data:image/gif;base64,{Convert.ToBase64String(data)}", result.Outputs);
                return View("Index", viewModel);
            }
            
            return Index();
            
        }
    }
}