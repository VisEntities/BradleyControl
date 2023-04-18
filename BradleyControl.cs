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
using Pool = Facepunch.Pool;

namespace Oxide.Plugins
{
    [Info("BradleyControl", "Dana", "1.0.0")]
    [Description(" ")]
    public class BradleyControl : RustPlugin
    {
        #region Fields

        private static BradleyControl _instance;
        private static Configuration _config;

        private HashSet<FireBall> _fireBalls = new HashSet<FireBall>();
        private HashSet<HelicopterDebris> _debris = new HashSet<HelicopterDebris>();
        private HashSet<LockedByEntCrate> _crates = new HashSet<LockedByEntCrate>();

        private Vector3 _lastBradleyPosition;
        private HashSet<BradleyAPC> _spawnedBradleys = new HashSet<BradleyAPC>();

        private const string _fireBallPrefab = "assets/bundled/prefabs/oilfireballsmall.prefab";
        private const string _bradleyCratePrefab = "assets/prefabs/npc/m2bradley/bradley_crate.prefab";
        private const string _bradleyDebrisPrefab = "assets/prefabs/npc/m2bradley/servergibs_bradley.prefab";
        
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

            [JsonProperty("Flame")]
            public FlameOptions Flame { get; set; }

            [JsonProperty("Debris")]
            public DebrisOptions Debris { get; set; }
        }

        private class AutomatedSpawnOptions
        {

        }

        private class HealthOptions
        {
            [JsonProperty("Starting Health")]
            public float StartingHealth { get; set; }

            [JsonProperty("Maximum Health")]
            public float MaximumHealth { get; set; }
        }

        private class LootOptions
        {
            [JsonProperty("Maximum Crates To Spawn")]
            public int MaximumCratesToSpawn { get; set; }
        }
        
        private class DebrisOptions
        {
            [JsonProperty("Drop Debris")]
            public bool DropDebris { get; set; }

            [JsonProperty("Health")]
            public float Health { get; set; }

            [JsonProperty("Too Hot Until")]
            public float TooHotUntil { get; set; }
        }

        private class FlameOptions
        {
            [JsonProperty("Spawn On Crates")]
            public bool SpawnOnCrates { get; set; }

            [JsonProperty("Spawn On Debris")]
            public bool SpawnOnDebris { get; set; }

            [JsonProperty("Minimum Life Time")]
            public float MinimumLifeTime { get; set; }

            [JsonProperty("Maximum Life Time")]
            public float MaximumLifeTime { get; set; }

            [JsonProperty("Spread Chance")]
            public int SpreadChance { get; set; }

            [JsonProperty("Spread Delay Ratio")]
            public int SpreadDelayRatio { get; set; }

            [JsonProperty("Water To Extinguish")]
            public int WaterToExtinguish { get; set; }

            [JsonProperty("Damage Frequency Per Second")]
            public float DamageFrequency { get; set; }

            [JsonProperty("Damage Amount")]
            public float DamagePerSecond { get; set; }
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
                Health = new HealthOptions
                {
                    StartingHealth = 1000f,
                    MaximumHealth = 1000f
                },
                Loot = new LootOptions
                {
                    MaximumCratesToSpawn = 3
                },
                Flame = new FlameOptions
                {
                    SpawnOnCrates = true,
                    SpawnOnDebris = false,
                    MinimumLifeTime = 30f,
                    MaximumLifeTime = 30f,
                    SpreadChance = 50,
                    SpreadDelayRatio = 50,
                    WaterToExtinguish = 100,
                    DamageFrequency = 0.5f,
                    DamagePerSecond = 1f,
                }
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
            foreach (BaseNetworkable entity in BaseNetworkable.serverEntities)
            {
                BradleyAPC bradley = entity as BradleyAPC;
                if (bradley == null)
                    continue;

                InitializeBradley(bradley);
            }
        }

        private void Unload()
        {
            _lastBradleyPosition = Vector3.zero;
            _instance = null;
            _config = null;
        }

        private void OnEntitySpawned(BradleyAPC bradley)
        {
            if (bradley == null)
                return;

            InitializeBradley(bradley);
        }

        private void OnEntityDeath(BradleyAPC bradley, HitInfo info)
        {
            Vector3 bradleyPosition = bradley.transform.position;

            NextTick(() =>
            {
                CaptureEntitiesNearbyBradley(bradleyPosition);
                foreach (LockedByEntCrate bradleyCrate in _crates)
                {
                    if (bradleyCrate != null)
                    {
                        SpawnFireBall(bradleyCrate);
                    }
                }
            });
        }

        #endregion Oxide Hooks

        #region Functions

        private void SpawnFireBallCircle(Vector3 center, float radius, int fireballCount)
        {
            for (int i = 0; i < fireballCount; i++)
            {
                float angle = i * 360f / fireballCount;
                Vector3 spawnPosition = center + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
                //SpawnFireBall();
            }
        }


        private void OnFireBallSpread(FireBall ball, BaseEntity fire)
        {
            PrintToChat("OnFireBallSpread works!");
        }

