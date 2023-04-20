using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Pool = Facepunch.Pool;
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

            [JsonProperty("Health")]
            public HealthOptions Health { get; set; }

            [JsonProperty("Loot")]
            public LootOptions Loot { get; set; }

            [JsonProperty("Flame")]
            public FlameOptions Flame { get; set; }

            [JsonProperty("Debris")]
            public DebrisOptions Debris { get; set; }

            [JsonProperty("Movement")]
            public MovementOptions Movement { get; set; }

            [JsonProperty("Targeting")]
            public TargetingOptions Targeting { get; set; }

            [JsonProperty("Coax Turret")]
            public CoaxTurretOptions CoaxTurret { get; set; }

            [JsonProperty("Cannon")]
            public CannonOptions Cannon { get; set; }
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
            [JsonProperty("Maximum Crates To Drop")]
            public int MaximumCratesToDrop { get; set; }
        }
        
        private class DebrisOptions
        {
            [JsonProperty("Drop On Destruction")]
            public bool DropOnDestruction { get; set; }

            [JsonProperty("Harvestable Hit Points")]
            public float HarvestableHitPoints { get; set; }

            [JsonProperty("Harvest Cooldown")]
            public float HarvestCooldown { get; set; }
        }

        private class FlameOptions
        {
            [JsonProperty("Set Crates On Fire")]
            public bool SetCratesOnFire { get; set; }

            [JsonProperty("Minimum Life Time")]
            public float MinimumLifeTime { get; set; }

            [JsonProperty("Maximum Life Time")]
            public float MaximumLifeTime { get; set; }

            [JsonProperty("Spread Chance")]
            public int SpreadChance { get; set; }

            [JsonProperty("Spread At Lifetime Percent")]
            public int SpreadAtLifetimePercent { get; set; }

            [JsonProperty("Damage Per Second")]
            public float DamagePerSecond { get; set; }

            [JsonProperty("Damage Rate")]
            public float DamageRate { get; set; }

            [JsonProperty("Water Required To Extinguish")]
            public int WaterRequiredToExtinguish { get; set; }
        }

        private class MovementOptions
        {
            [JsonProperty("Maximum Speed")]
            public float MaximumSpeed { get; set; }

            [JsonProperty("Spin Speed")]
            public float SpinSpeed { get; set; }

            [JsonProperty("Brake Force")]
            public float BrakeForce { get; set; }
        }

        private class TargetingOptions
        {
            [JsonProperty("Engagement Range")]
            public float EngagementRange { get; set; }

            [JsonProperty("Target Search Range")]
            public float TargetSearchRange { get; set; }

            [JsonProperty("Memory Duration")]
            public float MemoryDuration { get; set; }
        }

        private class CoaxTurretOptions
        {
            [JsonProperty("Time Between Bursts")]
            public float TimeBetweenBursts { get; set; }

            [JsonProperty("Maximum Shots Per Burst")]
            public int MaximumShotsPerBurst { get; set; }

            [JsonProperty("Bullet Damage")]
            public float BulletDamage { get; set; }
        }

        private class CannonOptions
        {
            [JsonProperty("Recoil Intensity")]
            public float RecoilIntensity { get; set; }
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
                    MaximumCratesToDrop = 3
                },
                Debris = new DebrisOptions
                {
                    DropOnDestruction = true,
                    HarvestableHitPoints = 500f,
                    HarvestCooldown = 480f
                },
                Flame = new FlameOptions
                {
                    SetCratesOnFire = true,
                    MinimumLifeTime = 20f,
                    MaximumLifeTime = 40f,
                    SpreadChance = 50,
                    SpreadAtLifetimePercent = 50,
                    DamagePerSecond = 2f,
                    DamageRate = 0.5f,
                    WaterRequiredToExtinguish = 200,
                },
                Movement = new MovementOptions
                {
                    MaximumSpeed = 2000f,
                    BrakeForce = 100f,
                    SpinSpeed = 2000f,
                },
                Targeting = new TargetingOptions
                {
                    EngagementRange = 100f,
                    TargetSearchRange = 100f,
                    MemoryDuration = 20f,
                },
                CoaxTurret = new CoaxTurretOptions
                {
                    TimeBetweenBursts = 0.06667f,
                    MaximumShotsPerBurst = 10,
                    BulletDamage = 15f,
                },
                Cannon = new CannonOptions
                {
                    RecoilIntensity = 200f,
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
                _spawnedBradleys.Add(bradley);
            }
        }

        private void Unload()
        {
            _spawnedBradleys.Clear();
            _instance = null;
            _config = null;
        }

        private void OnBradleyApcInitialize(BradleyAPC bradley)
        {
            InitializeBradley(bradley);
            _spawnedBradleys.Add(bradley);
        }
        
        private void OnEntityDeath(BradleyAPC bradley, HitInfo info)
        {
            _lastBradleyPosition = bradley.transform.position;

            NextTick(() =>
            {
                CaptureNearbyEntities(_lastBradleyPosition);
                InitializeDebris();
                InitializeCrates();

                _spawnedBradleys.Remove(bradley);
            });
        }

        #endregion Oxide Hooks

        #region Functions

        private void CaptureNearbyEntities(Vector3 position)
        {
            List<BaseEntity> nearbyEntities = Pool.GetList<BaseEntity>();
            Vis.Entities(position, 15f, nearbyEntities, LayerMask.GetMask("Ragdoll", "Default"), QueryTriggerInteraction.Ignore);

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
                        _debris.Add(debris);
                }
            }

            Pool.FreeList(ref nearbyEntities);
        }

        private void InitializeBradley(BradleyAPC bradley)
        {
            bradley.InitializeHealth(_config.Health.StartingHealth, _config.Health.MaximumHealth);          
            bradley.maxCratesToSpawn = _config.Loot.MaximumCratesToDrop;
            
            bradley.moveForceMax = _config.Movement.MaximumSpeed;
            bradley.brakeForce = _config.Movement.BrakeForce;
            bradley.turnForce = _config.Movement.SpinSpeed;

            bradley.viewDistance = _config.Targeting.EngagementRange;
            bradley.searchRange = _config.Targeting.TargetSearchRange;
            bradley.memoryDuration = _config.Targeting.MemoryDuration;

            bradley.coaxFireRate = _config.CoaxTurret.TimeBetweenBursts;
            bradley.coaxBurstLength = _config.CoaxTurret.MaximumShotsPerBurst;
            bradley.bulletDamage = _config.CoaxTurret.BulletDamage;

            bradley.recoilScale = _config.Cannon.RecoilIntensity;
        }

        private void InitializeDebris()
        {
            if (!_config.Debris.DropOnDestruction)
            {
                foreach (HelicopterDebris debris in _debris)
                    if (debris != null)
                        debris.Kill();
            }
            else
            {
                foreach (HelicopterDebris debris in _debris)
                {
                    if (debris != null)
                    {
                        debris.tooHotUntil = Time.realtimeSinceStartup + _config.Debris.HarvestCooldown;
                        debris.health = _config.Debris.HarvestableHitPoints;
                    }
                }
            }
        }

        private void InitializeCrates()
        {
            if (_config.Flame.SetCratesOnFire)
            {
                foreach (LockedByEntCrate crate in _crates)
                {
                    if (crate != null)
                    {
                        FireBall fireBall = SpawnFireBall(crate);
                        InitializeFireBall(fireBall, crate);
                        _fireBalls.Add(fireBall);
                    }
                }
            }
        }

        private void InitializeFireBall(FireBall fireBall, LockedByEntCrate crate)
        {
            fireBall.SetParent(crate, true, true);
            fireBall.Spawn();
            fireBall.GetComponent<Rigidbody>().isKinematic = true;
            fireBall.GetComponent<Collider>().enabled = false;

            fireBall.tickRate = _config.Flame.DamageRate;
            fireBall.lifeTimeMin = _config.Flame.MinimumLifeTime;
            fireBall.lifeTimeMax = _config.Flame.MaximumLifeTime;
            fireBall.damagePerSecond = _config.Flame.DamagePerSecond;
            fireBall.waterToExtinguish = _config.Flame.WaterRequiredToExtinguish;
            fireBall.generation = (_config.Flame.SpreadChance == 0) ? 9f : (1f - (_config.Flame.SpreadChance / 100f)) / 0.1f;

            fireBall.Think();
            crate.SendMessage("SetLockingEnt", fireBall.gameObject, SendMessageOptions.DontRequireReceiver);

            float lifeTime = Random.Range(fireBall.lifeTimeMax, fireBall.lifeTimeMin);
            fireBall.Invoke(() => fireBall.Extinguish(), lifeTime);

            float spreadDelay = lifeTime * (_config.Flame.SpreadAtLifetimePercent / 100f);
            fireBall.Invoke(() => fireBall.TryToSpread(), spreadDelay);
        }

        private FireBall SpawnFireBall(LockedByEntCrate crate)
        {
            FireBall fireBall = GameManager.server.CreateEntity(_fireBallPrefab, crate.transform.position) as FireBall;
            if (!fireBall)
                return null;

            return fireBall;
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
    }
}