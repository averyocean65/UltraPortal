# ULTRAPORTAL 0.1.5
## Changes
### Bugfixes
- Fixed mirror portal projectile from teleporting enemies if portal was despawned.
- Fixed configuration not loading properly on startup.
- Ground portals now detect the proper colliders in the cybergrind.

## Known Bugs
- It is possible to trigger a portal from behind, which causes things like wall andi floor collisions to disable prematurely.
- Enemies travelling through portals is unreliable.
  - Makes it impossible to achieve style modifiers such as *+TERMINAL VELOCITY*.