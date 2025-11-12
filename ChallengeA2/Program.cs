using System.Net.Http.Json;
using System.Text;
using Client.HttpClient;
using HTF2025_Client.Dto.AbyssalLifeformClassifier;
using HTF2025_Client.Dto.DNAStrandValidator;
using HTF2025_Client.Dto.Team;

var htfUrl = "https://involvedhtf2025.azurewebsites.net/";
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(htfUrl);

var htfHttpClient = new HackTheFutureClient(httpClient);

var tokenResponse = await htfHttpClient.GetTeamTokenAsync("TheOutdoorBytes", "712d9a038f3345aebe97f183706b0392");
var token = await tokenResponse.Content.ReadFromJsonAsync<GetTokenDto>();

htfHttpClient.SetToken(token);
Console.WriteLine(await htfHttpClient.StartAbyssalLifeformClassifierAsync());

LifeformCategoryConfig configs = await htfHttpClient.GetAbyssalLifeformClassifierConfigAsync();
List<LifeformCategory> categories = configs.Categories;

AbyssalLifeformClassifierDto classifierDto = await htfHttpClient.GetAbyssalLifeformClassifierPuzzleAsync();
List<Lifeform> lifeforms = classifierDto.Lifeforms;

Dictionary<string, int> spectralResonance = ClassifyLifeforms(lifeforms, categories);

Console.WriteLine(generateReturnString(spectralResonance));
Console.WriteLine(await htfHttpClient.PostAbyssalLifeformClassifierPuzzleAsync(
    new AbyssalLifeFormClassifierSolutionDto()
        { SpectralResonance = generateReturnString(spectralResonance) }));

static Dictionary<string, int> ClassifyLifeforms(List<Lifeform> lifeforms, List<LifeformCategory> categories)
{
    var spectralResonance = categories.ToDictionary(c => c.Name, c => 0);

    foreach (var lifeform in lifeforms)
    {
        var categoryScores = new Dictionary<string, decimal>();

        foreach (var category in categories)
        {
            decimal score = 0;
            foreach (var rule in category.Rules)
            {
                var trait = rule.Key.ToLower();
                var rules = rule.Value;

                if (trait.Equals("bioluminescencefrequency"))
                {
                    if (rule.Value.Min.HasValue && rule.Value.Max.HasValue)
                    {
                        if (lifeform.BioluminescenceFrequency >= rule.Value.Min  &&
                            lifeform.BioluminescenceFrequency <= rule.Value.Max )

                        {
                            score += rules.Weight;
                        }
                    }
                }
                else if (trait.Equals("movementpattern") &&
                         lifeform.MovementPattern.Equals(rules.Value, StringComparison.OrdinalIgnoreCase))
                {
                    score += rules.Weight;
                }
                else if (trait.Equals("thermalsignature") &&
                         lifeform.ThermalSignature.Equals(rules.Value, StringComparison.OrdinalIgnoreCase))
                {
                    score += rules.Weight;
                }
                else if (trait.Equals("sizeestimate") &&
                         lifeform.SizeEstimate.Equals(rules.Value, StringComparison.OrdinalIgnoreCase))
                {
                    score += rules.Weight;
                }
            }

            categoryScores[category.Name] = score;
        }

        var best = categoryScores
            .OrderByDescending(c => c.Value)
            .First();

        if (best.Value <= 0.1m)
            continue;

        var cat = categories.First(c => c.Name == best.Key);
        spectralResonance[cat.Name] += cat.SpectralWeight;
    }

    return spectralResonance;
}

string generateReturnString(Dictionary<string, int> spectralResonance)
{
    var bestCategory = spectralResonance.OrderByDescending(x => x.Value)
        .ThenBy(x => x.Key)
        .First();

    var totalSum = spectralResonance.Values.Sum();
    return $"{bestCategory.Key}_{bestCategory.Value}_{totalSum}";
}