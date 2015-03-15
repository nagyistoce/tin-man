# Introduction #

The soccer server _rcssserver3d_ exposes two TCP ports for different purposes.

  1. The _Agent_ port (3100) through which agents are controlled.  `AgentHost` connects to and interacts with this port.
  1. The _Monitor_ port (3200) through which games may be manipulated in various godly ways.  `Wizard` connects to and interacts with this port.

The default monitor _rcssmonitor3d_ gets enough information via the monitor port to display a 3D view of the field, ball, agents and environment.  The server also receives various commands from monitors.

Using the `Wizard` class, your code can send these commands too.

# Example #

A common reason to use the `Wizard` is to perform **machine learning** of some game task.  Lets say you want to use some kind of [evolutionary computation](http://en.wikipedia.org/wiki/Evolutionary_computation) or [reinforcement learning](http://en.wikipedia.org/wiki/Reinforcement_learning) to **kick the ball** as far as possible.

For this, you need to run your test many times.  Firstly, you **set up** your scenario:

  * Position the agent exactly
  * Position the ball in front of the agent, exactly

Then run your test.

Finally you need to **measure** performance.

  * Where is the ball?
  * Did your agent fall over?

Rinse and repeat.

# Using the `Wizard` class #

The class is well documented and fairly straightforward.  You can [read the source code](https://code.google.com/p/tin-man/source/browse/trunk/TinMan/Wizard.cs) online too.

```
var wizard = new Wizard();
wizard.HostName = "dorothy";

// Hook up events that fire whenever the wizard observes movement.
// Note that these values are exact, unlike those observed by agents.
wizard.BallTransformUpdated  += (gameTime,transform) => Console.WriteLine("Ball position: {0}", transform.GetTranslation());
wizard.AgentTransformUpdated += (gameTime,transform) => Console.WriteLine("Agent position: {0}", transform.GetTranslation());

// 'Run' creates a message processing loop on the calling thread.
// For this sample, create a dedicated thread.
new Thread(() => wizard.Run()).Start();

// ...wait long enough for connection to occur...

// Position the left team's player with uniform number 1 in front of their goal
wizard.SetAgentPosition(1, FieldSide.Left, new Vector3(-FieldGeometry.FieldXLength/2, 0, 0));

// Simulate an attack on the goal that the agent's defending
wizard.SetBallPositionAndVelocity(new Vector3(-5, 1, 1.5), new Vector3(-1, -0.1, 0.1));

// There are quite a few other tricks up the Wizard's sleeve.  Check the API.

// Stop the message processing loop.  The thread we created above will exit.
wizard.Stop();
```

Note that connecting the Wizard can take a few seconds, so if repeated use is anticipated, keep the object alive and reuse it.