using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Loki.Bot;
using Loki.Common;
using System.Windows.Controls;
using Loki.Game;
using DangerDodger.Utils;
using Loki.Game.Objects;
using Loki.Game.GameData;
using Loki.Bot.Logic.Bots.OldGrindBot;
using DangerDodger.Classes;
using System.ComponentModel;

namespace DangerDodger
{
    class DangerDodger : IPlugin
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private Gui _instance;

        private const int SCAN_RADIUS = 60;

        private const float BEACON_RADIUS = 40;
        private const int BEACON_COOLDOWN = 1000;

        private const float BONESPIRE_RADIUS = 30;
        private const int BONESPIRE_COOLDOWN = 1000;

        private const float SUICIDE_RADIUS = 10;
        private const int SUICIDE_COOLDOWN = 1000;

        private const int BOSS_DETECTION_COOLDOWN = 1000;


        private Stopwatch beaconStopwatch = Stopwatch.StartNew();
        private Stopwatch bonespireStopwatch = Stopwatch.StartNew();
        private Stopwatch suicideStopwatch = Stopwatch.StartNew();
        private Stopwatch bossDetectionStopwatch = Stopwatch.StartNew();
        private Stopwatch monsterStopwatch = Stopwatch.StartNew();

        private List<Boss> bossesToDodge = new List<Boss>();

        public DangerDodger()
        {
            SetBossesToDodge();
            DangerDodgerSettings.Instance.PropertyChanged += OnSettingsPropertyChanged;
        }

