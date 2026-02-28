# ULTRAPORTAL
Adding portals to ULTRAKILL, because why not?

# Development Setup
## Requirements
In order for this mod to compile, you must have `Configgy` downloaded to your ULTRAKILL BepInEx plugins folder, with the specific path being: `ULTRAKILL/BepInEx/plugins/Configgy.dll`

## Using a custom ULTRAKILL path?
If you're using a custom ULTRAKILL path, for example to preserve hard drive space on your OS partition, or for other reasons, ULTRAPORTAL will not compile out of the box.
You need to create a file in the project root directory called `ULTRAKILLPATH.user`, where you can specify your custom game path like this:
```
<Project>
  <PropertyGroup>
    <ULTRAKILLPath>INSERT THE PATH TO THE DIRECTORY CONTAINING ULTRAKILL.exe/</ULTRAKILLPath>
  </PropertyGroup>
</Project>
```

Keep in mind that the path needs to be to the directory that contains your ULTRAKILL executable file. For example `D:/Steam/steamapps/common/ULTRAKILL`