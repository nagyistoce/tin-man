# Introduction #

TinMan looks for certain optional configuration keys when your agent starts.  If they are not found, default values are used.  However, specifying these values in config means you can modify certain aspects of the agent's behaviour without recompiling your application.

# Details #

Here is a sample configuration file.  You can view it in the [source code](http://code.google.com/p/tin-man/source/browse/trunk/TinMan/App.config) as well.

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <!--
      Lengths/Distances are in metres.
      Mass is in kilograms.
    -->
    <add key="TinMan.Measures.BallRadiusMetres"     value="0.04" />
    <add key="TinMan.Measures.BallMassKilograms"    value="0.026" />
    <add key="TinMan.Measures.FieldYLength"         value="14.0" />
    <add key="TinMan.Measures.FieldXLength"         value="21.0" />
    <add key="TinMan.Measures.FieldZHeight"         value="40.0" />
    <add key="TinMan.Measures.GoalYLength"          value="2.1" />
    <add key="TinMan.Measures.GoalZLength"          value="0.8" />
    <add key="TinMan.Measures.GoalXLength"          value="0.6" />
    <add key="TinMan.Measures.PenaltyAreaXLength"   value="1.8" />
    <add key="TinMan.Measures.PenaltyAreaYLength"   value="3.9" />
    <add key="TinMan.Measures.FreeKickDistance"     value="1.3" />
    <add key="TinMan.Measures.FreeKickMoveDistance" value="1.5" />
    <add key="TinMan.Measures.GoalKickDistance"     value="1.0" />
  </appSettings>
</configuration>
```

All supported keys are shown here, with default values.  Because the values above match the defaults used by TinMan, the above configuration file has no effect at runtime.

TinMan configuration is currently only used for tracking physical properties of the simulation.  The values specified here are exposed to your agent via the [Measures](http://code.google.com/p/tin-man/source/browse/trunk/TinMan/Measures.cs) class.