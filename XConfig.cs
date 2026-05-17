using BepInEx.Configuration;

namespace xsoverlay_font_changer
{
    internal class XConfig
    {
        public static ConfigEntry<bool> KeyboardEnable;
        public static ConfigEntry<string> KeyboardPath;
        public static ConfigEntry<int> KeyboardSize;

        public static ConfigEntry<bool> NotificationEnable;
        public static ConfigEntry<string> NotificationPath;
        public static ConfigEntry<int> NotificationSize;

        public static ConfigEntry<bool> SettingsEnable;
        public static ConfigEntry<string> SettingsPath;
        public static ConfigEntry<int> SettingsSize;

        public static ConfigEntry<bool> TooltipEnable;
        public static ConfigEntry<string> TooltipPath;
        public static ConfigEntry<int> TooltipSize;

        public static ConfigEntry<bool> WindowSettingsEnable;
        public static ConfigEntry<string> WindowSettingsPath;
        public static ConfigEntry<int> WindowSettingsSize;

        public static ConfigEntry<bool> WristEnable;
        public static ConfigEntry<string> WristPath;
        public static ConfigEntry<int> WristSize;

        public static void AllConfig(ConfigFile cfg)
        {
            KeyboardEnable = cfg.Bind("Keyboard", "Enable", true, "Enabled Keyboard font patching.");
            KeyboardPath = cfg.Bind("Keyboard", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Keyboard font name.");
            KeyboardSize = cfg.Bind("Keyboard", "Size", 16, "Keyboard font size.");

            NotificationEnable = cfg.Bind("Notification", "Enable", true, "Enabled Notification font patching.");
            NotificationPath = cfg.Bind("Notification", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Notification font name.");
            NotificationSize = cfg.Bind("Notification", "Size", 16, "Notification font size.");

            SettingsEnable = cfg.Bind("Settings", "Enable", true, "Enabled Settings font patching.");
            SettingsPath = cfg.Bind("Settings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Settings font name.");
            SettingsSize = cfg.Bind("Settings", "Size", 16, "Settings font size.");

            TooltipEnable = cfg.Bind("Tooltip", "Enable", true, "Enabled Tooltip font patching.");
            TooltipPath = cfg.Bind("Tooltip", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Tooltip font name.");
            TooltipSize = cfg.Bind("Tooltip", "Size", 16, "Tooltip font size.");

            WindowSettingsEnable = cfg.Bind("WindowSettings", "Enable", true, "Enabled Window Overlay font patching.");
            WindowSettingsPath = cfg.Bind("WindowSettings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Window Overlay font name.");
            WindowSettingsSize = cfg.Bind("WindowSettings", "Size", 16, "Window Overlay font size.");

            WristEnable = cfg.Bind("Wrist", "Enable", true, "Enabled Wrist font patching.");
            WristPath = cfg.Bind("Wrist", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Wrist font name.");
            WristSize = cfg.Bind("Wrist", "Size", 16, "Wrist font size.");
        }
    }
}
