# Changelog
All notable changes to this package will be documented in this file.

## [1.1.0] - XX/04/2025

### BIMOS
- Added custom avatars
- Added haptics
  - Grabbing haptics
  - Hand collision haptics
  - Custom haptic events for grabbables
- Added custom project validation rules to allow for rapid setup without a guide
- Improved hand pose editor
  - Added ability to load custom avatar hands
  - Made editor more intuitive by highlighting the currently used sub-pose
- Changed tracked pose drivers to use OpenXR's "palm pose"
  - Added the "Palm Pose Emulator" from BIPED for standalone
- Attachers detaching from sockets now keep their detach animation velocity
- Grabs are now called grabbables (for more consistent naming with interactables)
- Fixed issue where seated magazines would be counted, disallowing further summoning
- References to local player objects are now available via a singleton
- Updated grab rank calculation to factor in hand rotation
- Added basic holster/body slot support
- Added roomscale crouching
- Moved body colliders back so they more accurately reflect where the avatar's body is

### Samples
- Created a full modular gun system
- Added an assault rifle (model by Mason Mad, again (my love))
- Given the following objects new models (thanks Mason Mad, my love):
  - Pistol
  - Grapple hook
  - Hammer
  - Axe
- Redone all hand poses to work with the new system
  - You will have to do this for your own custom hand poses
- Gun casings and bullet holes now use "pooling"

## [1.0.0] - 06/12/2024

### This is the first release of *BIMOS*.