using System.IO;
using Mediator.Net.Contracts;

namespace ComputerVision.Messages
{
    public class EvaluateImageQuery : IRequest
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
