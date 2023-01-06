using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{ 
    public class MapMonsterObject : MapRoleObject
    {
        override public string type => "MapMonsterObject";
        new public static MapMonsterObject create()
        {
            return new MapMonsterObject();
        }

        protected MapMonsterObjectConf _mapMonConf;
        public MapMonsterObjectConf mapMonConf => _mapMonConf;

        protected int _level;
        public int level => _level;

        override protected ActorObjectConfig _getActorConf(string confname)
        {
            if (UnityGameApp.Inst.MapManager.MapConf == null)
            {
                return null;
            }
            _mapMonConf = UnityGameApp.Inst.MapManager.MapConf.getMapMonsterConf(confname);

            if (UnityGameApp.Inst.CharacterManager.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.CharacterManager.CharacterConfs.getActorConf(_mapMonConf.actorConfName);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            _name = confname;

            if(_mapMonConf.movingConf != null)
            {
                this._rigiMovAct.setMovingConf(_mapMonConf.movingConf);
            }
        }

        public void setLevel(int level)
        {
            _level = level;
        }

        public void CombatComp_OnRecalcAttributes()
        {
            var combatComp = getComponent("CombatComponent") as CombatComponent;

            if(combatComp == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"MapMonsterObject [{_name}] without CombatComponent");
                return;
            }

            var bufAttrs = combatComp.bufAttrs.ToArray();

            // calc speed
            _rigiMovAct.onRecalcAttributes(bufAttrs);
        }
    }
}
