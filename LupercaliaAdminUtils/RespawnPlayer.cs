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
    
    
    [RequiresPermissions(@"css/slay")]
    private void CommandRespawnPlayer(CCSPlayerController? client, CommandInfo info) {
        if(client == null) 
            return;

        if (info.ArgCount <= 1)
        {
            if (PlayerUtil.IsPlayerAlive(client))
            {
                client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetIsStillAlive", client.PlayerName));
                return;
            }
            
            client.Respawn();
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("Respawn.Command.Broadcast.PlayerRespawned", client.PlayerName, client.PlayerName));
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
                if(target.IsValid || !target.IsBot || !target.IsHLTV)
                    continue;
                
                if (PlayerUtil.IsPlayerAlive(target))
                    continue;
            
                client.PrintToChat(_plugin.LocalizeStringWithPrefix("Respawn.Command.Notification.YouHaveRespawned", client.PlayerName));
                target.Respawn();
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("Respawn.Command.Broadcast.PlayerRespawned", client.PlayerName, targetName));
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (PlayerUtil.IsPlayerAlive(target))
            {
                client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetIsStillAlive", target.PlayerName));
                return;
            }
            
            target.Respawn();
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("Respawn.Command.Notification.YouHaveRespawned", client.PlayerName));
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("Respawn.Command.Broadcast.PlayerRespawned", client.PlayerName, target.PlayerName));
        }
    }
}