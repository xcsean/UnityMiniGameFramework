﻿using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = MiniGameFramework.Debug;

namespace UnityMiniGameFramework
{
    public class CMEgg: IMapLogicObject
    {
        protected CMEggConf _conf;
        protected LocalEggInfo _eggInfo;
        public int HP => _eggInfo.hp;

        protected HealthBar _hpBar;
        protected UIEggPanel _eggUI;
        public UIEggPanel eggUI => _eggUI;

        protected UnityEngine.GameObject _eggObject;

        protected long _incHpTime;

        protected bool _isInited;
        protected ChickenMasterGame _cmGame;

        public CMEgg()
        {
            _isInited = false;
        }

        public void Init(LocalEggInfo eggInfo)
        {
            _cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _eggInfo = eggInfo;
            _conf = _cmGame.gameConf.gameConfs.eggConf;

            _incHpTime = _eggInfo.lastIncHpTime + _conf.hpIncTime;

            _InitUI();

            if(_eggInfo.hp <= 0)
            {
                // TO DO : egg die, set ui and egg status right
            }
            _updateHpBar();
            _isInited = true;
        }

        protected void _InitUI()
        {
            var tr = (UnityGameApp.Inst.MainScene.map as Map).unityGameObject.transform.Find(_conf.EggObjectName);
            if(tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMEgg InitHpBar map [{(UnityGameApp.Inst.MainScene.map as Map).name}] egg [{_conf.EggObjectName}] object not exist");
                return;
            }
            _eggObject = tr.gameObject;

            // init hp bar
            _hpBar = new HealthBar();
            _hpBar.Init();

            _hpBar.barObject.transform.position = new UnityEngine.Vector3(
                tr.position.x,
                tr.position.y + 0.5f,
                tr.position.z);
            _hpBar.barObject.SetActive(false);

            // init egg UI
            _eggUI = UnityGameApp.Inst.UI.createUIPanel("EggUI") as UIEggPanel;
            _eggUI.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            UpdateUnityGoPos(tr.position);
        }

        protected void _updateHpBar()
        {
            if (_hpBar != null)
            {
                _hpBar.setHp((float)_eggInfo.hp / (float)_conf.maxHp);
            }

            if(_eggUI != null)
            {
                _eggUI.setHp((float)_eggInfo.hp / (float)_conf.maxHp);
            }
        }

        public void OnUpdate()
        {
            if(!_isInited)
            {
                return;
            }

            if(_eggUI != null)
            {
                var screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera).worldToScreenPos(_eggObject.transform.position));
                _eggUI.setPoisition(screenPos.x + _conf.EggUIOffset.x, screenPos.y + _conf.EggUIOffset.y);
            }

            long nowTickMilliseconds = DateTime.Now.Ticks / 10000;
            if (_eggInfo.hp > 0)
            {
                // wait increase
                if (_incHpTime <= nowTickMilliseconds)
                {

                    _eggInfo.hp += 1 + (int)((nowTickMilliseconds - _incHpTime) / _conf.hpIncTime);
                    if (_eggInfo.hp > _conf.maxHp)
                    {
                        _eggInfo.hp = _conf.maxHp;
                    }


                    _updateHpBar();

                    _eggInfo.lastIncHpTime = nowTickMilliseconds;
                    _incHpTime = _eggInfo.lastIncHpTime + _conf.hpIncTime;
                    ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                    cmGame.baseInfo.markDirty();
                }
            }
            else
            {
                // wait recovery
                if (_eggInfo.nextRecoverTime <= nowTickMilliseconds)
                {
                    // recovery
                    _eggInfo.hp = _conf.maxHp;
                    _eggInfo.nextRecoverTime = 0;

                    ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                    cmGame.baseInfo.markDirty();

                    _updateHpBar();

                    _onEggRecover();
                }
                else
                {
                    _eggUI.refreshRecoveryTime(_eggInfo.nextRecoverTime - nowTickMilliseconds);
                }
            }

            // TO DO : move hpbar show/hide code to level start/end 
            if (UnityGameApp.Inst.MainScene.map.currentLevel != null && UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
            {
                // in level
                //if(!_hpBar.barObject.activeSelf)
                //{
                //    _hpBar.show();
                //}
                //if (_eggUI.isShow)
                //{
                //    _eggUI.hideUI();
                //}
                _eggUI.changeEggState(true);
            }
            else
            {
                // not in level
                //if(_hpBar.barObject.activeSelf)
                //{
                //    _hpBar.hide();
                //}
                //if (!_eggUI.isShow)
                //{
                //    var vec = (_eggObject.transform.position - (UnityGameApp.Inst.Game as ChickenMasterGame).Self.mapHero.unityGameObject.transform.position);
                //    if (vec.magnitude <= _conf.EggUIShowRange)
                //    {
                //        _eggUI.showUI();
                //    }
                //}
                _eggUI.changeEggState(false);
            }

        }

        public void subHp()
        {
            _eggInfo.hp -= 1;
            if(_eggInfo.hp <= 0)
            {
                _eggInfo.hp = 0;

                _onEggDie();
            }

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.baseInfo.markDirty();

            _updateHpBar();

            int curLv = (cmGame.baseInfo.getData() as LocalBaseInfo).currentLevel;
            UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData11($"{curLv}"));

            // TO DO : show effect
        }

        protected void _onEggRecover()
        {
            _eggUI.onEggRecover();

            // TO DO : on recovery
        }

        protected void _onEggDie()
        {
            _eggInfo.nextRecoverTime = DateTime.Now.Ticks / 10000 + _conf.recoverTime;
            _eggUI.onEggDie();

            // TO DO : egg die
        }

        public void recoverEgg()
        {
            // recovery
            long nowTickMilliseconds = DateTime.Now.Ticks / 10000;
            _eggInfo.nextRecoverTime = nowTickMilliseconds;
        }

        public Vector2Int LogicPos { get; set; }
        public void UpdateUnityGoPos(Vector3 pos)
        {
            _cmGame.MapLogicObjects.Remove(LogicPos);
            LogicPos = AstarUtility.GetLogicPos(pos);
            _cmGame.MapLogicObjects[LogicPos] = this;
        }
    }
}
