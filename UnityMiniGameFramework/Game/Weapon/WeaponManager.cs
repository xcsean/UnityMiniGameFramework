using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class WeaponManager
    {
        protected WeaponConfig _weaponConf;
        public WeaponConfig WeaponConf => _weaponConf;

        protected HashSet<ExplosiveObject> _explosiveObjects;

        public WeaponManager()
        {
            _explosiveObjects = new HashSet<ExplosiveObject>();
        }

        public void Init()
        {
            _weaponConf = (WeaponConfig)UnityGameApp.Inst.Conf.getConfig("weapons");
        }

        public ExplosiveObject CreateExplosiveObject(ExplosiveConf conf)
        {
            var obj = new ExplosiveObject();
            if(!obj.Init(conf))
            {
                return null;
            }

            _explosiveObjects.Add(obj);
            return obj;
        }

        public void onExplosiveDestory(ExplosiveObject obj)
        {
            _explosiveObjects.Remove(obj);
        }

        public void OnUpdate(float deltaTime)
        {
            foreach(var expObj in _explosiveObjects)
            {
                expObj.OnUpdate();
            }
        }
    }
}
