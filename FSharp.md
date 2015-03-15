Here is some code that shows a skeleton of an agent in F#.

```
#light

module TinMan.Samples.FSharp

open TinMan

type GetSmartAgent() =
    inherit AgentBase<NaoBody>(new NaoBody()) with
        override this.Think(state) =
            // TODO perform thoughtful things
            ()

// Create and run an agent
let agent = new GetSmartAgent();
let host = new AgentHost()
host.TeamName <- "Agent86"
host.HostName <- "HQ"
host.Run(agent) |> ignore
```

F# sample code is included in the source code.