using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class VFXComponentConfig
    {
        public string componentType { get; set; }
    }

    public class VFXComponent : GameObjectComponent
    {
        override public string type => "VFXComponent";
        public static VFXComponent create()
        {
            return new VFXComponent();
        }

        protected Dictionary<string, VFX> _vfxs; // vfx name => vfx
        protected List<VFX> _currVfxs;
        
        public List<VFX> currVFXs => _currVfxs;

        public VFX createVFX(string vfxName)
        {
            return null;
        }

        override public void Init(object config)
        {
            base.Init(config);
        }
        override public void Dispose()
        {
            base.Dispose();
        }

        override public void OnUpdate(uint timeElasped)
        {

        }
        override public void OnPostUpdate(uint timeElasped)
        {

        }
    }
}
