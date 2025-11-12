// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using Client.HttpClient;
using HTF2025_Client.Dto.Team;

var htfUrl = "https://involvedhtf2025.azurewebsites.net/";
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(htfUrl);

var htfHttpClient = new HackTheFutureClient(httpClient);

var tokenResponse = await htfHttpClient.GetTeamTokenAsync("TheOutdoorBytes", "712d9a038f3345aebe97f183706b0392");
var token = await tokenResponse.Content.ReadFromJsonAsync<GetTokenDto>();

htfHttpClient.SetToken(token);

await htfHttpClient.StartThermalLinkAsync();

var dto = await htfHttpClient.GetThermalLinkSampleAsync();
Console.WriteLine(dto.XorKey);
Console.WriteLine(dto.CurrentLocation.X);
Console.WriteLine(dto.CurrentLocation.Y);
var gridSize = 10;

await htfHttpClient.PostThermalLinkSampleMoveAsync("N2");
await htfHttpClient.PostThermalLinkSampleMoveAsync("W2");
var scanResult = "";

var translateTable = new List<(int min, int max, char value)>
{
    (0, 63, '.'),
    (64, 127, 'R'),
    (128, 191, 'A'),
    (192, 254, 'T'),
    (255, 255, '-')
};

char[,] fullGrid = new char[gridSize, gridSize];

int chunkSize = 5;
int chunksPerRow = gridSize / chunkSize;
int chunkIndex = 0;

for (int i = 0; i < chunksPerRow; i++)
{
    for (int j = 0; j < chunksPerRow - 1; j++)
    {
        scanResult += (await htfHttpClient.GetThermalLinkSampleScanAsync()).ScanResult;
        if (i % 2 == 0)
        {
            await htfHttpClient.PostThermalLinkSampleMoveAsync("E5");
        }
        else
        {
            await htfHttpClient.PostThermalLinkSampleMoveAsync("W5");
        }
    }

    scanResult += (await htfHttpClient.GetThermalLinkSampleScanAsync()).ScanResult;

    var output = "";
    for (var k = 0; k < scanResult.Length; k += 8)
    {
        var value = Convert.ToInt32(scanResult.Substring(k, 8), 2) ^ Convert.ToInt32(dto.XorKey, 2);
        foreach (var translate in translateTable)
        {
            if (translate.min <= value && value <= translate.max)
            {
                output += translate.value;
            }
        }
    }

    Console.WriteLine(output);
    Console.WriteLine("==============================================");

    var rowCount = 5 * i;
    var columnCount = 0;
    for (int j = 0; j < output.Length; j++)
    {
        if (output[j] == '-')
        {
            rowCount++;
            if (rowCount % 5 == 0)
            {
                rowCount = 5 * i;
            }
            if (j % 5 == 0)
            {
                columnCount = 5 * (j / 29);
            }
            else
            {
                columnCount = 0;
            }
        }
        else
        {
            fullGrid[rowCount, columnCount++] = output[j];
        }
    }

    scanResult = "";
    await htfHttpClient.PostThermalLinkSampleMoveAsync("S5");
}

for (int r = 0; r < gridSize; r++)
{
    for (int c = 0; c < gridSize; c++)
        Console.Write(fullGrid[r, c]);
    Console.WriteLine();
}