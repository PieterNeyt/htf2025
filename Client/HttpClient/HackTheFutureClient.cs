using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Client.Dto.DriftScan;
using Client.Dto.Team;
using HTF2025_Client.Dto.AbyssalExpedition;
using HTF2025_Client.Dto.AbyssalLifeformClassifier;
using HTF2025_Client.Dto.AbyssalSurvey;
using HTF2025_Client.Dto.DNAStrandValidator;
using HTF2025_Client.Dto.DriftScan;
using HTF2025_Client.Dto.Team;
using HTF2025_Client.Dto.TheHarmonicInscription;
using HTF2025_Client.Dto.ThermaLink;

namespace Client.HttpClient;

public class HackTheFutureClient(System.Net.Http.HttpClient httpClient)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private async Task<T?> GetAsync<T>(string url)
    {
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    private async Task<HttpResponseMessage> PostAsync<T>(string url, T body)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        return response;
    }
    
    // Set token
    public void SetToken(GetTokenDto? tokenDto)
    {
        if (tokenDto is not null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.Token);
        }
    }

    // ---------------------------
    // A-1 Easy: DNA Strand Validator
    // ---------------------------
    public Task<HttpResponseMessage> StartDnaStrandValidatorAsync() =>
        httpClient.GetAsync("/api/a/easy/start");

    public Task<DNAStrandValidatorDto?> GetDnaStrandValidatorSampleAsync() =>
        GetAsync<DNAStrandValidatorDto>("/api/a/easy/sample");

    public Task<HttpResponseMessage> PostDnaStrandValidatorSampleAsync(DnaStrandValidatorSolutionDto solution) =>
        PostAsync("/api/a/easy/sample", solution);

    public Task<DNAStrandValidatorDto?> GetDnaStrandValidatorPuzzleAsync() =>
        GetAsync<DNAStrandValidatorDto>("/api/a/easy/puzzle");

    public Task<HttpResponseMessage> PostDnaStrandValidatorPuzzleAsync(DnaStrandValidatorSolutionDto solution) =>
        PostAsync("/api/a/easy/puzzle", solution);

    // ---------------------------
    // A-2 Medium: Abyssal Lifeform Classifier
    // ---------------------------
    public Task<LifeformCategoryConfig?> GetAbyssalLifeformClassifierConfigAsync() =>
        GetAsync<LifeformCategoryConfig>("/api/a/medium/config");

    public Task<HttpResponseMessage> StartAbyssalLifeformClassifierAsync() =>
        httpClient.GetAsync("/api/a/medium/start");

    public Task<AbyssalLifeformClassifierDto?> GetAbyssalLifeformClassifierSampleAsync() =>
        GetAsync<AbyssalLifeformClassifierDto>("/api/a/medium/sample");

    public Task<HttpResponseMessage> PostAbyssalLifeformClassifierSampleAsync(AbyssalLifeFormClassifierSolutionDto solution) =>
        PostAsync("/api/a/medium/sample", solution);

    public Task<AbyssalLifeformClassifierDto?> GetAbyssalLifeformClassifierPuzzleAsync() =>
        GetAsync<AbyssalLifeformClassifierDto>("/api/a/medium/puzzle");

    public Task<HttpResponseMessage> PostAbyssalLifeformClassifierPuzzleAsync(AbyssalLifeFormClassifierSolutionDto solution) =>
        PostAsync("/api/a/medium/puzzle", solution);

    // ---------------------------
    // A-3 Hard: The Harmonic Inscription
    // ---------------------------
    public Task<FrequencyMapDto?> GetTheHarmonicInscriptionFrequencyMapAsync() =>
        GetAsync<FrequencyMapDto>("/api/a/hard/frequencyMap");

    public Task<HttpResponseMessage> StartTheHarmonicInscriptionAsync() =>
        httpClient.GetAsync("/api/a/hard/start");

    public Task<TheHarmonicInscriptionDto?> GetTheHarmonicInscriptionSampleAsync() =>
        GetAsync<TheHarmonicInscriptionDto>("/api/a/hard/sample");

    public Task<HttpResponseMessage> PostTheHarmonicInscriptionSampleAsync(TheHarmonicInscriptionSolutionDto solution) =>
        PostAsync("/api/a/hard/sample", solution);

    public Task<TheHarmonicInscriptionDto?> GetTheHarmonicInscriptionPuzzleAsync() =>
        GetAsync<TheHarmonicInscriptionDto>("/api/a/hard/puzzle");

    public Task<HttpResponseMessage> PostTheHarmonicInscriptionPuzzleAsync(TheHarmonicInscriptionSolutionDto solution) =>
        PostAsync("/api/a/hard/puzzle", solution);

    // ---------------------------
    // B-1 Easy: Drift Scan
    // ---------------------------
    public Task<HttpResponseMessage> StartDriftScanAsync() =>
        httpClient.GetAsync("/api/b/easy/start");

    public Task<DriftScanDto?> GetDriftScanSampleAsync() =>
        GetAsync<DriftScanDto>("/api/b/easy/sample");

    public Task<HttpResponseMessage> PostDriftScanSampleAsync(DriftScanSolutionDto solution) =>
        PostAsync("/api/b/easy/sample", solution);

    public Task<DriftScanDto?> GetDriftScanPuzzleAsync() =>
        GetAsync<DriftScanDto>("/api/b/easy/puzzle");

    public Task<HttpResponseMessage> PostDriftScanPuzzleAsync(DriftScanSolutionDto solution) =>
        PostAsync("/api/b/easy/puzzle", solution);
        
        
    // ---------------------------
    // B-2 Medium: Abyssal Survey
    // ---------------------------
    public Task<HttpResponseMessage> StartAbyssalSurveyAsync() =>
        httpClient.GetAsync("/api/b/medium/start");

    public Task<AbyssalSurveyDto?> GetAbyssalSurveySampleAsync() =>
        GetAsync<AbyssalSurveyDto>("/api/b/medium/sample");

    public Task<HttpResponseMessage> PostAbyssalSurveySampleAsync(AbyssalSurveySolutionDto solution) =>
        PostAsync("/api/b/medium/sample", solution);

    public Task<AbyssalSurveyDto?> GetAbyssalSurveyPuzzleAsync() =>
        GetAsync<AbyssalSurveyDto>("/api/b/medium/puzzle");

    public Task<HttpResponseMessage> PostAbyssalSurveyPuzzleAsync(AbyssalSurveySolutionDto solution) =>
        PostAsync("/api/b/medium/puzzle", solution);

    // Movement and Scan for Sample
    public async Task<LocationDto?> PostAbyssalSurveySampleMoveAsync(string moveCommand)
    {
        var response = await PostAsync("/api/b/medium/sample/move", moveCommand);
        return await Deserialize<LocationDto>(response);
    }

    public Task<ScanResponseDto?> GetAbyssalSurveySampleScanAsync() =>
        GetAsync<ScanResponseDto>("/api/b/medium/sample/scan");

    // Movement and Scan for Puzzle
    public async Task<LocationDto?> PostAbyssalSurveyPuzzleMoveAsync(string moveCommand)
    {
        var response = await PostAsync("/api/b/medium/puzzle/move", moveCommand);
        return await Deserialize<LocationDto>(response);
    }

    public Task<ScanResponseDto?> GetAbyssalSurveyPuzzleScanAsync() =>
        GetAsync<ScanResponseDto>("/api/b/medium/puzzle/scan");

    // ---------------------------
    // B-3 Hard: Thermal Link
    // ---------------------------
    public Task<HttpResponseMessage> StartThermalLinkAsync() =>
        httpClient.GetAsync("/api/b/hard/start");

    public Task<ThermaLinkDto?> GetThermalLinkSampleAsync() =>
        GetAsync<ThermaLinkDto>("/api/b/hard/sample");

    public Task<HttpResponseMessage> PostThermalLinkSampleAsync(ThermalLinkSolutionDto solution) =>
        PostAsync("/api/b/hard/sample", solution);

    public Task<ThermaLinkDto?> GetThermalLinkPuzzleAsync() =>
        GetAsync<ThermaLinkDto>("/api/b/hard/puzzle");

    public Task<HttpResponseMessage> PostThermalLinkPuzzleAsync(ThermalLinkSolutionDto solution) =>
        PostAsync("/api/b/hard/puzzle", solution);

    // Movement and Scan for Sample
    public async Task<LocationDto?> PostThermalLinkSampleMoveAsync(string moveCommand)
    {
        var response = await PostAsync("/api/b/hard/sample/move", moveCommand);
        return await Deserialize<LocationDto>(response);
    }

    public Task<ScanResponseDto?> GetThermalLinkSampleScanAsync() =>
        GetAsync<ScanResponseDto>("/api/b/hard/sample/scan");

    // Movement and Scan for Puzzle
    public async Task<LocationDto?> PostThermalLinkPuzzleMoveAsync(string moveCommand)
    {
        var response = await PostAsync("/api/b/hard/puzzle/move", moveCommand);
        return await Deserialize<LocationDto>(response);
    }

    public Task<ScanResponseDto?> GetThermalLinkPuzzleScanAsync() =>
        GetAsync<ScanResponseDto>("/api/b/hard/puzzle/scan");


    // ---------------------------
    // Team Endpoints
    // ---------------------------
    public Task<HttpResponseMessage> GetChallengeAsync() =>
        httpClient.GetAsync("/api/Challenge");

    public Task<HttpResponseMessage> GetTeamTokenAsync(string teamName, string password) =>
        httpClient.GetAsync($"/api/Team/token?teamName={teamName}&password={password}");

    public Task<HttpResponseMessage> GetTeamProgressAsync() =>
        httpClient.GetAsync("/api/Team/progress");

    public Task<HttpResponseMessage> GetAllProgressAsync() =>
        httpClient.GetAsync("/api/Team/all-progress");

    public Task<HttpResponseMessage> PostTeamMoveAsync(UpdatePositionDto position) =>
        PostAsync("/api/Team/move", position);

    // ---------------------------
    // Helper for deserialization from HttpResponseMessage
    // ---------------------------
    private async Task<T?> Deserialize<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }
}