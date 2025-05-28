using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RszTool.Efx;

namespace RszTool.Tools;

internal sealed class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0) {
            PrintHelp();
            return 0;
        }

        var cmd = args[0];
        switch (cmd) {
            case "efx-templates":
                GenerateEFXTemplates(GetArgFolderPath(args, 1));
                break;
            case "efx-structs":
                var version = GetArgEfxVersion(args, 2);
                if (version.HasValue) {
                    EfxTools.GenerateEFXStructsJson(GetArgFilePath(args, 1), version.Value);
                } else {
                    var folder = GetArgFolderPath(args, 1);
                    foreach (var ver in Enum.GetValues<EfxVersion>()) {
                        EfxTools.GenerateEFXStructsJson(Path.Combine(folder, Enum.GetName<EfxVersion>(ver) + ".json"), ver);
                    }
                }
                break;
            case "test":
                RunTests();
                break;
            default:
                PrintHelp();
                return 1;
        }
        return 0;
    }

    private static string GetArgFolderPath(string[] args, int index)
    {
        return GetArgOrPrintHelp(args, index, v => v, f => Directory.Exists(f) ? null : "Argument {0} must be path to a valid folder");
    }

    private static string GetArgFilePath(string[] args, int index)
    {
        return GetArgOrPrintHelp(args, index, v => v, f => Directory.Exists(Path.GetDirectoryName(f)) ? null : "Argument {0} must be valid filepath to an existing folder");
    }

    private static EfxVersion? GetArgEfxVersion(string[] args, int index)
    {
        return GetArgOrPrintHelp(
            args,
            index,
            v => (EfxVersion?)Enum.Parse<EfxVersion>(v),
            validation: v => Enum.TryParse<EfxVersion>(v, out var ver) && ver != EfxVersion.Unknown ? null : "Argument {0} must be a valid EfxVersion enum: " + string.Join(" | ", Enum.GetNames<EfxVersion>()),
            optional: true
        );
    }

    private static T GetArgOrPrintHelp<T>(
        string[] args,
        int index,
        Func<string, T> argMapper,
        Func<string, string?>? validation = null,
        bool optional = false
    )
    {
        if (index < 0 || index >= args.Length) {
            if (optional) {
                return default!;
            }

            PrintHelp();
            Environment.Exit(1);
        }

        if (validation != null) {
            var err = validation.Invoke(args[index]);
            if (err != null) {
                Console.Error.WriteLineAsync("ERROR: " + err.Replace("{0}", index.ToString()));
                Console.WriteLine();
                PrintHelp();
                Console.WriteLine();
                Console.Error.WriteLineAsync("ERROR: " + err.Replace("{0}", index.ToString()));
                Environment.Exit(1);
            }
        }

        return argMapper(args[index]);
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Rsztool.Tools.exe <COMMAND> [ARGS...]");
        Console.WriteLine(" Available COMMANDs:");
        Console.WriteLine("  efx-templates <outputFolder> // dumps a 010 binary template for most of efx files");
        Console.WriteLine("  efx-structs <outputFolder> // dumps a JSON containing all the known objects and fields related to EFX files for all games");
        Console.WriteLine("  efx-structs <outputFile> <efxVersion> // dumps a JSON containing all the known objects and fields related to EFX files for a game");
    }

    private static void GenerateEFXTemplates(string outputFolder)
    {
        var enumsFile = Path.Combine(outputFolder, "RE_EFX_ENUMS.bt");
        var structsFile = Path.Combine(outputFolder, "RE_EFX_STRUCTS.bt");
        var switchFile = Path.Combine(outputFolder, "RE_EFX_TYPES.bt");

        var generator = new Generators.Rsz010TemplateGenerator();
        var basedir = Directory.GetCurrentDirectory();
        basedir = basedir.Substring(0, basedir.IndexOf("\\RszTool.Tools"));

        var files = new SyntaxTree [] {
            CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(basedir, "RszTool/OtherFiles/EfxFile.cs")))
        }.Concat(Directory.GetFiles(Path.Combine(basedir, "RszTool/OtherFiles/EFX")).Select(fn => CSharpSyntaxTree.ParseText(File.ReadAllText(fn))));

        var compilation = CSharpCompilation.Create("CSharpCodeGen.GenerateAssembly")
            .AddSyntaxTrees(files)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(BaseModel).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputComp, out var diags);

        var autogeneratedNotice = "// autogenerated using shadowcookie's RszTool: https://github.com/kagenocookie/RszTool";
        var structsContent = new StringBuilder();
        var enumsContent = new StringBuilder();
        var switchContent = new StringBuilder();

        enumsContent.AppendLine(autogeneratedNotice);
        structsContent.AppendLine(autogeneratedNotice).AppendLine("""
            #include "EFX_Fixed_structs.bt";

            """);
        switchContent.AppendLine("""
            void GetEFXItemStruct(string itemType)
            {
                switch(itemType)
                {
            """);

        var constSection = """
                local int currentAttributeIndex = -1;
                uint itemType;
                uint unknSeqNum <read=Str("%d = X: %d, Y: %d, Z: %d", this, this & 0xff, (this & 0xff00)>>8, (this & 0xff0000)>>16)>;

            """;

        var ignoredEnums = new HashSet<string>() { };

        var ignoredStructs = new HashSet<string>() {
                "EffectGroup",
                "UndeterminedFieldType",
                "EFXFieldParameterValue",
            };
        var structCases = new Dictionary<string, string>();
        var runResult = driver.GetRunResult();
        if (runResult.Diagnostics.Any()) {
            throw new Exception(runResult.Diagnostics.First().ToString());
        }

        foreach (var res in runResult.Results) {
            foreach (var src in res.GeneratedSources) {
                var isAttr = src.HintName.StartsWith("EFXAttribute");
                var isEnum = src.HintName.StartsWith("Enum_");
                var name = src.HintName.Replace(".cs", "").Replace("EFXAttribute", "").Replace("Enum_", "");
                var source = src.SourceText.ToString();

                if (isEnum) {
                    if (!ignoredEnums.Contains(name)) {
                        enumsContent.AppendLine(source);
                    }
                } else {
                    if (ignoredStructs.Contains(name) || name.StartsWith("EFXExpression") || name.StartsWith("EFXMaterialExpression")) continue;
                    if (isAttr) {
                        source = source.Insert(source.IndexOf('{') + 2, constSection);
                    }

                    if (isAttr) {
                        var split = name.Split("__");
                        var basename = split[0];
                        var attrName = split[1];
                        var vername = split[2].Split('_');
                        var mycase = $"if ({string.Join(" || ", vername.Select(v => $"Version == EfxVersion_{Enum.Parse<EfxVersion>(v)}"))}) {basename}_{vername.First()} attribute;";
                        if (!structCases.TryGetValue(attrName, out var list)) {
                            structCases[attrName] = mycase;
                        } else {
                            structCases[attrName] = list + "\n\t\t\telse " + mycase;
                        }
                        source = source.Replace($"struct {basename}\r\n", $"struct {basename}_{vername.First()}\r\n");
                    }
                    structsContent.Append(source).AppendLine();
                }

            }
        }

        foreach (var (t, opts) in structCases) {
            static string GetWithoutCondition(string str) => str.Substring(str.IndexOf(')') + 1).Trim();

            var caseCount = opts.Where(ch => ch == '(').Count();
            string structText;
            if (caseCount == 1) {
                structText = "{ " + GetWithoutCondition(opts) + " }";
            } else if (caseCount > 1) {
                var last = GetWithoutCondition(opts.Split('\n').Last());
                structText = $"{{\n\t\t\t{opts}\n\t\t\telse {last}\n\t\t}}";
                // structText = $"{{\n\t\t\t{opts}\n\t\t}}";
            } else {
                structText = $"/* Unhandled */";
            }
            // var structText = opts;
            switchContent.AppendLine($"""
                        case "{t}": {structText} break;
                """);
        }
        switchContent.AppendLine("""
                    default:
                        {
                            local uint errorItemType = ReadUInt();
                            local uint errorUnknSeqNum = ReadUInt(FTell()+4);
                            Printf("Struct ID does not match any known structs, template stopping.\nItem Type:%i\nunknSeqNum:%i\nPosition:%i\n",errorItemType,errorUnknSeqNum,FTell());
                            Warning("Struct Error, template stopping. Unknown Item Type: %i",errorItemType);
                            writeErrorToFile(999);
                        };
                }
            }
            """);

        // File.WriteAllText(outputFileSwitch, switchContent.ToString());
        structsContent.Append(switchContent);
        // enumsContent.AppendLine("#include \"EFX_VERSION_DETECT.bt\";");
        // enumsContent.AppendLine("local int Version = GetVersion(Atoi(SubStr(FileNameGetExtension(GetFileName()), 1)));");
        // enumsContent.Append(structsContent);
        // File.WriteAllText(outputFile, structsContent.ToString());

        // setup type remapping
        var remapContent = new StringBuilder();
        remapContent.AppendLine(autogeneratedNotice);
        var validVersions = EfxFile.AllVersions.Except([EfxVersion.Unknown]);
        foreach (var game in validVersions) {
            var map = EfxAttributeTypeRemapper.GetAllTypes(game);
            var cases = map.OrderBy(m => m.Key).Select((pair) => $"\t\tcase {pair.Key}: return \"{pair.Value}\"; ");
            remapContent
                .AppendLine($"string getStructName{game}(uint itemType)")
                .AppendLine("{")
                .AppendLine("\tswitch(itemType)")
                .AppendLine("\t{")
                .AppendJoin("\n", cases)
                .AppendLine("\n\t\tdefault: return \"\";")
                .AppendLine("\t}").AppendLine("}");
        }
        var mainSwitchContent = string.Join("\n\t\t",
            validVersions.Select(v => $"case EfxVersion_{v}: return getStructName{v}(itemType);"));
        remapContent.AppendLine($$"""
            string GetEFXStructName(uint itemType)
            {
                switch(Version)
                {
                    {{mainSwitchContent}}
                    default: return "";
                }
            }
            """);

        File.WriteAllText(enumsFile, enumsContent.ToString());
        File.WriteAllText(structsFile, structsContent.ToString());
        File.WriteAllText(switchFile, remapContent.ToString());
    }

    private static void RunTests()
    {
        TestSourceGenerators();
    }

    #region TESTS
    private static void TestSourceGenerators()
    {
        var TestCsharp = """
            using System.Collections.Generic;

            namespace EFXTEST
            {
                public partial class NestedTest
                {
                    [RszGenerate, RszAutoReadWrite]
                    public partial class RszHeaderStruct : BaseModel
                    {
                        public uint magic;
                        public uint version;
                        public int objectCount;
                        public int instanceCount;
                        public long userdataCount;
                        public long instanceOffset;
                        public long dataOffset;
                        [RszConditional("handler.FileVersion > ", GameVersion.re7)]
                        public long userdataOffset;
                    }
                }

                [RszGenerate]
                public partial class EFXAttributeEmitterShape3D
                {
                    [RszConditional(nameof(Version), ">=", EfxVersion.RE4, EndAt = nameof(lengthed))]
                    public int field;
                    [RszFixedSizeArray(nameof(field), '/', 4)] public float[]? expression;

                    [RszInlineWString]
                    public string? lengthed;
                    [RszInlineWString(-1)]
                    public string? unlength;
                }
            }

            namespace EFXTest2
            {
                [RszGenerate]
                public partial class EFXAttributeEmitterShape3D
                {
                    public uint num;
                }
            }
            """;

        var generator = new Generators.RszSerializerGenerator();
        var basedir = Directory.GetCurrentDirectory();
        basedir = basedir.Substring(0, basedir.IndexOf("\\RszTool.Tools"));

        var compilation = CSharpCompilation.Create("CSharpCodeGen.GenerateAssembly")
            .AddSyntaxTrees(
                CSharpSyntaxTree.ParseText(TestCsharp)
                )
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(BaseModel).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputComp, out var diags);

        var result = driver.GetRunResult();
    }

    #endregion
}
