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

public class SetTeamName(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "SetTeamName";
    public override string ModuleChatPrefix => "[SetTeamName]";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_teamname", "Set team's name", CommandSetTeam);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_teamname", CommandSetTeam);
    }
    

    [RequiresPermissions(@"css/generic")]
    private void CommandSetTeam(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "SetTeamName.Command.Notification.Usage"));
            return;
        }

        if (!int.TryParse(info.GetArg(1), out int teamNumberToModify))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.InvalidArgumentsInput"));
            return;
        }

        if (teamNumberToModify is < 2 or > 3)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix(client, "General.Command.Notification.InvalidValue", "2~3"));
            return;
        }


        string newTeamName = info.ArgByIndex(2);
        CsTeam targetTeam = (CsTeam) teamNumberToModify;
        
        string executorName = PlayerUtil.GetPlayerName(client);

        CsTeamUtil.SetTeamName(targetTeam, newTeamName);
        PrintLocalizedChatToAll("SetTeamName.Command.Broadcast.NameChanged", executorName, targetTeam, newTeamName);
    }
}