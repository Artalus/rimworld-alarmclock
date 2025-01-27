# Timers

This mod adds interface to set alarm clocks / timers based on the ingame time and date.

~~[Download@Steam](/)~~
~~[Download@Github](/)~~


## Rationale

Did you roleplay two pawns planning to do something for a specific date in future?

Are you rotating pawns between your [Outposts](https://steamcommunity.com/sharedfiles/filedetails/?id=2688941031&searchtext=Vanilla+Outposts+Expanded) based on a schedule?

Is your [Insectoid Tamer](https://steamcommunity.com/sharedfiles/filedetails/?id=2636329500&searchtext=vanilla+memes) working around the clock to convert a herd of Megapedes before they leave the map?

Maybe one of your Ideology leaders' abilities keep the colony from collapsing?
(tho you might want to check [Abilities Cooldown Notification](https://steamcommunity.com/sharedfiles/filedetails/?id=2558672812&searchtext=notification) for that!)

Have some peculiar ideas about [Insectors](https://steamcommunity.com/sharedfiles/filedetails/?id=3260509684&searchtext=insector)?

*OR* do you have some other ridiculously reasonable requests and entirely ethical enquiries for the game to remind you of something in N days/hours?

I know I do!


## Implementation

- A new button in the play-settings panel (bottom-right corner) toggles the draggable window with a list of timers.
- The plus button under said list opens a dialog to set up a new timer.
- Clicking on any of the existing timers will open a dialog to edit it.
- Upon triggering, the timer with create a pop-up dialog pausing the game.
- Expired timers are kept in the list for 1 in-game day and can be reset to run again with same settings.


### Implementation details

- Harmony patch is used to postfix the `PlaySettings.DoPlaySettingsGlobalControls` method that renders the grid of toggle buttons.
- Timers are handled internally via `GameComponent` subclass. It runs along with ingame Ticks, so the timers do not advance while the game is on pause.
- UI windows are subclassed from a regular `Window` for now.


## Future improvements & TODOs:

- Localization support! #1
- Scaling support! #2
- Rewrite UI if there is a better alternative to raw `Window`s (wtf are `Listing`s?)
- Sliders in the edit window kinda suck; need better controls there somehow


## Credits

My first mod; with inspiration from [Simple Checklist](https://github.com/Garethp/rimworld-simple-checklist) by **Gareth** and some code techniques spied on [RimHud](https://github.com/Jaxe-Dev/RimHUD) by **Jaxe**.

Sources: [Github](https://github.com/Artalus/rimworld-alarmclock)
