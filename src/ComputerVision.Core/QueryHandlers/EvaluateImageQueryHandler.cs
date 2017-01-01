using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ComputerVision.Core.ImageEvaluation;
using ComputerVision.Core.Models.CIFAR10;
using ComputerVision.Messages;
using Mediator.Net.Context;
using Microsoft.MSR.CNTK.Extensibility.Managed;
using Mediator.Net.Contracts;

namespace ComputerVision.Core.QueryHandlers
{
    public class EvaluateImageQueryHandler : IRequestHandler<EvaluateImageQuery, EvaluateImageQueryResult>
    {
      
     
        public Task<EvaluateImageQueryResult> Handle(ReceiveContext<EvaluateImageQuery> context)
        {
            using (var model = new IEvaluateModelManagedF())
            {
                model.CreateNetwork(string.Format("modelPath=\"{0}\"", context.Message.ModelFilePath), deviceId: -1);

                // Prepare input value in the appropriate structure and size
                var inDims = model.GetNodeDimensions(NodeGroup.Input);
                if (inDims.First().Value != context.Message.Width * context.Message.Height * 3)
                {
                    throw new CNTKRuntimeException(string.Format("The input dimension for {0} is {1} which is not the expected size of {2}.", inDims.First(), inDims.First().Value, 224 * 224 * 3), string.Empty);
                }

                Bitmap bmp = new Bitmap(context.Message.ImageStream);

                var resized = bmp.Resize(context.Message.Width, context.Message.Height, true);
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
