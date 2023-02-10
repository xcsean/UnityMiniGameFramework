using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;
using Debug = MiniGameFramework.Debug;

namespace UnityMiniGameFramework
{
    public class SelfControl : CMHeros
    {
        bool _isInited;

        LocalBaseInfo _baseInfo;

        protected CMGameConfig _cmGameConf;

        public MapHeroObject selfMapHero => _mapHeroObj;

        public int userLevel => _baseInfo.level;

        protected UITowerHeroPanel _heroUI;
        public UITowerHeroPanel heroUI => _heroUI;
        protected Dictionary<string, int> _heroNearState;
        protected Dictionary<string, HeroPosInfo> _heroPosInfo;

        public SelfControl()
        {
            _isInited = false;
            _heroNearState = new Dictionary<string, int>();
            _heroPosInfo = new Dictionary<string, HeroPosInfo>();
        }

        protected struct HeroPosInfo
        {
            public Transform transform;
            public CMNPCHeros npcHero;
        }

        public void Init()
        {
            _cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _cmGameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;

            _baseInfo = _cmGame.baseInfo.getData() as LocalBaseInfo;

            // init self
            _Init(_baseInfo.selfHero);

            UnityGameApp.Inst.MainScene.camera.follow(_mapHeroObj);
            _mapHeroObj.markAsSelf();
            //todo:需要根据场景变更来切换
            _mapHeroObj.unityGameObject.AddComponent<AudioListener>();
            UnityGameApp.Inst.MainScene.unityCamera.gameObject.GetComponent<AudioListener>().enabled = false;
            _isInited = true;

            // init hero ui
            _heroUI = UnityGameApp.Inst.UI.createUIPanel("TowerHeroUI") as UITowerHeroPanel;
            _heroUI.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _heroUI.hideUI();

            foreach (var npcPair in (UnityGameApp.Inst.MainScene.map as Map).npcs)
            {
                // npcPair.Value.OnMapNPCTriggerEnter += _OnMapNPCTriggerEnter;
                // npcPair.Value.OnMapNPCTriggerStay += _OnMapNPCTriggerStay;
                // npcPair.Value.OnMapNPCTriggerExit += _OnMapNPCTriggerExit;
                
                var posInfo = new HeroPosInfo {transform = npcPair.Value.unityGameObject.transform};
                _heroPosInfo[npcPair.Value.name] = posInfo;
            }
        }

        private void _OnMapNPCTriggerEnter(string triggerObjectName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
            var ugObj = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if (ugObj == null)
            {
                return;
            }

            var monObj = ugObj.mgGameObject as MapHeroObject;
            if (monObj != _mapHeroObj)
            {
                // not self
                return;
            }

            var heroConf = _cmGameConf.getCMHeroConf(npcObj.name);
            if (heroConf == null)
            {
                return;
            }

            // 主角移动中不触发打开界面
            if (_mapHeroObj.moveAct.isMoving)
            {
                return;
            }

            //_heroUI.ShowHero(npcObj.name);
        }

        private void _OnMapNPCTriggerStay(string triggerObjectName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
            var ugObj = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if (ugObj == null)
            {
                return;
            }

            var monObj = ugObj.mgGameObject as MapHeroObject;
            if (monObj != _mapHeroObj)
            {
                // not self
                return;
            }

            var _heroConf = _cmGameConf.getCMHeroConf(npcObj.name);
            if (_heroConf == null)
            {
                return;
            }

            // 主角移动中不触发打开界面
            if (_mapHeroObj.moveAct.isMoving)
            {
                return;
            }
            // 未达到可解锁等级 不显示界面
            if (_heroConf.userLevelRequire > 0 && _baseInfo.currentLevel < _heroConf.userLevelRequire)
            {
                return;
            }

            int nearState = 0;
            _heroNearState.TryGetValue(npcObj.name, out nearState);
            if (nearState == 0)
            {
                // last frame is nearby, enter nearby
                _heroNearState[npcObj.name] = 1;

                if (!_heroUI.isShow)
                {
                    _heroUI.ShowHero(npcObj.name);
                }
            }
        }

        private void _OnMapNPCTriggerExit(string triggerObjectName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
            var ugObj = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if (ugObj == null)
            {
                return;
            }

            var monObj = ugObj.mgGameObject as MapHeroObject;
            if (monObj != _mapHeroObj)
            {
                // not self
                return;
            }

            var _heroConf = _cmGameConf.getCMHeroConf(npcObj.name);
            if (_heroConf == null)
            {
                return;
            }

            int nearState = 0;
            _heroNearState.TryGetValue(npcObj.name, out nearState);
            if (nearState != 0)
            {
                // last frame is nearby, exit nearby
                _heroNearState[npcObj.name] = 0;
            }

            _heroUI.hideUI();
        }


