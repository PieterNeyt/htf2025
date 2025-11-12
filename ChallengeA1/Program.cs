// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using Client.HttpClient;
using HTF2025_Client.Dto.DNAStrandValidator;
using HTF2025_Client.Dto.Team;

var htfUrl = "https://involvedhtf2025.azurewebsites.net/";
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(htfUrl);

var htfHttpClient = new HackTheFutureClient(httpClient);

var tokenResponse = await htfHttpClient.GetTeamTokenAsync("TheOutdoorBytes", "712d9a038f3345aebe97f183706b0392");
var token = await tokenResponse.Content.ReadFromJsonAsync<GetTokenDto>();

htfHttpClient.SetToken(token);


Console.WriteLine(await htfHttpClient.StartDnaStrandValidatorAsync());

DNAStrandValidatorDto sample = await htfHttpClient.GetDnaStrandValidatorPuzzleAsync();

var validStrands = ValidateStrands(sample.DNAStrands);

int totalValue = 0;
foreach (var strand in validStrands)
{
    totalValue += CalculateStrandValue(strand);
}

Console.WriteLine(totalValue);
Console.WriteLine(await htfHttpClient.PostDnaStrandValidatorPuzzleAsync(new DnaStrandValidatorSolutionDto()
    { TotalBiologicalValue = totalValue }));

int CalculateStrandValue(string strand)
{
    int strandValue = 0;
    char previous = '\0';
    int count = 1;

    foreach (var nucleotide in strand)
    {
        if (nucleotide == previous)
        {
            count++;
        }
        else
        {
            count = 1;
            previous = nucleotide;
        }

        int baseValue = nucleotide switch
        {
            'A' => 1,
            'T' => 2,
            'C' => 3,
            'G' => 4,
            _ => 0
        };

        strandValue += baseValue * count;
    }

    Console.WriteLine($"Strand: {strand}, Value: {strandValue}");
    return strandValue;
}

List<string> ValidateStrands(List<string> strands)
{
    var validStrands = new List<string>();

    foreach (var strand in strands)
    {
        
        bool isValid = strand.Any(c => "ATCG".Contains(c));
        
        if (strand.Length % 2 != 0)
        {
            isValid = false;
        }

        
        if (isValid)
        {
            char previous = '\0';
            int count = 1;

            foreach (var ch in strand)
            {
                if (ch == previous)
                {
                    count++;
                    if (count > 3)
                    {
                        isValid = false;
                        break;
                    }
                }
                else
                {
                    count = 1;
                    previous = ch;
                }
            }
        }

        if (isValid)
            validStrands.Add(strand);
    }

    return validStrands;
}
