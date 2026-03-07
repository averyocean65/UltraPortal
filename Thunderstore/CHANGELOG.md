# ULTRAPORTAL 0.1.5
## Changes
### Bugfixes
- Fixed the portal guns not displaying properly in middle position ([see issue](https://github.com/averyocean65/UltraPortal/issues/6)).
- Fixed various edge-cases when equipping/unequipping the portal gun with no other weapons selected.
- Fixed clipping issues with disabling portals whilst a player was inside them.
- Decreased chance of collisions not working properly after disabling portals.
- Fixed rooms despawning when a portal is inside them.

### Gameplay
- Made portals explode if it has travelers inside of it when it gets despawned.
- Added new style bonus *+SAFETY HAZARD*.
- Changed input method to close portals.
  - Portals can now be closed by pressing your primary and alternative fire at once whilst holding a portal gun.
- Portals now stick to moving surfaces.

### Config
- Added config to change portal explosion color.
- Added config to set if a portal explosion is an ultraboost.
- Added config to set *+SAFETY HAZARD* style color.

## Known Bugs
- It is possible to trigger a portal from behind, which causes things like wall andi floor collisions to disable prematurely.
- Enemies travelling through portals is unreliable.
  - Makes it impossible to achieve style modifiers such as *+TERMINAL VELOCITY*.