        //protected void _initSelfMapHero()
        //{
        //    _baseInfo = _cmGame.baseInfo.getData() as LocalBaseInfo;
        //    var heroConf = UnityGameApp.Inst.MapManager.MapConf.getMapHeroConf(_baseInfo.hero.mapHeroName);

        //    // create hero
        //    var unityHeroObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(heroConf.prefabName);
        //    unityHeroObj = UnityEngine.GameObject.Instantiate(unityHeroObj);
        //    var mgObj = unityHeroObj.GetComponent<UnityGameObjectBehaviour>();
        //    if(mgObj == null)
        //    {
        //        Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init map hero prefab [{heroConf.prefabName}] without UnityGameObjectBehaviour");
        //        return;
        //    }

        //    _selfMapHeroObj = mgObj.mgGameObject as MapHeroObject;
        //    if (_selfMapHeroObj == null)
        //    {
        //        Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init map hero prefab [{heroConf.prefabName}] not MapHeroObject");
        //        return;
        //    }

        //    _inputControl = new InputActorControllerComp();
        //    _selfMapHeroObj.AddComponent(_inputControl);
        //    _inputControl.Init(null); // TO DO : add config

        //    _selfCombatComp = new CMSelfComponent();
        //    _selfMapHeroObj.AddComponent(_selfCombatComp);
        //    _selfCombatComp.Init((UnityGameApp.Inst.Game as ChickenMasterGame).gameConf.gameConfs.selfCombatConf);

        //    //_movAct = new RigibodyMoveAct(_selfMapHeroObj);
        //    //_selfMapHeroObj.actionComponent.AddAction(_movAct);

        //    // add to scene
        //    unityHeroObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

        //    if(_baseInfo.hero.position == null)
        //    {                
        //        // set born position
        //        //unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getRandomBornPos();
        //        unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getNamedBornPos("b1");
        //        _baseInfo.hero.position = new JsonConfVector3()
        //        {
        //            x = unityHeroObj.transform.position.x,
        //            y = unityHeroObj.transform.position.y,
        //            z = unityHeroObj.transform.position.z,
        //        };
        //    }
        //    else
        //    {
        //        // restore position
        //        unityHeroObj.transform.position = new UnityEngine.Vector3(_baseInfo.hero.position.x, unityHeroObj.transform.position.y, _baseInfo.hero.position.z);
        //    }

        //    UnityGameApp.Inst.MainScene.camera.follow(_selfMapHeroObj);

        //    // create weapon
        //    if (_baseInfo.hero.holdWeapon == null)
        //    {
        //        var weaponInfo = new LocalWeaponInfo()
        //        {
        //            id = 1,
        //            level = 1
        //        };

        //        _baseInfo.hero.holdWeapon = weaponInfo;
        //    }

        //    onChangeGun();

        //    _isInited = true;
        //}

        //public void onChangeGun()
        //{
        //    if(_gun != null)
        //    {
        //        _gun.Dispose();
        //        UnityEngine.GameObject.Destroy(_gun.unityGameObject);
        //        _gun = null;
        //    }

        //    var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
        //    var cmGunConf = cmGame.gameConf.getCMGunConf(_baseInfo.hero.holdWeapon.id);
        //    if(cmGunConf == null)
        //    {
        //        Debug.DebugOutput(DebugTraceType.DTT_Error, $"init self gun id[{_baseInfo.hero.holdWeapon.id}] not exist");
        //        return;
        //    }

        //    var unitGunObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(cmGunConf.prefabName);
        //    unitGunObj = UnityEngine.GameObject.Instantiate(unitGunObj);
        //    var mgObj = unitGunObj.GetComponent<UnityGameObjectBehaviour>();
        //    if (mgObj == null)
        //    {
        //        Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init gun prefab [{cmGunConf.prefabName}] without UnityGameObjectBehaviour");
        //        return;
        //    }

        //    _gun = mgObj.mgGameObject as GunObject;
        //    if (_gun == null)
        //    {
        //        mgObj.mgGameObject.Dispose();
        //        UnityEngine.GameObject.Destroy(unitGunObj);
        //        Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init gun prefab [{cmGunConf.prefabName}] not GunObject");
        //        return;
        //    }

