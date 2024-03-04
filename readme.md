## Product TO-DO

### Backend Logic

- account permanence
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

- Index `dotnet-mgcb` etc. tools?
- Prevent `dotnet-mgcb` from updating `Content/*` when nothing's changed in `Assets/*` instead of using `<Watch Remove="Content\**\*" />` in `Game.csproj`
