using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{

    //TO CHYUBA NIE MA SENSU?  WYCHODZI NA TO ZE MOZE JEDNAK MA BO OSIAGA NAJLEPSZE WYNIKI 
    public class EntropyCriterion : ISplitCriterion
    {
        private double Entropy(List<WineSample> data)
        {
            if (!data.Any()) return 0.0;

            var total = data.Count;
            var counts = data.GroupBy(s => s.Cultivar).Select(g => g.Count());
            double entropy = 0.0;

            foreach (var count in counts)
            {
                double p = (double)count / total;
                entropy -= p * Math.Log2(p);
            }
            return entropy;
        }

        public double Calculate(List<WineSample> trueSet, List<WineSample> falseSet)
        {
            int total = trueSet.Count + falseSet.Count;
            return (trueSet.Count * Entropy(trueSet) + falseSet.Count * Entropy(falseSet)) / total;
        }
    }
}