        //    // attach gun to hero
        //    string attachToName;
        //    if(_gun.conf.attachToBone != null)
        //    {
        //        // attach to node indicate by gun config
        //        attachToName = _gun.conf.attachToBone;
        //    }
        //    else
        //    {
        //        // attach to default bone
        //        attachToName = "";
        //    }

        //    var trAttachTo = _selfMapHeroObj.unityGameObject.transform.Find(attachToName);
        //    if(trAttachTo == null)
        //    {
        //        _gun.Dispose();
        //        UnityEngine.GameObject.Destroy(_gun.unityGameObject);
        //        _gun = null;

        //        Debug.DebugOutput(DebugTraceType.DTT_Error, $"gun attach to hero[{_selfMapHeroObj.name}] bone [{trAttachTo}] not exist");
        //        return;
        //    }
        //    _gun.unityGameObject.transform.SetParent(trAttachTo);
        //    _gun.unityGameObject.transform.localScale = UnityEngine.Vector3.one;

        //    if (_gun.conf.attachPos != null)
        //    {
        //        _gun.unityGameObject.transform.localPosition = new UnityEngine.Vector3(_gun.conf.attachPos.x, _gun.conf.attachPos.y, _gun.conf.attachPos.z);
        //    }
        //    else
        //    {
        //        _gun.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
        //    }

        //    if (_gun.conf.attachRot != null)
        //    {
        //        _gun.unityGameObject.transform.localRotation = UnityEngine.Quaternion.Euler(_gun.conf.attachRot.x, _gun.conf.attachRot.y, _gun.conf.attachRot.z);
        //    }
        //    else
        //    {
        //        _gun.unityGameObject.transform.forward = trAttachTo.forward;
        //    }

        //    // TO DO : init gun combat attribute
        //    _gun.initAttack(cmGunConf.attack, _baseInfo.hero.holdWeapon.level);
        //}

        protected override void _initCombatConfig()
        {
            _combatComp.Init((UnityGameApp.Inst.Game as ChickenMasterGame).gameConf.gameConfs.selfCombatConf);
        }

        protected override void _initPosition(UnityEngine.GameObject unityHeroObj, LocalHeroInfo heroInfo)
        {
            if (heroInfo.position == null)
            {
                // set born position
                //unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getRandomBornPos();
                unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getNamedBornPos("b1");
                heroInfo.position = new JsonConfVector3()
                {
                    x = unityHeroObj.transform.position.x,
                    y = unityHeroObj.transform.position.y,
                    z = unityHeroObj.transform.position.z,
                };
            }
            else
            {
                // restore position
                unityHeroObj.transform.position = new UnityEngine.Vector3(heroInfo.position.x, unityHeroObj.transform.position.y, heroInfo.position.z);
            }
        }

        protected override void _initWeapon(LocalHeroInfo heroInfo)
        {
            //// create weapon
            //if (heroInfo.holdWeapon == null)
            //{
            //    var weaponInfo = new LocalWeaponInfo()
            //    {
            //        id = 1,
            //        level = 1
            //    };

            //    heroInfo.holdWeapon = weaponInfo;
            //}
        }

