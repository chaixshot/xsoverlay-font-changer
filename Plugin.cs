using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

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
        Logger = base.Logger;
        XConfig.AllConfig(Config);

        //** Keyboard
        if (XConfig.KeyboardEnable.Value)
        {
            harmony.PatchAll(typeof(Patches.PatchKeyboardFont));

            if (Chainloader.PluginInfos.ContainsKey("nwnt.keyboardosc"))
            {
                harmony.PatchAll(typeof(Patches.PatchKeyboardOSCFont));
            }
        }

        //** Notification
        if (XConfig.NotificationEnable.Value)
            Patches.PatchNotificationFont.PatchCSS();

        //** Settings
        if (XConfig.SettingsEnable.Value)
        {
            if (Chainloader.PluginInfos.ContainsKey("nwnt.keyboardosc"))
                Patches.PatchKeyboardOSCFont.PatchSettingCSS();
            else
                Patches.PatchSettingsFont.PatchCSS();
        }

        //** Tooltip
        if (XConfig.TooltipEnable.Value)
            Patches.PatchTooltipFont.PatchCSS();

        //** Window Overlay Settings
        if (XConfig.WindowSettingsEnable.Value)
            Patches.PatchWindowSettingsFont.PatchCSS();

        //** Wrist
        if (XConfig.WristEnable.Value)
            Patches.PatchWristFont.PatchCSS();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        Instance = this;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
