using System.Collections.Generic;

namespace ComputerVision.Core.Models.CIFAR10
{
    public static class Cifar10
    {
        private static Dictionary<int, string> _labels ;

        public static Dictionary<int, string> Labels
        {
            get
            {
                _labels = new Dictionary<int, string>
                {
                    {1, "airplane"},
                    {2, "automobile"},
                    {3, "bird"},
                    {4, "cat"},
                    {5, "deer"},
                    {6, "dog"},
                    {7, "frog"},
                    {8, "horse"},
                    {9, "ship"},
                    {10, "truck"}
                };
                return _labels;
            }
        }
        
        
    }
}