        protected void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Log.InfoFormat("[DangerDodger] OnSettingsPropertyChanged");
            SetBossesToDodge();
        }

        public void SetBossesToDodge()
        {
            Log.InfoFormat("[DangerDodger] SetBossesToDodge");
            bossesToDodge = new List<Boss>();
            if (DangerDodgerSettings.Instance.Bosses != null)
            {
                foreach (Boss boss in Bosses.AllBosses)
                {
                    if (DangerDodgerSettings.Instance.Bosses.Any(b => b.Text == boss.Name && b.IsChecked))
                    {
                        bossesToDodge.Add(boss);
                    }
                }
            }
        }

        #region Implementation of IAuthored

        /// <summary> The name of the plugin. </summary>
        public string Name
        {
            get
            {
                return "DangerDodger";
            }
        }

        /// <summary>The author of the plugin.</summary>
        public string Author
        {
            get
            {
                return "Buddyfu";
            }
        }

        /// <summary> The description of the plugin. </summary>
        public string Description
        {
            get
            {
                return "A plugin to avoid dangerous bloolines mods.";
            }
        }

        /// <summary>The version of the plugin.</summary>
        public string Version
        {
            get
            {
                return "0.0.1.0";
            }
        }

        #endregion

        #region Implementation of IBase

        /// <summary>Initializes this plugin.</summary>
        public void Initialize()
        {
            Log.DebugFormat("[DangerDodger] Initialize");
        }

        /// <summary>Deinitializes this object. This is called when the object is being unloaded from the bot.</summary>
        public void Deinitialize()
        {
            Log.DebugFormat("[DangerDodger] Deinitialize");
        }

        #endregion

        #region Implementation of IConfigurable

        /// <summary>The settings object. This will be registered in the current configuration.</summary>
        public JsonSettings Settings
        {
            get
            {
                return DangerDodgerSettings.Instance;
            }
        }

        /// <summary> The plugin's settings control. This will be added to the Exilebuddy Settings tab.</summary>
        public UserControl Control
        {
            get
            {
                return (_instance ?? (_instance = new Gui()));
            }
        }

        #endregion

        #region Implementation of IRunnable

        /// <summary> The plugin start callback. Do any initialization here. </summary>
        public void Start()
        {
            Log.DebugFormat("[DangerDodger] Start");
        }

        /// <summary> The plugin tick callback. Do any update logic here. </summary>
        public void Tick()
        {
        }

        /// <summary> The plugin stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            Log.DebugFormat("[DangerDodger] Stop");
        }

        #endregion

        #region Implementation of ILogic

        /// <summary>
        /// Coroutine logic to execute.
        /// </summary>
        /// <param name="type">The requested type of logic to execute.</param>
        /// <param name="param">Data sent to the object from the caller.</param>
        /// <returns>true if logic was executed to handle this type and false otherwise.</returns>
        public async Task<bool> Logic(string type, params dynamic[] param)
        {
            if (!LokiPoe.IsInGame || LokiPoe.Me.IsDead || LokiPoe.Me.IsInTown || LokiPoe.Me.IsInHideout)
                return false;

            if ((!DangerDodgerSettings.Instance.DodgeExplodingBeacons || beaconStopwatch.ElapsedMilliseconds < BEACON_COOLDOWN) &&
                (!DangerDodgerSettings.Instance.DodgeBonespire || bonespireStopwatch.ElapsedMilliseconds < BONESPIRE_COOLDOWN) &&
                (!DangerDodgerSettings.Instance.DodgeSuicideExplosion || suicideStopwatch.ElapsedMilliseconds < SUICIDE_COOLDOWN) &&
                (!bossesToDodge.Any() || bossDetectionStopwatch.ElapsedMilliseconds < BOSS_DETECTION_COOLDOWN) &&
                ((!DangerDodgerSettings.Instance.DodgeUniqueMonsters &&
                !DangerDodgerSettings.Instance.DodgeRareMonsters &&
                !DangerDodgerSettings.Instance.DodgeMonsterPacks) ||
                monsterStopwatch.ElapsedMilliseconds < DangerDodgerSettings.Instance.MonsterCooldown))
                return false;

            IEnumerable<CachedNetworkObject> surroundingObjects;
            IEnumerable<CachedMonster> surroundingMonsters;

            //caching monster to avoid exceptions.
            lock (LokiPoe.ObjectManager.Objects)
            {
                surroundingObjects = LokiPoe.ObjectManager.Objects
                    .Where(o => o.Distance <= SCAN_RADIUS)
                    .OrderBy(m => m.Distance)
                    .Select(o => new CachedNetworkObject()
                    {
                        Name = o.Name,
                        Distance = o.Distance,
                        Position = new Vector2i(o.Position.X, o.Position.Y)
                    });
                surroundingMonsters = LokiPoe.ObjectManager.GetObjectsByType<Monster>()
                    .Where(m => m.Distance <= SCAN_RADIUS && m.IsHostile && !m.IsDead)
                    .OrderBy(m => m.Distance)
                    .Select(m => new CachedMonster()
                    {
                        Name = m.Name,
                        Distance = m.Distance,
                        Position = new Vector2i(m.Position.X, m.Position.Y),
                        Rarity = m.Rarity,
                        MonsterTypeMetadata = m.MonsterTypeMetadata,
                        currentSkillName = (m.CurrentAction == null) ? "" : (m.CurrentAction.Skill == null) ? "" : m.CurrentAction.Skill.Name,
                        currentSkillDestination = (m.CurrentAction == null) ? new Vector2i() : m.CurrentAction.Destination,
                        hasAuraMonsterCannotDie = m.HasAura("monster_aura_cannot_die"),
                        hasSkillSuicideExplosion = m.AvailableSkills.Any(s => s.Name == "suicide_explosion")
                    });

            }

            //Handle exploding beacons
            if (DangerDodgerSettings.Instance.DodgeExplodingBeacons && beaconStopwatch.ElapsedMilliseconds >= BEACON_COOLDOWN)
            {
                var dangerousObjects = surroundingObjects.Where(o => o.Name.Contains("Metadata/Effects/Spells/monsters_effects/elemental_beacon"));
                await PerformKiting(dangerousObjects, dangerousObjects.FirstOrDefault(), BEACON_RADIUS, beaconStopwatch);
            }

            //Handle bonespires's spikes
            if (DangerDodgerSettings.Instance.DodgeBonespire && bonespireStopwatch.ElapsedMilliseconds >= BONESPIRE_COOLDOWN)
            {//TODO: Change logic to make the bot kite "forward". We need to get away from the bonespire, but keep close to the monster casting them.
                var dangerousObjects = surroundingMonsters.Where(m => m.MonsterTypeMetadata == "Metadata/Monsters/Daemon/TalismanT1Bonespire");
                await PerformKiting(dangerousObjects, dangerousObjects.FirstOrDefault(), BONESPIRE_RADIUS, bonespireStopwatch);
            }

            //Handle suicide explosions
            if (DangerDodgerSettings.Instance.DodgeSuicideExplosion && suicideStopwatch.ElapsedMilliseconds >= SUICIDE_COOLDOWN)
            {
                var dangerousObjects = surroundingMonsters.Where(m => m.hasSkillSuicideExplosion);
                await PerformKiting(dangerousObjects, dangerousObjects.FirstOrDefault(m => m.currentSkillName == "suicide_explosion"), SUICIDE_RADIUS, suicideStopwatch);
            }

            //Handle boss attack dodging
            if (false && bossDetectionStopwatch.ElapsedMilliseconds >= BOSS_DETECTION_COOLDOWN)
            {
                bool detectedBoss = false;
                foreach (CachedMonster monster in surroundingMonsters.Where(m => m.Rarity == Rarity.Unique))
                {
                    Boss correspondingBoss = bossesToDodge.FirstOrDefault(b => b.Name == monster.Name);
                    if (correspondingBoss != null)
                    {
                        if (correspondingBoss.Attack.Select(a => a.AttackName).Contains(monster.currentSkillName))
                        {

                            return false;
                        }
                        detectedBoss = true;
                    }
                }
                if (!detectedBoss)
                {
                    bossDetectionStopwatch.Restart();
                }
            }

            //return if a monster with the immortal aura is nearby. We don't want to run away from such monsters.
            if (surroundingMonsters.Any(m => m.hasAuraMonsterCannotDie))
                return false;//TODO: Add logic to move towards this monster?

            //Handle monster kiting
            if (monsterStopwatch.ElapsedMilliseconds >= DangerDodgerSettings.Instance.MonsterCooldown)
            {
                monsterStopwatch.Restart();

                if (DangerDodgerSettings.Instance.DodgeUniqueMonsters &&
                    await PerformKiting(surroundingMonsters, surroundingMonsters.FirstOrDefault(m => m.Rarity == Rarity.Unique && !m.Name.Contains("Tormented")), DangerDodgerSettings.Instance.MonsterDangerRadius))
                    return false;

                if (DangerDodgerSettings.Instance.DodgeRareMonsters &&
                    await PerformKiting(surroundingMonsters, surroundingMonsters.FirstOrDefault(m => m.Rarity == Rarity.Rare), DangerDodgerSettings.Instance.MonsterDangerRadius))
                    return false;

                if (DangerDodgerSettings.Instance.DodgeMonsterPacks &&
                    surroundingMonsters.Count() >= DangerDodgerSettings.Instance.MonsterPackSize &&
                    await PerformKiting(surroundingMonsters, surroundingMonsters.FirstOrDefault(), DangerDodgerSettings.Instance.MonsterDangerRadius))
                    return false;
            }

            //TODO: Add exploding monsters kiting?
            //Monster Name: [Alira's Martyr], Skill Name: [suicide_explosion], Skill Destination: [{1685, 1302}], Skill Destination Distance: [0]

            //TODO: Add corpse kiting if detonate dead nearby?
            //TODO: Add storm call dodging? Does not appear in LokiPoe.ObjectManager.Objects 
            //TODO: Add Flameblast dodging? Does not appear in LokiPoe.ObjectManager.Objects. Could try monster.CurrentAction.Skill.Name and monster.CurrentAction.Destination
            //TODO: Allow the user to give specific monster names? Dodge monster by name
            //TODO: Add support to dodge boss attacks? I'm wayyyyy to lazy to implement the attack detection for all the bosses.
            //TODO: Add exploding monster dodging if too many nearby (Alira's Martyr, Carrion Minion and Unstable Larvae)

            //TODO: Add leapslam dodging is too many monster are doing it at the same time
            //Monster Name: Goatman, Skill Name: Leap Slam, Skill Destination: {922, 996}, Skill Destination Distance: 4

            //TODO: Dodge poison bombs / caustic arrow? Do they even do that much dmg? Maybe if the monster casting is Rare / Unique
            //TODO: Dodge poison cloud created on zombie death?

            return false;
        }

        public async Task<bool> PerformKiting(IEnumerable<CachedNetworkObject> dangerousObjects, CachedNetworkObject nearestThreat, float dangerRadius, Stopwatch stopwatchToReset = null)
        {
            if (dangerousObjects.Any())
            {
                if (nearestThreat != null && nearestThreat.Distance <= dangerRadius)
                {
                    Log.InfoFormat("[DangerDodger] Initiating kiting.");
                    List<Vector2> positions = dangerousObjects.Select(o => o.Position.ToVector2()).ToList();
                    Vector2 dangerCenter = GeometryHelper.getAveragePoint(positions);
                    Log.InfoFormat("[DangerDodger] NearestThreat.Name: [{0}], NearestThreat.Position: [{1}], DangerCenter: [{2}]", nearestThreat.Name, nearestThreat.Position, dangerCenter);
                    //The escape angle represents the shortest way out of the danger.
                    double escapeAngle = GeometryHelper.getAngleBetweenPoints(dangerCenter, LokiPoe.Me.Position.ToVector2());
                    Vector2i newPosition = MoveHelper.CalcSafePosition(escapeAngle, DangerDodgerSettings.Instance.StepLength);

                    if (newPosition != LokiPoe.Me.Position)
                    {
                        //PlayerMover.MoveTowards(newPosition) Does not seem to work when the character is busy.
                        Log.InfoFormat("[DangerDodger] Kiting towards a safer position. CurrentPosition: [{0}], NewPosition: [{1}]", LokiPoe.Me.Position, newPosition);
                        if (!await Coroutines.MoveToLocation(newPosition, 5, 1000))
                        {
                            Log.ErrorFormat("[DangerDodger] Error kiting towards safe position. ");
                        }
                    }
                    return true;
                }
            }
            else
            {
                if (stopwatchToReset != null)
                {//In some cases, we don't restart the stopwatch when the threat is still nearby. The bot could decide to go pick up some item in the middle of danger zone at any moment ;.;
                    stopwatchToReset.Restart();
                }
            }
            return false;
        }

        /// <summary>
        /// Non-coroutine logic to execute.
        /// </summary>
        /// <param name="name">The name of the logic to invoke.</param>
        /// <param name="param">The data passed to the logic.</param>
        /// <returns>Data from the executed logic.</returns>
        public object Execute(string name, params dynamic[] param)
        {
            return null;
        }

        #endregion

        #region Implementation of IEnableable

        /// <summary> The plugin is being enabled.</summary>
        public void Enable()
        {
            Log.DebugFormat("[DangerDodger] Enable");
        }

        /// <summary> The plugin is being disabled.</summary>
        public void Disable()
        {
            Log.DebugFormat("[DangerDodger] Disable");
        }

        #endregion

        #region Override of Object

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Name + ": " + Description;
        }

        #endregion
    }
}
