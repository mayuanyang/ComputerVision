using System.Collections.Generic;

namespace ComputerVision.Messages
{
    public class EvaluateImageQueryResult
    {
        public List<float> Outputs { get; set; }
        public int MatchingResultIndex { get; set; }

        public EvaluateImageQueryResult(List<float> outputs, int matchingResultIndex)
        {
            Outputs = outputs;
            MatchingResultIndex = matchingResultIndex;
        }
    }
}