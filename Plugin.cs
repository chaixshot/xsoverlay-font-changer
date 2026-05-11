using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XSOverlay;

namespace xsoverlay_font_changer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private static readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);
    internal static Overlay_Manager overlayManager;
    internal static Dictionary<string, string> configData;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        configData = File.ReadAllLines(@".\BepInEx\config\xsoverlay_font_changer.cfg")
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            .Select(line => line.Split('=', (char)2))
            .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

        harmony.PatchAll(typeof(Patches.PatchKeyboardFont));
        Patches.PatchWristFont.Patch();
        Patches.PatchSettingsFont.Patch();
        Patches.PatchNotificationFont.Patch();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        overlayManager = Overlay_Manager.Instance;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
