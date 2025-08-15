using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using LupercaliaAdminUtils.util;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class RespawnPlayer(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "RespawnPlayer";
    public override string ModuleChatPrefix => "[RespawnPlayer]";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_respawn", "Respawn a player", CommandRespawnPlayer);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_respawn", CommandRespawnPlayer);
    }
    
    
    [RequiresPermissions(@"css/generic")]
    private void CommandRespawnPlayer(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 1)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "Respawn.Command.Notification.Usage"));
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
                
                if (PlayerUtil.IsPlayerAlive(target))
                    continue;
                
                if (target.Team is CsTeam.None or CsTeam.Spectator)
                    continue;
            
                target.PrintToChat(LocalizeWithPluginPrefix(client, "Respawn.Command.Notification.YouHaveRespawned", executorName));
                target.Respawn();
            }

            string targetName = LocalizeString(client, TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            PrintLocalizedChatToAll("Respawn.Command.Broadcast.PlayerRespawned", executorName, targetName);
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.TargetIsStillAlive", target.PlayerName));
                return;
            }

            if (target.Team is CsTeam.None or CsTeam.Spectator)
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.TargetNotFound"));
            }
            
            target.Respawn();
            target.PrintToChat(LocalizeWithPluginPrefix(client, "Respawn.Command.Notification.YouHaveRespawned", executorName));
            PrintLocalizedChatToAll("Respawn.Command.Broadcast.PlayerRespawned", executorName, target.PlayerName);
        }
    }
}