using System.IO;
using Enexure.MicroBus;

namespace ComputerVision.Messages
{
    public class EvaluateImageQuery : IQuery<EvaluateImageQuery, EvaluateImageQueryResult>
    {
        public string ModelFilePath { get; set; }
        public Stream ImageStream { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public EvaluateImageQuery(string modelFilePath, Stream imageStream, int height, int width)
        {
            ModelFilePath = modelFilePath;
            ImageStream = imageStream;
            Height = height;
            Width = width;
        }
    }
}
