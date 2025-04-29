using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class TerminateRound(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "TerminateRound";
    public override string ModuleChatPrefix => "[TerminateRound]";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_endround", "Terminate the current round", CommandTerminateRound);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_endround", CommandTerminateRound);
    }

    [RequiresPermissions(@"css/generic")]
    private void CommandTerminateRound(CCSPlayerController? client, CommandInfo info)
    {
        PrintLocalizedChatToAll("TerminateRound.Command.Notification.Terminating");
        GameRulesUtil.TerminateRound(40.0F, RoundEndReason.RoundDraw);
    }
}