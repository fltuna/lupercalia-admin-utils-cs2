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

public class SetTeam: IPluginModule
{

    public string PluginModuleName => "MoveTeam";
    
    private readonly LupercaliaAdminUtils _plugin;

    public SetTeam(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_team", "Set player's team", CommandSetTeam);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_team", CommandSetTeam);
    }
    

    [RequiresPermissions(@"css/generic")]
    private void CommandSetTeam(CCSPlayerController? client, CommandInfo info)
    {
        if (client == null)
            return;
        
        if (info.ArgCount <= 2)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("SetTeam.Command.Notification.Usage"));
            return;
        }
        
        
        int teamNumberToMove = 0;

        try
        {
            teamNumberToMove = Convert.ToInt32(info.GetArg(2));
        }
        catch (FormatException _)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        catch(Exception e)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.UnknownError"));
            _plugin.Logger.LogError($"Command set team failed due to:\n{e.StackTrace}");
            return;
        }

        if (teamNumberToMove is < 1 or > 3)
        {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidValue", "1~3"));
            return;
        }
        
        
        
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            client.PrintToChat(_plugin.LocalizeStringWithPrefix("General.Command.Notification.TargetNotFound"));
            return;
        }

        CsTeam targetTeam = (CsTeam) teamNumberToMove;
        ;
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
                
                target.ChangeTeam(targetTeam);
            }

            string targetName = TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1));
            
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("SetTeam.Command.Broadcast.SetTeam", client.PlayerName, targetName, targetTeam));
        }
        else
        {
            CCSPlayerController target = targets.First();
            
            
            target.ChangeTeam(targetTeam);
            Server.PrintToChatAll(_plugin.LocalizeStringWithPrefix("SetTeam.Command.Broadcast.SetTeam", client.PlayerName, target.PlayerName, targetTeam));
        }
    }
}