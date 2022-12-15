﻿using System;
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
            // TO DO : init map hero 
        }

        public void setLevel(int level)
        {
            _level = level;
        }
    }
}
