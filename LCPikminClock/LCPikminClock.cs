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

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            Patch();
            LoadAsset();
            BindConfigs();
            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }
        public void BindConfigs()
        {
            var ShowClockig = Config.Bind("HUD", "Show Clock", false, "Shows the actual time below the bar icon.");
            ShowTime = ShowClockig.Value;
            ShowClockig.SettingChanged += (obj, args) =>
            {
                ShowTime = ShowClockig.Value;
            };

            if (IsDependencyLoaded("ainavt.lc.lethalconfig"))
            {

                var TimeBool2 = new BoolCheckBoxConfigItem(ShowClockig, new BoolCheckBoxOptions
                {
                    RequiresRestart = true,
                });
                LethalConfigManager.AddConfigItem(TimeBool2);
            }
        }
        internal static void LoadAsset()
        {
            ClockUI = AssetLoader.LoadAsset<GameObject>("Assets/LethalminAssets/HUD/PikminTime.prefab");
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
