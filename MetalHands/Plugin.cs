﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MetalHands.Items.Equipment;
using MetalHands.Managment;
using Nautilus.Handlers;
using System.Reflection;

namespace MetalHands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static IngameConfigMenu ICMConfig { get; private set; }

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            //Load Config and Ingame Menu
            ICMConfig = OptionsPanelHandler.RegisterModOptions<IngameConfigMenu>();

            // Initialize custom prefabs
            InitializePrefabs();

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            MetalHandsMK1Prefab.Register();
            MetalHandsMK2Prefab.Register();
            MetalHandsClawModulePrefab.Register();
        }
    }
}