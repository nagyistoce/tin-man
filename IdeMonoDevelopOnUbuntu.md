# Introduction #

[Mono](http://www.mono-project.com) is "an open source, cross-platform, implementation of C# and the CLR that is binary compatible with Microsoft.NET".

[MonoDevelop](http://monodevelop.com/) on the other hand is "an open Source C# and .NET development environment for Linux, Windows, and Mac OS X".

The machines used in most Robocup competitions are based on the Ubuntu distribution of Linux.  This page shows how to develop a RoboCup agent using TinMan and C# on MonoDevelop on Ubuntu 10.04 LTS.

The process outlined here does not vary for stable releases of Ubuntu 10.10.  If you find any differences for newer distributions, please let me know so I can update this documentation.

![https://tin-man.googlecode.com/svn-history/r201/wiki/tin-man-monodevelop-window.png](https://tin-man.googlecode.com/svn-history/r201/wiki/tin-man-monodevelop-window.png)

# Installation #

## Preparation ##

  1. Ensure that your Internet is connected and working.
  1. Go to _System>Administration>Update Manager_.  Press "Check" to check for updates to the system and it software and "Install Updates", if any. Press "Close" when finished.
  1. Go to _System>Administration>Software Source_.  Ensure that "Canonical-supported Open Source software (main)" and "Community-maintained Open Source software (universe)" are checked and "Server for United States" is selected from the drop-down menu for "Download from:", in the first, "Ubuntu Software" tab. Press "Close" when finished.

## Installing Mono & MonoDevelop ##

  1. Go to _Applications>Ubuntu Software Center_
  1. Type `MonoDevelop` in the search bar near the top-right corner of the window
  1. Click the _Install_ button when you see the MonoDevelop application in the list below

This will install MonoDevelop as well as the Mono framework.

## Running MonoDevelop ##

  1. Go to _Applications>Programming>MonoDevelop_
  1. The IDE opens with a welcome page and a "HelloWorld" project
  1. Press F5 to compile and run the project
  1. Notice the text "`Hello World`" in the _Application Output_ panel at the bottom of the IDE
  1. Go to _File>Close Solution_ to close the HelloWorld project

## Creating a TinMan project ##

Note that at this stage MonoDevelop is only installed with support for C# and VB.NET. We shall use C# in this tutorial, though most of the steps are identical for other languages.

  1. Download the latest version of TinMan from [Google Code](http://code.google.com/p/tin-man/downloads/list)
  1. Make a directory for your project somewhere on your hard drive
  1. Unzip the TinMan ZIP file to the folder you just created, perhaps in a `lib` subfolder.  You should have both `TinMan.dll` (library) and `TinMan.xml` (documentation) files.
  1. In MonoDevelop, go to _File>New>Solution_
  1. Select _C#_ and _Console Project_
  1. Give your project a name and specify the folder you created above, perhaps in a `src` subfolder, and press "Forward"
  1. Leave all items unchecked in the next dialog and press "OK"

MonoDevelop is now displaying your project.  It has created a `Main.cs` file with the same Hello World program that we saw earlier.  You can run it again (press F5) just to make sure it's still working if you like.

You must reference the TinMan library from your project:

  1. Go to the "Solution" panel (in a tab on the left hand side of the IDE by default)
  1. Right-click "References" within your project
  1. Select "Edit References" from the menu
  1. Scroll down the assembly list and check `System.Core`
  1. Go to the ".NET Assembly" tab
  1. Navigate to the `TinMan.dll` file you downloaded earlier, select it and press "Add"

Your project's references should look like this:

![https://tin-man.googlecode.com/svn-history/r201/wiki/tin-man-monodevelop-solution-references.png](https://tin-man.googlecode.com/svn-history/r201/wiki/tin-man-monodevelop-solution-references.png)

Congratulations, you're now ready to start coding your agent! Return to the [GettingStarted](GettingStarted#Your_First_Agent.md) page for some sample agent code to get started with.

# Shortcut Keys #

  * **F5** compile and run your agent
  * **Shift + F5** stop your program

_Many thanks to Osama Khan of Karachi Koalas for his help in putting together this guide!_