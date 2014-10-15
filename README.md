## Advanced Fly-By-Wire v1.2
### Kerbal Space Program input system overhaul mod

Latest version - 1.2.2

[Download (Windows, x86)](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/raw/master/builds/ksp-advanced-flybywire_v1.2.2_x86.zip)

[Download (Windows, x64)](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/raw/master/builds/ksp-advanced-flybywire_v1.2.2_x64.zip)

[Licensed under the MIT License](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/blob/master/LICENSE)

You can also find the mod on:
- [Official forums](http://forum.kerbalspaceprogram.com/threads/95022-0-24-2-Advanced-Fly-by-wire-v1-0-%28Better-controller-support%29)
- [Curse.com](http://www.curse.com/ksp-mods/kerbal/224592-advanced-fly-by-wire)
- [KerbalStuff](https://kerbalstuff.com/mod/232/Advanced%20Fly-By-Wire)

## [[Poll] What should future development focus on?](https://docs.google.com/forms/d/1ao4iKmPQX0pbt0O6CqKFn-FbSyEkO6qPJCH64mp0pNg/viewform?c=0&w=1)

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
![screenshot](http://i.imgur.com/hrbVE7H.png)

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

### Limitations
There is only one known limitation so far - most EVA controls are unavailable, you should continue using the stock input system for EVA.

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

### Roadmap

- Linux/ MacOSX support
- Support for controller hats and trackballs
- EVA actions

### Bug reports
Please report any bugs using [GitHub's issue tracker](https://github.com/AlexanderDzhoganov/ksp-advanced-flybywire/issues).

### Changelog

[Click here for changelog](https://raw.githubusercontent.com/AlexanderDzhoganov/ksp-advanced-flybywire/master/CHANGELOG)


<form action="https://www.paypal.com/cgi-bin/webscr" method="post" target="_top">
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----
">
<input type="image" src="https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif" border="0" name="submit" alt="PayPal - The safer, easier way to pay online!">
<img alt="" border="0" src="https://www.paypalobjects.com/en_US/i/scr/pixel.gif" width="1" height="1">
</form>
