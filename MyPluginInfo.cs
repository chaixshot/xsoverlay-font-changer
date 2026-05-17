using BepInEx.Bootstrap;

namespace xsoverlay_font_changer
{
    internal class MyPluginInfo
    {
        public const string PLUGIN_GUID = "xsoverlay.font.changer";
        public const string PLUGIN_NAME = "XSOverlay Font Changer";
        public const string PLUGIN_VERSION = "1.0.0";

        public static bool IsKeyboardOscInstalled()
        {
            return Chainloader.PluginInfos.ContainsKey("nwnt.keyboardosc");
        }
    }
}
