## My team name is appearing as TinManBots.  How can I change it? ##

`AgentHost` uses `TinManBots` as the default team name.  You can specify your own name like so:
```
    var host = new AgentHost();
    host.TeamName = "WizardsOfOz";
    host.Run(new WizardAgent());
```

If you like brevity, this is equivalent:

```
    var host = new AgentHost { TeamName = "WizardsOfOz" }.Run(new WizardAgent());
```

## How do I connect to a remote server? ##

By default, `AgentHost` connects to localhost.  You can specify an alternative host name like so:
```
    var host = new AgentHost();
    host.HostName = "dorothy";
    host.Run(new WizardAgent());
```

If you like brevity, this is equivalent:

```
    var host = new AgentHost { HostName = "dorothy" }.Run(new WizardAgent());
```

## How to I do XYZ with a hinge? ##

Read the page devoted to [Hinges](Hinges.md).  If it doesn't answer your question, please send me an email.

## Can I implement my own agent body model? ##

Yes, you can.  You would need to create the appropriate Ruby Scene Graph (RSG) files on the server, and an implementation of `IBody` for TinMan.

## Will this run with Mono on Linux? ##

Yes.  The released binary works fine under Mono.  It has been tested on Ubuntu 11.10 using MonoDevelop and Mono 2.10.8.

## TinMan is logging to the console.  How can I redirect its log messages to my logging framework? ##

Logging actions may be redirected via static properties on the `TinManLog` class.  For further details, see `LoggingAgent.cs` in the source code.

If you don't have a logging framework of your own, you could use TinMan's.

## How do I access the current game and simulation times? ##

Your implementation of `IAgent.Think` is passed an instance of `PerceptorState` which has two `TimeSpan` properties:

  * `SimulationTime` is the time since the simulation started.  This value is always increase between simulation cycles.  It will never decrease or remain the same between cycles.  Use this value when calculating the motion of your agent.
  * `GameTime` is the elapsed time within the game period.  It will remain at zero until play commences (kick off) and will stop again at the end of the first half.  When the second half commences, it will revert to zero.  Use this value when calculating your game play strategy.

## Help! It says it can't find `MoveToWithGain`. ##

If you get the error...

> `Error CS1061: Type 'TinMan.Hinge' does not contain a definition for 'MoveToWithGain' and no extension method 'MoveToWithGain' of type 'TinMan.Hinge' could be found (are you missing a using directive or an assembly reference?)`

...then make sure you have a reference to `System.Core.dll` in your project.

## Will _Agent Sync Mode_ work? ##

Yes.  You don't need to make any changes to your agent to run in either [agent sync mode](http://simspark.sourceforge.net/wiki/index.php/AgentSyncMode) or real time mode.  TinMan will always send the requisite [`(syn)` command](http://simspark.sourceforge.net/wiki/index.php/Effectors#Synchronize_Effector) at the end of each simulation cycle.