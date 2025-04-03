using LupercaliaAdminUtils.model;

namespace LupercaliaAdminUtils;

public class ClassTemplate: IPluginModule
{

    public string PluginModuleName => "TEMPLATE";
    
    private readonly LupercaliaAdminUtils _plugin;

    public ClassTemplate(LupercaliaAdminUtils plugin)
    {
        _plugin = plugin;
    }
    
    public void AllPluginsLoaded()
    {
    }

    public void UnloadModule()
    {
    }
}