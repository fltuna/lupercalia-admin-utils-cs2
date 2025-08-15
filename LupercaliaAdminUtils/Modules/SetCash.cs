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

public class SetCash(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "SetCash";
    public override string ModuleChatPrefix => "[SetCash]";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_cash", "Set player's cash", CommandSetCash);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_cash", CommandSetCash);
    }
    
    
    [RequiresPermissions(@"css/generic")]
    private void CommandSetCash(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "SetCash.Command.Notification.Usage"));
            return;
        }

        if (!int.TryParse(info.GetArg(2), out int targetCash))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.InvalidArgumentsInput"));
            return;
        }

        if (targetCash <= 0)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.InvalidValue", "1<"));
            return;
        }
            
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.TargetNotFound"));
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

                PlayerUtil.SetPlayerMoney(target, targetCash);
            }

            string targetName = LocalizeString(client, TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            PrintLocalizedChatToAll("SetCash.Command.Broadcast.SetCash", executorName, targetName, targetCash);
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.TargetIsDead", target.PlayerName));
                return;
            }
            
            PlayerUtil.SetPlayerMoney(target, targetCash);
            PrintLocalizedChatToAll("SetCash.Command.Broadcast.SetCash", executorName, target.PlayerName, targetCash);
        }
    }
}