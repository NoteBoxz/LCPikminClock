using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;

namespace LCPikminClock
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
    public class LCPikminClock : BaseUnityPlugin
    {
        public static LCPikminClock Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }
        public static GameObject? ClockUI = null!;
        public static bool ShowTime = false;
        public static string IconColor, DotsColor, LinesColor, TimeColor;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            Patch();
            LoadAsset();
            if (IsDependencyLoaded("ainavt.lc.lethalconfig"))
            {
                BindLCConfigs();
            }
            else
            {
                BindConfigs();
            }
            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }
        public void BindConfigs()
        {
            var ShowClockig = Config.Bind("HUD", "Show Clock", false, "Shows the actual time below the bar icon.");
            var iconColorConfig = Config.Bind("Colors", "Icon Color", "(255,100,0,255)", "Color for the icon. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            var dotsColorConfig = Config.Bind("Colors", "Dots Color", "(255,0,0,255)", "Color for the dots. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            var linesColorConfig = Config.Bind("Colors", "Lines Color", "(255,0,0,255)", "Color for the lines. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            var timeColorConfig = Config.Bind("Colors", "Time Color", "(255,124,0,255)", "Color for the time. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            TimeColor = timeColorConfig.Value;
            LinesColor = linesColorConfig.Value;
            DotsColor = dotsColorConfig.Value;
            IconColor = iconColorConfig.Value;
            ShowTime = ShowClockig.Value;
            ShowClockig.SettingChanged += (obj, args) =>
            {
                ShowTime = ShowClockig.Value;
            };
            iconColorConfig.SettingChanged += (obj, args) =>
            {
                IconColor = iconColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
            dotsColorConfig.SettingChanged += (obj, args) =>
            {
                DotsColor = dotsColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
            linesColorConfig.SettingChanged += (obj, args) =>
            {
                LinesColor = linesColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
            timeColorConfig.SettingChanged += (obj, args) =>
            {
                TimeColor = timeColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
        }
        
        public void BindLCConfigs()
        {
            var ShowClockig = Config.Bind("HUD", "Show Clock", false, "Shows the actual time below the bar icon.");
            var iconColorConfig = Config.Bind("Colors", "Icon Color", "(255,100,0,255)", "Color for the icon. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            var dotsColorConfig = Config.Bind("Colors", "Dots Color", "(255,0,0,255)", "Color for the dots. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            var linesColorConfig = Config.Bind("Colors", "Lines Color", "(255,0,0,255)", "Color for the lines. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            var timeColorConfig = Config.Bind("Colors", "Time Color", "(255,124,0,255)", "Color for the time. Format the input like this (R,G,B,A)/(000,000,000,000). No letters, No Spaces.");
            TimeColor = timeColorConfig.Value;
            LinesColor = linesColorConfig.Value;
            DotsColor = dotsColorConfig.Value;
            IconColor = iconColorConfig.Value;
            ShowTime = ShowClockig.Value;
            ShowClockig.SettingChanged += (obj, args) =>
            {
                ShowTime = ShowClockig.Value;
            };
            iconColorConfig.SettingChanged += (obj, args) =>
            {
                IconColor = iconColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
            dotsColorConfig.SettingChanged += (obj, args) =>
            {
                DotsColor = dotsColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
            linesColorConfig.SettingChanged += (obj, args) =>
            {
                LinesColor = linesColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
            timeColorConfig.SettingChanged += (obj, args) =>
            {
                TimeColor = timeColorConfig.Value;
                HUDManagerPatch.SetColors();
            };
            if (IsDependencyLoaded("ainavt.lc.lethalconfig"))
            {
                var TimeBool2 = new BoolCheckBoxConfigItem(ShowClockig, new BoolCheckBoxOptions
                {
                    RequiresRestart = true,
                });
                LethalConfigManager.AddConfigItem(TimeBool2);

                // Add configuration entries for color strings
                var iconColorItem = new TextInputFieldConfigItem(iconColorConfig, new TextInputFieldOptions
                {
                    RequiresRestart = false,
                });
                LethalConfigManager.AddConfigItem(iconColorItem);

                var dotsColorItem = new TextInputFieldConfigItem(dotsColorConfig, new TextInputFieldOptions
                {
                    RequiresRestart = false,
                });
                LethalConfigManager.AddConfigItem(dotsColorItem);

                var linesColorItem = new TextInputFieldConfigItem(linesColorConfig, new TextInputFieldOptions
                {
                    RequiresRestart = false,
                });
                LethalConfigManager.AddConfigItem(linesColorItem);

                var timeColorItem = new TextInputFieldConfigItem(timeColorConfig, new TextInputFieldOptions
                {
                    RequiresRestart = false,
                });
                LethalConfigManager.AddConfigItem(timeColorItem);
            }
        }
        internal static void LoadAsset()
        {
            ClockUI = AssetLoader.LoadAsset<GameObject>("Assets/ClockAssets/PikminTime.prefab");
        }
        public static bool IsDependencyLoaded(string pluginGUID)
        {
            return Chainloader.PluginInfos.ContainsKey(pluginGUID);
        }
        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
