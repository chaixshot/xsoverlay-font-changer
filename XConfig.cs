using BepInEx.Configuration;

namespace xsoverlay_font_changer
{
    internal class XConfig
    {
        public static ConfigEntry<string> KeyboardPath;
        public static ConfigEntry<bool> KeyboardEnable;

        public static ConfigEntry<string> NotificationPath;
        public static ConfigEntry<bool> NotificationEnable;

        public static ConfigEntry<string> SettingsPath;
        public static ConfigEntry<bool> SettingsEnable;

        public static ConfigEntry<string> TooltipPath;
        public static ConfigEntry<bool> TooltipEnable;

        public static ConfigEntry<string> WindowSettingsPath;
        public static ConfigEntry<bool> WindowSettingsEnable;

        public static ConfigEntry<string> WristPath;
        public static ConfigEntry<bool> WristEnable;

        public static void AllConfig(ConfigFile cfg)
        {
            KeyboardPath = cfg.Bind("Keyboard", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Keyboard font file.");
            KeyboardEnable = cfg.Bind("Keyboard", "Enable", true, "When enabled, Keyboard font will patch.");

            NotificationPath = cfg.Bind("Notification", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Notification font file.");
            NotificationEnable = cfg.Bind("Notification", "Enable", true, "When enabled, Notification font will patch.");

            SettingsPath = cfg.Bind("Settings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Settings font file.");
            SettingsEnable = cfg.Bind("Settings", "Enable", true, "When enabled, Settings font will patch.");

            TooltipPath = cfg.Bind("Tooltip", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Tooltip font file.");
            TooltipEnable = cfg.Bind("Tooltip", "Enable", true, "When enabled, Tooltip font will patch.");

            WindowSettingsPath = cfg.Bind("WindowSettings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Window Overlay Settings font file.");
            WindowSettingsEnable = cfg.Bind("WindowSettings", "Enable", true, "When enabled, Window Overlay Settings font will patch.");

            WristPath = cfg.Bind("Wrist", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Wrist font file.");
            WristEnable = cfg.Bind("Wrist", "Enable", true, "When enabled, Wrist font will patch.");
        }
    }
}