        private void SpawnFireBall(LockedByEntCrate crate)
        {
            FireBall fireBall = GameManager.server.CreateEntity(_fireBallPrefab, crate.transform.position) as FireBall;
            if (!fireBall)
                return;

            fireBall.SetParent(crate, true, true);
            fireBall.Spawn();
            fireBall.GetComponent<Rigidbody>().isKinematic = true;
            fireBall.GetComponent<Collider>().enabled = false;

            fireBall.waterToExtinguish = _config.Flame.WaterToExtinguish;
            fireBall.tickRate = _config.Flame.DamageFrequency;
            fireBall.damagePerSecond = _config.Flame.DamagePerSecond;
            fireBall.lifeTimeMax = _config.Flame.MaximumLifeTime;
            fireBall.lifeTimeMin = _config.Flame.MinimumLifeTime;
            fireBall.generation = (_config.Flame.SpreadChance == 0) ? 9f : (1f - (_config.Flame.SpreadChance / 100f)) / 0.1f;
            fireBall.Think();
            
            float lifeTime = Random.Range(fireBall.lifeTimeMax, fireBall.lifeTimeMin);
            float spreadDelay = lifeTime * (_config.Flame.SpreadDelayRatio / 100f);

            timer.Once(spreadDelay, () =>
            {
                if (fireBall != null)
                    fireBall.TryToSpread();
            });

            timer.Once(lifeTime, () =>
            {
                if (fireBall != null)
                    fireBall.Extinguish();
            });
        }

        private void CaptureEntitiesNearbyBradley(Vector3 bradleyPosition)
        {
            List<BaseEntity> nearbyEntities = Pool.GetList<BaseEntity>();
            Vis.Entities(bradleyPosition, 30f, nearbyEntities, LayerMask.GetMask("Ragdoll", "Default"), QueryTriggerInteraction.Ignore);

            if (nearbyEntities.Count != 0)
            {
                foreach (BaseEntity capturedEntity in nearbyEntities)
                {
                    LockedByEntCrate crate = capturedEntity as LockedByEntCrate;
                    if (crate != null && crate.PrefabName == _bradleyCratePrefab)
                    {
                        _crates.Add(crate);

                        FireBall fireBall = GetChildOfType<FireBall>(crate);
                        if (fireBall != null)
                            fireBall.Extinguish();
                    }

                    HelicopterDebris debris = capturedEntity as HelicopterDebris;
                    if (debris != null && debris.PrefabName == _bradleyDebrisPrefab)
                    {
                        _debris.Add(debris);
                        debris.Kill();
                    }
                }
            }

            Pool.FreeList(ref nearbyEntities);
        }

        private void InitializeBradley(BradleyAPC bradley)
        {
            bradley.InitializeHealth(_config.Health.StartingHealth, _config.Health.MaximumHealth);
            bradley.maxCratesToSpawn = _config.Loot.MaximumCratesToSpawn;

            _spawnedBradleys.Add(bradley);
        }

        #endregion Functions

        #region Helper Functions

        private static T GetChildOfType<T>(BaseEntity entity, string prefabName = null) where T : BaseEntity
        {
            foreach (var child in entity.children)
            {
                var childOfType = child as T;
                if (childOfType != null && (prefabName == null || child.PrefabName == prefabName))
                    return childOfType;
            }

            return null;
        }

        #endregion Helper Functions

        #region Helper Classes

        private static class Draw
        {
            public static void Cube(BasePlayer player, float duration, Color color, Vector3 originPosition, float radius)
            {
                player.SendConsoleCommand("ddraw.box", duration, color, originPosition, radius);
            }

            public static void Sphere(BasePlayer player, float duration, Color color, Vector3 originPosition, float radius)
            {
                player.SendConsoleCommand("ddraw.sphere", duration, color, originPosition, radius);
            }

            public static void Line(BasePlayer player, float duration, Color color, Vector3 originPosition, Vector3 targetPosition)
            {
                player.SendConsoleCommand("ddraw.line", duration, color, originPosition, targetPosition);
            }

            public static void Arrow(BasePlayer player, float duration, Color color, Vector3 originPosition, Vector3 targetPosition, float headSize)
            {
                player.SendConsoleCommand("ddraw.arrow", duration, color, originPosition, targetPosition, headSize);
            }

            public static void Text(BasePlayer player, float duration, Color color, Vector3 originPosition, string text)
            {
                player.SendConsoleCommand("ddraw.text", duration, color, originPosition, text);
            }
        }

        #endregion Helper Classes

        #region Commands

        [ConsoleCommand("bradley.info")]
        private void cmdInfo(ConsoleSystem.Arg conArgs)
        {
            BasePlayer player = conArgs?.Player();
            if (!player.IsValid())
                return;

            if (_fireBalls.Count != 0)
            {
                PrintToChat("There're fire balls");
                foreach (FireBall fireBall in _fireBalls)
                {
                    if (fireBall != null)
                        Draw.Sphere(player, 20f, Color.red, fireBall.transform.position, 1f);
                }
            }
            PrintToChat("No fire balls");

            if (_fireBalls.Count != 0)
            {
                PrintToChat("There'r crates");
                foreach (LockedByEntCrate crate in _crates)
                    Draw.Cube(player, 20f, Color.green, crate.transform.position, 1f);
            }
            PrintToChat("No crates");

            if (_fireBalls.Count != 0)
            {
                PrintToChat("There'r _debris");
                foreach (var crate in _debris)
                    Draw.Cube(player, 20f, Color.yellow, crate.transform.position, 1f);
            }
            PrintToChat("No _debris");
        }


        [ConsoleCommand("bradley.fire")]
        private void cmdFire(ConsoleSystem.Arg conArgs)
        {
            BasePlayer player = conArgs?.Player();
            if (!player.IsValid())
                return;

            if (_fireBalls.Count != 0)
            {
                PrintToChat("There're fire balls");
                foreach (FireBall fireBall in _fireBalls)
                {
                    if (fireBall != null)
                        fireBall.Extinguish();
                }
            }
        }

        #endregion Commands
    }
}