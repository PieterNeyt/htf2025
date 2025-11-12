using System.Net.Http.Json;
using Client.HttpClient;
using HTF2025_Client.Dto.Team;
using HTF2025_Client.Dto.TheHarmonicInscription;

var htfUrl = "https://involvedhtf2025.azurewebsites.net/";
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(htfUrl);

var htfHttpClient = new HackTheFutureClient(httpClient);

var tokenResponse = await htfHttpClient.GetTeamTokenAsync("TheOutdoorBytes", "712d9a038f3345aebe97f183706b0392");
var token = await tokenResponse.Content.ReadFromJsonAsync<GetTokenDto>();

htfHttpClient.SetToken(token);

Console.WriteLine(await htfHttpClient.StartTheHarmonicInscriptionAsync());

FrequencyMapDto frequencyMaps = (await htfHttpClient.GetTheHarmonicInscriptionFrequencyMapAsync());
Dictionary<string, int> frequencyMap = frequencyMaps.FrequencyMap;

foreach (var item in frequencyMap)
{
    Console.WriteLine($"{item.Key}: {item.Value}");
}

TheHarmonicInscriptionDto puzzle = await htfHttpClient.GetTheHarmonicInscriptionPuzzleAsync();
List<string> inscriptions = puzzle.ResonanceStrings;
Console.WriteLine(inscriptions.Count);
foreach (var item in inscriptions)
{
    Console.WriteLine(item);
}

var encodedResults = DecodeInscriptions(inscriptions, frequencyMap);
Console.WriteLine(await htfHttpClient.PostTheHarmonicInscriptionPuzzleAsync(new TheHarmonicInscriptionSolutionDto()
{
    HarmonicResponse = encodedResults
}));

int CalculateResonence(int[,] pairsFrequency)
{
    var pair1 = (pairsFrequency[0, 0] * 33) + pairsFrequency[0, 1];
    var pair2 = (pairsFrequency[1, 0] * 33) + pairsFrequency[1, 1];
    return pair1 + pair2;
}


List<string> DecodeInscriptions(List<string> inscriptions, Dictionary<string, int> frequencyMap)
{
    List<string> encodedResults = new List<string>();
    foreach (var inscriptionPair in inscriptions)
    {
        var pairs = inscriptionPair.Split(' ');
        int[,] pairsFrequency = new int[pairs.Length, 2];
        int index = 0;
        foreach (var pair in pairs)
        {
            var parts = pair.Split('S');

            int lValue = int.Parse(parts[0].Substring(1));
            int sValue = int.Parse(parts[1]);

            int lMapped = GetFrequencyMapValue(lValue, frequencyMap);
            int sMapped = GetFrequencyMapValue(sValue, frequencyMap);

            Console.WriteLine($"{pair} → L={lMapped}, S={sMapped}");
            pairsFrequency[index, 0] = lMapped;
            pairsFrequency[index, 1] = sMapped;
            index++;
        }

        var totalPairResonence = CalculateResonence(pairsFrequency);
        var harmonicPairValues = GetHarmonicValues(totalPairResonence);
        var encodedString = EncodeHarmonicValues(harmonicPairValues, frequencyMap);
        encodedResults.Add(encodedString);
    }

    return encodedResults;
}


string EncodeHarmonicValues(int[,] harmonicValues, Dictionary<string, int> frequencyRanges)
{
    int lNumber = GetNumberFromCategory(harmonicValues[0, 0], frequencyRanges);
    int sNumber = GetNumberFromCategory(harmonicValues[0, 1], frequencyRanges);

    return $"L{lNumber}S{sNumber}";
}


int GetNumberFromCategory(int category, Dictionary<string, int> frequencyMap)
{
    Random random = new Random();
    var rangeKey = frequencyMap.FirstOrDefault(kvp => kvp.Value == category).Key;

    var parts = rangeKey.Split('-');
    int min = int.Parse(parts[0]);
    int max = int.Parse(parts[1]);

    return random.Next(min, max + 1);
}


int[,] GetHarmonicValues(int totalPairResonence)
{
    int loudValue = totalPairResonence / 33;
    int softValue = totalPairResonence % 33;
    return new int[,] { { loudValue, softValue } };
}

int GetFrequencyMapValue(int value, Dictionary<string, int> frequencyMap)
{
    foreach (var range in frequencyMap)
    {
        var parts = range.Key.Split('-');
        int min = int.Parse(parts[0]);
        int max = int.Parse(parts[1]);
        if (value >= min && value <= max)
        {
            return range.Value;
        }
    }

    return -1;
}