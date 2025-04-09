# Changelog
All notable changes to this package will be documented in this file.

## [1.1.0] - XX/04/2025

### BIMOS
- Added custom avatars
- Improved hand pose editor
  - Added ability to load custom avatar hands
  - Made editor more intuitive by highlighting the currently used sub-pose
- Changed tracked pose drivers to use OpenXR's "palm pose"
  - Added the "Palm Pose Emulator" from BIPED for standalone
- Attachers detaching from sockets now keep their detach animation velocity

### Samples
- Redone all thumb poses to work with the new system
  - You will have to do this for your own custom hand poses
- TODO: Made the pistol's casings and bullet holes use pooling
- TEST: Fixed issue where seated magazines would be counted, disallowing further summoning

## [1.0.0] - 06/12/2024

### This is the first release of *BIMOS*.