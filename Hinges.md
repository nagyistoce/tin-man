

# Introduction #

RoboCup3D agents are modelled as articulated rigid bodies and can be thought of as limbs connected via hinges.  In order to move the limbs, your agent software controls the hinge angles.

# Hinge Anatomy #

The physical hinge is accompanied by two active components:

  1. A sensor that measures the angle of the hinge, called a **[HingeJointPerceptor](Perceptors#HingeJointPerceptor.md)**.
  1. A motor that changes the angle of the hinge, called a **[HingeJointEffector](Effectors#HingeJointEffector.md)**.

When a hinge's angle is zero, then the axes of the two limbs it joins are parallel.  For example, when the elbow hinge is at zero degrees, the arm is straight.  Similarly, when the knee is at zero degrees, the leg is straight.

[Nao](Nao.md) has 22 hinges in his body.  So we say that Nao has 22 _degrees of freedom_ (DOF).  When all of Nao's hinges are at zero degrees he is standing upright, looking straight ahead and with arms extended out in front of him, like a zombie.

# Reading Current Hinge Position #

The state of each hinge is updated in every simulation cycle, before TinMan calls your `Think` method.  The current angle is set on each `Hinge` object on your agent's body.  For example, when using Nao.

```
// In class MyAgent : AgentBase<NaoBody>

Angle currentAngle = Body.HJ1.Angle;
Console.WriteLine("Nao's neck is at {0} degrees", currentAngle.Degrees);
```

# Moving a Hinge #

The most basic way of manipulating a hinge is to set its speed:

```
body.HJ1.DesiredSpeed = AngularSpeed.FromDegreesPerSecond(15);
```

However, in practice this rarely turns out to address your needs.  Usually you want to attain a specific angle.  Only speed values can be sent to the server, so it is up to your agent to determine what series of speeds to send for the hinge in order to achieve a given angle.

The hinge remembers its speed across multiple simulation cycles.  If, during the next call to `IAgent.Think`, you don't set a new speed for that hinge then the hinge will continue moving until it reaches its extremity.  Most speed control strategies will usually reduce the speed to zero eventually.

# Hinge Control #

It's important to note that your agent's hinges do not, by default, have any feedback loop built in.  Any control circuitry must be built explicitly into your agent.  This gives quite a bit of flexibility but can be complex.  TinMan provides a simple solution that works most of the time, but allows you to add your own in cases where it is needed.

## Simple, Proportional Control ##

TinMan comes with an example of such a control circuit via the method `MoveToWithGain(Angle,double)`.  You can use it like this:

```
public override void Think(PerceptorState state)
{
    // We only do some work if a key has been pressed, otherwise fall through
    if (Console.KeyAvailable)
    {
        char c = Console.ReadKey(true).KeyChar;
        
        // If the pressed key was a digit
        if (c >= '0' && c <= '9')
        {
            // Multiply the number pressed by 10 to get the desired angle
            int angle = (c - '0') * 10;
            
            // Move the left shoulder to that angle
            Body.LAJ1.MoveToWithGain(Angle.FromDegrees(angle), 1);
        }
    }
}
```

This agent's arm can be moved at runtime by pressing the keys `0` through `9` on the keyboard.

The key point here is:

  * The call to `MoveToWithGain` is only made once after a key press, yet movement is subsequently controlled across multiple simulation cycles.

Internally, `MoveToWithGain` registers a **callback function** that operates as the **closed loop** control circuit.  This control function remains until it is replaced, or a `DesiredSpeed` is set explicitly.

Even after the hinge reaches its desired angle and motion has ceased, the hinge is still being controlled.  For example, say your agent is hit by the ball and topples over.  The impact with the ground will cause hinges to bend.  The control function will detect this deviation from the target angle, and recommence motion to correct it.

## PID Control ##

TinMan includes a simulated [PID controller](PidHingeController.md).

## Custom Control ##

There are some cases where gain-based control is not the best choice.  For example, when the maximum power output is needed more than smooth motion, perhaps when kicking a ball, or diving across the goal in defense.

If you find that `MoveToWithGain` doesn't suit your every need then the good news is that you can use your own hinge control functions as well.  Here's a ridiculous example that sets a random speed for the every 250 milliseconds (taken from the [WavingAgent](http://code.google.com/p/tin-man/source/browse/trunk/TinMan.Samples.CSharp/WavingAgent.cs) sample).

```
var random = new Random();

Body.LAJ1.SetControlFunction((hinge, context, perceptorState) => 
    perceptorState.SimulationTime.Milliseconds % 250 == 0 
    ? AngularSpeed.FromDegreesPerSecond(random.Next(200) - 100) 
    : AngularSpeed.NaN);
```

Each hinge has a `SetControlFunction` that takes a delegate as a callback function.  In this case, we pass a lambda expression that computes a random speed.  Any sensible implementation would examine the hinge's current angle in order to close the feedback loop.

# Hinge Limits #

Just like real hinges, simulated hinges can only move through a given arc.  The `Hinge` class defines these limits:

```
// Prints: Nao's neck can rotate from -120 to 120 degrees.
Console.WriteLine("Nao's neck can rotate from {0} to {1} degrees.",
                  Body.HJ1.MinAngle.Degrees,
                  Body.HJ1.MaxAngle.Degrees);
```

Some convenience methods are defined on the `Hinge` class as well:

```
var badAngle = Angle.FromDegrees(121);

// Throws ArgumentOutOfRangeException
NaoHinge.HJ1.ValidateAngle(badAngle);

// Returns false
NaoHinge.HJ1.IsValidAngle(badAngle);

// Returns the closest valid angle for the hinge
var goodAngle = NaoHinge.HJ1.LimitAngle(badAngle);
```

<a href='Hidden comment: 

=Hinges in Practice=

RoboCup3D attempts to provide a realistic simulation of how a real robot would perform in the real world.

* Physics is simulated, including gravity, inertia and friction
* [Perceptors] have calibration error and random noise
* Joint [effectors] are modelled as electric motors

This page aims to break down the complexity.

=Characterising a Hinge=

In order to understand how hinges are modelled, let"s run through some experiments.

Let"s apply a very short burst of "force" to a shoulder hinge and see how far the arm moves before coming to rest.

Here"s an implementation of IAgent.Step that does this:

TBC

[http://en.wikipedia.org/wiki/Servo_drive Servo drive]
A servo drive is a special electronic amplifier used to power electric servo motors. It monitors feedback signals from the motor and continually adjusts for deviation from expected behavior.
'></a>

<a href='Hidden comment: 
```
public void Think(ISimulationContext context, PerceptorState state)
{
    var speed = AngularSpeed.FromDegreesPerSecond(200);
    bool isIncreasing = (state.SimulationTime.Seconds%2==0);

    // Set the neck hinge speed
    Body.HJ1.Speed = isIncreasing ? speed : -speed;
}
```

That code moves HJ1 (the neck) back and forth at 200 degrees/sec, alternating every second of the simulation.  This Step method will be called every 20 milliseconds of simulated time (which may be slower or faster than real time).  Nao will stand there shaking his head, indefinitely.  Of course, while he"s doing this his camera is reading the location of landmarks on the field, the ball and other players, all of which are being provided to this method as properties of the PerceptorState passed into this Step method.

Sometimes you need to move part of the body to an exact angle.  As only angular speeds may be sent to the hinge motor, we need a way of controlling the motion of the hinge with a negative feedback loop.

```
public void Think(ISimulationContext context, PerceptorState state)
{
    // Alternate between +/- 45 degrees, switching every two seconds
    bool isIncreasing = state.SimulationTime.Seconds%4 >= 2;
    var targetAngle = Angle.FromDegrees(isIncreasing ? 45 : -45);

    // Move Nao"s left arm, at the first joint
    MoveToAngle(_body.LAJ1, targetAngle);
}

private static void MoveToAngle(Hinge hinge, Angle targetAngle)
{
    // The speed we set is a factor of the distance.  As we approach the
    // target angle, the distance decreases and so does the angular speed.
    Angle angularDistance = targetAngle - hinge.Angle;

    // If we"re within one degree of the target, completely stop the hinge moving.
    if (angularDistance.Abs < Angle.FromDegrees(1))
      hinge.Speed = AngularSpeed.Zero;
    else
      hinge.Speed = angularDistance / TimeSpan.FromSeconds(5);
}
```

Note that we must set a value of zero for the hinge"s speed in order for the hinge to stop moving.  Each hinge maintains the same speed value across multiple simulation cycles unless it is changed.

There is no single approach for moving hinges to a particular angle.  You may want to use several approaches within the one agent for different scenarios, needing precision in one case and power in another.
'></a>