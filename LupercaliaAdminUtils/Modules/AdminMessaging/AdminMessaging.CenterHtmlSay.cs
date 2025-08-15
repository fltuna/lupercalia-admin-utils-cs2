using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Timers;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace LupercaliaAdminUtils.Modules.AdminMessaging;

public sealed partial class AdminMessaging
{
    [RequiresPermissions(@"css/generic")]
    private void CommandCenterHtmlSay(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 3)
        {
            PrintMessageToServerOrPlayerChat(client, LocalizeWithPluginPrefix(client, "AdminMessaging.Command.WithDuration.Usage", info.ArgByIndex(0).ToLower().Replace("css_", "")));
            return;
        }

        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any())
        {
            PrintMessageToServerOrPlayerChat(client, LocalizeWithPluginPrefix(client, "General.Command.Notification.TargetNotFound"));
            return;
        }
        
        if(!float.TryParse(info.ArgByIndex(2), out float duration))
        {
            PrintMessageToServerOrPlayerChat(client, LocalizeWithPluginPrefix(client, "General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        
        if (duration is < MinDuration or > MaxDuration)
        {
            PrintMessageToServerOrPlayerChat(client, LocalizeWithPluginPrefix(client, "General.Command.Notification.InvalidValue", $"{MinDuration} - {MaxDuration}"));
            return;
        }

        string executorName = PlayerUtil.GetPlayerName(client);
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));
        string message = GetPlayerMessageFromArgStringWithDuration(info.ArgString);

        Timer timer;

        if (hasTypedTargets && targets.Count() >= 2)
        {
            timer = Plugin.AddTimer(0.01F, () =>
            {
                foreach(CCSPlayerController target in targets) {
                    if(target.IsBot || target.IsHLTV)
                        continue;
                
                    target.PrintToCenterHtml(LocalizeString(target, "AdminMessaging.HtmlSay.HtmlMessage", executorName, message.ReplaceColorTags()));
                }
            },TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
        }
        else
        {
            timer = Plugin.AddTimer(0.01F, () =>
            {
                CCSPlayerController target = targets.First();
                target.PrintToCenterHtml(LocalizeString(target, "AdminMessaging.HtmlSay.HtmlMessage", executorName, message.ReplaceColorTags()));
            },TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
        }
        
        Plugin.AddTimer(duration, () =>
        {
            timer.Kill();
        }, TimerFlags.STOP_ON_MAPCHANGE);
    }
}