# ULTRAPORTAL
Adding portals to ULTRAKILL, because why not?

# Development Setup
## Using a custom ULTRAKILL path?
If you're using a custom ULTRAKILL path, for example to preserve harddrive space on your OS partition, or for other reasons, ULTRAPORTAL will not compile out of the box.
You need to create a file in the project root directory called `ULTRAKILLPATH.user`, where you can specify your custom game path like this:
```
<Project>
  <PropertyGroup>
    <ULTRAKILLPath>INSERT THE PATH TO THE DIRECTORY CONTAINING ULTRAKILL.exe, example: D:/Games/ULTRAKILL/</ULTRAKILLPath>
  </PropertyGroup>
</Project>
```