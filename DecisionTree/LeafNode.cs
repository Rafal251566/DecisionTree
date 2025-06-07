namespace DecisionTree
{
    //To jest ten ostatni wezeł co juz daje wynik
    public class LeafNode : IDecisionNode
    {
        public string Cultivar { get; set; }

        public LeafNode(string cultivar)
        {
            Cultivar = cultivar;
        }

        public string Predict(WineSample sample)
        {
            return Cultivar;
        }

        public void PrintTree(List<WineSample> data, string indent = "", bool isLast = true)
        {

            //ZWYKLE WYSWIETLANIE

            Console.Write(indent);
            Console.Write(isLast ? "└── " : "├── ");  // UWAGA ten pierwszy kikut to PRAWDA a ten drugi to FAŁSZ
            Console.WriteLine($"Kultywar: {Cultivar}");


            //SZCZEGÓŁOWE WYSWIETLANIE 

            //Console.Write(indent);
            //Console.Write(isLast ? "└── " : "├── ");
            //var cultivarCounts = data.GroupBy(s => s.Cultivar).OrderBy(g => g.Key).Select(g => $"{g.Key}: {g.Count()}").ToList();
            //Console.WriteLine($"Kultywar: {Cultivar} (Łącznie próbek: {data.Count}) - Rozkład: [{string.Join(", ", cultivarCounts)}]");
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
