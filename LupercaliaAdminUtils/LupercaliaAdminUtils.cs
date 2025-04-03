using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace LupercaliaAdminUtils {
    public partial class LupercaliaAdminUtils: BasePlugin {
        private static readonly string PluginPrefix = $" {ChatColors.DarkRed}[{ChatColors.Blue}LPŘ AU{ChatColors.DarkRed}]{ChatColors.Default}";

        public string LocalizeStringWithPrefix(string languageKey, params object[] args)
        {
            return $"{PluginPrefix} {Localizer[languageKey, args]}";
        }
        
        private static LupercaliaAdminUtils _instance = null!;

        public static LupercaliaAdminUtils GetInstance()
        {
            return _instance;
        }

        public override string ModuleName => "Lupercalia Admin Utils";
        public override string ModuleVersion => "0.0.1";
        public override string ModuleAuthor => "faketuna";
        public override string ModuleDescription => "Provides convenient administration feature.";


        public override void Load(bool hotReload) {
            _instance = this;
            
            AddCommand("css_exttime", "Extend round time", CommandExtendRoundTime);
            AddCommand("css_ert", "Extend round time", CommandExtendRoundTime);
            AddCommand("css_endround", "Terminate the current round", CommandTerminateRound);
        }
    }
}

