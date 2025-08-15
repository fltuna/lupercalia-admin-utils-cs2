using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using TNCSSPluginFoundation.Models.Plugin;

namespace LupercaliaAdminUtils.Modules;

public sealed class AdminSay(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "AdminSay";
    public override string ModuleChatPrefix => "unused";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    public FakeConVar<string> RequiredPermission = new ("css_admin_say_required_permission", "Required permission to use @say format command", "css/generic");

    protected override void OnInitialize()
    {
        Plugin.AddCommandListener("say", SayCommandListener, HookMode.Pre);
        Plugin.AddCommandListener("say_team", SayCommandListener, HookMode.Pre);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommandListener("say", SayCommandListener, HookMode.Pre);
        Plugin.RemoveCommandListener("say_team", SayCommandListener, HookMode.Pre);
    }


    private HookResult SayCommandListener(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount < 1)
            return HookResult.Continue;
        
        if (client == null)
            return HookResult.Continue;
        
        char firstChar = info.GetArg(1)[0];
        if (firstChar != '@')
            return HookResult.Continue;
        
        if (!AdminManager.PlayerHasPermissions(client, RequiredPermission.Value))
            return HookResult.Continue;



        PrintMessage(client, CreateMessageFromArg(info.ArgString));
        
        return HookResult.Handled;
    }


    private void PrintMessage(CCSPlayerController executor, string message)
    {
        foreach (CCSPlayerController client in Utilities.GetPlayers())
        {
            if (client.IsBot || client.IsHLTV)
                continue;

            if (AdminManager.PlayerHasPermissions(client, RequiredPermission.Value))
            {
                client.PrintToChat(LocalizeWithPluginPrefix(client, "AdminSay.Broadcast.Admin", executor.PlayerName, message));
            }
            else
            {
                client.PrintToChat(LocalizeWithPluginPrefix(client, "AdminSay.Broadcast.NonAdmin", message));
            }
        }
    }

    private string CreateMessageFromArg(string argString)
    {
        StringBuilder builder = new();
        
        builder.Append(argString);

        int prefixPos = argString.IndexOf('@');

        if (prefixPos >= 0)
        {
            builder.Remove(prefixPos, 1);
        }

        
        // Remove "" if arg is started by "
        // This is a different say command usage behaviour between chat box and console
        if (builder[0] == '\"')
        {
            builder.Remove(0, 1);
            builder.Remove(builder.Length - 1, 1);
        }

        return builder.ToString();
    }
}