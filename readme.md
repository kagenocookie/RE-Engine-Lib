# RE-Engine-Lib

RE-Engine-Lib is a .NET library that aims to support as many as possible file formats of RE ENGINE games. Most of the important file formats are already supported. A PAK file unpacker is also included and the library can read files directly from the original PAK files.

The original code is based off of [RszTool](https://github.com/czastack/RszTool), but heavily expanded to support more file formats and provide more convenience features for usage as a reusable library, as well as some clean up of the public API.

The project relies on game-specific data that can't be fully automated for some of its features, those are fetched as needed from the [REE-Lib-Resources repository](https://github.com/kagenocookie/REE-Lib-Resources), or they can be manually specified.

## Project goals
Support reading of all the RE ENGINE resource file that are relevant for developing mods and tools for games made in the engine.

This project does not provide any actual tooling by itself, it is intended to be used as an independent library.

## How to build this project

1. Install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
2. run `dotnet build`

## How to use this tool

- Clone the repository and build the project with `dotnet build`, link the resulting .dll in your own project
- Create a new `GameConfig` instance. This can be either through the `GameConfig.CreateFromRepository` method, which will download any resources automatically, or you can set up your own configuration from an ini file, json file, hardcoded, whatever you wish. Different configuration files are required depending on what kind of operation you wish to do.
- Create a new `Workspace` instance and provide it a base cache folder through the `Init(string cachePath)` method for where it should hold its cache data
- All file format readers are named in the format `<FileType>File` and their interface should be more or less identical, except that some of them also require a RszFileOption instance


### Examples
**Read and modify a user.2 file**
```cs
// GameConfig.CreateFromRepository() will fetch any required resources / JSON files according to https://github.com/kagenocookie/REE-Lib-Resources
// these resources are cached locally in `resources/resource-cache.json` by default, can be changed with `ResourceRepository.LocalResourceRepositoryFilepath`
// the config can also be manually set up instead
var config = GameConfig.CreateFromRepository(GameIdentifier.dd2);

// Workspace is the main data class holding all the different resources and singleton instances for a specific game
var env = new Workspace(config);

// Now we can open a file
// new FileHandler(String) loads the whole file into memory before doing any data reading to make it go much faster
using var userFile = new UserFile(env.RszFileOption, new FileHandler("path/to/file.user.2"));
userFile.Read();
Console.WriteLine("Deserialized user file class: " + userFile.ObjectList[0].RszClass.name);

userFile.RSZ.ObjectList[0].SetFieldValue("field", "new value");
// RebuildInfoTable() is required before saving whenever structural changes are done (new or removed objects), unless you handle the instance indexes manually
userFile.RebuildInfoTable();
userFile.WriteTo(new FileHandler("output/file/path.user.2"));
```

**Read and modify a scn.20 file**
```cs
var config = GameConfig.CreateFromRepository(GameIdentifier.dd2);
var env = new Workspace(config);
using var scn = new ScnFile(env.RszFileOption, new FileHandler("path/to/file.scn.20"));
scn.Read();
// this will build up the object tree so you get the proper hierarchy for all the objects in the file.
scn.SetupGameObjects();
Console.WriteLine("Deserialized scn file");

var rootFolders = scn.FolderDatas;
var rootGameObjects = scn.GameObjects;

scn.RebuildInfoTable();
scn.WriteTo(new FileHandler("output/file/path.scn.20"));
```
All file readers follow a similar pattern - just create the file and call `.Read()`, and `.SetupGameObjects()` in the case of scn and pfb.

## Project structure

1. ReeLib
    - The core REE-Lib serialization library. Provides the file read/write features for all supported file types.
2. ReeLib.Tools
    - Additional tools that may be useful for integration with other tools that aren't needed by the core serialization library.
3. ReeLib.Generators
    - Internal source generator library for easier serializer development.

## License

REE-Lib is under MIT license. See LICENSE for more detail. Some of the original code that is based on RszTool is also under MIT license.

## Credits
- chenstack - Base RszTool file handling code
- alphaZomega, praydog, NSACloud, and other modders - File structure research
- [Ekey/REE.PAK.Tool](https://github.com/Ekey/REE.PAK.Tool) - PAK file decryption algorithms

## Disclaimer
This project is in no way affiliated or connected to RE ENGINE, Capcom, or any of their affiliates. All trademarks are the property of their respective owners.
