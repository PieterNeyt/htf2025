namespace HTF2025_Client.Dto.AbyssalLifeformClassifier;

public class LifeformCategory
{
    public string Name { get; set; } = string.Empty;
    
    public int SpectralWeight { get; set; }
    
    public Dictionary<string, TraitRule> Rules { get; set; } = new();
}