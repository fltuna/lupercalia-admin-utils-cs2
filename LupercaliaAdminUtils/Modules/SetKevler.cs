using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class SetKevlar(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "SetKevlar";
    public override string ModuleChatPrefix => "[SetKevlar]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_setkev", "Set player's kevlar armor", CommandSetKevlar);
        Plugin.AddCommand("css_setkevlar", "Set player's kevlar armor", CommandSetKevlar);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_setkev", CommandSetKevlar);
        Plugin.RemoveCommand("css_setkevlar", CommandSetKevlar);
    }



    [RequiresPermissions(@"css/generic")]
    private void CommandSetKevlar(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetKevlar.Command.Notification.Usage"));
            return;
        }
        
        int targetKevlarAmount = 0;
        int hasHelmet = -1;
        int hasHeavyArmor = -1;

        try
        {
            targetKevlarAmount = Convert.ToInt32(info.GetArg(2));
            if (info.ArgCount >= 4)
            {
                hasHelmet = Convert.ToInt32(info.GetArg(3));
            }

            if (info.ArgCount >= 5)
            {
                hasHeavyArmor = Convert.ToInt32(info.GetArg(4));
            }
        }
        catch (FormatException)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }
        catch(Exception e)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.UnknownError"));
            Plugin.Logger.LogError($"Command set kevlar failed due to:\n{e.StackTrace}");
            return;
        }
        
            
        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetNotFound"));
            return;
        }

        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
                
                if (!PlayerUtil.IsPlayerAlive(target))
                    continue;

                var service = new CCSPlayer_ItemServices(target.PlayerPawn.Value!.ItemServices!.Handle);

                var helmet = hasHelmet == -1 ? service.HasHelmet : Convert.ToBoolean(hasHelmet);
                var heavyArmor = hasHeavyArmor == -1 ? service.HasHeavyArmor : Convert.ToBoolean(hasHeavyArmor);
                
                PlayerUtil.SetPlayerArmor(target, targetKevlarAmount, helmet, heavyArmor);
            }

            string targetName = LocalizeString(TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetKevlar.Command.Notification.SetKevlar", targetName, targetKevlarAmount, Convert.ToBoolean(hasHelmet), Convert.ToBoolean(hasHeavyArmor)));
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetIsDead", target.PlayerName));
                return;
            }
            

            var service = new CCSPlayer_ItemServices(target.PlayerPawn.Value!.ItemServices!.Handle);

            var helmet = hasHelmet == -1 ? service.HasHelmet : Convert.ToBoolean(hasHelmet);
            var heavyArmor = hasHeavyArmor == -1 ? service.HasHeavyArmor : Convert.ToBoolean(hasHeavyArmor);

            PlayerUtil.SetPlayerArmor(target, targetKevlarAmount, helmet, heavyArmor);
            info.ReplyToCommand(LocalizeWithPluginPrefix("SetKevlar.Command.Notification.SetKevlar", target.PlayerName, targetKevlarAmount, helmet, heavyArmor));
        }
    }
}