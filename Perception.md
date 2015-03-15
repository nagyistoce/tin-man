# Introduction #

Your agent is fitted with several sensors known as _perceptors_ in RoboCup 3D parlance because they _perceive_ things.

These perceptors report their output to the agent's brain (your `think` method) via an instance of the `PerceptorState` class:

```
  public override void Think(PerceptorState state)
  {
      // 'state' holds information from perceptors
      Console.Out.WriteLine("The simulation has been running for " + state.SimulationTime);
  }
```

# Examples #

## Seeing ##

Your agent is fitted with a camera in its head.  When any of the following are within its field of view, you will be notified:

  * The ball
  * The bottoms of the corner flags
  * The top corners of both goals
  * Team mates and opposition players, and the relative arrangement of their limbs
  * Any visible field lines

Let's assume you have a localiser service that determines your agent's position and orientation on the field.  It has an interface like this:

```
  public interface ILocalizer
  {
      /// <summary>
      /// Provides the localizer with updated information about the agent's position.
      /// </summary>
      /// <param name="observedPositions">The set of observed landmark positions.</param>
      /// <param name="simulationTime">The simulation time at which the observations were recorded.</param>
      void Update(IEnumerable<LandmarkPosition> observedPositions, TimeSpan simulationTime);

      /// <summary>
      /// Gets whether the localizer has determined the agent's position and orientation.
      /// </summary>
      bool IsValueAvailable { get; }

      /// <summary>
      /// Gets the agent's current position, relative to the center of the field.
      /// </summary>
      Vector3 CurrentPosition { get; }

      /// <summary>
      /// Gets the agent's current orientation relative to the world coordinate system.
      /// </summary>
      Vector3 CurrentOrientation { get; }
  }
```

In your agent's `Think` method, you would do something like this:

```
  public override void Think(PerceptorState state)
  {
      _localizer.Update(state.LandmarkPositions, state.SimulationTime);

      if (_localizer.IsValueAvailable)
      {
          Console.Out.WriteLine("Agent is at {0} with orientation {1}",
                                _localizer.CurrentPosition, _localizer.CurrentOrientation);
      }
  }
```

## Hearing ##

Your agent can hear messages sent from its team-mates, opposition and even from itself.

```
  public override void Think(PerceptorState state)
  {
      foreach (var message in state.HeardMessages)
      {
          if (!message.IsFromSelf)
          {
              Console.Out.WriteLine("Heard message '{0}' from direction {1} at {2}",
                                    message.Text, message.RelativeDirection, message.HeardAtTime);
          }
      }
  }
```

Messages are limited in length.  The distance they can travel and the rate at which they can be transmitted is also limited by the simulation server.

Send a message using the simulation context available via `IAgent.Context` :

```
  public override void Think(PerceptorState state)
  {
      Context.Say("HelloWorld");
  }
```

## Feeling ##

Nao has a force resistance perceptor (FRPs) in either foot.  You can find out whether the foot is touching anything, and if so, the direction of the corresponding force and the location on the foot:

```
  public override void Think(ISimulationContext context, PerceptorState state)
  {
      ForceState rightFootForce = state.ForceStates.SingleOrDefault(f => f.Label == "rf");
      ForceState leftFootForce = state.ForceStates.SingleOrDefault(f => f.Label == "lf");

      Console.Out.WriteLine("Right foot force is {0} at position {1}", rightFootForce.ForceVector, rightFootForce.PointOnBody);
      Console.Out.WriteLine("Left  foot force is {0} at position {1}", leftFootForce.ForceVector,  leftFootForce.PointOnBody);
  }
```

# Reference #

