# ULTRAPORTAL
Adding portals to ULTRAKILL, because why not?

# Development Setup

## UltraPortalAssets
### Credits
All art assets were created by [Rose Warbug](https://bsky.app/profile/rosewarbug.bsky.social)

### Requirements
To use the UltraPortalAssets project, you will need Unity `2022.3.28f1` in order to open the project.

### How to compile asset bundles
After you're done creating assets, compile the asset bundles with the menu item: `Assets/Build Asset Bundles`.

### Where to insert asset bundles?
Right now we don't have a system to automatically bundle the asset bundles with ULTRAPORTAL, so you have to manually copy and paste the asset bundle and manifest files into the folder `<ULTRAKILL>/BepInEx/plugins/UltraPortal/Bundles`.

## UltraPortal
### Requirements
In order for this mod to compile, you must have `Configgy` downloaded to your ULTRAKILL BepInEx plugins folder, with the specific path being: `ULTRAKILL/BepInEx/plugins/Configgy.dll`.

### Using a custom ULTRAKILL path?
If you're using a custom ULTRAKILL path, for example to preserve hard drive space on your OS partition, or for other reasons, ULTRAPORTAL will not compile out of the box.
You need to create a file in the project root directory called `ULTRAKILLPATH.user`, where you can specify your custom game path like this:
```
<Project>
  <PropertyGroup>
    <ULTRAKILLPath>INSERT THE PATH TO THE DIRECTORY CONTAINING ULTRAKILL.exe/</ULTRAKILLPath>
  </PropertyGroup>
</Project>
```

Keep in mind that the path needs to be to the directory that contains your ULTRAKILL executable file. For example `D:/Steam/steamapps/common/ULTRAKILL`.