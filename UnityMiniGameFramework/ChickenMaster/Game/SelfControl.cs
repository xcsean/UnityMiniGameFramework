using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class SelfControl : CMHeros
    {
        bool _isInited;

        LocalBaseInfo _baseInfo;

        protected CMGameConfig _cmGameConf;

        public MapHeroObject selfMapHero => _mapHeroObj;

        public int userLevel => _baseInfo.level;

        protected UIHeroPanel _heroUI;
        public UIHeroPanel heroUI => _heroUI;
        protected Dictionary<string, int> _heroNearState;

        public SelfControl()
        {
            _isInited = false;
            _heroNearState = new Dictionary<string, int>();
        }

        public void Init()
        {
            _cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _cmGameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;

            _baseInfo = _cmGame.baseInfo.getData() as LocalBaseInfo;

            // init self
            _Init(_baseInfo.selfHero);

            UnityGameApp.Inst.MainScene.camera.follow(_mapHeroObj);

            _isInited = true;

            // init hero ui
            _heroUI = UnityGameApp.Inst.UI.createUIPanel("HeroUI") as UIHeroPanel;
            _heroUI.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _heroUI.hideUI();

            foreach(var npcPair in (UnityGameApp.Inst.MainScene.map as Map).npcs)
            {
                npcPair.Value.OnMapNPCTriggerEnter += _OnMapNPCTriggerEnter;
                npcPair.Value.OnMapNPCTriggerExit += _OnMapNPCTriggerExit;
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

            _heroUI.ShowHero(npcObj.name);
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

            var heroConf = _cmGameConf.getCMHeroConf(npcObj.name);
            if (heroConf == null)
            {
                return;
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
            if(!_isInited)
            {
                return;
            }

            if(_cmGame.uiMainPanel.Joystick.isMoving)
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
            else if(_mapHeroObj.moveAct.isMoving)
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
                var vec = (heroPair.Value.mapHero.unityGameObject.transform.position - this.mapHero.unityGameObject.transform.position);
                if (vec.magnitude > 1.0)
                {
                    // not near by
                    int nearState = 0;
                    _heroNearState.TryGetValue(heroPair.Key, out nearState);
                    if (nearState != 0)
                    {
                        // last frame is nearby, exit nearby
                        _heroNearState[heroPair.Key] = 0;

                        if(_heroUI.isShow)
                        {
                            _heroUI.hideUI();
                        }
                    }
                }
                else
                {
                    // near by
                    int nearState = 0;
                    _heroNearState.TryGetValue(heroPair.Key, out nearState);
                    if (nearState == 0)
                    {
                        // last frame is not nearby, enter nearby
                        _heroNearState[heroPair.Key] = 1;

                        if (!_heroUI.isShow)
                        {
                            _heroUI.ShowHero(heroPair.Value.heroInfo.mapHeroName);
                            break;
                        }
                    }
                }
            }
        }

        public void AddBackpackProduct(string name, int count)
        {
            LocalPackProductInfo toAddItem = null;
            for(int i=0; i< _baseInfo.backPackProds.Count; ++i)
            {
                var item = _baseInfo.backPackProds[i];
                if (item.productName == name)
                {
                    toAddItem = item;
                    break;
                }
            }

            if(toAddItem != null)
            {
                toAddItem.count += count;
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

            _cmGame.uiMainPanel.refreshMeat();
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

            if(toSubItem == null)
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
            if(gold<=0)
            {
                return false;
            }

            if(_baseInfo.gold < gold)
            {
                return false;
            }

            _baseInfo.gold -= gold;

            _cmGame.baseInfo.markDirty();

            _cmGame.uiMainPanel.refreshGold(_baseInfo.gold);

            return true;
        }
        public void AddGold(int gold)
        {
            if(gold <= 0)
            {
                return;
            }

            _baseInfo.gold += gold;
            _cmGame.baseInfo.markDirty();

            _cmGame.uiMainPanel.refreshGold(_baseInfo.gold);
        }

        public void AddExp(int exp)
        {
            if (exp <= 0)
            {
                return;
            }

            _baseInfo.exp += exp;

            int levelUpExp = _cmGame.gameConf.getLevelUpExpRequire(_baseInfo.level);
            if(levelUpExp > 0)
            {
                if(_baseInfo.exp >= levelUpExp)
                {
                    // level up
                    _baseInfo.exp = _baseInfo.exp - levelUpExp;
                    _baseInfo.level = _baseInfo.level + 1;

                    _OnLevelUp();
                }
            }

            _cmGame.baseInfo.markDirty();

            _cmGame.uiMainPanel.refreshExp(_baseInfo.exp, levelUpExp);
        }

        protected void _OnLevelUp()
        {
            _cmGame.uiMainPanel.refreshLevel(_baseInfo.level);

            // TO DO : Level up
        }
    }
}