Here is the full list of data available via the [PerceptorState](http://code.google.com/p/tin-man/source/browse/trunk/TinMan/PerceptorState/PerceptorState.cs) class:

|Property|Type|Description|
|:-------|:---|:----------|
|`SimulationTime`|`TimeSpan`|Gets the simulation time at which this state applies.  Simulation time is distinct from `GameTime` in that it is always increasing, even when the game's `PlayMode` means a game is not in progress.  The majority of agent hinge movement should be timed via this value.|
|`GameTime`|`TimeSpan`|Gets the length of time into the current game period.  If the `PlayMode` means that a game period is not currently in progress, then this value will be static. Note also that this value can jump backwards after the first game period.|
|`PlayMode`|`PlayMode` (enum)|Gets the current state of the soccer game.  Values may be one of: `Unknown`, `BeforeKickOff`, `KickOffLeft`, `KickOffRight`, `PlayOn`, `KickInLeft`, `KickInRight`, `CornerKickLeft`, `CornerKickRight`, `GoalKickLeft`, `GoalKickRIght`, `OffsideLeft`, `OffsideRight`, `GameOver`, `GoalLeft`, `GoalRight`, `FreeKickLeft`, `FreeKickRight`, `None`|
|`GyroStates`|`IEnumerable<GyroState>`|Gets the state of any gyrometers present in the agent's body. |
|`HingeStates`|`IEnumerable<HingeState>`|Gets the state of any hinges present in the agent's body.  Note that these hinge values are automatically applied to your agent's body prior to `Think` being called.  You shouldn't need to read these values via this property.|
|`UniversalJointStates`|`IEnumerable<UniversalJointState>`|Gets the state of any universal joins present in the agent's body.  Note that Nao doesn't use any universal joints.|
|`TouchStates`|`IEnumerable<TouchState>`|Gets the state of any touch perceptors in the agent's body.  Note that Nao doesn't use touch perceptors.|
|`ForceStates`|`IEnumerable<ForceState>`|Gets the state of any force resistance perceptors (FRP) in the agent's body.  Nao has one in either foot.|
|`AccelerometerStates`|`IEnumerable<AccelerometerState>`|Gets the state of any accelerometers in the agent's body.|
|`LandmarkPositions`|`IEnumerable<LandmarkPosition>`|Gets the relative positions of any landmarks (goals, flags) that can be seen by the agent's vision perceptor.|
|`BallPosition`|`Polar?` (nullable)|Gets the relative position of the ball, provided it can be seen by that agent's vision perceptor.|
|`TeamMatePositions`|`IEnumerable<PlayerPosition>`|Gets the relative position of any team mates that can be seen by that agent's vision perceptor.|
|`OppositionPositions`|`IEnumerable<PlayerPosition>`|Gets the relative position of any team mates that can be seen by that agent's vision perceptor.|
|`AgentBattery`|`double?` (nullable)|Gets the level remaining in the agent's battery.|
|`AgentTemperature`|`double?` (nullable)|Gets the current temperature level of the agent.|
|`HeardMessages`|`IEnumerable<HeardMessage>`|Gets any messages that were heard by the agent's audio perceptor.|
|`AgentPosition`|`Vector3?` (nullable)|Gets the position of the agent on the field in world coordinates via the Vector3's X and Y properties, with the agent's heading in the Z property. Note that this property is only ever populated by the server when a special configuration option is used.  This option is not used in competitions, however it can be useful for training your agent.  To enable this value, set `(setSenseMyPos true)` in `rcssserver3d/rsg/agent/nao/naoneckhead.rsg`.|

# Noise #

Some perceptors are inherently noisy and should have their values smoothed somehow.

Here is an example of applying a weighted average to the accelerometer's output:

```
public sealed class AccelerationSmoother
{
    private Vector3 _acc;
        
    public double Weight { get; set; }

    public AccelerationSmoother()
    {
        Weight = 0.9;
        _acc = Vector3.Origin;
    }

    public Vector3 GetSmoothedAcceleration(Vector3 acc)
    {
        _acc = (_acc*Weight) + (acc*(1 - Weight));
        return _acc;
    }
}
```

# Tips #

Some points to note when dealing with these perceptor values:

  * Certain values will always be `null` when using `NaoBody` (such as `UniversalJointStates`, and possibly `TouchStates`, `AgentBattery` and `AgentTemperature`.)
  * Some perceptors do not report their value every simulation cycle (the vision perceptor is a good example).  Make sure your design caters for this.
  * You can call `ToString()` on the `PerceptorState` object to get a formatted string of all percepts contained within the state snapshot.  This can be useful for debugging.