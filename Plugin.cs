using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Threading.Tasks;

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

        //** Keyboard
        if (XConfig.KeyboardEnable.Value)
            harmony.PatchAll(typeof(Patches.PatchKeyboard));

        //** KeyboardOSC - https://github.com/nyakowint/xsoverlay-keyboard-osc
        if (Utils.IsKeyboardOscInstalled())
        {
            if (XConfig.SettingsEnable.Value)
                harmony.PatchAll(typeof(Patches.PatchKeyboardOscSettings));
            if (XConfig.KeyboardEnable.Value)
                harmony.PatchAll(typeof(Patches.PatchKeyboardOscBar));
        }

        //** WebView
        harmony.PatchAll(typeof(Patches.PatchWebView));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Task.Run(Utils.CheckVersion);
    }

    private void Start()
    {
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
