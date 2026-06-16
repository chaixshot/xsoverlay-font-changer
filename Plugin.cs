using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Threading.Tasks;
using xsoverlay_font_changer.Utils;

namespace xsoverlay_font_changer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static Plugin Instance;

    private static readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        // Plugin startup logic
        Instance = this;
        Logger = base.Logger;
        XConfig.AllConfig(Config);

        harmony.PatchAll(typeof(GlobalizeJsModule));

        harmony.PatchAll(typeof(Patches.PatchKeyboard));
        harmony.PatchAll(typeof(Patches.WebView.PatchWebView));
        harmony.PatchAll(typeof(Patches.Setting.SettingPage));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        if (XConfig.UpdateNotification.Value)
            Task.Run(() =>
            {
                Utils.Update.CheckForUpdate(false);
            });

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
