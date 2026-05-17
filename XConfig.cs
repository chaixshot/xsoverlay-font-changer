using BepInEx.Configuration;

namespace xsoverlay_font_changer
{
    internal class XConfig
    {
        public static ConfigEntry<bool> KeyboardEnable;
        public static ConfigEntry<string> KeyboardPath;

        public static ConfigEntry<bool> NotificationEnable;
        public static ConfigEntry<string> NotificationPath;

        public static ConfigEntry<bool> SettingsEnable;
        public static ConfigEntry<string> SettingsPath;

        public static ConfigEntry<bool> TooltipEnable;
        public static ConfigEntry<string> TooltipPath;

        public static ConfigEntry<bool> WindowSettingsEnable;
        public static ConfigEntry<string> WindowSettingsPath;

        public static ConfigEntry<bool> WristEnable;
        public static ConfigEntry<string> WristPath;

        public static void AllConfig(ConfigFile cfg)
        {
            KeyboardEnable = cfg.Bind("Keyboard", "Enable", true, "Enabled Keyboard font patching.");
            KeyboardPath = cfg.Bind("Keyboard", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Keyboard font name.");

            NotificationEnable = cfg.Bind("Notification", "Enable", true, "Enabled Notification font patching.");
            NotificationPath = cfg.Bind("Notification", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Notification font name.");

            SettingsEnable = cfg.Bind("Settings", "Enable", true, "Enabled Settings font patching.");
            SettingsPath = cfg.Bind("Settings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Settings font name.");

            TooltipEnable = cfg.Bind("Tooltip", "Enable", true, "Enabled Tooltip font patching.");
            TooltipPath = cfg.Bind("Tooltip", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Tooltip font name.");

            WindowSettingsEnable = cfg.Bind("WindowSettings", "Enable", true, "Enabled Window Overlay font patching.");
            WindowSettingsPath = cfg.Bind("WindowSettings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Window Overlay font name.");

            WristEnable = cfg.Bind("Wrist", "Enable", true, "Enabled Wrist font patching.");
            WristPath = cfg.Bind("Wrist", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Wrist font name.");
        }
    }
}
