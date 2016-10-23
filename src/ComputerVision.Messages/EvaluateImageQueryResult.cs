using System.Collections.Generic;

namespace ComputerVision.Messages
{
    public class EvaluateImageQueryResult
    {
        public Dictionary<string, float> Outputs { get; set; }
        public int MatchingResultIndex { get; set; }

        public EvaluateImageQueryResult(Dictionary<string, float> outputs, int matchingResultIndex)
        {
            Outputs = outputs;
            MatchingResultIndex = matchingResultIndex;
        }
    }
}