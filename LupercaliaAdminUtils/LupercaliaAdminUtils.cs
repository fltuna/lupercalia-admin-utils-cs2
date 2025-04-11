using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using LupercaliaAdminUtils.Modules;
using TNCSSPluginFoundation;

namespace LupercaliaAdminUtils;

public class LupercaliaAdminUtils: TncssPluginBase
{

    public override string ModuleName => "Lupercalia Admin Utils";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "faketuna";
    public override string ModuleDescription => "Provides convenient administration feature.";

    public override string BaseCfgDirectoryPath => Path.Combine(Server.GameDirectory, "csgo/cfg/LupercaliaAdminUtils/");
    public override string ConVarConfigPath => Path.Combine(BaseCfgDirectoryPath, "LupercaliaAdminUtils.cfg");
    protected override string PluginPrefix => $" {ChatColors.DarkRed}[{ChatColors.Blue}LPŘ AU{ChatColors.DarkRed}]{ChatColors.Default}";

    protected override void TncssOnPluginLoad(bool hotReload)
    {
        RegisterModule<TerminateRound>();
        RegisterModule<ExtendRoundTime>();
        RegisterModule<RespawnPlayer>();
        RegisterModule<SetHealth>();
        RegisterModule<SetCash>();
        RegisterModule<SetKevlar>();
        RegisterModule<SetTeam>();
        RegisterModule<ToggleBuyZone>();
        RegisterModule<SetPlayerModel>();
        RegisterModule<UserList>();
        RegisterModule<SetPlayerName>();
    }
}