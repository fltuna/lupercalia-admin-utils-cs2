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

public class SetPlayerClanTag(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "SetName";
    public override string ModuleChatPrefix => "[SetName]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_clantag", "Set player's name", CommandSetClanTag);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_clantag", CommandSetClanTag);
    }


    [RequiresPermissions(@"css/generic")]
    private void CommandSetClanTag(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetPlayerClanTag.Command.Notification.Usage"));
            return;
        }
        
        string clanTag = info.GetArg(2);
            
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
                
                PlayerUtil.SetPlayerClanTag(target, clanTag);
            }

            string targetName = LocalizeString(TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetPlayerClanTag.Command.Broadcast.TagChanged", executorName, targetName, clanTag));
        }
        else
        {
            CCSPlayerController target = targets.First();

            PlayerUtil.SetPlayerClanTag(target, clanTag);
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetPlayerClanTag.Command.Broadcast.TagChanged", executorName, target.PlayerName, clanTag));
        }
    }
}