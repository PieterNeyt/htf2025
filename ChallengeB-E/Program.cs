// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using Client.Dto.DriftScan;
using Client.HttpClient;
using HTF2025_Client.Dto.DriftScan;
using HTF2025_Client.Dto.Team;

var htfUrl = "https://involvedhtf2025.azurewebsites.net/";
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(htfUrl);

var htfHttpClient = new HackTheFutureClient(httpClient);

var tokenResponse = await htfHttpClient.GetTeamTokenAsync("TheOutdoorBytes", "712d9a038f3345aebe97f183706b0392");
var token = await tokenResponse.Content.ReadFromJsonAsync<GetTokenDto>();

htfHttpClient.SetToken(token);

await htfHttpClient.StartDriftScanAsync();
var response = await htfHttpClient.GetDriftScanPuzzleAsync();
var binary = response!.BinaryData;
Console.WriteLine(binary);
var key = Convert.ToInt32(response.XorKey, 2);
Console.WriteLine(key);
var output = new List<string>();

var translateTable = new List<(int min, int max, string value)>
{
    (0, 63, "."),
    (64, 127, "R"),
    (128, 191, "A"),
    (192, 254, "T"),
    (255, 255, "-")
};

for (var i = 0; i < binary.Length; i += 8)
{
    var value = Convert.ToInt32(binary.Substring(i, 8), 2) ^ key;
    foreach (var translate in translateTable)
    {
        if (translate.min <= value && value <= translate.max)
        {
            output.Add(translate.value);   
        }
    }
}

List<List<string>> groups = new();
List<string> currentGroup = new();
foreach (var item in output)
{
    if (item.Equals(translateTable[4].value))
    {
        groups.Add(currentGroup);
        currentGroup = new List<string>();
    }
    else
    {
        currentGroup.Add(item);
    }
}

Console.WriteLine(await htfHttpClient.PostDriftScanPuzzleAsync(new DriftScanSolutionDto
    { Map = groups.Select(group => group.ToArray()).ToArray() }));