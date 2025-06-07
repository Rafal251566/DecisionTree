namespace DecisionTree
{
    //TO jest wewnetrzny wezeł z decyzjami
    public class InternalNode : IDecisionNode
    {
        public string FeatureName { get; set; }
        public float Threshold { get; set; }
        public IDecisionNode TrueChild { get; set; }
        public IDecisionNode FalseChild { get; set; }

        public InternalNode(string featureName, float threshold, IDecisionNode trueChild, IDecisionNode falseChild)
        {
            FeatureName = featureName;
            Threshold = threshold;
            TrueChild = trueChild;
            FalseChild = falseChild;
        }

        public string Predict(WineSample sample)
        {
            if (sample.GetFeatureValue(FeatureName) > Threshold)
            {
                return TrueChild.Predict(sample);
            }
            else
            {
                return FalseChild.Predict(sample);
            }
        }

        public void PrintTree(List<WineSample> data, string indent = "", bool isLast = true)
        {
            Console.Write(indent);
            Console.Write(isLast ? "└── " : "├── ");
            Console.WriteLine($"Test: {FeatureName} > {Threshold:F2} (Próbki: {data.Count})");

            var trueData = new List<WineSample>();
            var falseData = new List<WineSample>();

            foreach (var sample in data)
            {
                if (sample.GetFeatureValue(FeatureName) > Threshold)
                {
                    trueData.Add(sample);
                }
                else
                {
                    falseData.Add(sample);
                }
            }

            FalseChild.PrintTree(falseData, indent + (isLast ? "    " : "│   "), false);
            TrueChild.PrintTree(trueData, indent + (isLast ? "    " : "│   "), true);
        }

        public int CalculateError(List<WineSample> data)
        {
            int errors = 0;
            foreach (var sample in data)
            {
                if (Predict(sample) != sample.Cultivar)
                {
                    errors++;
                }
            }
            return errors;
        }
    }
}
