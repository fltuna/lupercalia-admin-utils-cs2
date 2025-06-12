using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules.AdminMessaging;

public sealed partial class AdminMessaging(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "AdminMessaging";
    public override string ModuleChatPrefix => "unused";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;
    
    // Duration of message time
    private const float MaxDuration = 15.0F;
    private const float MinDuration = 1.0F;
    

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_csay", "Prints a message to player's center hud", CommandCenterHudSay);
        Plugin.AddCommand("css_asay", "Prints a message to player's center alert hud", CommandCenterAlertSay);
        Plugin.AddCommand("css_psay", "Send a private message to player", CommandPrivateSay);
        Plugin.AddCommand("css_hsay", "Prints a message to player's center HTML hud", CommandCenterHtmlSay);
        Plugin.AddCommand("css_isay", "Prints a message to player's instructor hud", CommandInstructorSay);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_csay", CommandCenterHudSay);
        Plugin.RemoveCommand("css_asay", CommandCenterAlertSay);
        Plugin.RemoveCommand("css_psay", CommandPrivateSay);
        Plugin.RemoveCommand("css_hsay", CommandCenterHtmlSay);
        Plugin.RemoveCommand("css_isay", CommandInstructorSay);
    }

    private string GetPlayerMessageFromArgString(string userInput)
    {
        var msg = userInput.Split(" ").ToList();
        msg.RemoveAt(0);
        return string.Join(" ", msg);
    }

    private string GetPlayerMessageFromArgStringWithDuration(string userInput)
    {
        var msg = userInput.Split(" ").ToList();
        msg.RemoveRange(0, 2);
        return string.Join(" ", msg);
    }
}