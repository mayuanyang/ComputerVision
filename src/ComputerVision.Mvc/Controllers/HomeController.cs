using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ComputerVision.Core;
using ComputerVision.Mvc.Models;
using Microsoft.MSR.CNTK.Extensibility.Managed;

namespace ComputerVision.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                
                List<float> outputs;

                using (var model = new IEvaluateModelManagedF())
                {

                    var modelFilePath = Path.Combine(Server.MapPath("~/PreTrainedModel/"), "model.cmf");


                    model.CreateNetwork(string.Format("modelPath=\"{0}\"", modelFilePath), deviceId: -1);

                    // Prepare input value in the appropriate structure and size
                    var inDims = model.GetNodeDimensions(NodeGroup.Input);
                    if (inDims.First().Value != 32 * 32 * 3)
                    {
                        throw new CNTKRuntimeException(string.Format("The input dimension for {0} is {1} which is not the expected size of {2}.", inDims.First(), inDims.First().Value, 224 * 224 * 3), string.Empty);
                    }


                    Bitmap bmp = new Bitmap(file.InputStream);

                    var resized = bmp.Resize(32, 32, true);
                    var resizedCHW = resized.ParallelExtractCHW();
                    var inputs = new Dictionary<string, List<float>>() { { inDims.First().Key, resizedCHW } };

                    // We can call the evaluate method and get back the results (single layer output)...
                    var outDims = model.GetNodeDimensions(NodeGroup.Output);
                    outputs = model.Evaluate(inputs, outDims.First().Key);
                }

                // Retrieve the outcome index (so we can compare it with the expected index)
                var max = outputs.Select((value, index) => new { Value = value, Index = index })
                    .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                    .Index;

                var target = new MemoryStream();
                file.InputStream.Position = 0;
                file.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                var viewModel = new ImageClassifierModel(max == 0 ? "Horse" : "Lion", "",
                    $"data:image/gif;base64,{Convert.ToBase64String(data)}", outputs);
                return View("Index", viewModel);
            }
            
            return Index();
            
        }
    }
}