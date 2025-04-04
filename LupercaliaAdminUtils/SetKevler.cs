﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.model;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils;

public class SetKevlar: IPluginModule
{

    public string PluginModuleName => "SetKevlar";
    
    private readonly LupercaliaAdminUtils _plugin;

    public SetKevlar(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_setkev", "Set player's kevlar armor", CommandSetKevlar);
        _plugin.AddCommand("css_setkevlar", "Set player's kevlar armor", CommandSetKevlar);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_setkev", CommandSetKevlar);
        _plugin.RemoveCommand("css_setkevlar", CommandSetKevlar);
    }



    [RequiresPermissions(@"css/generic")]
    private void CommandSetKevlar(CCSPlayerController? client, CommandInfo info)
    {
        if (client == null)
            return;
        
        
        if (info.ArgCount <= 2)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetKevlar.Command.Notification.Usage"));
            return;
        }
        
        int targetKevlarAmount = 0;

        try
        {
            targetKevlarAmount = Convert.ToInt32(info.GetArg(2));
        }
        catch (FormatException _)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        catch(Exception e)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.UnknownError"));
            _plugin.Logger.LogError($"Command set health failed due to:\n{e.StackTrace}");
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
                
                target.PlayerPawn.Value!.ArmorValue = targetKevlarAmount;
                Utilities.SetStateChanged(target.PlayerPawn.Value!, "CCSPlayerPawn", "m_ArmorValue");
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetKevlar.Command.Notification.SetKevlar", targetName, targetKevlarAmount));
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetIsDead", target.PlayerName));
                return;
            }
            
            target.PlayerPawn.Value!.ArmorValue = targetKevlarAmount;
            Utilities.SetStateChanged(target.PlayerPawn.Value!, "CCSPlayerPawn", "m_ArmorValue");
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetKevlar.Command.Notification.SetKevlar", target.PlayerName + "'s", targetKevlarAmount));
        }
    }
}