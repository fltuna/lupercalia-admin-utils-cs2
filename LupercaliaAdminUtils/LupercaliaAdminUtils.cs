using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using LupercaliaAdminUtils.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NativeVoteAPI.API;
using TNCSSPluginFoundation;

namespace LupercaliaAdminUtils;

public class LupercaliaAdminUtils: TncssPluginBase
{

    public override string ModuleName => "Lupercalia Admin Utils";
    public override string ModuleVersion => "0.1.0";
    public override string ModuleAuthor => "faketuna";
    public override string ModuleDescription => "Provides convenient administration feature.";

    public override string BaseCfgDirectoryPath => Path.Combine(Server.GameDirectory, "csgo/cfg/LupercaliaAdminUtils/");
    public override string ConVarConfigPath => Path.Combine(BaseCfgDirectoryPath, "LupercaliaAdminUtils.cfg");
    public override string PluginPrefix => $" {ChatColors.DarkRed}[{ChatColors.Blue}LPŘ AU{ChatColors.DarkRed}]{ChatColors.Default}";

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
        RegisterModule<SetTeamScore>();
        RegisterModule<SetTeamName>();
        RegisterModule<SetPlayerClanTag>();
        RegisterModule<ExtendTimeLimit>();
        RegisterModule<GiveWeapon>();
    }

    protected override void LateRegisterPluginServices(IServiceCollection serviceCollection, IServiceProvider provider)
    {
        INativeVoteApi? nativeVoteApi = null;
        try
        {
            nativeVoteApi = INativeVoteApi.Capability.Get();
        }
        catch (Exception)
        {
            Logger.LogError("Native vote API not found! some modules may not work properly!!!!");
        }

        if (nativeVoteApi != null)
        {
            serviceCollection.AddSingleton<INativeVoteApi>(nativeVoteApi);
        }
    }
}