using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using LupercaliaAdminUtils.model;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils;

public class ExtendRoundTime: IPluginModule 
{
    
    public string PluginModuleName => "ExtendRoundTime";

    private readonly LupercaliaAdminUtils _plugin;
    
    public ExtendRoundTime(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
        
        _plugin.AddCommand("css_ert", "Extend round time", CommandExtendRoundTime);
        _plugin.AddCommand("css_exttime", "Extend round time", CommandExtendRoundTime);
    }
    
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
        _plugin.RemoveCommand("css_ert",  CommandExtendRoundTime);
        _plugin.RemoveCommand("css_exttime", CommandExtendRoundTime);
    }
    
    
    [RequiresPermissions(@"css/root")]
    private void CommandExtendRoundTime(CCSPlayerController? client, CommandInfo info)
    {
        if(info.ArgCount < 2)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("ExtendRoundTime.Command.Notification.Usage"));
            return;
        }

        int extendTime = 0;

        try
        {
            extendTime = Convert.ToInt32(info.GetArg(1));
        } catch (FormatException _)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        } catch(Exception e)
        {
            info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("General.Command.Notification.UnknownError"));
            _plugin.Logger.LogError($"Command extend round time failed due to:\n{e.StackTrace}");
            return;
        }
            
        CCSGameRulesProxy gameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First()!;
        CCSGameRules gameRules = gameRulesProxy.GameRules!;

        int roundTimeBefore = gameRules.RoundTime;
        gameRules.RoundTime = gameRules.RoundTime + extendTime;


        Utilities.SetStateChanged(gameRulesProxy, "CCSGameRulesProxy", "m_pGameRules");

        info.ReplyToCommand(_plugin.LocalizeStringWithPrefix("ExtendRoundTime.Command.Notification.SuccessfullyExtended", gameRules.RoundTime, roundTimeBefore));
    }
}