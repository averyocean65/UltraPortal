[![forthebadge](https://forthebadge.com/api/badges/generate?panels=2&primaryLabel=ULTRA+&secondaryLabel=PORTAL&primaryBGColor=%234075e3&primaryTextColor=%23FFFFFF&secondaryBGColor=%23ff750f&secondaryTextColor=%23FFFFFF&primaryFontSize=12&primaryFontWeight=600&primaryLetterSpacing=2&primaryFontFamily=Roboto&primaryTextTransform=uppercase&secondaryFontSize=12&secondaryFontWeight=600&secondaryLetterSpacing=2&secondaryFontFamily=Montserrat&secondaryTextTransform=uppercase&borderRadius=8&primaryTextShadowColor=%23ffffff&primaryTextShadowBlur=3.5&secondaryTextShadowColor=%23ffffff&secondaryTextShadowBlur=3.5)](https://forthebadge.com)

Adding portals to ULTRAKILL, because why not?

# Development Setup

## UltraPortal
### Requirements
In order for this mod to compile, you must have BepInEx installed, as well as the following mods:
- Configgy, `ULTRAKILL/BepInEx/plugins/Configgy.dll`
- [Avery's ULTRAKILL Utilities](https://github.com/averyocean65/auu), `ULTRAKILL/BepInEx/plugins/AUU.dll`

To run the mod you also need to have the following patchers installed:
- [FixPluginTypesSerialization](https://thunderstore.io/c/ultrakill/p/averyocean65/FixPluginTypesSerialization/), anywhere in `BepInEx/patchers`

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

## UltraPortalAssets
### Requirements
To use the UltraPortalAssets project, you will need Unity `2022.3.28f1` in order to open the project.

### How to compile asset bundles
After you're done creating assets, compile the asset bundles with the menu item: `Assets/Build Asset Bundles`.

### Where to insert asset bundles?
Right now we don't have a system to automatically bundle the asset bundles with ULTRAPORTAL, so you have to manually copy and paste the asset bundle and manifest files into the folder `<ULTRAKILL>/BepInEx/plugins/UltraPortal/Bundles`.

### Credits
- Most art assets were created by [RoseWarbug](https://bsky.app/profile/rosewarbug.bsky.social).
- Voronoi Shader for portal blockage taken from [ronja-tutorials](https://github.com/ronja-tutorials/ShaderTutorials/tree/master/Assets/028_Voronoi_Noise).
