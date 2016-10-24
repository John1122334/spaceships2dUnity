*******************
PREAMBLE:
*******************

Copyright Dylan Ede (AKA ZRM) 2013.

This plugin is licensed under the Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported license.
You can read the terms of this license at

http://creativecommons.org/licenses/by-nc-nd/3.0/

This is an early version of a plugin in active development. As such, all feedback is greatly appreciated (though please make sure that your feedback is not redundant), preferably on the development thread:

http://forum.kerbalspaceprogram.com/showthread.php/28044-WIP-Plugin-Analytical-engine-and-RCS-thrust-balancing

The primary purpose of this plugin is to balance the thrust of RCS thrusters and main engines. You can also choose from a variety of different modes for controlling your vessel.

*******************
INSTALLATION:
*******************

To install the plugin itself, extract the GameData directory to your KSP root directory. Merge folders when asked. Only replace ModuleManager.dll if it is a newer version than the one you already have installed.

If you have MechJeb 2 installed, you may wish the KerbCom Avionics module to be added to your MechJeb 2 AR202 case. In this case you should also extract the GameData inside MechJeb_config to your KSP root directory.

Otherwise, you can install KerbCom Avionics to every stock command pod. Extract the GameData directory within stock_configs to your KSP root directory.

The installation process does not overwrite any files made by others (except ModuleManager.dll if it is newer).

You can uninstall this mod by deleting the KerbComAvionics directory within your GameData directory.

*******************
CHANGELOG (for version 0.2 onwards):
*******************
0.2.0.1:
 - Fixed a small bug that caused RCS to have infinite fuel.
0.2:
 - Complete redesign of the plugin. New solver, new interface, new logic. New engine balancing support.