        public void OnUpdate()
        {
            if (!_isInited)
            {
                return;
            }

            if (_cmGame.uiMainPanel.Joystick.isMoving)
            {
                // TO DO : use rigibody mov
                //_movAct.moveTo(_selfMapHeroObj.unityGameObject.transform.position + _uiMainPanel.Joystick.movVector3 * 10.0f);
                _mapHeroObj.moveAct.moveToward(_cmGame.uiMainPanel.Joystick.movVector3);

                _baseInfo.selfHero.position.x = _mapHeroObj.unityGameObject.transform.position.x;
                _baseInfo.selfHero.position.y = _mapHeroObj.unityGameObject.transform.position.y;
                _baseInfo.selfHero.position.z = _mapHeroObj.unityGameObject.transform.position.z;

                _cmGame.baseInfo.markDirty();

                //// for Debug ...
                //if (_gun != null)
                //{
                //    _gun.Fire();
                //}
            }
            else if (_mapHeroObj.moveAct.isMoving)
            {
                _mapHeroObj.moveAct.stop();

                //// for Debug ...
                //if (_gun != null)
                //{
                //    _gun.StopFire();
                //}
            }

            foreach (var heroPair in _cmGame.cmNPCHeros)
            {
                string heroName = heroPair.Value.heroInfo.mapHeroName;
                if (_heroPosInfo.ContainsKey(heroName))
                {
                    var heroPosInfo = _heroPosInfo[heroName];
                    heroPosInfo.transform = heroPair.Value.mapHero.unityGameObject.transform;
                    heroPosInfo.npcHero = heroPair.Value;
                    _heroPosInfo[heroName] = heroPosInfo;
                }   
            }

            foreach (var heroPair in _heroPosInfo)
            {
                Transform heroPos = heroPair.Value.transform;
                CMNPCHeros npcHero = heroPair.Value.npcHero;
                string heroName = heroPair.Key;

                var vec = (heroPos.position - this.mapHero.unityGameObject.transform.position);
                if (vec.magnitude > 1.0)
                {
                    // not near by
                    int nearState = 0;
                    _heroNearState.TryGetValue(heroPair.Key, out nearState);
                    if (nearState != 0)
                    {
                        // last frame is nearby, exit nearby
                        _heroNearState[heroPair.Key] = 0;

                        if (_heroUI.isShow)
                        {
                            _heroUI.hideUI();
                        }
                    }
                }
                else
                {
                    // 选中拖动中
                    if (npcHero != null && npcHero.isPicked)
                    {
                        continue;
                    }

                    // 主角移动中不触发打开界面
                    if (_mapHeroObj.moveAct.isMoving)
                    {
                        continue;
                    }

                    // near by
                    int nearState = 0;
                    _heroNearState.TryGetValue(heroPair.Key, out nearState);
                    if (nearState == 0)
                    {
                        // last frame is not nearby, enter nearby
                        _heroNearState[heroPair.Key] = 1;

                        if (!_heroUI.isShow)
                        {
                            var heroConf = _cmGameConf.getCMHeroConf(heroName);
                            if (heroConf.userLevelRequire > 0 && _baseInfo.currentLevel < heroConf.userLevelRequire)
                                continue;
                            _heroUI.ShowHero(heroName);
                            break;
                        }
                    }
                }
            }
        }

        public void AddBackpackProduct(string name, int count)
        {
            LocalPackProductInfo toAddItem = null;
            for (int i = 0; i < _baseInfo.backPackProds.Count; ++i)
            {
                var item = _baseInfo.backPackProds[i];
                if (item.productName == name)
                {
                    toAddItem = item;
                    break;
                }
            }
            int totalCount = count;
            if (toAddItem != null)
            {
                toAddItem.count += count;
                totalCount = toAddItem.count;
            }
            else
            {
                _baseInfo.backPackProds.Add(new LocalPackProductInfo()
                {
                    productName = name,
                    count = count
                });
            }

            _cmGame.baseInfo.markDirty();

            UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData4($"鸡肉数量：{totalCount}，获得鸡肉：{count}"));

            _cmGame.uiMainPanel.refreshMeat();

            if (name == "meat")
            {
                _cmGame.uiMainPanel.addMeat(count);
            }
        }

        public int SubBackpackProduct(string name, int count)
        {
            LocalPackProductInfo toSubItem = null;
            for (int i = 0; i < _baseInfo.backPackProds.Count; ++i)
            {
                var item = _baseInfo.backPackProds[i];
                if (item.productName == name)
                {
                    toSubItem = item;
                    break;
                }
            }

            if (toSubItem == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SubBackpackProduct [{name}] not exist");
                return 0;
            }

            _cmGame.baseInfo.markDirty();

            if (toSubItem.count >= count)
            {
                toSubItem.count -= count;
                return count;
            }

            toSubItem.count = 0;

            _cmGame.uiMainPanel.refreshMeat();

            return toSubItem.count;
        }

        public LocalPackProductInfo GetBackpackProductInfo(string name)
        {
            for (int i = 0; i < _baseInfo.backPackProds.Count; ++i)
            {
                var item = _baseInfo.backPackProds[i];
                if (item.productName == name)
                {
                    return item;
                }
            }

            return null;
        }


        public void AddBackpackItem(string name, int count)
        {
            // TO DO : show double award ui

            _RealAddBackpackItem(name, count);
        }

        public void _RealAddBackpackItem(string name, int count)
        {
            LocalItemInfo toAddItem = null;
            for (int i = 0; i < _baseInfo.backPackItems.Count; ++i)
            {
                var item = _baseInfo.backPackItems[i];
                if (item.itemName == name)
                {
                    toAddItem = item;
                    break;
                }
            }

            if (toAddItem != null)
            {
                toAddItem.count += count;
            }
            else
            {
                _baseInfo.backPackItems.Add(new LocalItemInfo()
                {
                    itemName = name,
                    count = count
                });
            }

            _cmGame.baseInfo.markDirty();
        }

