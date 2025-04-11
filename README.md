# Lupercalia Admin Utils

This plugin provides convenient command for server administration.

#  Requirements

You need install these to plugin work:
- [TNCSSPluginFoundation](https://github.com/fltuna/TNCSSPluginFoundation/releases/latest)
- [NativeVoteAPI](https://github.com/fltuna/NativeVoteAPI-CS2/releases/latest)

## Features

### Terminate round

Terminate the round with Round draw status.

`css_endround` or type `!endround` in chat to use.


### Extend round time

Extends a round time while in the round.

`css_exttime <number of seconds>` or type `!exttime <number of seconds>` in chat to use.

or you can use `css_ert <number of seconds>` and `!ert <number of seconds>` instead.


### Set Health

Set health of player.

`css_hp <target> <health>` or type `!hp <target> <health>` in chat to use.


### Set Kevlar

Set armor value of player.

`css_setkevlar <target> <amount>` or type `!setkevlar <target> <amount>` in chat to use.

or you can use `css_setkev <target> <amount>` and `!setkev <target> <amount>` instead.


### Respawn Player

Respawns a player.

`css_respawn <target>` or type `!respawn <target>` in chat to use.


### Set Cash

Set player's cash.

`css_cash <target> <amount>` or type `!cash <target> <amount>` in chat to use.


### Set Team

Set player's team.

`css_team <target> <team num>` or type `!team <target> <team num>` in chat to use.

1 = Spectator
2 = Terrorist
3 = CounterTerrorist


### Set Model

Set player's model.

`css_setmodel <target> <modelPath>` or type `!setmodel <target> <modelPath>` in chat to use.

### Get Model

`css_getmodel <target>` or type `!getmodel <target>` in chat to use.

If there is a multiple target found, Plugin will print the information to the console instead of chat. 


### Get Users

`css_users` or type `!users` in chat to use.

It will print the these information about all clients in the server to client console.

1. PlayerType: Bot, HLTV or Player
2. IsAlive: Alive or Dead
3. PlayerName: Player name, but when player name is longer than 32 characters, it will be cropped name is shown
4. PlayerSlot: This slot number is useful for targeting players. (for instance: !hp #<slotNumber> 100)
5. SteamID: Prints a SteamID64
6. IpAddress: Only user who has a root role can see this information
7. Ping: User's ping