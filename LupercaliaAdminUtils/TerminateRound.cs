using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Admin;

namespace LupercaliaAdminUtils {
    public partial class LupercaliaAdminUtils {


        [RequiresPermissions(@"css/root")]
        private void CommandTerminateRound(CCSPlayerController? client, CommandInfo info) {
            if(client == null) 
                return;

            
            Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules?.TerminateRound(40.0F, RoundEndReason.RoundDraw);
        }
    }
}