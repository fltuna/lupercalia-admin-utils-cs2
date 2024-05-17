using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace LupercaliaAdminUtils {
    public partial class LupercaliaAdminUtils: BasePlugin {
        public static readonly string PLUGIN_PREFIX  = $" {ChatColors.DarkRed}[{ChatColors.Blue}LPŘ AU{ChatColors.DarkRed}]{ChatColors.Default}";

        public static string MessageWithPrefix(string message) {
            return $"{PLUGIN_PREFIX} {message}";
        }

        public override string ModuleName => "Lupercalia Admin Utils";
        public override string ModuleVersion => "0.0.1";
        public override string ModuleAuthor => "faketuna";
        public override string ModuleDescription => "Provides convenient administration feature.";


        public override void Load(bool hotReload) {
            AddCommand("css_exttime", "Extend round time", CommandExtendRoundTime);
            AddCommand("css_ert", "Extend round time", CommandExtendRoundTime);
            AddCommand("css_endround", "Terminate the current round", CommandTerminateRound);
        }
    }
}

