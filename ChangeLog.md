# 0.5.7 #

_Released 2011-05-14_

  * Fixed parser bug when receiving "nan" string for double values.
  * Added an assembly version for the first time.

# 0.5.6 #

_Released 2011-04-20_

  * Fixed parse error seen when observing an opponent player with non alpha-numeric characters in its team name (such as `Nexus3:D`)

# 0.5.5 #

_Released 2011-04-19_

  * Fixed bug where hearing certain messages from other agents would cause parser errors.
  * Added validation on `Sphere.RadiusMetres`.
  * Added dot product method to `Vector3`.
  * Added negation operator to `Vector3`.
  * Added `UnitX`, `UnitY`, `UnitZ` constants to `Vector3`.
  * Several changes to `TransformationMatrix` (still in alpha).

# 0.5.4 #

_Released 2011-04-06_

  * Updated default field dimensions to 14x21m (was 12x18m).
  * Perceptor data parser now handles line data from server, making it available on PerceptorState via instances of VisibleLine.
  * Now logging a warning if the agent attempts to beam during an invalid play mode.
  * Added `PlayMode` property to `SimulationContext`.
  * Renamed `FieldGeometry` to `Measures`, and made it an instance class.
  * Renamed `IAgent.Initialise` to `OnInitialise`.
  * Renamed `IAgent.ShutDown` to `OnShuttingDown`.
  * Renamed `AgentHost.UniformNumber` to `DesiredUniformNumber`.
  * Added `Measures` property to `AgentBase`.
  * Measures may now be optionally specified via configuration files.
  * Added `KillSimulator` method to `Wizard` class.
  * `AgentHost` now throws exceptions on property setters for invalid values, or for setting values after `Run` has been called.
  * `AgentHost` now throws an exception if team name contains characters that the server would not accept.
  * Created new `PidHingeController` class which is an implementation of a PID controller, providing an alternative control mechanism for hinge joints.
  * Appended new argument to the signature for control functions.  They now accept an instance of `PerceptorState` as well.  This was required to analyse the simulation timer for `PidHingeController` and is probably of use to other implementations of hinge control functions.
  * Created sample agent `PidAgent` for experimentation with the new `PidHingeController` class.
  * [Wizard](Wizard.md) sends correct formatting for vector values to server, irrespective of the user's current culture.  This problem is seen in regions where numbers are formatted 1.234,56 instead of 1,234.56 (such as Russia, Germany and France.)
  * Removed some comma separators from between numeric values to avoid confusion in cultures where commas are used as decimal separators.
  * Updated `WizardExample` to set agent Z-position to a sensible value such that the body does not intersect the field.
  * Improved Debug.Assert failure messages for RoboViz shape serialisation.
  * [RoboViz](RoboViz.md) `Shapes` now throw exceptions if attempts are made to set double properties to NaN or infinite values.
  * Removed unused class `PerceptorDataParser`, moving useful comments to `Parser Notes.txt`
  * Extended API documentation.

# 0.5.3 #

_Released 2011-03-11_

  * Fixed bug in `FieldAnnotation` where changing the Text property wasn't updating the bytes sent in the message.
  * Put short-circuit in RoboViz `Shape` property setters where value is unchanged to avoid setting dirty flag in some cases (this could be applied more widely later.).
  * Improved XML documentation for [Wizard](Wizard.md) class.

# 0.5.2 #

_Released 2011-02-23_

  * Added support for `FieldAnnotation`s and agent annotations, allowing textual labels to be drawn on the RoboViz field.

# 0.5.1 #

_Released 2011-02-13_

  * Additional documentation for new public members.
  * Made a few new members internal for safety.
  * Fixed bug in F# sample agent (FunctionalAgent.fs).

# 0.5.0 #

_Released 2011-02-12_

  * Added support for drawing various shapes in the [RoboViz](https://sites.google.com/site/umroboviz) monitor while the agent is running, allowing visual insight into your agent's calculations and intent.
  * Some very minor API changes that may cause compilation errors:
    * The simulation context is now a property rather than an argument to `Think`.
    * `UniformNumber` has been moved from `PerceptorState` to `ISimulationContext` in the same way that `TeamSide` moved a few releases ago.
  * Created additional `Initialise` and `Shutdown` methods that agents may optionally override to perform tasks at startup and exit.
  * Added additional constants to `FieldGeometry`.

# 0.4.3 #

_Released 2011-02-12_

  * Fixed a bug in the parser that surfaced in cultures where periods are used as thousand separators, not decimal separators.  This is an important fix for teams intending to run their agents in competitions hosted in foreign countries!

# 0.4.2 #

_Released 2011-02-10_

  * Moved `TeamSide` property from `PerceptorState` to `ISimulationContext`, fixing a bug where the property's value wasn't being set.  The server only sends this value at startup.
  * Fixed bug whereby all observed players were reported as being opponents, even if they were actually team mates.

Thanks to Saleha Raza for reporting these issues.

# 0.4.1 #

_Released 2011-02-07_

  * Fixed a bug whereby setting `IAgent.IsAlive` to false wouldn't tear down the `AgentHost`.
  * Fixed a potential runtime exception in `PerceptorState.ToString`.
  * Added generic type parameter constraint that `TBody` in `Agent<TBody>` must be a class.
  * Comparisons between floating point number now use comparison with epsilon.
  * Standardising code formatting, and replacing leading tabs with whitespace.
  * Completed API XML documentation.
  * Small improvements to `AgentHost` for both performance and readability.
  * Created `AgentHost.DefaultHostName` property.
  * Removed unnecessary method from `Wizard`.
  * Some documentation corrections for `Wizard`.
  * Added comments to disable certain ReSharper inspections.
  * Fixed a presentation bug in `PlayerPosition.ToString()`.
  * First release to include both Release and Debug builds.

# 0.4 #

_Released 2010-10-26_

  * Added parser support for `(setSenseMyPos true)` option in `naoneckhead.rsg`.  Value appears in `PerceptorState.AgentPosition`.
  * API documentation improvements.
  * New `FieldGeometry` consts `BallRadiusMetres`, `BallMassKilograms`.
  * Removed references to host `"yoda"` from sample code.