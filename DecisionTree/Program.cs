using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DecisionTree;

public class Program
{
    //REKORD NAUKI 97,22% dla GINIEGO a srednia to na oko 90%  a dla reszty 100% max i tez 90% sredniak
    public static void Main(string[] args)
    {
        string filePath = "wine.txt";
        List<WineSample> allWineData;

        try
        {
            allWineData = LoadWineDataFromFile(filePath);
            if (!allWineData.Any())
            {
                Console.WriteLine($"Plik '{filePath}' jest pusty lub nie można go wczytać.");
                return;
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Plik '{filePath}' nie został znaleziony.");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas wczytywania danych");
            return;
        }

        List<string> featureNames = new List<string>
        {
            "Alcohol", "MalicAcid", "Ash", "AlkalinityOfAsh", "Magnesium",
            "TotalPhenols", "Flavanoids", "NonflavanoidPhenols", "Proanthocyanins",
            "ColorIntensity", "Hue", "OD280_OD315_of_DilutedWines", "Proline"
        };


        var random = new Random();
        var shuffledData = allWineData.OrderBy(x => random.Next()).ToList();

        int trainingSize = (int)(allWineData.Count * 0.8); //daje klasyczne 80% na terning
        List<WineSample> trainingData = shuffledData.Take(trainingSize).ToList();
        List<WineSample> testData = shuffledData.Skip(trainingSize).ToList();

        while (true)
        {
            Console.WriteLine("Wybierz algorytm budowy drzewa:");
            Console.WriteLine("1. Gini");
            Console.WriteLine("2. Entropia");
            Console.WriteLine("3. C4.5");
            Console.WriteLine("0. Wyjście");
            Console.Write("Twój wybór: ");
            var input = Console.ReadLine();

            if (input == "0")
            {
                Console.WriteLine("Zamykanie programu...");
                break;
            }

            ISplitCriterion? criterion = input switch
            {
                "1" => new GiniCriterion(),
                "2" => new EntropyCriterion(),
                "3" => new C45Criterion(),
                _ => null
            };

            if (criterion == null) continue;

            Console.WriteLine($"Całkowita liczba próbek: {allWineData.Count}");
            Console.WriteLine($"Próbek treningowych: {trainingData.Count}");
            Console.WriteLine($"Próbek testowych: {testData.Count}");
            Console.WriteLine("------------------------------------------");

            var trainer = new DecisionTreeTrainer(5,4, criterion);
            IDecisionNode trainedTree = trainer.BuildTree(trainingData, featureNames);

            Console.WriteLine("Trening zakończony.");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Ocena na danych testowych:");

            EvaluateTree(trainedTree, testData);

            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Struktura drzewa decyzyjnego (└── = PRAWDA , ├── = FAŁSZ)");
            trainedTree.PrintTree();
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("Naciśnij Enter, aby kontynuować, lub '0' aby zakończyć.");
            if (Console.ReadLine() == "0") break;


            //TODO może umozliwic uzytkownikowi jescze dodanie na koncu nowego Sampla na testa ?? ale nie wiem czy ma to sens
        }
    }

    public static List<WineSample> LoadWineDataFromFile(string filePath)
    {
        var data = new List<WineSample>();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var values = line.Split(',');

            if (values.Length != 14)
            {
                Console.WriteLine($"Linia ma nieprawidłową liczbę kolumn i zostanie pominięta: {line}");
                continue;
            }

            try
            {
                data.Add(new WineSample
                {
                    Cultivar = values[0].Trim(),
                    Alcohol = float.Parse(values[1].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    MalicAcid = float.Parse(values[2].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    Ash = float.Parse(values[3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    AlkalinityOfAsh = float.Parse(values[4].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    Magnesium = float.Parse(values[5].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    TotalPhenols = float.Parse(values[6].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    Flavanoids = float.Parse(values[7].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    NonflavanoidPhenols = float.Parse(values[8].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    Proanthocyanins = float.Parse(values[9].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    ColorIntensity = float.Parse(values[10].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    Hue = float.Parse(values[11].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    OD280_OD315_of_DilutedWines = float.Parse(values[12].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    Proline = float.Parse(values[13].Trim(), System.Globalization.CultureInfo.InvariantCulture)
                });
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Błąd prasowania wartości w linii: {line}");
            }
        }

        return data;
    }

    public static void EvaluateTree(IDecisionNode tree, List<WineSample> testData)
    {
        if (!testData.Any())
        {
            Console.WriteLine("Brak danych testowych do oceny.");
            return;
        }

        int correctPredictions = 0;
        var allCultivars = testData.Select(s => s.Cultivar).Distinct().OrderBy(c => c).ToList();
        var confusionMatrix = new Dictionary<string, Dictionary<string, int>>();

        foreach (var cultivar1 in allCultivars)
        {
            confusionMatrix[cultivar1] = new Dictionary<string, int>();
            foreach (var cultivar2 in allCultivars)
            {
                confusionMatrix[cultivar1][cultivar2] = 0;
            }
        }

        foreach (var sample in testData)
        {
            string predicted = tree.Predict(sample);
            string actual = sample.Cultivar;

            confusionMatrix[actual][predicted]++;

            if (predicted == actual)
            {
                correctPredictions++;
            }
        }

        double accuracy = (double)correctPredictions / testData.Count;
        Console.WriteLine($"Dokładność: {accuracy:P2}");

        Console.WriteLine("\nConfusion Matrix:");
        Console.Write("Actual\\Predicted\t");
        foreach (var cultivar in allCultivars)
        {
            Console.Write($"{cultivar}\t        ");
        }
        Console.WriteLine();

        foreach (var actualCultivar in allCultivars)
        {
            Console.Write($" \t{actualCultivar}\t\t");
            foreach (var predictedCultivar in allCultivars)
            {
                Console.Write($"{confusionMatrix[actualCultivar][predictedCultivar]}\t\t");
            }
            Console.WriteLine();
        }
    }
}