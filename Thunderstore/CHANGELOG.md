# ULTRAPORTAL 0.2.0
## Changes
### Bugfixes
- Fixed mirror portal projectile from teleporting enemies if portal was despawned.
- Ground portals now detect the proper colliders in the cybergrind. ([see issue](https://github.com/averyocean65/UltraPortal/issues/14))
- Portals now reset colliders when they're destroyed.
- Increased size of portal gun projectile to decrease collision errors at higher projectile speeds.

## Config
- Renamed `Advanced` sub-categories to `Experimental` to indicate that tweaking the values may cause significant bugs.
- Added `Debug` category to configuration.
- Added ability to hide portal borders in `Visuals`.

## Known Bugs
- It is possible to trigger a portal from behind, which causes things like wall andi floor collisions to disable prematurely.
- Enemies travelling through portals is unreliable.
  - Makes it impossible to achieve style modifiers such as *+TERMINAL VELOCITY*.
