// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using Client.HttpClient;
using HTF2025_Client.Dto.AbyssalSurvey;
using HTF2025_Client.Dto.Team;

var htfUrl = "https://involvedhtf2025.azurewebsites.net/";
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(htfUrl);

var htfHttpClient = new HackTheFutureClient(httpClient);

var tokenResponse = await htfHttpClient.GetTeamTokenAsync("TheOutdoorBytes", "712d9a038f3345aebe97f183706b0392");
var token = await tokenResponse.Content.ReadFromJsonAsync<GetTokenDto>();

htfHttpClient.SetToken(token);

await htfHttpClient.StartAbyssalSurveyAsync();

var dto = await htfHttpClient.GetAbyssalSurveyPuzzleAsync();
Console.WriteLine(dto.XorKey);
Console.WriteLine(dto.CurrentLocation.X);
Console.WriteLine(dto.CurrentLocation.Y);
Console.WriteLine(dto.GridSize);
Console.WriteLine();

await htfHttpClient.PostAbyssalSurveyPuzzleMoveAsync("N47");
await htfHttpClient.PostAbyssalSurveyPuzzleMoveAsync("W47");
var scanResult = "";

for (int i = 0; i < dto.GridSize / 5; i++)
{
    for (int j = 0; j < dto.GridSize / 5 - 1; j++)
    {
        scanResult += (await htfHttpClient.GetAbyssalSurveyPuzzleScanAsync()).ScanResult;
        if (i % 2 == 0)
        {
            await htfHttpClient.PostAbyssalSurveyPuzzleMoveAsync("E5");
        }
        else
        {
            await htfHttpClient.PostAbyssalSurveyPuzzleMoveAsync("W5");
        }
    }

    scanResult += (await htfHttpClient.GetAbyssalSurveyPuzzleScanAsync()).ScanResult;
    await htfHttpClient.PostAbyssalSurveyPuzzleMoveAsync("S5");
}

var postDto = new AbyssalSurveySolutionDto { TotalScienceValue = 0 };
var translateTable = new List<(int min, int max, string value)>
{
    (0, 63, "."),
    (64, 127, "R"),
    (128, 191, "A"),
    (192, 254, "T"),
    (255, 255, "-")
};
var output = new List<string>();
for (var i = 0; i < scanResult.Length; i += 8)
{
    var value = Convert.ToInt32(scanResult.Substring(i, 8), 2) ^ Convert.ToInt32(dto.XorKey, 2);
    foreach (var translate in translateTable)
    {
        if (translate.min <= value && value <= translate.max)
        {
            output.Add(translate.value);
        }
    }
}

var translatePoints = new Dictionary<string, int>
{
    { ".", 0 },
    { "R", 13 },
    { "T", 49 },
    { "A", 269 },
    { "-", 0 }
};

foreach (var item in output)
{
    postDto.TotalScienceValue += translatePoints[item];
}

Console.WriteLine(postDto.TotalScienceValue);

Console.WriteLine(await htfHttpClient.PostAbyssalSurveyPuzzleAsync(postDto));