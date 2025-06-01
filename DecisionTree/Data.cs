using System;
using System.Collections.Generic;
using System.Linq;

public class WineSample
{
    public float Alcohol { get; set; }
    public float MalicAcid { get; set; }
    public float Ash { get; set; }
    public float AlkalinityOfAsh { get; set; }
    public float Magnesium { get; set; }
    public float TotalPhenols { get; set; }
    public float Flavanoids { get; set; }
    public float NonflavanoidPhenols { get; set; }
    public float Proanthocyanins { get; set; }
    public float ColorIntensity { get; set; }
    public float Hue { get; set; }
    public float OD280_OD315_of_DilutedWines { get; set; }
    public float Proline { get; set; }
    public string Cultivar { get; set; }

    public float GetFeatureValue(string featureName)
    {
        return featureName switch
        {
            "Alcohol" => Alcohol,
            "MalicAcid" => MalicAcid,
            "Ash" => Ash,
            "AlkalinityOfAsh" => AlkalinityOfAsh,
            "Magnesium" => Magnesium,
            "TotalPhenols" => TotalPhenols,
            "Flavanoids" => Flavanoids,
            "NonflavanoidPhenols" => NonflavanoidPhenols,
            "Proanthocyanins" => Proanthocyanins,
            "ColorIntensity" => ColorIntensity,
            "Hue" => Hue,
            "OD280_OD315_of_DilutedWines" => OD280_OD315_of_DilutedWines,
            "Proline" => Proline,
            _ => throw new ArgumentException($"Nieznana cecha: {featureName}")
        };
    }
}

//Interfejsy bo CÓŻ SZKODZI ZROBIĆ
public interface ISplitCriterion
{
    double Calculate(List<WineSample> trueSet, List<WineSample> falseSet);
}

public interface IDecisionNode
{
    string Predict(WineSample sample);
    void PrintTree(string indent = "", bool isLast = true);
}

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

    public void PrintTree(string indent = "", bool isLast = true)
    {
        Console.Write(indent);
        Console.Write(isLast ? "└── " : "├── ");  // UWAGA ten pierwszy kikut to PRAWDA a ten drugi to FAŁSZ
        Console.WriteLine($"Kultywar: {Cultivar}");
    }
}

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

    public void PrintTree(string indent = "", bool isLast = true)
    {
        Console.Write(indent);
        Console.Write(isLast ? "└── " : "├── "); // UWAGA ten pierwszy kikut to PRAWDA a ten drugi to FAŁSZ
        Console.WriteLine($"Test: {FeatureName} > {Threshold:F2}");

        FalseChild.PrintTree(indent + (isLast ? "    " : "│   "), false);
        TrueChild.PrintTree(indent + (isLast ? "    " : "│   "), true); 
    }
}