This package should include 2 folders:

1) template:
    A clean template for fast and easy building of .NET plugins for Notepad++
2) demo:
    An example .NET plugin for Notepad++, build upon the template above.
    It demonstrates the same functionality as the original demo plugin by Don HO:
    http://notepad-plus.sourceforge.net/commun/pluginDemoTemplate/NppPluginTemplate.zip
    I don't know if I've added new bugs, but I've corrected some small mistakes which
    are in the original demo. I've also added example code for registering icons for
    the tab of a dockable dialog and for Notepad++'s tool bar (and how to toogle its
    state).

Both template and demo come with solution files for:
   . Visual Studio 2005
   . Visual Studio 2008
   . SharpDevelop 3.1


Requirements:
----------------
   . works with .NET Runtime 2.0 and above
   . UNICODE only! ANSI is doable, but not supported so far


How to:
-------
   In the template's project folder you will find a commandline tool called "CreateLoader.exe".
   Use it to setup a ready-to-use mixed mode library (called "MyPluginName.dll" in the template
   folder and "NppManagedPluginDemo.dll" in the demo folder). This is the library which will act
   like a loader for your real plugin library. In order to qualify your created .NET plugin
   to be found by the loader, you have to place it into a subfolder which has to be named with
   the filename of the loader. Another precondition is that your (main) .NET plugin aseembly's
   root namespace has to be "NppPluginNET", else the loader will not be able to connect your
   plugin with Notepad++.
   Example:
   Notepad++ \ plugins \ SuperMegaPlugin.dll
                       \ SuperMegaPlugin     \ YourMainManagedPlugin.dll
                                             \ SomeAdditionalLib1.dll
                                             \ SomeOtherLib2.dll
   Of course you can have multiple instances of your plugin this way. Simply give the other loaders
   plus their subfolders other names.


FAQ:
----
   Q: Why place the .NET plugins into subfolders?
   A: Because Notepad++ tries to load every DLL in the plugins folder and would ask you every time
      to remove it after failing to load it. Also this way the loader is more dynamic. The same
      mixed mode DLL can be used multiple times for your .NET plugin.

   Q: Why only UNICODE support?
   A: Gimme some sense to support ANSI and I will see what I can do.
   
   Q: Why are template and demo only done in C#?
   A: Gimme some conversions and I will happily add them.


Greets & thanks:
----------------
   to the whole NPP community.. keep on rocking royally!


Enjoy
UFO