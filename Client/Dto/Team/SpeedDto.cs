using System.Runtime.Serialization;

namespace HTF2025_Client.Dto.Team;

public enum SpeedDto
{
    [EnumMember(Value = "L")] Low,
    [EnumMember(Value = "M")] Medium,
    [EnumMember(Value = "H")] Fast,
    [EnumMember(Value = "R")] Reverse
}