        public int SubBackpackItem(string name, int count)
        {
            LocalItemInfo toSubItem = null;
            for (int i = 0; i < _baseInfo.backPackItems.Count; ++i)
            {
                var item = _baseInfo.backPackItems[i];
                if (item.itemName == name)
                {
                    toSubItem = item;
                    break;
                }
            }

            if (toSubItem == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SubBackpackItem [{name}] not exist");
                return 0;
            }

            _cmGame.baseInfo.markDirty();

            if (toSubItem.count >= count)
            {
                toSubItem.count -= count;
                return count;
            }

            toSubItem.count = 0;

            return toSubItem.count;
        }

        public LocalItemInfo GetBackpackItemInfo(string name)
        {
            for (int i = 0; i < _baseInfo.backPackItems.Count; ++i)
            {
                var item = _baseInfo.backPackItems[i];
                if (item.itemName == name)
                {
                    return item;
                }
            }

            return null;
        }

        public bool TrySubGold(int gold)
        {
            if (gold <= 0)
            {
                return false;
            }

            if (_baseInfo.gold < gold)
            {
                return false;
            }

            _baseInfo.gold -= gold;
            _cmGame.baseInfo.markDirty();

            UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData13($"总金币：{_baseInfo.gold}，消耗金币：{gold}"));

            _cmGame.uiMainPanel.refreshGold(_baseInfo.gold);

            return true;
        }
        public void AddGold(int gold)
        {
            if (gold <= 0)
            {
                return;
            }

            _baseInfo.gold += gold;
            _cmGame.baseInfo.markDirty();

            UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData3($"总金币：{_baseInfo.gold}，获得金币：{gold}"));

            _cmGame.uiMainPanel.refreshGold(_baseInfo.gold);
            _cmGame.uiMainPanel.addGold(gold);
        }

        public void AddExp(int exp)
        {
            if (exp <= 0)
            {
                return;
            }

            /** 增幅-双倍经验 */
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            if (_baseInfo.buffs.doubleExp >= nowMillisecond)
            {
                exp *= 2;
            }

            _baseInfo.exp += exp;

            _cmGame.baseInfo.markDirty();
            CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            int levelUpExp = _cmGame.gameConf.getLevelUpExpRequire(_baseInfo.level);
            if (levelUpExp > 0)
            {
                if (_baseInfo.exp >= levelUpExp)
                {
                    // level up
                    _baseInfo.exp = _baseInfo.exp - levelUpExp;
                    _baseInfo.level = _baseInfo.level + 1;

                    levelUpExp = _cmGame.gameConf.getLevelUpExpRequire(_baseInfo.level);

                    _OnLevelUp();
                    _cmGame.baseInfo.markDirty();
                    CheckLevelUp();
                }
            }

            _cmGame.uiMainPanel.refreshExp(_baseInfo.exp, levelUpExp);
        }

        /// <summary>
        ///  GM测试
        /// </summary>
        public void GM_SetPlayerLevel(int lv)
        {
            // 修改等级和经验
            int setLv = lv;
            var lvConfs = _cmGame.gameConf.gameConfs.levelUpExpRequire;
            if (lvConfs == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl levelUpExpRequire config not exist");
                return;
            }
            while (setLv > 0 && !lvConfs.ContainsKey(setLv))
            {
                setLv--;
            }
            if (setLv <= 0)
            {
                return;
            }
            int needUpExp = 0;
            for (int i = 1; i < setLv; i++)
            {
                needUpExp += _cmGame.gameConf.getLevelUpExpRequire(i);
            }

            if (needUpExp >= 0)
            {
                // modify level
                _baseInfo.exp = needUpExp;
                _baseInfo.level = setLv;

                Debug.DebugOutput(DebugTraceType.DTT_Debug, $"GM_SetPlayerLevel modify exp[{needUpExp}],level[{setLv}]");

                _cmGame.uiMainPanel.refreshLevel(_baseInfo.level);
                _cmGame.uiMainPanel.refreshExp(_baseInfo.exp, 0);

                _cmGame.baseInfo.markDirty();
            }
        }

        protected void _OnLevelUp()
        {
            _cmGame.uiMainPanel.refreshLevel(_baseInfo.level);
            UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData2($"用户等级：{_baseInfo.level}"));
            // TO DO : Level up
        }
    }
}
