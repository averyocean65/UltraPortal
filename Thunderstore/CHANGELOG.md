# ULTRAPORTAL 0.2.0
## Changes
### Bugfixes
- Fixed mirror portal projectile from teleporting enemies if portal was despawned.
- Ground portals now detect the proper colliders in the cybergrind. ([see issue](https://github.com/averyocean65/UltraPortal/issues/14))
- Portals now reset colliders when they're destroyed.
- Increased size of portal gun projectile to decrease collision errors at higher projectile speeds.
- *+SAFETY HAZARD* now gets shown reliably.

### Gameplay
- Changed *+SAFETY HAZARD* from 500 points to 100 points.
- Added new style bonus *+DISPLACEMENT*.
- Added alternative fire for mirror gun.
- Added the Twist Gun.

### Config
- Changed categorization of config values
  - Renamed `Advanced` sub-categories to `Experimental` to indicate that tweaking the values may cause significant bugs.
  - Moved portal color properties to `Visuals/Portals`.
- Added `Debug` category to configuration.
- Added setting to disable portal borders.
- Added setting to change max recursions of portals.

## Known Bugs
- It is possible to trigger a portal from behind, which causes things like wall andi floor collisions to disable prematurely.
