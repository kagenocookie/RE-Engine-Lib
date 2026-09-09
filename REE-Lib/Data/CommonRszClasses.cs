using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ReeLib.Common;
using ReeLib.Data;
using ReeLib.Il2cpp;

namespace ReeLib;

public sealed partial class CommonRszClasses(RszParser parser)
{
    public readonly RszClass GameObject = parser.GetRSZClass("via.GameObject") ?? throw new Exception("Class not found: via.GameObject");
    public readonly RszClass Transform = parser.GetRSZClass("via.Transform") ?? throw new Exception("Class not found: via.Transform");
    public readonly RszClass Mesh = parser.GetRSZClass("via.render.Mesh") ?? throw new Exception("Class not found: via.render.Mesh");
    public readonly RszClass Folder = parser.GetRSZClass("via.Folder") ?? throw new Exception("Class not found: via.Folder");
    public readonly RszClass Prefab = parser.GetRSZClass("via.Prefab") ?? throw new Exception("Class not found: via.Prefab");
}
