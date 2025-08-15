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
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

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
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "ExtendRoundTime.Command.Notification.Usage"));
            return;
        }

        if (!int.TryParse(info.GetArg(1), out var extendTime))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        
        int roundTimeBefore = GameRulesUtil.GetRoundTime();
        int newTime = roundTimeBefore + extendTime;
        GameRulesUtil.SetRoundTime(newTime);

        string executorName = PlayerUtil.GetPlayerName(client);

        PrintLocalizedChatToAll("ExtendRoundTime.Command.Broadcast.SuccessfullyExtended", executorName, newTime, roundTimeBefore);
    }
}