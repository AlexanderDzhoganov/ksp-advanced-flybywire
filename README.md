## Advanced Fly-By-Wire v1.4
### Kerbal Space Program input system overhaul mod

Latest version - 1.4

[Download v1.4 (Windows, KSP x86)](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/raw/master/builds/ksp-advanced-flybywire_v1.4_x86.zip)

[Download v1.4 (Linux, KSP x86/x64)](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/raw/master/builds/ksp-advanced-flybywire_v1.4_linux.zip) (Requires libsdl2)

[Licensed under the MIT License](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/blob/master/LICENSE)

You can also find the mod on:
- [Official forums](http://forum.kerbalspaceprogram.com/threads/95022-0-24-2-Advanced-Fly-by-wire-v1-0-%28Better-controller-support%29)
- [Curse.com](http://www.curse.com/ksp-mods/kerbal/224592-advanced-fly-by-wire)
- [KerbalStuff](https://kerbalstuff.com/mod/232/Advanced%20Fly-By-Wire)

### What is this?
This is a mod for [Kerbal Space Program](http://kerbalspaceprogram.com), a spaceship building/ space exploration game by Squad.
It dramatically enhances the stock input system with a bunch of fixes and many new features.

### How is it better than stock?

- Edit your control setup at any time during flight, no need to go back to the main menu to change bindings.
- Supports almost all controller types through the SDL wrapper.
- Native support for the Xbox 360 controller (and PS3 controller using MotionJoy) through the XInput wrapper. A built-in example preset is available.
- Unlimited number of controller buttons and axes
- Full keyboard & mouse support
- Lower latency and better analog precision than KSP's stock input
- Smart dead-zone detection and analog calibration 
- Remaps analog inputs to achieve better precision
- Acceleration-based discrete inputs for precise keyboard flight
- Supports key combinations with an infinite number of keys
- Multiple presets per controller
- Extremely simple to configure and use
- Works alongside the stock input system. The mod will not override or break your current setup.

### How to use
![screenshot](http://i.imgur.com/NgIVDIQ.png)

The mod adds a new button to the mod toolbar.

Click the game controller icon or press Shift + L during flight to bring up Fly-By-Wire's main configuration screen.

From there you will see a list of detected controllers. You can click on "Enable" to enable a controller from the list - two new buttons will appear - "Presets" and "Configure".

"Presets" will open up the preset editor which is very similar to KSP's stock bindings editor. Using the preset editor you can modify your controller layout at any time.

"Configure" will open up the controller configuration screen. It allows you to set some configuration values as well as calibrate the controller if necessary.

After setting up your controller you should save your game (by using quicksave or exiting to space center), which will automatically save your controller configuration as well.

### Operating system compatibility
Fully tested and compatible with KSP x32 and x64 on Windows operating systems.
Linux/ MacOSX support available but still in early testing.
XInput support unavailable on Linux and MacOSX.

### Performance considerations
The mod is extremely lightweight both on performance and memory. It only does a bit of arithmetic and remapping of incoming inputs which should have
no noticeable effect on CPU usage. Memory usage is in the order of a few megabytes.

### Compatibility with other mods
The mod has been tested to work with over 50 of the most popular mods.

Here is a (non-exhaustive) list of mods that have been tested to be compatible, in no particular order:
- MechJeb
- Ferram Aerospace Research (FAR)
- KW Rocketry
- B9 Aerospace 
- OKS/ MKS
- Kethane
- Infernal Robotics
- Kerbal Engineer Redux
- ORS
- PreciseNode
- TextureReplacer
- Deadly Reentry
- Crossfeed Enabler
- Chatterer
- Environmental Visual Enhancements
- TAC Life Support .. and more

### Bug reports
Please report any bugs using [GitHub's issue tracker](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/issues).

### Changelog

[Click here for changelog](https://raw.githubusercontent.com/AlexanderDzhoganov/ksp-advanced-flybywire/master/CHANGELOG)

