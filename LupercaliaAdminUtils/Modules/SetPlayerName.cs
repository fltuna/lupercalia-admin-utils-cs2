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

public class SetPlayerName(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "SetName";
    public override string ModuleChatPrefix => "[SetName]";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_setname", "Set player's name", CommandSetName);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_setname", CommandSetName);
    }


    [RequiresPermissions(@"css/generic")]
    private void CommandSetName(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetPlayerName.Command.Notification.Usage"));
            return;
        }
        
        string nameChangeTo = info.GetArg(2);
            
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
                
                PlayerUtil.SetPlayerName(target, nameChangeTo);
            }

            string targetName = LocalizeString(TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            PrintLocalizedChatToAll("SetPlayerName.Command.Broadcast.NameChanged", executorName, targetName, nameChangeTo);
        }
        else
        {
            CCSPlayerController target = targets.First();

            string oldName = target.PlayerName;
            PlayerUtil.SetPlayerName(target, nameChangeTo);
            PrintLocalizedChatToAll("SetPlayerName.Command.Broadcast.NameChanged", executorName, oldName, nameChangeTo);
        }
    }
}