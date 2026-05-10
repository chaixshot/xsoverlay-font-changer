using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;


    Overlay_Manager overlayManager;

    private static readonly Dictionary<string, string> configData = File.ReadAllLines(@".\BepInEx\config\xsoverlay_font_changer.cfg")
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            .Select(line => line.Split('=', (char)2))
            .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
    private readonly Font keyboardFont = new(configData["KeyboardFontPath"].Trim('"'));
    private readonly Dictionary<string, bool> hasInitialized = new(){
        { "Keyboard", false },
    };

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        overlayManager = Overlay_Manager.Instance;

        ServerClientBridge.Instance.Api.Commands["Keyboard"] += delegate
        {
            InitializeKeyboard();
        };
    }

    //?? InitializeKeyboard
    private void InitializeKeyboard()
    {
        if (hasInitialized["Keyboard"]) return;
        hasInitialized["Keyboard"] = true;

        Log("InitializeKeyboard");

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(keyboardFont);
        foreach (TextMeshProUGUI textMesh in overlayManager.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            //Log(textMesh.ToString());
            textMesh.font = fontAsset;
        }

    }

    private void Log(string msg)
    {
        Logger.LogInfo($"{msg}");
    }
}
