using BepInEx;
using MonoDetour;
[assembly: HG.Reflection.SearchableAttribute.OptIn]
namespace FairerScavs;


[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        Log.Init(Logger);
        ConfigOptions.BindAllConfigOptions(Config);
        MonoDetourManager.InvokeHookInitializers(typeof(Plugin).Assembly);
    }
}