using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchKeyboardOscBar
    {
        private static bool isPatched = false;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Keyboard OSC Bar font patcher is loaded");
        }

        [HarmonyPatch(nameof(Overlay_Manager.EnableKeyboard))]
        [HarmonyPostfix]
        public static void PatchFont(Overlay_Manager __instance)
        {
            if (isPatched) return;

            KeyboardGlobalManager keyboardManager = __instance.Keyboard_Overlay.gameObject.GetComponentInChildren<KeyboardGlobalManager>(true);

            if (!keyboardManager != null && keyboardManager.HasKeyboardBeenOpened)
            {
                if (File.Exists(XConfig.KeyboardPath.Value))
                {
                    Font font = new(XConfig.KeyboardPath.Value);
                    TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);

                    if (Chainloader.PluginInfos.TryGetValue("nwnt.keyboardosc", out PluginInfo pluginInfo))
                    {
                        object instance = pluginInfo.Instance;
                        if (instance == null) return;

                        // Use BindingFlags.Public since the field is public
                        // Include BindingFlags.Instance because it belongs to the plugin instance
                        FieldInfo field = instance.GetType().GetField("oscBarCanvas", BindingFlags.Public | BindingFlags.Instance);

                        if (field != null)
                        {
                            // Get the value and cast it to GameObject
                            GameObject oscBarCanvas = field.GetValue(instance) as GameObject;

                            if (oscBarCanvas != null)
                            {
                                Plugin.Logger.LogInfo("Successfully found oscBarCanvas!");

                                foreach (TextMeshProUGUI textMesh in oscBarCanvas.GetComponentsInChildren<TextMeshProUGUI>(true))
                                    textMesh.font = fontAsset;
                            }
                            else
                                Plugin.Logger.LogWarning("oscBarCanvas field found, but it is currently null.");
                        }
                        else
                            Plugin.Logger.LogError("Could not find a field named 'oscBarCanvas' in the target mod.");
                    }

                    isPatched = true;
                    Plugin.Logger.LogInfo($"Keyboard font patched \"{XConfig.KeyboardPath.Value}\"");
                }
                else
                    Plugin.Logger.LogError($"Keyboard - \"{XConfig.KeyboardPath.Value}\" does not exist.");
            }
        }
    }
}
