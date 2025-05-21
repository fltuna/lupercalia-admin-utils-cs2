# Lupercalia Admin Utils

This plugin provides convenient command for server administration.

#  Requirements

You need install these to plugin work:
- [TNCSSPluginFoundation](https://github.com/fltuna/TNCSSPluginFoundation/releases/latest)

# Installation

1. Download release file from [latest release](https://github.com/fltuna/lupercalia-admin-utils-cs2/releases/latest)
2. Download requirements (NOT REQUIRED MOST OF TIME. Requirements are included in release zip)
3. Extract zip and put into `game/csgo/couterstrikesharp` folder
4. Run server
5. Done

# Features

## Game related

### Terminate round

Terminate the round with Round draw status.

`css_endround` or type `!endround` in chat to use.


### Extend round time

Extends a round time while in the round.

`css_exttime <number of seconds>` or type `!exttime <number of seconds>` in chat to use.

or you can use `css_ert <number of seconds>` and `!ert <number of seconds>` instead.


### Extend time limit

Extends a map time limit.

This feature is moved to [MapChooserSharp](https://github.com/fltuna/MapChooserSharp)


### Extend time limit vote

Extends a map time limit if vote succeed.

This feature is moved to [MapChooserSharp](https://github.com/fltuna/MapChooserSharp)


## Player related

### Set name

Set name of player.

`css_setname <target> <name>` or type `!setname <target> <name>` in chat to use.


### Set clan tag

Set clan tag of player.

`css_clantag <target> <name>` or type `!clantag <target> <name>` in chat to use.


### Set Health

Set health of player.

`css_hp <target> <health>` or type `!hp <target> <health>` in chat to use.


### Set Kevlar

Set armor value of player.

`css_setkevlar <target> <amount> [helmet?] [heavyArmor?]` or type `!setkevlar <target> <amount> [helmet?] [heavyArmor?]` in chat to use.

or you can use `css_setkev <target> <amount> [helmet?] [heavyArmor?]` and `!setkev <target> <amount> [helmet?] [heavyArmor?]` instead.


### Respawn Player

Respawns a player.

`css_respawn <target>` or type `!respawn <target>` in chat to use.


### Set Cash

Set player's cash.

`css_cash <target> <amount>` or type `!cash <target> <amount>` in chat to use.


### Give weapon

Give weapon to player.

`css_weapon <target> <weapon> [clip] [reserveAmmo]` or type `!weapon <target> <weapon> [clip] [reserveAmmo]` in chat to use.


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

## Team related

### Set team name

Set team name appear in scoreboard.

`css_teamname <teamID> <name>` or type `!teamname <teamID> <name>` in chat to use.


### Set team score

Set team score appear in scoreboard, etc...

`css_teamscore <teamID> <name>` or type `!teamscore <teamID> <name>` in chat to use.