## How to run project

`dotnet run`

## Product TO-DO

### Backend Logic

- spec generic allowed passwords (configurable with limits)
- spec generic allowed usernames (configurable with limits)
- login status check
- login
- lock account if too many wrong logins (configurable)
- cannot register user twice, same/different password
- can register same password as different user
- time before automatic logout (configurable)
- time before account inactivation/deletion (configurable)
- account inactivation status
- time-window with automatic login renewal (configurable)
- login history
- 2fact (email, mobile)
- prompt 2fact at login if missing
- lock account if too many prompts (configurable)

### Backend Integration

- endpoint testing
- page server testing

### Frontend Logic

- allowed username/password checker

### Frontend UI

- component/css asserts?

### Fullstack Smoketest

- Register user and log out, then log in again

## Game TO-DO

### How Update changes state

- Assert villager moves every 4th second in a random direction
- Assert animation can cycle frames over time
- Assert villager cycles animations between idle & moving

### How state changes Draw asset render queues

- Split `GameIntegrationTests.CanRenderPlayer` into "assert villager pos affects spriteBatch queue"
- Assert diffrent animation frames generate different sprite draw calls

### How asset render queues actually render

- Split `GameIntegrationTests.CanRenderPlayer` into "assert pos 0,0 32x32 sprite at new Rectangle(-16, -16, 32, 32) render at center of screen"

## Build system TO-DO

- Index `dotnet-mgcb` etc. tools?
- Prevent `dotnet-mgcb` from updating `Content/*` when nothing's changed in `Assets/*` instead of using `<Watch Remove="Content\**\*" />` in `Game.csproj`
