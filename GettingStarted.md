# Environment Set Up #

[Download](http://code.google.com/p/tin-man/downloads/list) either the source code or the compiled `TinMan.dll` and reference it from your project.

If you don't have a RoboCup 3D server (`rcssserver3d.exe`) installed on your computer, check one of these pages:

  * [Installation on Windows](http://simspark.sourceforge.net/wiki/index.php/Installation_on_Windows)
  * [Installation on Linux](http://simspark.sourceforge.net/wiki/index.php/Installation_on_Linux)
  * [Installation on OSX](http://simspark.sourceforge.net/wiki/index.php/Installation_on_Mac_OS_X)

You will need a monitor application to actually see what's taking place on the simulated soccer field.  You have a choice between:

  * RoboViz, which is a separate download and has advanced features.  TinMan has special support for certain RoboViz features.
  * `rcssmonitor3d`, which comes bundled with the RoboCup 3D server.

If you want to set up an IDE, then the following may be helpful:

  * [MonoDevelop setup on Ubuntu 10+](IdeMonoDevelopOnUbuntu.md)

# Your First Agent #

You don't need very much code to get started.  Here's about the minimum you can get away with:

```
using TinMan;

class MinimalAgent : AgentBase<NaoBody>
{
    public MinimalAgent()
      : base(new NaoBody()) {}

    public override void Think(PerceptorState state)
    {
        // TODO kick goal
    }

    static void Main()
    {
        // This call blocks while your agent runs
        new AgentHost().Run(new MinimalAgent());
    }
}
```

Those few lines of code will cause a Nao robot to materialise on your soccer field, arms outstretched like a zombie, practically begging you to code some life into it.

To actually move the robot, you could do something like this:

```
public override void Think(PerceptorState state)
{
    // Rotate head 90 degrees to left
    Body.HJ1.MoveToWithGain(Angle.FromDegrees(-90), 1);
}
```

Note that the `AgentHost` will call `Think` for every cycle of the simulation.  In the above example `MoveToWithGain` would be called repeatedly, but this is unnecessary.  Calling `MoveToWithGain` will move the `HJ1` hinge to the specified position and hold it there, adjusting the hinge gradually over several cycles.

# Further Reading #

  * [FAQ](FAQ.md) has some good general tips
  * [Hinges](Hinges.md) provides details on moving your agent's body
  * [Perception](Perception.md) explains how to sense your agent's environment