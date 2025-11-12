using HTF2025_Client.Dto.AbyssalExpedition;

namespace HTF2025_Client.Dto.AbyssalSurvey;

public class AbyssalSurveyDto 
{
    public string XorKey { get; set; }
    
    public LocationDto CurrentLocation { get; set; }
    
    public int GridSize { get; set; }
}