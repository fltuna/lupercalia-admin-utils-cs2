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

public class SetHealth(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "SetHealth";
    public override string ModuleChatPrefix => "[SetHealth]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_hp", "Set player's health", CommandSetHealth);
        Plugin.AddCommand("css_health", "Set player's health", CommandSetHealth);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_hp", CommandSetHealth);
        Plugin.RemoveCommand("css_health", CommandSetHealth);
    }


    [RequiresPermissions(@"css/generic")]
    private void CommandSetHealth(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetHealth.Command.Notification.Usage"));
            return;
        }

        if (!int.TryParse(info.GetArg(2), out int targetHealth))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }

        if (targetHealth <= 0)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidValue", "1<"));
            return;
        }
            
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetNotFound"));
            return;
        }

        string executorName = PlayerUtil.GetPlayerName(client);
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
                
                if (!PlayerUtil.IsPlayerAlive(target))
                    continue;

                PlayerUtil.SetPlayerHealth(target, targetHealth);
                PlayerUtil.SetPlayerMaxHealth(target, targetHealth);
            }

            string targetName = LocalizeString(TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            PrintLocalizedChatToAll("SetHealth.Command.Broadcast.SetHealth", executorName, targetName, targetHealth);
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetIsDead", target.PlayerName));
                return;
            }
            
            PlayerUtil.SetPlayerHealth(target, targetHealth);
            PlayerUtil.SetPlayerMaxHealth(target, targetHealth);
            PrintLocalizedChatToAll("SetHealth.Command.Broadcast.SetHealth", executorName, target.PlayerName, targetHealth);
        }
    }
}