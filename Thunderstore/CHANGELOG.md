# ULTRAPORTAL 0.2.2

## Changes
### Backend
- Changed `PortalTransform` property of `DynamicPortalExit.cs` to `NativePortalTransform` to adopt changes made by the ULTRAKILL developers in Patch 17c.

<details>
<summary>0.2.1 Changelog</summary>

## Changes
### Config
- Increased default value of `Gameplay/Portals/Experimental/Portal Sphere Check Radius`.

### Backend
- Mod now relies on `FixPluginTypesSerialization` by xiaoxiao921 to load assemblies. ([see package for ULTRAKILL](https://thunderstore.io/c/ultrakill/p/averyocean65/FixPluginTypesSerialization/))
- Changed a logger call in `DynamicPortalExit.cs` to verbose to prevent lag.
- Swapped several functions with functions from [Avery's ULTRAKILL Utilities](https://github.com/averyocean65/auu).

### Bugfixes
- Fix bug where secondary fire cooldown was linked to primary fire cooldown

## Known Bugs
- It is possible to trigger a portal from behind, which causes things like wall and floor collisions to disable when they shouldn't.
  - Bug can only occur with floor/ceiling portals as of 0.2.1.
- You cannot save a sandbox level if it contains portals.
- Enemies may phase through the floor sometimes if teleported after surpassing a certain velocity.
</details>

<details>
<summary>0.2.0 Changelog</summary>

## Changes
### Bugfixes
- Fixed mirror portal projectile from teleporting enemies if portal was despawned.
- Ground portals now detect the proper colliders in the cybergrind. ([see issue](https://github.com/averyocean65/UltraPortal/issues/14))
- Portals now reset colliders when they're destroyed.
- Increased size of portal gun projectile to decrease collision errors at higher projectile speeds.
- *+SAFETY HAZARD* now gets shown reliably.
- Fixed portal duplication with frozen sandbox objects.
- Fixed leaderboard submission not being handled properly.

### Gameplay
- Added the **Twist Gun**.
- Changed *+SAFETY HAZARD* from 500 points to 100 points.
- Added new style bonus *+DISPLACEMENT*.
- Added alternative fire for mirror gun.
- *+USED PORTAL GUN* in the level end screen is only enabled when a portal gun was actually fired.
- Buffed damage of portal projectile.
- Portal projectiles only teleport light enemies now.
- Portal guns can now be configured to instantly shoot portals instead of using projectiles. ([see issue](https://github.com/averyocean65/UltraPortal/issues/11))
- Custom portals can now be modified in sandbox.

### Audio
- Added sound effects for portal guns.
  - Portal Open
  - Portal Close
- Added ambiance sound effect for (custom) portals.

### Config
- Changed categorization of config values
  - Renamed `Advanced` sub-categories to `Experimental` to indicate that tweaking the values may cause significant bugs.
  - Moved portal color properties to `Visuals/Portals`.
- Added individual colors for all portals.
  - Found under: `Visuals/Portal Gun`, `Visuals/Mirror Gun` and `Visuals/Twist Gun`
- Added setting to disable portal borders.
  - Found at: `Visuals/Portals/Can See Portal Borders`
- Added setting to change max recursions of portals.
  - Found at: `Visuals/Portals/Maximum Portal Recursions` and `Visuals/Portals/Infinite Portal Recursions`
- Added setting to scale portals.
  - Found at: `Gameplay/Portals/Portal Scale Modifier`
- Added setting to shoot portals instantaneously. ([see issue](https://github.com/averyocean65/UltraPortal/issues/11))
  - Found at: `Gameplay/Projectiles/Use Beam For Projectiles`
- Added `Debug` category to configuration.
- Added `Audio` category to configuration.

### Visuals
- Improved portal and portal gun designs by [RoseWarbug](https://bsky.app/profile/rosewarbug.bsky.social).
  - Voronoi Shader for portal blockage taken from [ronja-tutorials](https://github.com/ronja-tutorials/ShaderTutorials/tree/master/Assets/028_Voronoi_Noise).

## Known Bugs
- It is possible to trigger a portal from behind, which causes things like wall and floor collisions to disable when they shouldn't.
  - Chance of bug occuring has been reduced with 0.2.0, although it is not impossible.
- You cannot save a sandbox level if it contains portals.
- Enemies may phase through the floor sometimes if teleported after surpassing a certain velocity.
</details>