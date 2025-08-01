using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ReeLib.Common;
using ReeLib.Data;
using ReeLib.Il2cpp;

namespace ReeLib;

public sealed partial class CommonRszClasses(RszParser parser)
{
    private RszClass? _gameobject;
    public RszClass GameObject => _gameobject ??= parser.GetRSZClass("via.GameObject") ?? throw new Exception("Class not found: via.GameObject");

    private RszClass? _transform;
    public RszClass Transform => _transform ??= parser.GetRSZClass("via.Transform") ?? throw new Exception("Class not found: via.Transform");

    private RszClass? _Mesh;
    public RszClass Mesh => _Mesh ??= parser.GetRSZClass("via.render.Mesh") ?? throw new Exception("Class not found: via.render.Mesh");

    private RszClass? _Folder;
    public RszClass Folder => _Folder ??= parser.GetRSZClass("via.Folder") ?? throw new Exception("Class not found: via.Folder");
}
