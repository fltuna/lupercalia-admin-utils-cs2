using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class ExtendRoundTime(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "ExtendRoundTime";
    public override string ModuleChatPrefix => "[ExtendRoundTime]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_ert", "Extend round time", CommandExtendRoundTime);
        Plugin.AddCommand("css_exttime", "Extend round time", CommandExtendRoundTime);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_ert",  CommandExtendRoundTime);
        Plugin.RemoveCommand("css_exttime", CommandExtendRoundTime);
    }
    
    
    [RequiresPermissions(@"css/root")]
    private void CommandExtendRoundTime(CCSPlayerController? client, CommandInfo info)
    {
        if(info.ArgCount < 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendRoundTime.Command.Notification.Usage"));
            return;
        }

        int extendTime = 0;

        try
        {
            extendTime = Convert.ToInt32(info.GetArg(1));
        } catch (FormatException)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        } catch(Exception e)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.UnknownError"));
            Plugin.Logger.LogError($"Command extend round time failed due to:\n{e.StackTrace}");
            return;
        }
        
        int roundTimeBefore = GameRulesUtil.GetRoundTime();
        int newTime = roundTimeBefore + extendTime;
        GameRulesUtil.SetRoundTime(newTime);

        info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendRoundTime.Command.Notification.SuccessfullyExtended", newTime, roundTimeBefore));
    }
}