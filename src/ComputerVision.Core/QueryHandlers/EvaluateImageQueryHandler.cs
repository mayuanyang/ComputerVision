using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CNTK;
using ComputerVision.Core.Models.CIFAR10;
using ComputerVision.Messages;
using Mediator.Net.Context;
using Mediator.Net.Contracts;

namespace ComputerVision.Core.QueryHandlers
{
    public class EvaluateImageQueryHandler : IRequestHandler<EvaluateImageQuery, EvaluateImageQueryResult>
    {


        public Task<EvaluateImageQueryResult> Handle(ReceiveContext<EvaluateImageQuery> context)
        {
            return EvaluationSingleImage(DeviceDescriptor.CPUDevice, context);
        }

        public Task<EvaluateImageQueryResult> EvaluationSingleImage(DeviceDescriptor device, ReceiveContext<EvaluateImageQuery> context)
        {
            try
            {
                Console.WriteLine("\n===== Evaluate single image =====");

                string modelFilePath = context.Message.ModelFilePath;
                Function modelFunc = Function.LoadModel(modelFilePath, device);

                
                //Variable inputVar = modelFunc.Arguments.Single();
                Variable inputVar = modelFunc.Arguments.First();

                // Get shape data for the input variable
                NDShape inputShape = inputVar.Shape;
                int imageWidth = inputShape[0];
                int imageHeight = inputShape[1];
                int imageChannels = inputShape[2];
                int imageSize = inputShape.TotalSize;

                // The model has only one output.
                // If the model have more than one output, use the following way to get output variable by name.
                Variable outputVar = modelFunc.Outputs.Where(variable => string.Equals(variable.Name, "z")).Single();
                // Variable outputVar = modelFunc.Output;

                

                var inputDataMap = new Dictionary<Variable, Value>();
                var outputDataMap = new Dictionary<Variable, Value>();

                // Image preprocessing to match input requirements of the model.
                // This program uses images from the CIFAR-10 dataset for evaluation.
                // Please see README.md in <CNTK>/Examples/Image/DataSets/CIFAR-10 about how to download the CIFAR-10 dataset.
                
                
                Bitmap bmp = new Bitmap(context.Message.ImageStream);
                var resized = bmp.Resize((int)imageWidth, (int)imageHeight, true);
                List<float> resizedCHW = resized.ParallelExtractCHW();

                // Create input data map
                var inputVal = Value.CreateBatch(inputVar.Shape, resizedCHW, device);
                inputDataMap.Add(inputVar, inputVal);

                // Create ouput data map. Using null as Value to indicate using system allocated memory.
                // Alternatively, create a Value object and add it to the data map.
                outputDataMap.Add(outputVar, null);

                // Start evaluation on the device
                modelFunc.Evaluate(inputDataMap, outputDataMap, device);

                // Get evaluate result as dense output
                var outputVal = outputDataMap[outputVar];
                var outputData = outputVal.GetDenseData<float>(outputVar);

                var dic = new Dictionary<string, float>();
                int i = 1;
                foreach (var value in outputData[0])
                {
                    dic.Add(Cifar10.Labels[i], value);
                    i++;
                }

                var max = outputData[0].Select((value, index) => new { Value = value, Index = index })
                    .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                    .Index;

                return Task.FromResult(new EvaluateImageQueryResult(dic, max));

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}\nCallStack: {1}\n Inner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException != null ? ex.InnerException.Message : "No Inner Exception");
                throw ex;
            }
        }
    }
}
