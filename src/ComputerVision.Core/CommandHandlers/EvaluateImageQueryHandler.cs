using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ComputerVision.Core.ImageEvaluation;
using ComputerVision.Messages;
using Enexure.MicroBus;
using Microsoft.MSR.CNTK.Extensibility.Managed;

namespace ComputerVision.Core.CommandHandlers
{
    public class EvaluateImageQueryHandler : IQueryHandler<EvaluateImageQuery, EvaluateImageQueryResult>
    {
      
        public Task<EvaluateImageQueryResult> Handle(EvaluateImageQuery query)
        {
            using (var model = new IEvaluateModelManagedF())
            {

                model.CreateNetwork(string.Format("modelPath=\"{0}\"", query.ModelFilePath), deviceId: -1);

                // Prepare input value in the appropriate structure and size
                var inDims = model.GetNodeDimensions(NodeGroup.Input);
                if (inDims.First().Value != 32 * 32 * 3)
                {
                    throw new CNTKRuntimeException(string.Format("The input dimension for {0} is {1} which is not the expected size of {2}.", inDims.First(), inDims.First().Value, 224 * 224 * 3), string.Empty);
                }


                Bitmap bmp = new Bitmap(query.ImageStream);

                var resized = bmp.Resize(32, 32, true);
                var resizedCHW = resized.ParallelExtractCHW();
                var inputs = new Dictionary<string, List<float>>() { { inDims.First().Key, resizedCHW } };

                // We can call the evaluate method and get back the results (single layer output)...
                var outDims = model.GetNodeDimensions(NodeGroup.Output);
                var outputs = model.Evaluate(inputs, outDims.First().Key);
                var max = outputs.Select((value, index) => new { Value = value, Index = index })
                    .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                    .Index;

                return Task.FromResult(new EvaluateImageQueryResult(outputs, max));
            }
        }
    }
}
