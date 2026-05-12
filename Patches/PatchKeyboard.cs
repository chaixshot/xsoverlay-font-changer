using HarmonyLib;
using TMPro;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchKeyboard
    {
        private static bool isPatched = false;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Keyboard font patcher is loaded");
        }

        [HarmonyPatch(nameof(Overlay_Manager.EnableKeyboard))]
        [HarmonyPostfix]
        public static void PatchFont(Overlay_Manager __instance)
        {
            if (isPatched) return;

            KeyboardGlobalManager keyboardManager = __instance.Keyboard_Overlay.gameObject.GetComponentInChildren<KeyboardGlobalManager>(true);

            if (!keyboardManager != null && keyboardManager.HasKeyboardBeenOpened)
            {
                Font font = new(XConfig.KeyboardPath.Value);
                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);

                foreach (TextMeshProUGUI textMesh in __instance.Keyboard.GetComponentsInChildren<TextMeshProUGUI>(true))
                    textMesh.font = fontAsset;

                isPatched = true;
                Plugin.Logger.LogInfo($"Keyboard font patched \"{XConfig.KeyboardPath.Value}\"");
            }
        }
    }
}
