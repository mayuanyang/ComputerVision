using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using ComputerVision.Messages;
using ComputerVision.Mvc.Models;
using Enexure.MicroBus;

namespace ComputerVision.Mvc.Controllers
{
    public class ResNetController : Controller
    {
        private readonly IMicroBus _bus;
        public ResNetController(IMicroBus bus)
        {
            _bus = bus;
        }

        // GET: ResNet152
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload()
        {

            if (Request.Files.Count > 0)
            {
                var modelFilePath = Path.Combine(Server.MapPath("~/PreTrainedModel/"), "ResNet_152.model");

                var file = Request.Files[0];
                var result = await _bus.QueryAsync(new EvaluateImageQuery(modelFilePath, file.InputStream, 224, 224));

                var target = new MemoryStream();
                file.InputStream.Position = 0;
                file.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                
                var viewModel = new ImageClassifierModel("", "", $"data:image/gif;base64,{Convert.ToBase64String(data)}", result.Outputs);
                return RedirectToAction("Index", viewModel);
                
            }

            return RedirectToAction("Index");

        }
    }
}
