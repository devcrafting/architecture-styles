# Architecture / domain modelling styles

This repository was created for a talk at [AlpesCraft](https://www.alpescraft.fr/) and used in another talk on refactoring to DDD and Event Driven Architecture.

Here are :

- [slides around refactoring to DDD and Event Driven Architecture](https://docs.google.com/presentation/d/1gJvzkXb_-n93Sh8qHA9ROnuhImq3a3EnJkqJ-DQifsI/edit?usp=sharing) + [video in French (from 36' around Event Driven Architecture)](https://www.youtube.com/watch?v=lnG9aKXh0T4)
- [slides in English comparing different approaches](https://docs.google.com/presentation/d/1Msl5YVGeCy2psXepRe8phSBWBF_JIVLLrnkBydq-lvg/edit?usp=sharing)
- [slides in French](https://docs.google.com/presentation/d/1Iryl3NYZjmAI8_9vd1r8XsbUG5FV1YmOQz-xzNJKFnM/edit?usp=sharing)

Examples are all based on the [Trivia refactoring kata](https://github.com/jbrains/trivia) domain (but not on its legacy code, just domain). By the way, this refactoring kata is really great ;).

> SOME NOTES:
>
> - Some technical choices can be discussed, I mostly chose the straighter way to only focus on comparing architectures...I started to add some comments when I could have done it "better". **Do not hesitate to give feedbacks as issues (ideally one point per issue), it will at least keep track of them ;)**
> - I did not implement game end for now some

Proposed achitecture / domain modelling styles are :

- AnemicDomainModel dir (+.Tests dir) uses Anemic Domain Model with ORM, i.e Entities with getters/setters only, mapped directly to SQL DB with EntityFramework, and GameServices implementing domain logic
  - NB: there is a branch `anemicWithMultipleDao` where Repository pattern is a bit deviated, i.e we don't have a clear Aggregate which we always load entirely
- RichDomainModel dir (+.Tests dir) uses Rich Domain Model with ORM, i.e Entities have only getters (no setters) and methods encapsulating behaviors, GameServices just making glue between repository and Entities methods' calls
  - I used the induced way of using ORM: map Domain Model directly to DB. Another way to do it is to map Rich Domain Model to an intermediate model only used to map to DB with ORM (a 5th style to show ?). NB: I find it a bit complex compared to the next style not using ORM at all.
  - NB: I could have moved tests on Game instead of GameServices
- RichDomainModelWithoutORM dir (+.Tests dir) uses Rich Domain Model without ORM, using Domain Events returned by Entities, passed to Repository to persist state (no event sourcing yet)
  - The idea was to show how with/without ORM implementations differs, notably on read (replace SQL queries generation) and write (replace entities tracking provided by ORM, here through Domain Events)
  - NB: querying Games is not fully implemented
- EventSourcingCQRS dir (+.Tests dir) uses an Event Sourced Domain Model, i.e Entities use Domain Events as source of truth instead of relying on persisted state, the whole history of past events allows to rebuild the current state (the same as the persisted one in RichDomainModelWithoutORM example).
  - I use SQLite as event store (table Events), QuestionRepository use Read Model in SQLite (same as previous examples, without any Agregate for Question)
  - NB: querying Games is not implemented (CQRS)
