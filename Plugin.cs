using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xsoverlay_font_changer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static Plugin Instance;

    private static readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);
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
        Patches.PatchWristFont.PatchCSS();
        Patches.PatchSettingsFont.PatchCSS();
        Patches.PatchNotificationFont.PatchCSS();
        Patches.PatchWindowSettingsFont.PatchCSS();
        Patches.PatchTooltipFont.PatchCSS();

        //?? Patch Keyboard OSC custom settings font - https://github.com/nyakowint/xsoverlay-keyboard-osc
        if (Chainloader.PluginInfos.ContainsKey("nwnt.keyboardosc"))
        {
            harmony.PatchAll(typeof(Patches.PatchKeyboardOSCFont));
            Patches.PatchKeyboardOSCFont.PatchCSS();
        }

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        Instance = this;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
