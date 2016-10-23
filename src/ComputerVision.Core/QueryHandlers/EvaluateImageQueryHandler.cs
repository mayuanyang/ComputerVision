using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ComputerVision.Core.ImageEvaluation;
using ComputerVision.Core.Models.CIFAR10;
using ComputerVision.Messages;
using Enexure.MicroBus;
using Microsoft.MSR.CNTK.Extensibility.Managed;

namespace ComputerVision.Core.QueryHandlers
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
                if (inDims.First().Value != query.Width * query.Height * 3)
                {
                    throw new CNTKRuntimeException(string.Format("The input dimension for {0} is {1} which is not the expected size of {2}.", inDims.First(), inDims.First().Value, 224 * 224 * 3), string.Empty);
                }

                Bitmap bmp = new Bitmap(query.ImageStream);

                var resized = bmp.Resize(query.Width, query.Height, true);
                var resizedCHW = resized.ParallelExtractCHW();
                var inputs = new Dictionary<string, List<float>>() { { inDims.First().Key, resizedCHW } };

                // We can call the evaluate method and get back the results (single layer output)...
                var outDims = model.GetNodeDimensions(NodeGroup.Output);
                var outputs = model.Evaluate(inputs, outDims.First().Key);
                var dic = new Dictionary<string, float>();
                int i = 1;
                foreach (var value in outputs)
                {
                    dic.Add(Cifar10.Labels[i], value);
                    i++;
                }
                var max = outputs.Select((value, index) => new { Value = value, Index = index })
                    .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                    .Index;

                return Task.FromResult(new EvaluateImageQueryResult(dic, max));
            }
        }
    }
}
