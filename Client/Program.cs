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

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(await htfHttpClient.PostTeamMoveAsync(new UpdatePositionDto {Angle = 270, Speed = SpeedDto.Fast}));   
}
