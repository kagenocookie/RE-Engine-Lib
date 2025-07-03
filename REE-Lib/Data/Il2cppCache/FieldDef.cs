using System.Text.Json.Serialization;

namespace ReeLib.Il2cpp;

public class FieldDef
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("flags")]
    public string Flags { get; set; } = string.Empty;

    [JsonPropertyName("default")]
    public object? Default { get; set; }

    public bool IsStatic => Flags.Contains("Static");
    public bool IsPrivate => Flags.Contains("Private");
}