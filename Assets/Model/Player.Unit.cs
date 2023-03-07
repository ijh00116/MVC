using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;

namespace BlackTree.Model
{
    public static partial class PlayerUnit
    {
            public static Dictionary<UnitAbility, double> _passiveAbilitycaches;
            public static Dictionary<SkillKey, double> _activeAbilitycaches;
            public static List<SkillKey> playingskillList;

            public static Core.ControllerUnitInGame userUnit=null;
            public static int criticalUpgradeIdx;
            public static double Atk { get; set; }
            public static double AtkSpeed { get; set; }
            public static double CriRate { get; set; }
            public static double CriDmg { get; set; }
            public static double MoveSpeed { get; set; }

            public static System.Action CallbackAtAttack;
            public static System.Action CallbackAtAnimEnd;
            public static void Init()
            {
                playingskillList = new List<SkillKey>();
                _activeAbilitycaches = new Dictionary<SkillKey, double>();
                for (int i=0; i<(int)SkillKey.End; i++)
                {
                    _activeAbilitycaches.Add((SkillKey)i, 0);
                }
                StatusSync();
            }

            public static void StatusSync()
            {
                var baseAtk= 10 + Player.GoldUpgrade.GetValue(GoldUpgradeKey.AttackIncrease)+Player.SoulstoneUpgrade.GetValue(SoulStoneUpgradeKey.AttackIncrease);
                Atk = baseAtk * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.AttackIncrease])*0.01f)
                    * (1 + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.AttackIncrease]) * 0.01f)
                        * (1+Player.Relic.GetValue(RelicKey.AttackIncrease)*0.01f);
                var sd = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.AttackIncrease] * 0.01f;


                var sddsd = Player.Relic.GetValue(RelicKey.AttackSpeed) * 0.01f;
                AtkSpeed = 1 * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.AttackIncrease] * 0.01f))
                    * (1 + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.AttackIncrease] * 0.01f))
                    * (1 + Player.Relic.GetValue(RelicKey.AttackSpeed) * 0.01f);


                var adsd = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.MoveSpeed] * 0.01f;
                var asdw = Player.Relic.GetValue(RelicKey.MoveSpeed) * 0.01f;
                MoveSpeed = 1 * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.MoveSpeed] * 0.01f))
                    * (1 + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.MoveSpeed] * 0.01f))
                    * (1 + Player.Relic.GetValue(RelicKey.MoveSpeed) * 0.01f)
                    *(1+Player.SoulstoneUpgrade.GetValue(SoulStoneUpgradeKey.MoveSpeedIncrease)*0.01f);


                CalculateCriticalRate();
            }

            static void CalculateCriticalRate()
            {
                int index = 0;
                for(int i= (int)GoldUpgradeKey.CriticalRateIncrease_2; i<=(int)GoldUpgradeKey.CriticalRateIncrease_8192; i++)
                {
                    int level=Player.GoldUpgrade.GetLevel((GoldUpgradeKey)i);
                    index = i;
                    if (level < Player.GoldUpgrade.GetMaxLevel((GoldUpgradeKey)i))
                    {
                        break;
                    }
                }
                if (criticalUpgradeIdx!=index)
                {
                    criticalUpgradeIdx = index;
                    CriDmg = Mathf.Pow(2, index);
                }
                CriRate = Player.GoldUpgrade.GetValue((GoldUpgradeKey)criticalUpgradeIdx);
            }

            public static void SkillActiveUpdate(SkillKey _key, bool use = true)
            {
                if (use)
                {
                    playingskillList.Add(_key);
                }
                else
                {
                    playingskillList.Remove(_key);
                }
                SkillValueUpdate(_key);
            }

            static void SkillValueUpdate(SkillKey _key)
            {
                for (int i = 0; i < (int)SkillKey.End; i++)
                {
                    _activeAbilitycaches[(SkillKey)i] = 0;
                }

                foreach (var skill in playingskillList)
                {
                    var skilldata = StaticData.Wrapper.skillDatas[(int)skill];

                    if (skilldata.skillKey != SkillKey.None)
                    {
                        _activeAbilitycaches[_key] += skilldata.startValue;
                    }
                }
                StatusSync();
            }



        public static bool IsSkillActive(SkillKey _key)
        {
            return playingskillList.Contains(_key);
        }
    }

}
