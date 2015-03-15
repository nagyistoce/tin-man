# Comparing HelloWorldAgent using libbats and TinMan #

[libbats](https://launchpad.net/littlegreenbats) is an excellent and very mature library with which to develop RoboCup agents using C++.  It contains many features that make it an excellent choice for a base library when developing in C++.

A user of TinMan asked about porting the libbats HelloWorldAgent to TinMan in C#.  On this page we will walk through that exercise.  The intention of this exercise is to introduce users who are familiar with libbats users to API and coding style of TinMan.

Here is the implementation of the `think` method from libbats' HelloWorldAgent in C++:

```
#include "helloworldagent.ih"

void HelloWorldAgent::think()
{
  // The WorldModel keeps track of the state of the world, e.g. play mode, time,
  // players/opponents/ball positions, et cetera
  WorldModel& wm = SWorldModel::getInstance();
  
  // The AgentModel keeps track of the state of the robot, e.g. joint angles,
  // location of COM, et cetera
  AgentModel& am = SAgentModel::getInstance();
  
  // The Cerebellum collects actions to perform, integrating them where necesary
  Cerebellum& cer = SCerebellum::getInstance();
  
  // Get the current time
  double t = wm.getTime();
  
  // Get the current angles of some shoulder joints
  double angles[4];
  angles[0] = am.getJoint(Types::LARM1)->angle->getMu()(0);
  angles[1] = am.getJoint(Types::LARM2)->angle->getMu()(0);
  angles[2] = am.getJoint(Types::RARM1)->angle->getMu()(0);
  angles[3] = am.getJoint(Types::RARM2)->angle->getMu()(0);
  
  // Determine target angles for joints
  double targets[4];
  targets[0] = 0.5 * M_PI;
  targets[1] = 0.25 * M_PI * sin(t / 2.0 * 2 * M_PI) + 0.25 * M_PI;
  targets[2] = 0.5 * M_PI;
  targets[3] = -0.25 * M_PI * sin(t / 2.0 * 2 * M_PI) - 0.25 * M_PI;
  
  // Determine angular velocities needed to achieve these goal angles;
  double velocities[4];
  for (unsigned i = 0; i < 4; ++i)
    velocities[i] = 0.1 * (targets[i] - angles[i]);
  
  // Add actions to the Cerebellum
  cer.addAction(new MoveJointAction(Types::LARM1, velocities[0]));
  cer.addAction(new MoveJointAction(Types::LARM2, velocities[1]));
  cer.addAction(new MoveJointAction(Types::RARM1, velocities[2]));
  cer.addAction(new MoveJointAction(Types::RARM2, velocities[3]));
  
  // Tell Cerebellum to send the actions to the server
  cer.outputCommands(SAgentSocketComm::getInstance());
}
```

Firstly, let's convert to C# using TinMan in the most direct fashion possible.  The following class is the complete definition of the agent.

```
class HelloWorldAgent : AgentBase<NaoBody>
{
    public HelloWorldAgent() : base(new NaoBody())
    {}

    /// <param name="state">Represents the state of the world (play mode, time,
    /// players/opponents/ball positions, etc) and the state of the agent (joint
    /// angles, etc).</param>
    public override void Think(PerceptorState state)
    {
        // Get the current time.
        double t = state.SimulationTime.TotalSeconds;

        // Get the current angles of some shoulder joints.
        double[] angles = new double[4];
        angles[0] = Body.LAJ1.Angle.Radians;
        angles[1] = Body.LAJ2.Angle.Radians;
        angles[2] = Body.RAJ1.Angle.Radians;
        angles[3] = Body.RAJ2.Angle.Radians;

        // Determine target angles for joints.
        double[] targets = new double[4];
        targets[0] = 0.5*Math.PI;
        targets[1] = 0.25*Math.PI*Math.Sin((t/2)*2*Math.PI) + 0.25*Math.PI;
        targets[2] = 0.5*Math.PI;
        targets[3] = -0.25*Math.PI*Math.Sin((t/2)*2*Math.PI) - 0.25*Math.PI;

        // Determine angular velocities needed to achieve these goal angles.
        const double gain = 0.1;
        double[] velocities = new double[4];
        for (int i = 0; i < 4; i++)
            velocities[i] = gain*(targets[i] - angles[i]);

        // Set joint speeds.  This will automatically generate the appropriate
        // messages to send to the server.
        Body.LAJ1.DesiredSpeed = AngularSpeed.FromRadiansPerSecond(velocities[0]);
        Body.LAJ2.DesiredSpeed = AngularSpeed.FromRadiansPerSecond(velocities[1]);
        Body.RAJ1.DesiredSpeed = AngularSpeed.FromRadiansPerSecond(velocities[2]);
        Body.RAJ2.DesiredSpeed = AngularSpeed.FromRadiansPerSecond(velocities[3]);
    }
}
```

Notice that TinMan uses .NET types for angles and angular speeds (velocities).  This avoids inadvertent errors converting between degrees/radians, and provides compile-time checking against accidentally passing and angle where an angular speed is needed.

TinMan also avoids the use of singletons, allowing for multi-threaded processes to host more than one agent.  Although this is currently forbidden in competitions, it can be useful during development.

Using some of the features of the TinMan library, this code can be condensed to the following:

```
class HelloWorldAgent : AgentBase<NaoBody>
{
    public HelloWorldAgent() : base(new NaoBody())
    {}

    /// <param name="state">Represents the state of the world (play mode, time, players/opponents/ball positions, etc) and the state of the agent (joint angles, etc).</param>
    public override void Think(PerceptorState state)
    {
        double t = state.SimulationTime.TotalSeconds;
  
        // Determine target angles for joints
        Angle laj1Target = Angle.FromRadians(0.5*Math.PI);
        Angle laj2Target = Angle.FromRadians(0.25*Math.PI*Math.Sin((t/2)*2*Math.PI) + 0.25*Math.PI);
        Angle raj1Target = Angle.FromRadians(0.5*Math.PI);
        Angle raj2Target = Angle.FromRadians(-0.25*Math.PI*Math.Sin((t/2)*2*Math.PI) - 0.25*Math.PI);

        // Set desired joint angles, and the amount of gain to apply.
        // This will automatically generate the appropriate messages to send to the server.
        const double gain = 0.1;
        Body.LAJ1.MoveToWithGain(laj1Target, gain);
        Body.LAJ2.MoveToWithGain(laj2Target, gain);
        Body.RAJ1.MoveToWithGain(raj1Target, gain);
        Body.RAJ2.MoveToWithGain(raj2Target, gain);
    }
}
```

In order to run the agent we have created here, you need some code like this:

```
public static void Main()
{
    var host = new AgentHost();
    host.Run(new HelloWorldAgent());
}
```

The primary reduction of code here relates to the method `MoveToWithGain`.  This method implements the technique of setting a speed that moves a joint towards a target by some small increment each simulation cycle.