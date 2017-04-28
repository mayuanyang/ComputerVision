using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CNTK;
using ComputerVision.Messages;
using Mediator.Net.Context;
using Mediator.Net.Contracts;

namespace ComputerVision.Core.QueryHandlers
{
    public class EvaluateImageQueryHandler : IRequestHandler<EvaluateImageQuery, EvaluateImageQueryResult>
    {


        public Task<EvaluateImageQueryResult> Handle(ReceiveContext<EvaluateImageQuery> context)
        {
            EvaluationSingleImage(DeviceDescriptor.CPUDevice, context);
            return null;
        }

        public static void EvaluationSingleImage(DeviceDescriptor device, ReceiveContext<EvaluateImageQuery> context)
        {
            try
            {
                Console.WriteLine("\n===== Evaluate single image =====");

                string modelFilePath = context.Message.ModelFilePath;
                Function modelFunc = Function.LoadModel(modelFilePath, device);

                
                Variable inputVar = modelFunc.Arguments.Single();

                // Get shape data for the input variable
                NDShape inputShape = inputVar.Shape;
                int imageWidth = inputShape[0];
                int imageHeight = inputShape[1];
                int imageChannels = inputShape[2];
                int imageSize = inputShape.TotalSize;

                // The model has only one output.
                // If the model have more than one output, use the following way to get output variable by name.
                // Variable outputVar = modelFunc.Outputs.Where(variable => string.Equals(variable.Name, outputName)).Single();
                Variable outputVar = modelFunc.Output;

                var inputDataMap = new Dictionary<Variable, Value>();
                var outputDataMap = new Dictionary<Variable, Value>();

                // Image preprocessing to match input requirements of the model.
                // This program uses images from the CIFAR-10 dataset for evaluation.
                // Please see README.md in <CNTK>/Examples/Image/DataSets/CIFAR-10 about how to download the CIFAR-10 dataset.
                string sampleImage = "00000.png";
                
                Bitmap bmp = new Bitmap(Bitmap.FromFile(sampleImage));
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

                PrintOutput(outputVar.Shape.TotalSize, outputData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}\nCallStack: {1}\n Inner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException != null ? ex.InnerException.Message : "No Inner Exception");
                throw ex;
            }
        }

        /// <summary>
        /// Print out the evalaution results.
        /// </summary>
        /// <typeparam name="T">The data value type</typeparam>
        /// <param name="sampleSize">The size of each sample.</param>
        /// <param name="outputBuffer">The evaluation result data.</param>
        internal static void PrintOutput<T>(int sampleSize, IList<IList<T>> outputBuffer)
        {
            Console.WriteLine("The number of sequences in the batch: " + outputBuffer.Count);
            int seqNo = 0;
            int outputSampleSize = sampleSize;
            foreach (var seq in outputBuffer)
            {
                if (seq.Count % outputSampleSize != 0)
                {
                    throw new ApplicationException("The number of elements in the sequence is not a multiple of sample size");
                }

                Console.WriteLine($"Sequence {seqNo++} contains {seq.Count / outputSampleSize} samples.");
                int i = 0;
                int sampleNo = 0;
                foreach (var element in seq)
                {
                    if (i++ % outputSampleSize == 0)
                    {
                        Console.Write($"    sample {sampleNo}: ");
                    }
                    Console.Write(element);
                    if (i % outputSampleSize == 0)
                    {
                        Console.WriteLine(".");
                        sampleNo++;
                    }
                    else
                    {
                        Console.Write(",");
                    }
                }
            }
        }
    }
}
