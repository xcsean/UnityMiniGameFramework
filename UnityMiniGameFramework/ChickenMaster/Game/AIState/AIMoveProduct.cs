using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AIMoveProduct : AIState
    {
        public static AIMoveProduct create(ActorObject actor)
        {
            return new AIMoveProduct(actor);
        }

        protected RigibodyMoveAct _movAct;
        protected List<CMFactory> _factories;

        protected string _targetName;

        public AIMoveProduct(ActorObject actor) : base(actor)
        {
            _movAct = (actor as MapRoleObject).moveAct;

            _factories = new List<CMFactory>();
        }

        public override void Init(MapConfAIState conf)
        {
            base.Init(conf);

            _targetName = conf.targetName;
        }

        protected void _initFactories()
        {
            if(_factories.Count > 0)
            {
                return;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            if (_targetName == "trainStation")
            {
                // move product to train station

                // for Debug ...
                var fac = cmGame.GetFactory("factoryBuilding1");
                if(fac != null)
                {
                    _factories.Add(fac);
                }
                fac = cmGame.GetFactory("factoryBuilding2");
                if (fac != null)
                {
                    _factories.Add(fac);
                }
            }
        }


        override public void OnUpdate()
        {
            // for Debug ...
            // direct sell product
            _initFactories();

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            foreach (var fac in _factories)
            {
                if(fac == null)
                {
                    continue;
                }

                foreach(var outputProd in fac.localFacInfo.buildingOutputProducts)
                {
                    if(outputProd.count > 100)
                    {
                        var prodConf = cmGame.gameConf.getCMProductConf(outputProd.productName);
                        if(prodConf == null)
                        {
                            continue;
                        }
                        int fetchCount = fac.fetchProduct(outputProd.productName, 100);
                        if(fetchCount > 0)
                        {
                            cmGame.Self.AddGold(prodConf.price * fetchCount);
                        }
                    }
                }
            }
        }
    }
}
