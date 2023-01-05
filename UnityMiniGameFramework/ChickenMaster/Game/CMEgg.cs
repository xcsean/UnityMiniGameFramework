using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMEgg
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

        public CMEgg()
        {
            _isInited = false;
        }

        public void Init(LocalEggInfo eggInfo)
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _eggInfo = eggInfo;
            _conf = cmGame.gameConf.gameConfs.eggConf;

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

            // TO DO : move hpbar show/hide code to level start/end 
            if(UnityGameApp.Inst.MainScene.map.currentLevel != null && UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
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
                if(!_eggUI.isShow)
                {
                    var vec = (_eggObject.transform.position - (UnityGameApp.Inst.Game as ChickenMasterGame).Self.mapHero.unityGameObject.transform.position);

                    //if(vec.magnitude <= _conf.EggUIShowRange)
                    //{
                    //    _eggUI.showUI();
                    //}
                }
                _eggUI.changeEggState(false);
            }

            if(_eggUI != null)
            {
                var screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera).worldToScreenPos(_eggObject.transform.position));
                _eggUI.setPoisition((int)screenPos.x + (int)_conf.EggUIOffset.x, (int)screenPos.y + (int)_conf.EggUIOffset.y);
            }

            long nowTickMilliseconds = DateTime.Now.Ticks / 10000;
            if (_eggInfo.hp > 0)
            {
                // wait increase
                if (_incHpTime <= nowTickMilliseconds)
                {
                    _eggInfo.lastIncHpTime = nowTickMilliseconds;
                    _incHpTime = _eggInfo.lastIncHpTime + _conf.hpIncTime;

                    _eggInfo.hp += 1 + (int)((nowTickMilliseconds - _incHpTime) / _conf.hpIncTime);
                    if (_eggInfo.hp > _conf.maxHp)
                    {
                        _eggInfo.hp = _conf.maxHp;
                    }

                    ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                    cmGame.baseInfo.markDirty();

                    _updateHpBar();
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
    }
}
