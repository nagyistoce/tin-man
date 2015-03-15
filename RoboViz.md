# RoboViz #



RoboViz is a project by Justin Stoecker in collaboration with the RoboCup group at the University of Miami's Department of Computer Science.  It offers several features, as shown in this video:

<a href='http://www.youtube.com/watch?feature=player_embedded&v=RMJFWrV3GZg' target='_blank'><img src='http://img.youtube.com/vi/RMJFWrV3GZg/0.jpg' width='800' height=480 /></a>

The remainder of this article deals with TinMan's support for the programmable drawing feature (seen from around 1:20 in the video.)

You can read more about RoboViz at [the project website](http://sites.google.com/site/umroboviz/).

# Programmable Drawing #

In order to draw on the visualisation, your agent must connect directly to the monitor and transmit commands.  In TinMan, this is done via an object-oriented API.

## A Simple Example - Displaying Localisation Calculation ##

Let's implement an agent that draws a dot on the screen to show where it thinks it is.  TinMan does not currently include a localiser implementation, so let's assume we have a simple localisation service available with the following interface:

```
public interface ILocaliser
{
    bool Update(PerceptorState state);
    Vector3 GetPosition();
}
```

And here is the code for our complete agent:

```
public sealed class LocalisationSimpleVisualisationAgent : AgentBase<NaoBody>
{
    private readonly ILocaliser _localiser;
    private Dot _locationDot;

    public LocalisationSimpleVisualisationAgent(ILocaliser localiser)
        : base(new NaoBody())
    {
        _localiser = localiser;
    }

    public override void Initialise()
    {
        // Request that a RoboViz remote is created.
        // It will be disposed of automatically when the agent exits.
        var roboViz = new RoboVizRemote();

        // Create a 5-pixel, blue dot to indicate the agent's calculated position, hidden initially.
        // Keep a reference to this object so that we can manipulate it later.
        _locationDot = new Dot { PixelSize = 5, Color = Color.Blue, IsVisible = false };

        // Add it to the RoboViz remote using the group name 'localisation'.
        // This name will be prefixed with the side and agent number automatically.
        roboViz.Add(new ShapeSet("localisation") { _locationDot });
    }

    public override void Think(PerceptorState state)
    {
        // Provide the localiser with the latest state and see if it has a position for us.
        if (_localiser.Update(state))
        {
            // Update the dot's position.  This will automatically be reflected in the RoboViz UI.
            _locationDot.Position = _localiser.Position;

            // Make sure it's visible now that we have a position.
            _locationDot.IsVisible = true;
        }
    }
}
```

## A Complex Example ##

RoboViz ships with a demo, `RVTester`.  This class has been ported to `TinMan` and is visible as [RoboVizDemoAgent.cs](http://code.google.com/p/tin-man/source/browse/trunk/TinMan.Samples.CSharp/RoboVizDemoAgent.cs):

Here is a screenshot showing a variety of annotations drawn over the `RoboViz` field:

<img src='http://tin-man.googlecode.com/svn/trunk/Documentation/robocup-with-roboviz-annotations.png' />

## Key Concepts ##

The key concepts here are:

  1. Create a `RoboVizRemote`.
  1. Create a `ShapeSet` and add it to the remote.
  1. Create `Shape` object(s) and add them to the `ShapeSet`.
  1. Using stored references to the `Shape` object(s), manipulate their position, colour, geometry and visibility as the agent runs.

## Creating a `RoboVizRemote` ##

To create a remote using the default options, issue the call as in the above example:

```
    var roboViz = new RoboVizRemote();
```

If custom options are required, use this alternative overload:

```
    // Specify one or more non-default options
    var options = new RoboVizOptions
                        {
                            HostName = "somehost",
                            Port = 12345,
                            UseDefaultPrefix = false
                        };
    var roboViz = new RoboVizRemote(options);
```

## Shape Sets ##

`ShapeSet` is a composite type, meaning that it may contain instances of child `ShapeSet` objects and can therefore resemble a tree structure.  A `ShapeSet` may also contain zero or more `Shape` objects.  This facility allows sub-tree filtering of `Shape` objects in the RoboViz UI.

Each `ShapeSet` has a path that corresponds to its position in the hierarchy.  Examples are:

  * `R.A1.localisation.ball`
  * `R.A1.localisation.robot.translation`
  * `R.A1.localisation.robot.orientation`
  * `R.A1.localisation.robot.uprightvector`
  * `R.A1.planning.trajectory`

By default, an agent's team side (R or L) and uniform number (A1, A2...) are prefixed to the `ShapeSet`'s path.  This can be disabled via `RoboVizOptions.UseDefaultPrefix`.

`ShapeSet` defines the following public members:

```
string Path { get; }
void Add(Shape shape);
void AddRange(IEnumerable<Shape> shapes);
void Add(ShapeSet childSubSet);
void Translate(Vector3 offset);
void Remove(Shape shape);
```

## Shapes ##

RoboViz supports:

  * Dot
  * Line
  * Polygon
  * Circle
  * Sphere

Each of these has a class in the `TinMan.RoboViz`, and each of these classes derive from the abstract `Shape` class.  All shapes therefore inherit the following public members:

```
bool IsVisible { get; set; }
void Remove();
void Translate(Vector3 offset);
```

### Dot ###

A coloured point in 3D space.  Its size remains the same regardless of its distance from the camera, hence the size unit is pixels.  Note that `X`,`Y`,`Z` are equivalent to `Position`, so use whichever is more convenient at the time.

```
Color Color { get; set; }
double PixelSize { get; set; }
Vector3 Position { get; set; }
double X { get; set; }
double Y { get; set; }
double Z { get; set; }
```

### Line ###

A coloured point in 3D space.  Its size remains the same regardless of its distance from the camera, hence the size unit is pixels.  Note that `X1`,`Y1`,`Z1` are equivalent to `End1` (the same applies for `*2`), so use whichever is more convenient at the time.

```
Color Color { get; set; }
double PixelThickness { get; set; }
Vector3 End1 { get; set; }
Vector3 End2 { get; set; }
double X1 { get; set; }
double Y1 { get; set; }
double Z1 { get; set; }
double X2 { get; set; }
double Y2 { get; set; }
double Z2 { get; set; }
```

### Polygon ###

A borderless, filled polygon in 3D space formed from a list of vertices.

```
Color Color { get; set; }
Vector3 this[int index] { get; set; }
void Add(Vector3 vertex);
void AddRange(IEnumerable<Vector3> vertices);
void RemoveAt(int index);
void InsertAt(int index, Vector3 vertex);
void Clear();
```

### Circle ###

An unfilled 2D circle, constrained to the horizontal plane of the field.  Its line thickness remains the same regardless of its distance from the camera, hence the thickness unit is pixels.

```
Color Color { get; set; }
double RadiusMetres { get; set; }
double PixelThickness { get; set; }
double CenterX { get; set; }
double CenterY { get; set; }
```

### Sphere ###

A solid sphere in 3D space.  Note that `X`,`Y`,`Z` are equivalent to `Center`, so use whichever is more convenient at the time.

```
Color Color { get; set; }
double RadiusMetres { get; set; }
Vector3 Center { get; set; }
double X { get; set; }
double Y { get; set; }
double Z { get; set; }
```


### FieldAnnotation ###

A label of text pinned in 3D space.  Note that `X`,`Y`,`Z` are equivalent to `Center`, so use whichever is more convenient at the time.

```
Color Color { get; set; }
Vector3 Center { get; set; }
double X { get; set; }
double Y { get; set; }
double Z { get; set; }
string Text { get; set; }
```

## Agent Annotation ##

An agent may specify an annotation that floats above its body as it moves around the field.  This value is set directly upon `RoboVizRemote`.

```
public override void Initialise()
{
    var roboViz = new RoboVizRemote(this);
    roboViz.AgentText = "Hello World";
    roboViz.AgentTextColor = Color.Red;
}
```

If you need to change this text during the agent's lifetime (perhaps to display its current 'mode' or some kind of decision), just keep a reference to your instance of `RoboVizRemote` and update the `AgentText` property as needed from within your `Think` method.