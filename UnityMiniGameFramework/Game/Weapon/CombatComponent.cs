using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGameFramework;
using UnityEngine;
using GameObject = UnityEngine.GameObject;

namespace UnityMiniGameFramework
{
    public enum DamageTypeEnum
    {
        Attack = 1,
        Critical = 2,
        Dot = 3,
    }

    public class HealthBar
    {
        UnityEngine.GameObject _barObject;
        public UnityEngine.GameObject barObject => _barObject;
        private List<Vector2> m_meshUVs = new List<Vector2>(4);
        private MeshFilter _meshFilter;
        private Transform _iconRootTransform;
        private List<SpriteRenderer> _childList;
        private Stack<SpriteRenderer> _cacheStack;

        public void Init()
        {
            // TO DO : init bar object from manager

            // for Debug ...
            _barObject = UnityGameObjectPool.GetInstance().GetUnityPrefabObject("actor/HealthBar");
            //_barObject = UnityEngine.GameObject.Instantiate(_barObject);
            // TO DO : set transform to hp bar root
            _barObject.transform.SetParent(((MGGameObject) UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject
                .transform);
            _meshFilter = _barObject.GetComponent<MeshFilter>();
            _meshFilter.mesh.GetUVs(0, m_meshUVs);
            _iconRootTransform = _barObject.transform.Find("buffIconList");
            _childList = new List<SpriteRenderer>();
            _cacheStack = new Stack<SpriteRenderer>();
            for (int i = 0; i < _iconRootTransform.childCount; i++)
            {
                var sp = _iconRootTransform.GetChild(i).GetComponent<SpriteRenderer>();
                if (sp != null)
                    _cacheStack.Push(sp);
            }
            setHp(1);
        }

        public void AddBuffIcon(string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath))
                return;
            Sprite sprite = UnityGameApp.Inst.UnityResource.LoadSprite(iconPath);
            if (!sprite)
                return;
            SpriteRenderer sp = _cacheStack.Count != 0 ? _cacheStack.Pop() : null;
            if (sp == null)
            {
                GameObject go = new GameObject();
                go.name = iconPath;
                sp = go.AddComponent<SpriteRenderer>();
                go.transform.SetParent(_iconRootTransform);
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
            }

            sp.enabled = true;
            sp.transform.localPosition = new Vector3(0.32f * _childList.Count, 0, 0);
            sp.sprite = sprite;
            _childList.Add(sp);
        }

