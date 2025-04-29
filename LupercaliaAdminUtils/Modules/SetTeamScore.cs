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

public class SetTeamScore(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "TeamScore";
    public override string ModuleChatPrefix => "[TeamScore]";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_teamscore", "Set team's score", CommandSetTeam);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_teamscore", CommandSetTeam);
    }
    

    [RequiresPermissions(@"css/generic")]
    private void CommandSetTeam(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetTeamScore.Command.Notification.Usage"));
            return;
        }
        
        if (!int.TryParse(info.GetArg(1), out int teamNumberToModify))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }

        if (teamNumberToModify is < 2 or > 3)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidValue", "2~3"));
            return;
        }

        if (!int.TryParse(info.ArgByIndex(2), out int score))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        

        CsTeam targetTeam = (CsTeam) teamNumberToModify;
        
        string executorName = PlayerUtil.GetPlayerName(client);
        
        CsTeamUtil.SetTeamScore(targetTeam, score);
        PrintLocalizedChatToAll("SetTeamScore.Command.Broadcast.ScoreChanged", executorName, targetTeam, score);
    }
}