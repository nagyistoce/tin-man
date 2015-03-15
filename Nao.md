# Introduction #

The Nao robot is the current model for agents in the competitive RoboCup 3D Simulated Soccer league.

# Code #

In TinMan, the class [NaoBody](http://code.google.com/p/tin-man/source/browse/trunk/TinMan/NaoBody.cs) represents this agent model and provides named properties for accessing the hinges.  This class also models restrictions upon each hinge's angular range.

# Anatomy #

The Nao agent has 22 degrees of freedom (DOF) via its 22 hinges.  Each hinge is referred to by its name in code.

## Hinges ##

This diagram shows the assignment of hinge names across the agent's body.  In the posture shown, all joints are at zero degrees.

![http://tin-man.googlecode.com/svn/trunk/Documentation/tin-man-nao-joints.jpg](http://tin-man.googlecode.com/svn/trunk/Documentation/tin-man-nao-joints.jpg)

Another diagram showing hinge labels and orientations has been created by Stefan Glaser:

![![](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-joints-small.png)](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-joints.png)

Download [large PNG](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-joints.png),
[SVG](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-joints.svg)

## Body Parts ##

This diagram, also by Stefan Glaser, shows the various body parts in box form:

![![](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-boxmodel-small.png)](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-boxmodel.png)

Download [large PNG](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-boxmodel.png),
[SVG](http://tin-man.googlecode.com/svn/trunk/Documentation/nao-boxmodel.svg)