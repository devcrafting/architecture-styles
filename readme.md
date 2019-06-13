# Architecture / domain modelling styles

This repository was created for a talk at [AlpesCraft](https://www.alpescraft.fr/).

Here are [slides in English](https://docs.google.com/presentation/d/1Msl5YVGeCy2psXepRe8phSBWBF_JIVLLrnkBydq-lvg/edit?usp=sharing) and [slides in French](https://docs.google.com/presentation/d/1Iryl3NYZjmAI8_9vd1r8XsbUG5FV1YmOQz-xzNJKFnM/edit?usp=sharing)

Examples are all based on the [Trivia refactoring kata](https://github.com/jbrains/trivia) domain (but not on its legacy code, just domain). By the way, this refactoring kata is really great ;).

Proposed achitecture / domain modelling styles are :

- AnemicDomainModel dir (+.Tests dir) uses Anemic Domain Model with ORM, i.e Entities with getters/setters only, mapped directly to SQL DB with EntityFramework, and GameServices implementing domain logic
  - NB: there is a branch `anemicWithMultipleDao` where Repository pattern is a bit deviated, i.e we don't have a clear Aggregate which we always load entirely
- RichDomainModel dir (+.Tests dir) uses Rich Domain Model with ORM, i.e Entities have only getters (no setters) and methods encapsulating behaviors, GameServices just making glue between repository and Entities methods' calls
  - NB: I could have moved tests on Game instead of GameServices
- RichDomainModelWithoutORM dir (+.Tests dir) uses Rich Domain Model without ORM, using Domain Events returned by Entities, passed to Repository to persist state (no event sourcing yet)
  - NB: querying Games is not fully implemented
- EventSourcingCQRS dir (+.Tests dir) uses an Event Sourced Domain Model, i.e Entities use Domain Events as source of truth instead of relying on persisted state, the whole history of past events allows to rebuild the current state (the same as the persisted one in RichDomainModelWithoutORM example).
  - NB: querying Games is not implemented (CQRS)
