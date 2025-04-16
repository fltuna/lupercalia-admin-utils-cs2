using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using LupercaliaAdminUtils.util;
using Microsoft.Extensions.Logging;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class GiveWeapon(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "GiveWeapon";
    public override string ModuleChatPrefix => "[GiveWeapon]";

    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_weapon", "Give weapon to player", CommandGiveWeapon);
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_weapon", CommandGiveWeapon);
    }


    [RequiresPermissions(@"css/generic")]
    private void CommandGiveWeapon(CCSPlayerController? client, CommandInfo info)
    {
        if (info.ArgCount <= 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("GiveWeapon.Command.Notification.Usage"));
            return;
        }
        
        string weaponName = info.GetArg(2);


        
        if (!EnumUtility.TryGetEnumByEnumMemberValue("weapon_" + weaponName, out CsItem item))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("GiveWeapon.Command.Notification.WeaponNotFound", weaponName));

            StringBuilder msg = new StringBuilder();
            
            // TODO(): Cache all available weapon name.
            if (!EnumUtility.TryGetAllEnumMemberValues<CsItem>(out var list))
            {
                msg.Append("[GiveWeapon] Error! Failed to find items!");
                return;
            }
            msg.Append("========== AVAILABLE ITEMS ==========\n");

            if (client != null)
            {
                foreach (string value in list)
                {
                    if(value.Contains("item_"))
                        continue;
                    
                    client.PrintToConsole(value.Replace("weapon_", "") + '\n');
                }
            }
            else
            {
                foreach (string value in list)
                {
                    if(value.Contains("item_"))
                        continue;

                    info.ReplyToCommand(value.Replace("weapon_", "") + '\n');
                }
            }
            
            return;
        }

        if (!int.TryParse(info.ArgByIndex(3), out int clip))
        {
            clip = -1;
        }

        if (!int.TryParse(info.ArgByIndex(4), out int ammo))
        {
            ammo = -1;
        }
        
        

        TargetResult targets = info.GetArgTargetResult(1);
        
        if(!targets.Any()) {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetNotFound"));
            return;
        }

        
        string executorName = PlayerUtil.GetPlayerName(client);
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(info.GetArg(1));

        if (hasTypedTargets && targets.Count() >= 2)
        {
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
                
                if(!PlayerUtil.IsPlayerAlive(target))
                    continue;
                    
                GiveItemToPlayer(target, item, clip, ammo);
            }

            string targetName = LocalizeString(TargetTypeStringConverter.GetTargetTypeName(info.GetArg(1)));
            
            PrintLocalizedChatToAll("GiveWeapon.Command.Broadcast.GaveWeapon", executorName, item, targetName);
        }
        else
        {
            CCSPlayerController target = targets.First();

            if (!PlayerUtil.IsPlayerAlive(target))
            {
                info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.TargetIsDead"));
                return;
            }
            
            GiveItemToPlayer(target, item, clip, ammo);
            
            PrintLocalizedChatToAll("GiveWeapon.Command.Broadcast.GaveWeapon", executorName, item, target.PlayerName);
        }
    }


    private void GiveItemToPlayer(CCSPlayerController client, CsItem item, int clip = -1, int ammo = -1)
    {
        client.GiveNamedItem(item);

        if (clip == -1 && ammo == -1)
            return;
        
        Server.NextFrame(() =>
        {
            var playerPawn = client.PlayerPawn.Value;
            if (playerPawn == null)
                return;
            
            if (playerPawn.WeaponServices == null)
                return;

            var weaponServices = new CCSPlayer_WeaponServices(playerPawn.WeaponServices.Handle);
            
            foreach (CHandle<CBasePlayerWeapon> weapon in weaponServices.MyWeapons)
            {
                var weaponItem = weapon.Get();
                if (weaponItem == null)
                {
                    continue;
                }
            
            
                var weaponData = weaponItem.VData;
            
                if (weaponData == null)
                {
                    continue;
                }
        
                if(weaponItem.DesignerName != EnumUtils.GetEnumMemberAttributeValue(item))
                    continue;
                
                Server.NextFrame(() =>
                {
                    weaponItem.Clip1 = clip;
                    weaponItem.Clip2 = ammo;
                    weaponItem.ReserveAmmo[0] = ammo;
                    Utilities.SetStateChanged(weaponItem, "CBasePlayerWeapon", "m_iClip1");
                    Utilities.SetStateChanged(weaponItem, "CBasePlayerWeapon", "m_iClip2");
                    Utilities.SetStateChanged(weaponItem, "CBasePlayerWeapon", "m_pReserveAmmo");
                });
            }
        });
    }
}