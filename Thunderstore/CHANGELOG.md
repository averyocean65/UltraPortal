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
- Added new style bonus *+TELEPORTATION*.

### Config
- Renamed `Advanced` sub-categories to `Experimental` to indicate that tweaking the values may cause significant bugs.
- Added `Debug` category to configuration.
- Added ability to hide portal borders.

## Known Bugs
- It is possible to trigger a portal from behind, which causes things like wall andi floor collisions to disable prematurely.
