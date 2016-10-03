using System.IO;
using Enexure.MicroBus;

namespace ComputerVision.Messages
{
    public class EvaluateImageQuery : IQuery<EvaluateImageQuery, EvaluateImageQueryResult>
    {
        public string ModelFilePath { get; set; }
        public Stream ImageStream { get; set; }

        public EvaluateImageQuery(string modelFilePath, Stream imageStream)
        {
            ModelFilePath = modelFilePath;
            ImageStream = imageStream;
        }
    }
}
