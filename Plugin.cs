using BepInEx;
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
            harmony.PatchAll(typeof(Patches.PatchKeyboard));

            if (Utils.IsKeyboardOscInstalled())
                harmony.PatchAll(typeof(Patches.PatchKeyboardOscBar));
        }

        //** Settings
        if (XConfig.SettingsEnable.Value)
        {
            if (Utils.IsKeyboardOscInstalled())
                harmony.PatchAll(typeof(Patches.PatchKeyboardOscSettings));
            else
                harmony.PatchAll(typeof(Patches.PatchSettings));
        }

        //** Notification
        if (XConfig.NotificationEnable.Value)
            harmony.PatchAll(typeof(Patches.PatchNotificationFont));

        //** Tooltip
        if (XConfig.TooltipEnable.Value)
            harmony.PatchAll(typeof(Patches.PatchTooltip));

        //** Window Overlay Settings
        if (XConfig.WindowSettingsEnable.Value)
            harmony.PatchAll(typeof(Patches.PatchWindowSettings));

        //** Wrist
        if (XConfig.WristEnable.Value)
            harmony.PatchAll(typeof(Patches.PatchWrist));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        Instance = this;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
