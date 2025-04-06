using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using LupercaliaAdminUtils.model;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils;

public class SetPlayerModel: IPluginModule
{

    public string PluginModuleName => "SetPlayerModel";
    
    private readonly LupercaliaAdminUtils _plugin;

    public SetPlayerModel(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_setmodel", "Set player's model", CommandSetModel);
        _plugin.AddCommand("css_getmodel", "Get player's current model", CommandGetModel);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_setmodel", CommandSetModel);
        _plugin.RemoveCommand("css_getmodel", CommandGetModel);
    }
    

    [RequiresPermissions(@"css/generic")]
    private void CommandSetModel(CCSPlayerController? client, CommandInfo info)
    {
        if (client == null)
            return;
        
        if (info.ArgCount <= 2)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetPlayerModel.Command.Notification.Usage"));
            return;
        }

        string modelPath = info.GetArg(2);
        
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
                
                PlayerUtil.SetPlayerModel(target, modelPath);
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("SetPlayerModel.Command.Broadcast.SetModel", client.PlayerName, targetName, modelPath));
        }
        else
        {
            CCSPlayerController target = targets.First();
            
            PlayerUtil.SetPlayerModel(target, modelPath);
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("SetPlayerModel.Command.Broadcast.SetModel", client.PlayerName, target.PlayerName, modelPath));
        }
    }


    [RequiresPermissions(@"css/generic")]
    private void CommandGetModel(CCSPlayerController? client, CommandInfo info)
    {
        if (client == null)
            return;
        
        if (info.ArgCount <= 1)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("GetPlayerModel.Command.Notification.Usage"));
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
            client.PrintToConsole("=PLAYER MODEL INFORMATION=");
            client.PrintToConsole("User\t ModelName");
            client.PrintToConsole("-------------------------------");
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
                
                if (!PlayerUtil.IsPlayerAlive(target))
                    continue;
                
                client.PrintToConsole($"{target.PlayerName}\t {PlayerUtil.GetPlayerModel(target)}");
            }
            
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.SeeClientConsoleOutput"));
        }
        else
        {
            CCSPlayerController target = targets.First();

            string targetPlayerModel = PlayerUtil.GetPlayerModel(target);
            Server.PrintToChatAll(targetPlayerModel);
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("GetPlayerModel.Command.Notification.PlayerModel", target.PlayerName, targetPlayerModel));
        }
    }
}