using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules.AdminMessaging;

public sealed partial class AdminMessaging
{
    [RequiresPermissions(@"css/generic")]
    private void CommandSomething(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            PrintMessageToServerOrPlayerChat(client, LocalizeWithPluginPrefix(client, "LocalizationKey", info.ArgByIndex(0).ToLower().Replace("css_", "")));
            return;
        }

        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any())
        {
            PrintMessageToServerOrPlayerChat(client, LocalizeWithPluginPrefix(client, "General.Command.Notification.TargetNotFound"));
            return;
        }

        string executorName = PlayerUtil.GetPlayerName(client);
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));
        string message = GetPlayerMessageFromArgString(info.ArgString);

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsBot || target.IsHLTV)
                    continue;
                
                target.PrintToChat(LocalizeString(target, "", executorName, message.ReplaceColorTags()));
            }
        }
        else
        {
            CCSPlayerController target = targets.First();
            target.PrintToChat(LocalizeString(target, "", executorName, message.ReplaceColorTags()));
        }
    }
}