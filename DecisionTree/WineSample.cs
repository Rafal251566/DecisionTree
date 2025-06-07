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