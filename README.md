## Advanced Fly-By-Wire v1.0
### Kerbal Space Program input system overhaul mod

### Please note that the mod is still in testing and is NOT officially released. The author assumes no responsibility if anything or everything breaks.
### You can download the latest preview version from:
- 32 bit - https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/raw/master/builds/ksp_advanced_flybywire_x32_preview.zip
- 64 bit - https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/raw/master/builds/ksp_advanced_flybywire_x64_preview.zip

### What is this?
This is a mod for Kerbal Space Program (http://kerbalspaceprogram.com), a spaceship building/ space exploration game by Squad.
It dramatically enhances the stock input system with a bunch of fixes and many new features.

### How is it better than stock?

- Fully integrated with the stock input system. The mod will not override or break your current setup.
- Edit your control setup at any time during flight, no need to go back to the main menu to change bindings.
- Support for the Xbox 360 controller (and PS3 controller using MotionJoy) through the XInput wrapper. A built-in example preset is available.
- Supports all other controller types through the SDL wrapper.
- Unlimited number of controller buttons and axes
- Full mouse & keyboard support
- Significantly lower latency and better analog precision than KSP's stock input
- Smart dead-zone detection and analog calibration 
- Remaps analog inputs to achieve better precision for small input values
- Acceleration-based discrete inputs for precise keyboard flight
- Supports key combinations with infinite number of keys
- Multiple presets per controller
- Extremely simple to configure and use

### How to use
The mod adds a new button to the mod toolbar. Click the game controller icon or press Shift + L during flight to bring up Fly-By-Wire's main configuration screen.

From there you will see a list of detected controllers. You can click on "Enable" to enable a controller from the list - two new buttons will appear - "Presets" and "Configure".

"Presets" will open up the preset editor which is very similar to KSP's stock bindings editor. Using the preset editor you can modify your controller layout at any time.

"Configure" will open up the controller configuration screen. It allows you to set some configuration values as well as calibrate the controller if necessary.

After setting up your controller you should save your game (by using quicksave or exiting to space center), which will automatically save your controller configuration as well.

### Operating system compatibility
Fully tested and compatible with KSP x32 and x64 on Windows operating systems.
Linux/ MacOSX support available but still in early testing.
XInput support unavailable on Linux and MacOSX.

### Performance implications
The mod is extremely lightweight both on performance and memory. It only does a bit of arithmetic and remapping of incoming inputs which should have
no noticeable effect on CPU usage. Memory usage is in the order of a few megabytes.

### Limitations
There is only one known limitation so far - most EVA controls are unavailable, you should continue using the stock input system for EVA.

### Compatibility with other mods
The mod has been tested to work with over 50 of the most popular mods.

Here is a (non-exhaustive) list of mods that have been tested to be compatible:
- MechJeb
- Ferram  Aerospace Research (FAR)
- B9 Aerospace
- KW Rocketry
- OKS/ MKS
- Kethane
.. 
and many more

### Roadmap

- Linux/ MacOSX support
- Support for controller hats and trackballs
- EVA actions

### Bug reports
Please report any bugs using GitHub's built-in issue tracker or by email at alexander.dzhoganov (at) gmail (dot) com.