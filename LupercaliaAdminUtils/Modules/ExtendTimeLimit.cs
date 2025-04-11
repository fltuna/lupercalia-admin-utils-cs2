using System.Globalization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using Microsoft.Extensions.Logging;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class ExtendTimeLimit(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "ExtendRoundTime";
    public override string ModuleChatPrefix => "[ExtendRoundTime]";

    private ConVar? mp_timelimit => ConVar.Find("mp_timelimit");
    
    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_extend", "Extend mp_timelimit", CommandExtendTimeLimit);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_extend",  CommandExtendTimeLimit);
    }
    
    
    [RequiresPermissions(@"css/root")]
    private void CommandExtendTimeLimit(CCSPlayerController? client, CommandInfo info)
    {
        if(info.ArgCount < 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Notification.Usage"));
            return;
        }

        if (!int.TryParse(info.GetArg(1), out var extendTime))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }

        if (!IsTimeLimitBased())
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Notification.IsNotTimeLimitBased"));
            return;
        }

        string executorName = PlayerUtil.GetPlayerName(client);

        if (!ExtendCurrentMap(extendTime))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Notification.ExtendFailed"));
        }
        else
        {
            Server.PrintToChatAll(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Broadcast.TimeChanged", executorName, extendTime));
        }
    }


    private bool IsTimeLimitBased()
    {
        if (mp_timelimit == null)
            return false;

        return mp_timelimit.GetPrimitiveValue<float>() != 0.0;
    }

    private bool ExtendCurrentMap(float extendTime)
    {
        if(mp_timelimit == null)
            return false;

        float oldTime = mp_timelimit.GetPrimitiveValue<float>();
        float newTime = oldTime + extendTime;

        mp_timelimit.SetValue(newTime);
        
        foreach (CCSPlayerController player in Utilities.GetPlayers())
        {
            if (player.IsHLTV)
                continue;
            
            player.ReplicateConVar(mp_timelimit.Name, mp_timelimit.GetPrimitiveValue<float>().ToString(CultureInfo.InvariantCulture));
        }
        
        Server.PrintToChatAll($"{oldTime} | {newTime}");
        return true;
    }
}