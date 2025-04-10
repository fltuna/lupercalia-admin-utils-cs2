using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class SetTeam(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "MoveTeam";
    public override string ModuleChatPrefix => "[MoveTeam]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_team", "Set player's team", CommandSetTeam);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_team", CommandSetTeam);
    }
    

    [RequiresPermissions(@"css/generic")]
    private void CommandSetTeam(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetTeam.Command.Notification.Usage"));
            return;
        }
        
        
        int teamNumberToMove = 0;

        try
        {
            teamNumberToMove = Convert.ToInt32(info.GetArg(2));
        }
        catch (FormatException)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        catch(Exception e)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.UnknownError"));
            Plugin.Logger.LogError($"Command set team failed due to:\n{e.StackTrace}");
            return;
        }

        if (teamNumberToMove is < 1 or > 3)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidValue", "1~3"));
            return;
        }
        
        
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetNotFound"));
            return;
        }

        CsTeam targetTeam = (CsTeam) teamNumberToMove;
        
        string executorName = PlayerUtil.GetPlayerName(client);
        
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
                
                
                if (targetTeam == CsTeam.Spectator)
                {
                    target.CommitSuicide(false, true);
                }

                if (target.Team == CsTeam.Spectator && targetTeam != CsTeam.Spectator)
                {
                    MovePlayerFromSpectator(target, targetTeam);
                }
                
                PlayerUtil.SetPlayerTeam(target, targetTeam);
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            Server.PrintToChatAll(LocalizeWithPluginPrefix("SetTeam.Command.Broadcast.SetTeam", executorName, targetName, targetTeam));
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (targetTeam == CsTeam.Spectator)
            {
                target.CommitSuicide(false, true);
            }

            if (target.Team == CsTeam.Spectator && targetTeam != CsTeam.Spectator)
            {
                MovePlayerFromSpectator(target, targetTeam);
            }
            
            PlayerUtil.SetPlayerTeam(target, targetTeam);
            Server.PrintToChatAll(LocalizeWithPluginPrefix("SetTeam.Command.Broadcast.SetTeam", executorName, target.PlayerName, targetTeam));
        }
    }

    
    // This is the detour method for player is not move team correctly
    // When player is moved from spectator sometimes player is stuck at world origin.
    private void MovePlayerFromSpectator(CCSPlayerController client, CsTeam targetTeam)
    {
        PlayerUtil.SetPlayerTeam(client, targetTeam);
        client.Respawn();
        client.CommitSuicide(false, true);
        PlayerUtil.SetPlayerTeam(client, CsTeam.Spectator);
        PlayerUtil.SetPlayerTeam(client, targetTeam);
    }
}