using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class ToggleBuyZone(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "ToggleBuyZone";
    public override string ModuleChatPrefix => "[ToggleBuyZone]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_buyzone", "Toggles player's buyzone status", CommandSetBuyZone);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_buyzone",  CommandSetBuyZone);
    }

    [RequiresPermissions(@"css/generic")]
    private void CommandSetBuyZone(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ToggleBuyZone.Command.Notification.Usage"));
            return;
        }
        
        
        byte treatedAsUserInBuyZone = 0;

        try
        {
            treatedAsUserInBuyZone = Convert.ToByte(info.GetArg(2));
        }
        catch (FormatException)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidValue", "0 or 1"));
            return;
        }
        catch(Exception e)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.UnknownError"));
            Plugin.Logger.LogError($"Command toggle buy zone failed due to:\n{e.StackTrace}");
            return;
        }


        if (treatedAsUserInBuyZone >= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidValue", "0 or 1"));
            return;
        }

        bool playerBuyZoneStatus = treatedAsUserInBuyZone == 1;
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetNotFound"));
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
                
                PlayerUtil.SetPlayerBuyZoneStatus(target, playerBuyZoneStatus);
            }

            string targetName = LocalizeString(TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));

            if (playerBuyZoneStatus)
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("ToggleBuyZone.Command.Notification.Enabled", targetName));
            }
            else
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("ToggleBuyZone.Command.Notification.Disabled", targetName));
            }
        }
        else
        {
            CCSPlayerController target = targets.First();
            string playerName = target.PlayerName;

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetIsDead", playerName));
                return;
            }
            
            PlayerUtil.SetPlayerBuyZoneStatus(target, playerBuyZoneStatus);
            
            
            
            if (playerBuyZoneStatus)
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("ToggleBuyZone.Command.Notification.Enabled", playerName));
            }
            else
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("ToggleBuyZone.Command.Notification.Disabled", playerName));
            }
        }
    }
}