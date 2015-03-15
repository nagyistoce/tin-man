# PID Control #

From [Wikipedia](http://en.wikipedia.org/wiki/PID_controller):

<blockquote>
A proportional–integral–derivative controller (PID controller) is a generic control loop feedback mechanism (controller) widely used in industrial control systems – a PID is the most commonly used feedback controller. A PID controller calculates an "error" value as the difference between a measured process variable and a desired setpoint. The controller attempts to minimize the error by adjusting the process control inputs.<br>
<br>
The PID controller calculation (algorithm) involves three separate constant parameters, and is accordingly sometimes called three-term control: the proportional, the integral and derivative values, denoted P, I, and D. Heuristically, these values can be interpreted in terms of time: P depends on the present error, I on the accumulation of past errors, and D is a prediction of future errors, based on current rate of change. The weighted sum of these three actions is used to adjust the process via a control element such as the position of a control valve or the power supply of a heating element.<br>
<br>
In the absence of knowledge of the underlying process, a PID controller is the best controller. By tuning the three parameters in the PID controller algorithm, the controller can provide control action designed for specific process requirements. The response of the controller can be described in terms of the responsiveness of the controller to an error, the degree to which the controller overshoots the setpoint and the degree of system oscillation. Note that the use of the PID algorithm for control does not guarantee optimal control of the system or system stability.<br>
<br>
Some applications may require using only one or two actions to provide the appropriate system control. This is achieved by setting the other parameters to zero. A PID controller will be called a PI, PD, P or I controller in the absence of the respective control actions. PI controllers are fairly common, since derivative action is sensitive to measurement noise, whereas the absence of an integral term may prevent the system from reaching its target value due to the control action.<br>
</blockquote>

TinMan includes a software simulation of a PID controller to control hinge velocities relative to target positions.

![![](http://tin-man.googlecode.com/svn/trunk/Documentation/pid-control-small.png)](http://tin-man.googlecode.com/svn/trunk/Documentation/pid-control.png)

You can view the source of [PidHingeController.cs](http://code.google.com/p/tin-man/source/browse/trunk/TinMan/PidHingeController.cs).

# Usage #

The simulated PID controller is modelled via the [PidHingeController](http://code.google.com/p/tin-man/source/browse/trunk/TinMan/PidHingeController.cs) class.  It is quite simple to use:

```
class PidAgent : AgentBase<NaoBody>
{
    private PidHingeController _pidLAJ1;

    public PidAgent() : base(new NaoBody())
    {}

    public override void OnInitialise()
    {
        _pidLAJ1 = new PidHingeController(Body.LAJ1);
    }

    public override void Think(PerceptorState state)
    {
        _pidLAJ1.TargetAngle = Angle.FromDegrees(-90);
    }
}
```

The most likely pattern of usage is illustrated here.  An instance of `PidHingeController` is created during the initialisation stage and stored in a field.  Then, whenever required, the target angle for that joint is set and the hinge's error from that angle is minimised from that point on, until a new target angle is set.

# Adjusting Gain Constants #

Each of the three gain constants may be independently adjusted at any time:

```
_pidLAJ1.ProportionalGain = 20;
_pidLAJ1.IntegralGain = 0.1;
_pidLAJ1.DerivativeGain = 1000;
```

To disable an element of the feedback loop, set its gain to zero.  In this way you may make PI/PD/etc controllers.

# Comparison to `MoveToWithGain` #

The simplest way of controlling hinge positions is to use the `MoveToWithGain` extension method directly on the hinge:

```
Body.LAJ1.MoveToWithGain(Angle.FromDegrees(-90), 1));
```

This method applies purely proportional gain.  That is, it is a P-controller.

# Further Reading #

  * [PID without a PhD](http://igor.chudov.com/manuals/Servo-Tuning/PID-without-a-PhD.pdf) (PDF)
  * [PID controller](http://en.wikipedia.org/wiki/PID_controller) (Wikipedia)