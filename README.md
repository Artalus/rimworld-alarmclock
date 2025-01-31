# Alarm Clock mod for Rimworld

Adds interface to set alarm clocks / timers based on the ingame time and date.

[Download@Steam](/https://steamcommunity.com/sharedfiles/filedetails/?id=3418176587)
[Download@Github](/https://github.com/Artalus/rimworld-alarmclock/releases)


## Rationale

Did you roleplay your characters planning to do something for a specific date in future?

Are you rotating pawns between your [Outposts](https://steamcommunity.com/sharedfiles/filedetails/?id=2688941031) based on a schedule?

Is your [Insectoid Tamer](https://steamcommunity.com/sharedfiles/filedetails/?id=2636329500) working around the clock to convert a herd of Megapedes before they leave the map?

Maybe one of your Ideology leaders' abilities keep the colony from collapsing?
(tho you might want to check [Abilities Cooldown Notification](https://steamcommunity.com/sharedfiles/filedetails/?id=2558672812) for that!)

Have some peculiar ideas about [Insectors](https://steamcommunity.com/sharedfiles/filedetails/?id=3260509684)?

Got some other ridiculously reasonable requests and entirely ethical enquiries for the game to remind you of something in N days/hours?

I for sure do - but somehow there were no mods to keep track on in-game time. So I made one!


## Mod description

- A new button in the play-settings panel (bottom-right corner) toggles the draggable window with a list of timers.
- The plus button on the said list opens a dialog to set up a new timer.
- Clicking on any of the existing timers will open a dialog to edit it.
- Upon triggering, the timer will create a pop-up dialog pausing the game.
- Expired timers are kept in the list for 1 in-game day and can be reset to run again with same settings.
- Timers keep ticking even while the window is hidden.


## Compatibilities

- Can be added mid-game; can be remove mid-game with a few errors in the logs on the next load
Keep a good habit to **ALWAYS** backup your saves and modlists when changing stuff tho!
- Should be compatible with most of the "regular" mods; runs alright with my modlist of 170+ entries.
- Tested with My Little Planet; timezones should be working on non-standard planet sizes too.
- Have not been tested with any mods affecting flow of time and durations of days/months/years.
Likely won't break, but may produce inaccurate results.
- CE: yes, unless it screws with time in unimaginable ways I have not encountered yet.
- Performance-wise - GUI might have a sub-optimal implementation, but I did not see anything out of ordinary on Dub's Performance Analyzer.
Timers advance with ticks and are updated roughly twice per in-game hour.

Bug reports are welcome!


## Implementation details

- Harmony patch is used to postfix the `PlaySettings.DoPlaySettingsGlobalControls` method that renders the grid of toggle buttons.
- Timers are handled internally via `GameComponent` subclass.
It runs along with ingame Ticks, updating the state every ~1000 ticks - so the timers do not advance while the game is on pause.
- UI windows are subclassed from a regular `Window` for now.
- State is saved as part of the `.rws` XML tree.


## Known Issues

- No proper translation support yet; all strings are hardcoded English (tracked under [#1](/issues/1))
- No UI scaling support yet; timer windows **will** do stupid things if you have anything other than `1x` in UI Scale settings (tracked under [#2](/issues/2))


## Credits

My first Rimworld mod; with inspiration from [Simple Checklist](https://github.com/Garethp/rimworld-simple-checklist) by **Gareth** and some code techniques spied on [RimHud](https://github.com/Jaxe-Dev/RimHUD) by **Jaxe**.

Cute clock art on preview by [LWRZ](https://steamcommunity.com/id/170017)

Sources: [Github](https://github.com/Artalus/rimworld-alarmclock)
