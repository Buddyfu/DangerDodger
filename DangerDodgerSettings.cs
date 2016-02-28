using DangerDodger.Classes;
using Loki;
using Loki.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DangerDodger
{
    class DangerDodgerSettings : JsonSettings
    {
        private static DangerDodgerSettings _instance;

        /// <summary>The current instance for this class. </summary>
        public static DangerDodgerSettings Instance
        {
            get
            {
                return _instance ?? (_instance = new DangerDodgerSettings());
            }
        }

        /// <summary>The default ctor. Will use the settings path "DangerDodger".</summary>
        public DangerDodgerSettings()
			: base(GetSettingsFilePath(Configuration.Instance.Name, string.Format("{0}.json", "DangerDodger")))
		{
        }
        
        private bool _dodgeMonsterPacks;
        private bool _dodgeRareMonsters;
        private bool _dodgeUniqueMonsters;
        private bool _dodgeExplodingBeacon;
        private bool _dodgeSuicideExplosion;
        private bool _dodgeBonespire;
        private int _monsterCooldown;
        private int _monsterDangerRadius;
        private int _monsterPackSize;
        private int _stepLength;
        private List<CheckBoxListItem> _bosses;

        [DefaultValue(true)]
        public bool DodgeMonsterPacks
        {
            get { return _dodgeMonsterPacks; }
            set
            {
                if (value.Equals(_dodgeMonsterPacks))
                {
                    return;
                }
                _dodgeMonsterPacks = value;
                NotifyPropertyChanged(() => DodgeMonsterPacks);
            }
        }
        
        [DefaultValue(true)]
        public bool DodgeRareMonsters
        {
            get { return _dodgeRareMonsters; }
            set
            {
                if (value.Equals(_dodgeRareMonsters))
                {
                    return;
                }
                _dodgeRareMonsters = value;
                NotifyPropertyChanged(() => DodgeRareMonsters);
            }
        }
        
        [DefaultValue(true)]
        public bool DodgeUniqueMonsters
        {
            get { return _dodgeUniqueMonsters; }
            set
            {
                if (value.Equals(_dodgeUniqueMonsters))
                {
                    return;
                }
                _dodgeUniqueMonsters = value;
                NotifyPropertyChanged(() => DodgeUniqueMonsters);
            }
        }
        
        [DefaultValue(true)]
        public bool DodgeExplodingBeacons
        {
            get { return _dodgeExplodingBeacon; }
            set
            {
                if (value.Equals(_dodgeExplodingBeacon))
                {
                    return;
                }
                _dodgeExplodingBeacon = value;
                NotifyPropertyChanged(() => DodgeExplodingBeacons);
            }
        }
        
        [DefaultValue(true)]
        public bool DodgeBonespire
        {
            get { return _dodgeBonespire; }
            set
            {
                if (value.Equals(_dodgeBonespire))
                {
                    return;
                }
                _dodgeBonespire = value;
                NotifyPropertyChanged(() => DodgeBonespire);
            }
        }

        [DefaultValue(true)]
        public bool DodgeSuicideExplosion
        {
            get { return _dodgeSuicideExplosion; }
            set
            {
                if (value.Equals(_dodgeSuicideExplosion))
                {
                    return;
                }
                _dodgeSuicideExplosion = value;
                NotifyPropertyChanged(() => DodgeSuicideExplosion);
            }
        }

        [DefaultValue(1000)]
        public int MonsterCooldown
        {
            get { return _monsterCooldown; }
            set
            {
                if (value.Equals(_monsterCooldown))
                {
                    return;
                }
                _monsterCooldown = value;
                NotifyPropertyChanged(() => MonsterCooldown);
            }
        }
        
        [DefaultValue(20)]
        public int MonsterDangerRadius
        {
            get { return _monsterDangerRadius; }
            set
            {
                if (value.Equals(_monsterDangerRadius))
                {
                    return;
                }
                _monsterDangerRadius = value;
                NotifyPropertyChanged(() => MonsterDangerRadius);
            }
        }

        [DefaultValue(7)]
        public int MonsterPackSize
        {
            get { return _monsterPackSize; }
            set
            {
                if (value.Equals(_monsterPackSize))
                {
                    return;
                }
                _monsterPackSize = value;
                NotifyPropertyChanged(() => MonsterPackSize);
            }
        }

        [DefaultValue(20)]
        public int StepLength
        {
            get { return _stepLength; }
            set
            {
                if (value.Equals(_stepLength))
                {
                    return;
                }
                _stepLength = value;
                NotifyPropertyChanged(() => StepLength);
            }
        }

        public List<CheckBoxListItem> Bosses
        {
            get
            {
                return _bosses;
            }
            set
            {//never seems to get called, verify if it works/need to work
                if (value != null && _bosses != null && value.SequenceEqual(_bosses))
                {
                    return;
                }
                _bosses = value;
                NotifyPropertyChanged(() => Bosses);
            }
        }
    }
}
