using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMShootingLevel : MapLevel
    {
        protected class CMNamedDrop
        {
            public string name;
            public int count;
        }


        CMDefenseLevelAward _levelFisrtCompleteAward;
        UILevelMainPanel _levelUI;
        UIGameMainPanel _mainUI;
        int _level;

        public static CMShootingLevel create()
        {
            return new CMShootingLevel();
        }

        public override bool Init(string confName)
        {
            bool ret = base.Init(confName);

            _levelUI = UnityGameApp.Inst.UI.getUIPanel("LevelMainUI") as UILevelMainPanel;
            _mainUI = UnityGameApp.Inst.UI.getUIPanel("GameMainUI") as UIGameMainPanel;

            return ret;
        }

        public void SetDefenseLevelConf(CMDefenseLevelConf defLevelConf, int level)
        {
            _level = level;
            int levelRange = defLevelConf.levelRangeMax - defLevelConf.levelRangeMin;

            for(int i=0; i< _monSpawns.Count; ++i)
            {
                var sp = _monSpawns[i];
                if(defLevelConf.monsterLvRanges != null && defLevelConf.monsterLvRanges.ContainsKey(sp.mapMonSpawn.conf.monsterConfName))
                {
                    var monLvRange = defLevelConf.monsterLvRanges[sp.mapMonSpawn.conf.monsterConfName];
                    int monLv = monLvRange.levelRangeMin;
                    if (levelRange > 0)
                    {
                        // calc monster level
                        monLv = monLvRange.levelRangeMin + (monLvRange.levelRangeMax - monLvRange.levelRangeMin) * (level-1) / levelRange;
                    }
                    sp.mapMonSpawn.SetSpawnMonsterLevel(monLv);
                }
                else
                {
                    sp.mapMonSpawn.SetSpawnMonsterLevel(1);
                }
            }

            _levelFisrtCompleteAward = (UnityGameApp.Inst.Game as ChickenMasterGame).gameConf.getLevelFirstCompleteAward(level);
        }

        public override void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            //_levelUI.levelStateControl.timeLeftText.text = this.timeLeft.ToString();
            _mainUI.refreshLevelInfo(this);
        }

        protected int _dropRoll(CMDropRoll r)
        {
            var j = UnityGameApp.Inst.Rand.RandomBetween(0, 100000000);
            if(j < r.rate)
            {
                return UnityGameApp.Inst.Rand.RandomBetween(r.min, r.max);
            }

            return 0;
        }

        protected CMNamedDrop _dropSet(List<CMNamedDropSet> list)
        {
            int rateTotal = 0;
            foreach(var d in list)
            {
                rateTotal += d.rate;
            }

            var j = UnityGameApp.Inst.Rand.RandomBetween(0, rateTotal);

            var curRate = 0;
            foreach (var d in list)
            {
                curRate += d.rate;
                if(j < curRate)
                {
                    return new CMNamedDrop()
                    {
                        name = d.name,
                        count = UnityGameApp.Inst.Rand.RandomBetween(d.min, d.max)
                    };
                }
            }

            return new CMNamedDrop()
            {
                name = "null",
                count = 0
            };
        }

        override public void OnMonsterDie(MapMonsterObject mon)
        {
            base.OnMonsterDie(mon);

            // read drop from config and do drop
            var drop = (UnityGameApp.Inst.Game as ChickenMasterGame).gameConf.getMonsterDrops(mon.name, mon.level);
            if(drop == null)
            {
                return;
            }

            // do drop
            int exp = 0;
            if (drop.exp != null)
            {
                exp = _dropRoll(drop.exp);
            }
            int gold = 0;
            if(drop.gold != null)
            { 
                gold = _dropRoll(drop.gold);
            }    

            if(gold > 0)
            {
                (UnityGameApp.Inst.Game as ChickenMasterGame).Self.AddGold(gold);
            }
            if(exp > 0)
            {
                (UnityGameApp.Inst.Game as ChickenMasterGame).Self.AddExp(exp);
            }

            if(drop.product != null)
            {
                var prodDrop = _dropSet(drop.product);
                if (prodDrop.count > 0)
                {
                    (UnityGameApp.Inst.Game as ChickenMasterGame).Self.AddBackpackProduct(prodDrop.name, prodDrop.count);
                }
            }

            if(drop.item != null)
            {
                var itemDrop = _dropSet(drop.item);
                if (itemDrop.count > 0)
                {
                    (UnityGameApp.Inst.Game as ChickenMasterGame).Self.AddBackpackItem(itemDrop.name, itemDrop.count);
                }
            }
        }

        public override void OnMapNPCTriggerEnter(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
        }
        public override void OnMapNPCTriggerExit(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
        }

        public override void OnMapLevelTriggerEnter(string triggerObjName, UnityEngine.Collider other)
        {
            if(triggerObjName == "EggTrigger")
            {
                _OnEggTriggerEnter(other);
            }
        }
        public override void OnMapLevelTriggerExit(string triggerObjName, UnityEngine.Collider other)
        {
        }

        protected void _OnEggTriggerEnter(UnityEngine.Collider other)
        {
            var ugObj = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if(ugObj == null)
            {
                return;
            }

            var monObj = ugObj.mgGameObject as MapMonsterObject;
            if (monObj == null)
            {
                // not monster
                return;
            }

            // sub egg hp
            (UnityGameApp.Inst.Game as ChickenMasterGame).Egg.subHp();

            // monster reach egg
            AniVanishAct va = new AniVanishAct(monObj);
            va.setVanishAnimaionName(ActAnis.JumpAni);
            monObj.actionComponent.AddAction(va);

        }

        protected override bool _checkFinish()
        {
            if((UnityGameApp.Inst.Game as ChickenMasterGame).Egg.HP <= 0)
            {
                // egg hp <= 0, failed
                _OnLose();
                return true;
            }

            return base._checkFinish();
        }

        protected override void _OnTimeUp()
        {
            // for Debug ...
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_Notify, "Level Time up");
        }

        protected override void _OnWin()
        {
            // TO DO : show win ui, give reward

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);

            if(bi.currentLevel == _level)
            {
                bi.currentLevel++;
                if (bi.currentLevel > cmGame.gameConf.maxDefenseLevelCount)
                {
                    // max level reached
                    bi.currentLevel = cmGame.gameConf.maxDefenseLevelCount;
                }

                _mainUI.refreshCurrentLevel(bi.currentLevel);
            }

            if(bi.currentFetchedAwardLevel < bi.currentLevel)
            {
                // first complete, give award
                if (_levelFisrtCompleteAward != null)
                {
                    cmGame.Self.AddGold(_levelFisrtCompleteAward.gold);
                    cmGame.Self.AddExp(_levelFisrtCompleteAward.exp);

                    foreach(var item in _levelFisrtCompleteAward.items)
                    {
                        cmGame.Self.AddBackpackItem(item.itemName, item.count);
                    }
                }

                bi.currentFetchedAwardLevel = bi.currentLevel;
            }

            cmGame.baseInfo.markDirty();

            // for Debug ...
            cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_Notify, "Level Win !");

            // show pass-reward
            cmGame.hideAllUI();
            UIPassRewardPanel _passUI = UnityGameApp.Inst.UI.createUIPanel("PassRewardUI") as UIPassRewardPanel;
            _passUI.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _passUI.showUI();
        }

        protected override void _OnLose()
        {
            // TO DO : on level lose

            // for Debug ...
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_Notify, "Level lost !");
        }
    }
}
