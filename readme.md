# REE-Lib

REE lib is a .NET library that aims to support every, or as many as possible, file formats of RE ENGINE games. Most of the important file formats are already supported. A file unpacker is also included and the library can read files directly from the original PAK files.

The original code is based off of [RszTool](https://github.com/czastack/RszTool), but heavily expanded to support more file formats and provide more convenience features for usage as a reusable library.

## How to build this project

1. Install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download0)
2. run `dotnet build`

## How to use this tool

- Download the latest release [here](https://github.com/czastack/RszTool/releases).
- Create a new `GameConfig` instance. This can be read from an ini file, json file, hardcoded, whatever you wish. At the minimum, the game exe path should be set.
- Create a new `Workspace` instance and provide it a base cache folder through the `Init(string cachePath)` method for where it should hold its cache data
- All file format readers are named in the format `<FileType>File` and their interface should be more or less identical, except that some of them also require a RszParser instance

### Examples
**Read and modify a user.2 file**
```cs
var config = new GameConfig(GameName.dd2);
var env = new Workspace(config);
using var userFile = new UserFile(env.RszFileOption, new FileHandler("path/to/file.user.2"));
userFile.Read();
Console.WriteLine("Deserialized userfile class: " + userFile.ObjectList[0].RszClass.name);

userFile.RSZ.ObjectList[0].SetFieldValue("field", "new value");
// RebuildInfoTable() is required when structural changes are done (new or removed objects), unless you handle the instance indexes manually
userFile.RebuildInfoTable();
userFile.WriteTo(new FileHandler("output/file/path.user.2"));
```

## project struct

1. ReeLib
    - The core library of REE-Lib. Provides all file read/write features.
2. ReeLib.Generators
    - Internal source generator library for simpler serializer development.
3. ReeLib.Tools
    - Additional tools that may be useful for integration with other tools that aren't needed by the core serialization library.

## License

ReeLib is under MIT license. See LICENSE for more detail. Some of the original code that is based on RszTool is also under MIT license.

## Credits
- chenstack - Base RszTool file code
- alphaZomega, praydog, NSACloud, and other modders - File structure research
- [Ekey/REE.PAK.Tool](https://github.com/Ekey/REE.PAK.Tool) - PAK file decryption algorithms

## Disclaimer
This project is in no way affiliated or connected to RE ENGINE, Capcom, or any of their affiliates. All trademarks are the property of their respective owners.
