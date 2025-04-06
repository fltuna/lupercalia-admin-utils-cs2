using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.model;
using LupercaliaAdminUtils.util;

namespace LupercaliaAdminUtils;

public class RespawnPlayer: IPluginModule
{

    public string PluginModuleName => "RespawnPlayer";
    
    private readonly LupercaliaAdminUtils _plugin;

    public RespawnPlayer(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_respawn", "Respawn a player", CommandRespawnPlayer);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_respawn", CommandRespawnPlayer);
    }
    
    
    [RequiresPermissions(@"css/generic")]
    private void CommandRespawnPlayer(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 1)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("Respawn.Command.Notification.Usage"));
            return;
        }
        
            
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetNotFound"));
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
            
                target.PrintToChat(_plugin.LocalizeStringWithPrefix("Respawn.Command.Notification.YouHaveRespawned", executorName));
                target.Respawn();
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("Respawn.Command.Broadcast.PlayerRespawned", executorName, targetName));
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetIsStillAlive", target.PlayerName));
                return;
            }
            
            target.Respawn();
            target.PrintToChat(_plugin.LocalizeStringWithPrefix("Respawn.Command.Notification.YouHaveRespawned", executorName));
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("Respawn.Command.Broadcast.PlayerRespawned", executorName, target.PlayerName));
        }
    }
}