using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class Skelton
    {
        protected GameObject _skel;

        public GameObject skeltonObject => _skel;

        public GameObject root => _skel;

        public Skelton(GameObject o)
        {
            _skel = o;
        }

        public GameObject getBone(string bonePathName)
        {
            Transform trans = _skel.transform.Find(bonePathName);
            if (trans == null)
            {
                return null;
            }

            return trans.gameObject;
        }

        public GameObject findBone(string name)
        {
            // do hierarchy find
            Transform trans = _skel.transform.FirstChildOrDefault(o => o.name == name);
            if (trans == null)
            {
                return null;
            }

            return trans.gameObject;
        }

        public void setAddtionalTransform(string bonePathName, Matrix4x4 transMatrix)
        {
            // TO DO : rec and do additonal transform in update
        }
    }
}
