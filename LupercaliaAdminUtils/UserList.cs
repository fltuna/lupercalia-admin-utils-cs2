using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using LupercaliaAdminUtils.model;
using LupercaliaAdminUtils.util;

namespace LupercaliaAdminUtils;

public class UserList: IPluginModule
{

    public string PluginModuleName => "UserList";
    
    private readonly LupercaliaAdminUtils _plugin;

    public UserList(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_users", "Get all of player information from current server.", CommandUsers);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_users", CommandUsers);
    }
    
    private const int WidthPlayerType = 8;
    private const int WidthIsAlive = 8;
    private const int WidthPlayerSlot = 8;
    private const int WidthSteamId = 20;
    private const int WidthIpAddress = 24;
    private const int WidthPing = 5;
    private const int MaxPlayerNameLength = 36;

    private const string ConsoleLineSeparator = "-------------------------------------------------------------------------------------------------------------";
    
    // tuna: I've shortened the variable name because it was too long.
    // _os means _outputString
    private static readonly string OsPlayerType = "Type".PadRightByWidth(WidthPlayerType);
    private static readonly string OsIsAlive = "Alive".PadRightByWidth(WidthIsAlive);
    private static readonly string OsPlayerName = "Name".PadRightByWidth(MaxPlayerNameLength);
    private static readonly string OsPlayerSlot = "Slot".PadRightByWidth(WidthPlayerSlot);
    private static readonly string OsSteamId = "SteamId64".PadRightByWidth(WidthSteamId);
    private static readonly string OsIpAddress = "IpAddress".PadRightByWidth(WidthIpAddress);
    private static readonly string OsPing = "Ping".PadRightByWidth(WidthPing);

    [RequiresPermissions(@"css/generic")]
    private void CommandUsers(CCSPlayerController? client, CommandInfo info)
    {
        if (client == null)
            return;

        bool isAdminHasRootRole = AdminManager.PlayerHasPermissions(client, "css/root");

        if (isAdminHasRootRole)
        {
            client.PrintToConsole($"{OsPlayerType}{OsIsAlive}{OsPlayerName}{OsPlayerSlot}{OsSteamId}{OsIpAddress}{OsPing}");
        }
        else
        {
            client.PrintToConsole($"{OsPlayerType}{OsIsAlive}{OsPlayerName}{OsPlayerSlot}{OsSteamId}{OsPing}");
        }

        
        client.PrintToConsole(ConsoleLineSeparator);
        
        foreach (CCSPlayerController cl in Utilities.GetPlayers())
        {
            string playerType = GetPlayerTypeName(cl).PadRightByWidth(WidthPlayerType);
            
            string isAlive = PlayerUtil.IsPlayerAlive(cl) ? "Alive" : "Dead";
            isAlive = isAlive.PadRightByWidth(WidthIsAlive);

            string playerName = cl.PlayerName.TruncateByWidth(MaxPlayerNameLength).PadRightByWidth(MaxPlayerNameLength);
            
            string playerSlot = cl.Slot.ToString().PadRightByWidth(WidthPlayerSlot);

            SteamID? clSteamId = cl.AuthorizedSteamID;

            string playerSteamId;
            
            if (clSteamId != null)
            {
                playerSteamId = clSteamId.SteamId64.ToString().PadRightByWidth(WidthSteamId);
            }
            else
            {
                playerSteamId = string.Empty.PadRightByWidth(WidthSteamId);
            }

            string playerIp = cl.IpAddress ?? "NONE";
            playerIp = playerIp.PadRightByWidth(WidthIpAddress);

            string playerPing = cl.Ping.ToString().PadRightByWidth(WidthPing);


            if (isAdminHasRootRole)
            {
                client.PrintToConsole($"{playerType}{isAlive}{playerName}{playerSlot}{playerSteamId}{playerIp}{playerPing}");
            }
            else
            {
                client.PrintToConsole($"{playerType}{isAlive}{playerName}{playerSlot}{playerSteamId}{playerPing}");
            }
        }
        
        client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.SeeClientConsoleOutput"));
    }


    private string GetPlayerTypeName(CCSPlayerController client)
    {
        string result;

        if (client.IsBot)
        {
            result = "Bot";
        }
        else if (client.IsHLTV)
        {
            result = "HLTV";
        }
        else
        {
            result = "Player";
        }
        
        return result;
    }
}