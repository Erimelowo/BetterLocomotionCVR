# WIP. Not functional yet (I'm waiting on a lib for UI like UIExpansionKit :P)

# BetterLocomotion

A VRChat mod to enhance locomotion for VRChat using MelonLoader.  
Successor of [BetterDirections](https://github.com/d-magit/VRC-Mods).

## Features

- **Fixes the inconvenience that happens because of Euler angles when moving while looking up**, usually while laying down or cuddling, for example.
- **Allows you to set a threshold for movement to compensate for joystick drift** while keeping that smooth acceleration effect.
- **Choose between head, hip or chest locomotion.** This allows hip or chest locomotion, allowing you move towards your hip or chest instead of your head. _Just like Decamove but without the VRChat "head bias"._
- Hip and chest locomotion works in 4-point tracking, 6-point tracking or more.
- Improve Decamove support. On top of moving towards your hip, you will also go faster towards your hip instead of your head when using decamove. (Thanks [ballfun](https://github.com/ballfn))

## Settings descriptions

- **Locomotion mode**: which reference should be used for locomotion (head, hip, chest or decamove)
- **Joystick threshold (0-1)**: prevents you from moving if your joystick's inclination is below that threshold. 0 being no threshold and 1 requiring you to tilt your joystick all the way to move.
- **Show deca QM button**: toggle the Decamove calibration button on the quick menu.

## Dependency

## Credit/Special thanks

- Davi's [BetterDirection](https://github.com/d-magit/VRC-Mods) mod (original mod)
- [AxisAngle](https://twitter.com/DonaldFReynolds) (for the logic with angles/movement)
- SDraw's [Calibration Lines Visualizer](https://github.com/SDraw/ml_mods) mod (code to find SteamVR trackers)
- Patchuuri for naming Lolimotion :3
- [ballfun](https://github.com/ballfn) for improved Decamove support.

## Installation

**Before installing: I am not affiliated with ABI and this mod breaking is in no way ABI's fault.**

Install [MelonLoader](https://melonwiki.xyz/#/) and drop [BetterLocomotion.dll](https://github.com/Louka3000/BetterLocomotionCVR/releases/latest/download/BetterLocomotion.dll) in your mods folder.

**OR** you can use the [CVRMelonAssistant](https://github.com/knah/CVRMelonAssistant/releases/latest/download/CVRMelonAssistant.exe) to automatically take care of the install process for you.
