using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using LupercaliaAdminUtils.model;
using Microsoft.Extensions.Logging;

namespace LupercaliaAdminUtils;

public class LupercaliaAdminUtils: BasePlugin {
    private static readonly string PluginPrefix = $" {ChatColors.DarkRed}[{ChatColors.Blue}LPŘ AU{ChatColors.DarkRed}]{ChatColors.Default}";

    private readonly HashSet<IPluginModule> _loadedModules = new();

        
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


    public override void Load(bool hotReload)
    {
        _instance = this;
            
        InitializeModule(new TerminateRound(this));
        InitializeModule(new ExtendRoundTime(this));
        InitializeModule(new RespawnPlayer(this));
        InitializeModule(new SetHealth(this));
    }
        
    public override void Unload(bool hotReload)
    {
        UnloadAllModules();
    }
    
    private void InitializeModule(IPluginModule module)
    {
        _loadedModules.Add(module);
        Logger.LogInformation($"{module.PluginModuleName} has been initialized");
    }

    private void UnloadAllModules()
    {
        foreach (IPluginModule loadedModule in _loadedModules)
        {
            loadedModule.UnloadModule();
            Logger.LogInformation($"{loadedModule.PluginModuleName} has been unloaded.");
        }
    }
}