using System.Drawing;
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
    private void CommandInstructorSay(CCSPlayerController? client, CommandInfo info)
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

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsBot || target.IsHLTV)
                    continue;

                ShowInstructorHintToPlayer(target, (int)duration, LocalizeString(target, "AdminMessaging.InstructorSay.Message", executorName, message));
            }
        }
        else
        {
            CCSPlayerController target = targets.First();
            ShowInstructorHintToPlayer(target, (int)duration, LocalizeString(target, "AdminMessaging.InstructorSay.Message", executorName, message));
        }
    }

   private readonly Dictionary<int, int> _showingHintCounts = new();
    
    private void ShowInstructorHintToPlayer(CCSPlayerController client, int duration, string message)
    {
        if (!_showingHintCounts.TryGetValue(client.Slot, out int _))
        {
            _showingHintCounts[client.Slot] = 1;
        }
        else
        {
            _showingHintCounts[client.Slot]++;
        }
        
        client.ReplicateConVar("sv_gameinstructor_enable", "true");
        Server.NextFrame(() =>
        {
            
            var @event = new EventInstructorServerHintCreate(true)
            {
                Userid = client,
                HintName = "test_hint",
                HintReplaceKey = "test_replace_key",
                HintTarget = client.PlayerPawn.Value!.Handle,
                HintActivatorUserid = client,
                HintTimeout = duration,
                HintIconOnscreen = "icon_alert_red",
                HintIconOffscreen = "icon_alert",
                HintCaption = "#ThisIsDangerous",
                HintActivatorCaption = message,
                HintColor = "255,255,255",
                HintIconOffset = -40.0f,
                HintRange = -50.0f,
                HintFlags = 0,
                HintBinding = "use_binding",
                HintAllowNodrawTarget = true,
                HintNooffscreen = false,
                HintForcecaption = false,
                HintLocalPlayerOnly = true
            };

            @event.FireEventToClient(client);
        });
        
        Plugin.AddTimer(duration, () =>
        {
            _showingHintCounts[client.Slot]--;
            if (_showingHintCounts[client.Slot] > 0)
                return;
            
            client.ReplicateConVar("sv_gameinstructor_enable", "false");
        });
    }
}