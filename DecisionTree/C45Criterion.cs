using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    //TODO Pruning drzewa moze by trzeba zrobic ale jeszcze sie zastanowie

    public class C45Criterion : ISplitCriterion
    {
        private double Entropy(List<WineSample> data)
        {
            if (!data.Any()) return 0.0;

            var totalSamples = data.Count;
            var counts = data.GroupBy(s => s.Cultivar).Select(g => g.Count());
            double entropy = 0.0;

            foreach (var count in counts)
            {
                double p = (double)count / totalSamples;
                entropy -= p * Math.Log2(p);
            }
            return entropy;
        }

        public double Calculate(List<WineSample> trueSet, List<WineSample> falseSet)
        {
            int total = trueSet.Count + falseSet.Count;
            if (total == 0) return 0;

            double entropyBefore = Entropy(trueSet.Concat(falseSet).ToList());
            double entropyAfter = (trueSet.Count * Entropy(trueSet) + falseSet.Count * Entropy(falseSet)) / total;

            double infoGain = entropyBefore - entropyAfter;

            double pTrue = (double)trueSet.Count / total;
            double pFalse = (double)falseSet.Count / total;
            double splitInfo = 0.0;

            if (pTrue > 0) splitInfo -= pTrue * Math.Log2(pTrue);
            if (pFalse > 0) splitInfo -= pFalse * Math.Log2(pFalse);

            if (splitInfo == 0) return double.MaxValue;
            return 1.0 / (infoGain / splitInfo);
        }
    }
}
