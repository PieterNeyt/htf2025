namespace HTF2025_Client.Dto.AbyssalLifeformClassifier;

public class Lifeform
{
    public string Id { get; set; } = string.Empty;
    public decimal BioluminescenceFrequency { get; set; }
    public string MovementPattern { get; set; } = string.Empty;
    public string ThermalSignature { get; set; } = string.Empty;
    public string SizeEstimate { get; set; } = string.Empty;
}