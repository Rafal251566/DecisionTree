public class DecisionTreeTrainer
{
    private readonly int maxDepth;
    private readonly int minSamplesSplit;
    private readonly ISplitCriterion criterion;


    public DecisionTreeTrainer(int maximumDepth, int minimumSamplesSplit, ISplitCriterion criterionType)
    {
        maxDepth = maximumDepth;
        minSamplesSplit = minimumSamplesSplit;
        criterion = criterionType;
    }

    public IDecisionNode BuildTree(List<WineSample> data, List<string> features, int currentDepth = 0)
    {
        //najpierw warunki stopu sprawdzamy cn 
        if (data.All(s => s.Cultivar == data.First().Cultivar))
        {
            return new LeafNode(data.First().Cultivar);
        }

        if (currentDepth >= maxDepth)
        {
            return new LeafNode(GetMajorityCultivar(data));
        }

        if (data.Count < minSamplesSplit)
        {
            return new LeafNode(GetMajorityCultivar(data));
        }

        if (!features.Any())
        {
            return new LeafNode(GetMajorityCultivar(data));
        }

        string bestFeature = null;
        float bestThreshold = 0;
        double minScore = double.MaxValue;

        foreach (var feature in features)
        {
            var uniqueValues = data.Select(s => s.GetFeatureValue(feature)).Distinct().OrderBy(v => v).ToList();

            for (int i = 0; i < uniqueValues.Count - 1; i++)
            {
                float threshold = (uniqueValues[i] + uniqueValues[i + 1]) / 2.0f;
                var (trueSet, falseSet) = SplitData(data, feature, threshold);

                if (trueSet.Any() && falseSet.Any())
                {
                    double currentScore = criterion.Calculate(trueSet, falseSet);

                    if (currentScore < minScore)
                    {
                        minScore = currentScore;
                        bestFeature = feature;
                        bestThreshold = threshold;
                    }
                }
            }
        }

        if (bestFeature == null)
        {
            return new LeafNode(GetMajorityCultivar(data));
        }

        var (trueData, falseData) = SplitData(data, bestFeature, bestThreshold);

        IDecisionNode trueChild = BuildTree(trueData, features, currentDepth + 1);
        IDecisionNode falseChild = BuildTree(falseData, features, currentDepth + 1);

        return new InternalNode(bestFeature, bestThreshold, trueChild, falseChild);
    }

    public IDecisionNode PruneTree(IDecisionNode currentNode, List<WineSample> validationDataForThisNode)
    {
        if (currentNode is LeafNode)
        {
            return currentNode;
        }

        if (currentNode is InternalNode internalNode)
        {
            var (trueValSet, falseValSet) = SplitData(validationDataForThisNode, internalNode.FeatureName, internalNode.Threshold);

            internalNode.TrueChild = PruneTree(internalNode.TrueChild, trueValSet);
            internalNode.FalseChild = PruneTree(internalNode.FalseChild, falseValSet);


            int errorIfKept = internalNode.CalculateError(validationDataForThisNode);

            string majorityClass = GetMajorityCultivar(validationDataForThisNode);
            LeafNode candidateLeaf = new LeafNode(majorityClass);

            int errorIfPruned = candidateLeaf.CalculateError(validationDataForThisNode);

            if (errorIfPruned < errorIfKept)
            {
                Console.WriteLine($"Przycęto węzeł '{internalNode.FeatureName} > {internalNode.Threshold:F2}'. Błąd poddrzewa przed: {errorIfKept}, błąd po przycięciu: {errorIfPruned}.");
                return candidateLeaf;
            }
        }
        return currentNode;
    }


    private (List<WineSample> trueSet, List<WineSample> falseSet) SplitData(List<WineSample> data, string featureName, float threshold)
    {
        var trueSet = new List<WineSample>();
        var falseSet = new List<WineSample>();

        foreach (var sample in data)
        {
            if (sample.GetFeatureValue(featureName) > threshold)
            {
                trueSet.Add(sample);
            }
            else
            {
                falseSet.Add(sample);
            }
        }
        return (trueSet, falseSet);
    }

    private string GetMajorityCultivar(List<WineSample> data)
    {
        if (!data.Any()) return null;

        return data.GroupBy(s => s.Cultivar).OrderByDescending(g => g.Count()).First().Key;
    }
}