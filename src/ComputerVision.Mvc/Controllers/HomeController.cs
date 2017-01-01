using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using ComputerVision.Messages;
using ComputerVision.Mvc.Models;
using Mediator.Net;

namespace ComputerVision.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _bus;

        public HomeController(IMediator bus)
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
                var result = await _bus.RequestAsync<EvaluateImageQuery, EvaluateImageQueryResult>(new EvaluateImageQuery(modelFilePath, file.InputStream, 32, 32));
                
                var target = new MemoryStream();
                file.InputStream.Position = 0;
                file.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                
                var viewModel = new ImageClassifierModel("", "", $"data:image/gif;base64,{Convert.ToBase64String(data)}", result.Outputs);
                return View("Index", viewModel);
            }
            
            return Index();
            
        }
    }
}