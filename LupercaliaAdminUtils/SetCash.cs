﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.model;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils;

public class SetCash: IPluginModule
{

    public string PluginModuleName => "SetCash";
    
    private readonly LupercaliaAdminUtils _plugin;

    public SetCash(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_cash", "Set player's cash", CommandSetCash);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_cash", CommandSetCash);
    }
    
    
    [RequiresPermissions(@"css/generic")]
    private void CommandSetCash(CCSPlayerController? client, CommandInfo info)
    {
        if (client == null)
            return;
        
        
        if (info.ArgCount <= 2)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetCash.Command.Notification.Usage"));
            return;
        }
        
        int targetCash = 0;

        try
        {
            targetCash = Convert.ToInt32(info.GetArg(2));
        }
        catch (FormatException _)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        catch(Exception e)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.UnknownError"));
            _plugin.Logger.LogError($"Command set cash failed due to:\n{e.StackTrace}");
            return;
        }

        if (targetCash <= 0)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidValue", "1<"));
            return;
        }
            
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetNotFound"));
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
                
                target.InGameMoneyServices!.Account = targetCash;
                Utilities.SetStateChanged(target, "CCSPlayerController", "m_pInGameMoneyServices");
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetCash.Command.Notification.SetCash", targetName, targetCash));
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetIsDead", target.PlayerName));
                return;
            }
            
            target.InGameMoneyServices!.Account = targetCash;
            Utilities.SetStateChanged(target, "CCSPlayerController", "m_pInGameMoneyServices");
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetCash.Command.Notification.SetCash", target.PlayerName + "'s", targetCash));
        }
    }
}