using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Admin;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils {
    public partial class LupercaliaAdminUtils {

        [RequiresPermissions(@"css/root")]
        private void CommandExtendRoundTime(CCSPlayerController? client, CommandInfo info) {
            if(client == null) 
                return;

            if(info.ArgCount < 2) {
                client.PrintToChat(this.LocalizeStringWithPrefix("ExtendRoundTime.Command.Notification.Usage"));
                return;
            }

            int extendTime = 0;

            try {
                extendTime = Convert.ToInt32(info.GetArg(1));
            } catch (FormatException _){
                client.PrintToChat(this.LocalizeStringWithPrefix("General.Command.Notification.InvalidArgumentsInput"));
                return;
            } catch(Exception e) {
                client.PrintToChat(this.LocalizeStringWithPrefix("General.Command.Notification.UnknownError"));
                Logger.LogError($"Command extend round time failed due to:\n{e.StackTrace}");
                return;
            }
            
            CCSGameRulesProxy gameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First()!;
            CCSGameRules gameRules = gameRulesProxy.GameRules!;

            int roundTimeBefore = gameRules.RoundTime;
            gameRules.RoundTime = gameRules.RoundTime + extendTime;


            Utilities.SetStateChanged(gameRulesProxy, "CCSGameRulesProxy", "m_pGameRules");

            client.PrintToChat(this.LocalizeStringWithPrefix("ExtendRoundTime.Command.Notification.SuccessfullyExtended", gameRules.RoundTime, roundTimeBefore));
        }
    }
}