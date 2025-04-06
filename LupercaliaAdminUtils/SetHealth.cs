using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.model;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils;

public class SetHealth: IPluginModule
{

    public string PluginModuleName => "SetHealth";
    
    private readonly LupercaliaAdminUtils _plugin;

    public SetHealth(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_hp", "Set player's health", CommandSetHealth);
        _plugin.AddCommand("css_health", "Set player's health", CommandSetHealth);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_hp", CommandSetHealth);
        _plugin.RemoveCommand("css_health", CommandSetHealth);
    }



    [RequiresPermissions(@"css/generic")]
    private void CommandSetHealth(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("SetHealth.Command.Notification.Usage"));
            return;
        }
        
        int targetHealth = 0;

        try
        {
            targetHealth = Convert.ToInt32(info.GetArg(2));
        }
        catch (FormatException _)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        catch(Exception e)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.UnknownError"));
            _plugin.Logger.LogError($"Command set health failed due to:\n{e.StackTrace}");
            return;
        }

        if (targetHealth <= 0)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidValue", "1<"));
            return;
        }
            
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
                
                target.PlayerPawn.Value!.Health = targetHealth;
                target.PlayerPawn.Value.MaxHealth = targetHealth;
                Utilities.SetStateChanged(target.PlayerPawn.Value!, "CBaseEntity", "m_iHealth");
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("SetHealth.Command.Notification.SetHealth", targetName, targetHealth));
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetIsDead", target.PlayerName));
                return;
            }
            
            target.PlayerPawn.Value!.Health = targetHealth;
            target.PlayerPawn.Value.MaxHealth = targetHealth;
            Utilities.SetStateChanged(target.PlayerPawn.Value!, "CBaseEntity", "m_iHealth");
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("SetHealth.Command.Notification.SetHealth", target.PlayerName, targetHealth));
        }
    }
}