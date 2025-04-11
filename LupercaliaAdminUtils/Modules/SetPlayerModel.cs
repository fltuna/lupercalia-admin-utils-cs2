using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.util;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class SetPlayerModel(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "SetPlayerModel";
    public override string ModuleChatPrefix => "[SetPlayerModel]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_setmodel", "Set player's model", CommandSetModel);
        Plugin.AddCommand("css_getmodel", "Get player's current model", CommandGetModel);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_setmodel", CommandSetModel);
        Plugin.RemoveCommand("css_getmodel", CommandGetModel);
    }
    

    [RequiresPermissions(@"css/generic")]
    private void CommandSetModel(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetPlayerModel.Command.Notification.Usage"));
            return;
        }

        string modelPath = info.GetArg(2);

        if (!modelPath.EndsWith(".vmdl"))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetPlayerModel.Command.Notification.PathShouldEndWithVmdl"));
            return;
        }
        
        string executorName = PlayerUtil.GetPlayerName(client);
        
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
                
                PlayerUtil.SetPlayerModel(target, modelPath);
            }

            string targetName = LocalizeString(TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            PrintLocalizedChatToAll("SetPlayerModel.Command.Broadcast.SetModel", executorName, targetName, modelPath);
        }
        else
        {
            CCSPlayerController target = targets.First();
            
            PlayerUtil.SetPlayerModel(target, modelPath);
            PrintLocalizedChatToAll("SetPlayerModel.Command.Broadcast.SetModel", executorName, target.PlayerName, modelPath);
        }
    }


    [RequiresPermissions(@"css/generic")]
    private void CommandGetModel(CCSPlayerController? client, CommandInfo info)
    {
        if (client == null)
            return;
        
        if (info.ArgCount <= 1)
        {
            client.PrintToChat(LocalizeWithPluginPrefix("GetPlayerModel.Command.Notification.Usage"));
            return;
        }
        
        
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            client.PrintToChat(LocalizeWithPluginPrefix("General.Command.Notification.TargetNotFound"));
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
            
            client.PrintToChat(LocalizeWithPluginPrefix("General.Command.Notification.SeeClientConsoleOutput"));
        }
        else
        {
            CCSPlayerController target = targets.First();

            string targetPlayerModel = PlayerUtil.GetPlayerModel(target);
            client.PrintToConsole("=PLAYER MODEL INFORMATION=");
            client.PrintToConsole("User\t ModelName");
            client.PrintToConsole("-------------------------------");
            client.PrintToConsole($"{target.PlayerName}\t {PlayerUtil.GetPlayerModel(target)}");
            client.PrintToChat(LocalizeWithPluginPrefix("GetPlayerModel.Command.Notification.PlayerModel", target.PlayerName, targetPlayerModel));
        }
    }
}