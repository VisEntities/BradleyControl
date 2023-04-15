using ConVar;
using Facepunch;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Libraries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Physics = UnityEngine.Physics;
using Random = UnityEngine.Random;
using Time = UnityEngine.Time;

namespace Oxide.Plugins
{
    [Info("BradleyControl", "Dana", "1.0.0")]
    [Description(" ")]
    public class BradleyControl : RustPlugin
    {
        #region Fields

        private static BradleyControl _instance;
        private static Configuration _config;

        #endregion Fields

        #region Configuration

        private class Configuration
        {
            [JsonProperty("Version")]
            public string Version { get; set; }

            [JsonProperty("Automated Spawn")]
            public AutomatedSpawnOptions AutomatedSpawn { get; set; }

            [JsonProperty("Health")]
            public HealthOptions Health { get; set; }

            [JsonProperty("Loot")]
            public LootOptions Loot { get; set; }
        }

        private class AutomatedSpawnOptions
        {
            [JsonProperty("Health")]
            public float Version { get; set; }
        }

        private class HealthOptions
        {
            [JsonProperty("Health")]
            public float Version { get; set; }
        }

        private class LootOptions
        {
            [JsonProperty("Loot")]
            public float Version { get; set; }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            _config = Config.ReadObject<Configuration>();

            if (string.Compare(_config.Version, Version.ToString()) < 0)
                UpdateConfig();

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            _config = GetDefaultConfig();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(_config, true);
        }

        private void UpdateConfig()
        {
            PrintWarning("Detected changes in configuration! Updating...");

            Configuration defaultConfig = GetDefaultConfig();

            if (string.Compare(_config.Version, "1.0.0") < 0)
                _config = defaultConfig;

            PrintWarning("Configuration update complete! Updated from version " + _config.Version + " to " + Version.ToString());
            _config.Version = Version.ToString();
        }

        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                Version = Version.ToString(),
            };
        }

        #endregion Configuration

        #region Oxide Hooks

        private void Init()
        {
            _instance = this;
        }

        private void OnServerInitialized()
        {

        }

        private void Unload()
        {
            _instance = null;
            _config = null;
        }

        #endregion Oxide Hooks

        #region Functions

        #endregion Functions
    }
}