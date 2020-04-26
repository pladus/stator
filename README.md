# stator
Just yet another state machine. Simple and fast.


Close development scope for v1.1:
1. async/await handlers support;
2. bounded transition for few entityes at once;
3. pipline of multiple handlers for each stage of transition;

Known issues:
1. State transition is not atomar operation. There are no any rollback in case of exception when callbacks are executing.

Getting started:

1. Add nuget package: dotnet add package Stator --version 1.0.0 or attach it to References in Visual Studio;
2. Create Entity and declare property which is means state of this Entity;
3. Create all events you need. Events must implement IEvent<TEntity> interface, where tEntity is your entity created above.
4. Use builder for select priperty which you need to manage and register transitions for it.
5. Use CommitTransition Method of builded Stator object like it wrote beneath. 

Simple example:

            // init StatorBuilder
            var stator =
                Stator<User, UserStatus>.InitStator()
                // Select state property
                .Status(x => x.Status)
                // register event. Also you can register Handler for transition miss
                .ForEvent<UserStarredEvent>()
                    // Register transition and rise back to the Event registration. 
                    // Also you can register action afer and before transitions.  
                    // And also condition of transition with it own handler.
                    .SetTransition(UserStatus.Active, UserStatus.Premium).ConfirmTransition() 
                    // to the next Event
                    .ConfirmEvent()
                    // Builder must build!
                    .Build();
            // Create Entity
            var user = new User { Status = UserStatus.Active };
            // Create Event
            var @event = new UserBlockedEvent();

            // Wwwwwzhukh!!!
            stator.CommitTransition(user, @event);

Other examples you can find in the Test project.
