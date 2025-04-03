using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Admin;
using LupercaliaAdminUtils.model;

namespace LupercaliaAdminUtils;

public class TerminateRound: IPluginModule
{

    public string PluginModuleName => "TerminateRound";

    private readonly LupercaliaAdminUtils _plugin;
    
    public TerminateRound(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_endround", "Terminate the current round", CommandTerminateRound);
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_endround", CommandTerminateRound);
    }

    [RequiresPermissions(@"css/root")]
    private void CommandTerminateRound(CCSPlayerController? client, CommandInfo info)
    {
        if(client == null) 
            return;
            
        client.PrintToChat(_plugin.LocalizeStringWithPrefix("TerminateRound.Command.Notification.Terminating"));
        Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules?.TerminateRound(40.0F, RoundEndReason.RoundDraw);
    }
}