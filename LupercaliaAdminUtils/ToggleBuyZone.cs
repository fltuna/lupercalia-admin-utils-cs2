using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.model;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils;

public class ToggleBuyZone: IPluginModule
{

    public string PluginModuleName => "ToggleBuyZone";
    
    private readonly LupercaliaAdminUtils _plugin;

    public ToggleBuyZone(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_buyzone", "Toggles player's buyzone status", CommandSetBuyZone);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
    }


    [RequiresPermissions(@"css/generic")]
    private void CommandSetBuyZone(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("ToggleBuyZone.Command.Notification.Usage"));
            return;
        }
        
        
        byte treatedAsUserInBuyZone = 0;

        try
        {
            treatedAsUserInBuyZone = Convert.ToByte(info.GetArg(2));
        }
        catch (FormatException _)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidValue", "0 or 1"));
            return;
        }
        catch(Exception e)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.UnknownError"));
            _plugin.Logger.LogError($"Command toggle buy zone failed due to:\n{e.StackTrace}");
            return;
        }


        if (treatedAsUserInBuyZone >= 2)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidValue", "0 or 1"));
            return;
        }

        bool playerBuyZoneStatus = treatedAsUserInBuyZone == 1;
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetNotFound"));
            return;
        }

        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
                
                if (!PlayerUtil.IsPlayerAlive(target))
                    continue;
                
                target.PlayerPawn.Value!.InBuyZone = playerBuyZoneStatus;
                Utilities.SetStateChanged(target.PlayerPawn.Value!, "CCSPlayerPawn", "m_bInBuyZone");
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));

            if (playerBuyZoneStatus)
            {
                info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("ToggleBuyZone.Command.Notification.Enabled", targetName));
            }
            else
            {
                info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("ToggleBuyZone.Command.Notification.Disabled", targetName));
            }
        }
        else
        {
            CCSPlayerController target = targets.First();
            string playerName = target.PlayerName + "'s";

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetIsDead", playerName));
                return;
            }
            
            target.PlayerPawn.Value!.InBuyZone = playerBuyZoneStatus;
            Utilities.SetStateChanged(target.PlayerPawn.Value!, "CCSPlayerPawn", "m_bInBuyZone");
            
            
            if (playerBuyZoneStatus)
            {
                info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("ToggleBuyZone.Command.Notification.Enabled", playerName));
            }
            else
            {
                info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("ToggleBuyZone.Command.Notification.Disabled", playerName));
            }
        }
    }
}