        public void SubBuffIcon(string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath))
                return;
            int removeIndex = -1;
            for (int i = 0; i < _childList.Count; i++)
            {
                if (removeIndex == -1 && _childList[i].gameObject.name == iconPath)
                {
                    removeIndex = i;
                }

                if (removeIndex != -1 && i > removeIndex)
                {
                    _childList[i].transform.localPosition = new Vector3(0.32f * (i - removeIndex), 0, 0);
                }
            }

            if (removeIndex == -1)
                return;
            _cacheStack.Push(_childList[removeIndex]);
            _childList.RemoveAt(removeIndex);
        }

        public void Dispose()
        {
            UnityGameObjectPool.GetInstance().PutUnityPrefabObject("actor/HealthBar", _barObject);
            _barObject = null;
            for (int i = 0; i < _childList.Count; i++)
            {
                _childList[i].enabled = false;
            }

            _childList.Clear();
            _cacheStack.Clear();
        }

        //private static readonly int Fill = Shader.PropertyToID("_Fill");

        public void setHp(float per)
        {
            // TO DO : change shader to quad hp bar shader, set mat value, do batch reandering
            //_barObject.transform.localScale = new UnityEngine.Vector3(per * 0.5f, _barObject.transform.localScale.y, _barObject.transform.localScale.z);
            //_barObject.GetComponent<Renderer>().material.SetFloat(Fill, per > 0.3f ? 1.0f : 0.5f);

            if (per > 0.5f)
            {
                m_meshUVs[0] = new Vector2(0, 0);
                m_meshUVs[1] = new Vector2(1 + 0.5f - per, 0);
                m_meshUVs[2] = new Vector2(0, 1);
                m_meshUVs[3] = new Vector2(1 + 0.5f - per, 1);
            }
            else
            {
                m_meshUVs[0] = new Vector2(0.5f - per, 0);
                m_meshUVs[1] = new Vector2(1, 0);
                m_meshUVs[2] = new Vector2(0.5f - per, 1);
                m_meshUVs[3] = new Vector2(1, 1);
            }

            _meshFilter.mesh.SetUVs(0, m_meshUVs);
        }

        public void show()
        {
            _barObject = UnityGameObjectPool.GetInstance().GetUnityPrefabObject("actor/HealthBar");
        }

        public void hide()
        {
            Dispose();
        }

        public void setPosition(Vector3 pos)
        {
            if (_barObject == null)
                return;
            _barObject.transform.position = pos;
        }
    }

    abstract public class CombatComponent : GameObjectComponent
    {
        protected HealthBar _hpBar;
        public HealthBar hpBar => _hpBar;

        protected int _HP;
        protected int _maxHP;

        protected int _Def;

        public Action<ActorObject> OnDie;

        protected bool _isDie;

        protected HashSet<ActBufAttrConfig> _bufAttrs;
        public HashSet<ActBufAttrConfig> bufAttrs => _bufAttrs;

        protected CombatConf _ccConf;


        public override void Init(object config)
        {
            base.Init(config);

            _ccConf = config as CombatConf;

            _HP = _ccConf.hpMax;
            _maxHP = _ccConf.hpMax;
            _Def = _ccConf.def;

            // for Debug ...
            // init hp bar

            _hpBar = new HealthBar();
            _hpBar.Init();

            _isDie = false;
            // TO DO : init hp bar from config

            _bufAttrs = new HashSet<ActBufAttrConfig>();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_hpBar != null)
            {
                _hpBar.Dispose();
                _hpBar = null;
            }
        }

        public override void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            var mgGameObject = _gameObject as MGGameObject;
            var position = mgGameObject.unityGameObject.transform.position;
            _hpBar.setPosition(new UnityEngine.Vector3(
                position.x,
                position.y + 1.5f,
                position.z));
        }

        virtual public void OnHitByWeapon(WeaponObject weapon)
        {
            if (_isDie)
            {
                return;
            }

            // check missing
            if (UnityGameApp.Inst.Rand.IsRandomHit(weapon.attackInfo.missingRate))
            {
                // missed
                _onHitMissed(weapon.holder);
                return;
            }

            OnDamageCalculation(weapon);
        }

        protected virtual void OnDamageCalculation(WeaponObject weapon)
        {
            // rand dmg
            int dmg = _onDamageCalculation(weapon);

            bool critical = false;
            if (UnityGameApp.Inst.Rand.IsRandomHit(weapon.attackInfo.criticalHitRate))
            {
                // critical
                dmg = (int) (dmg * weapon.attackInfo.criticalHitPer);
                critical = true;
            }

            OnDamageBy(weapon.holder, dmg, critical ? DamageTypeEnum.Critical : DamageTypeEnum.Attack);
        }

        protected virtual int _onDamageCalculation(WeaponObject weapon)
        {
            return UnityGameApp.Inst.Rand.RandomBetween(weapon.attackInfo.attackMin, weapon.attackInfo.attackMax);
        }

        public virtual int OnDamageCalByConf(List<ActBufAttrConfig> buffAttrs, int dmg = 0)
        {
            return 0;
        }


        virtual public void OnDamageBy(ActorObject actor, int dmg, DamageTypeEnum damageType)
        {
            if (_isDie)
            {
                return;
            }

            dmg = dmg - _Def;

            _onDamage(actor, dmg, damageType);

            _HP -= dmg;
            if (_HP < 0)
            {
                _HP = 0;
            }

            _hpBar.setHp((float) _HP / _maxHP);

            if (_HP <= 0)
            {
                _isDie = true;

                _onDie(actor);

                if (OnDie != null)
                {
                    OnDie(_gameObject as ActorObject);
                }
            }
        }

        public virtual void AddBufAttr(ActBufAttrConfig attr)
        {
            _bufAttrs.Add(attr);
        }

        public virtual void RemoveBufAttr(ActBufAttrConfig attr)
        {
            _bufAttrs.Remove(attr);
        }

        public virtual void RecalcAttributes()
        {
        }

        virtual protected void _onHitMissed(ActorObject actor)
        {
        }

        virtual protected void _onDamage(ActorObject actor, int dmg, DamageTypeEnum damageTypeEnum)
        {
        }

        virtual protected void _onDie(ActorObject actor)
        {
        }

        public void AddBuffIcon(string iconPath)
        {
            _hpBar.AddBuffIcon(iconPath);
        }

        public void SubBuffIcon(string iconPath)
        {
            _hpBar.SubBuffIcon(iconPath);
        }
    }
}