using System.Collections.Generic;

namespace ComputerVision.Mvc.Models
{
    public class ImageClassifierModel
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string ImageData { get; set; }
        public Dictionary<string, float> Output { get; set; }
        public ImageClassifierModel(string category, string description, string imageData, Dictionary<string, float> output)
        {
            Category = category;
            Description = description;
            ImageData = imageData;
            Output = output;
        }
    }
}