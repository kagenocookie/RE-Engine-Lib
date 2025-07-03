namespace ReeLib.Il2cpp;

public class ObjectDef
{
    public string? flags { get; set; }
    public string? parent { get; set; }
    public Dictionary<string, FieldDef>? fields { get; set; }
    public bool IsAbstract => flags?.Contains("Abstract") == true;
}
