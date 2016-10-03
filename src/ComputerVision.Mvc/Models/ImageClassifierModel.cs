using System.Collections.Generic;

namespace ComputerVision.Mvc.Models
{
    public class ImageClassifierModel
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string ImageData { get; set; }
        public List<float> Output { get; set; }
        public ImageClassifierModel(string category, string description, string imageData, List<float> output)
        {
            Category = category;
            Description = description;
            ImageData = imageData;
            Output = output;
        }
    }
}