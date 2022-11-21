using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UnityGameCamera : MGGameObject, ICamera
    {
        override public string type => "UnityGameCamera";
        new public static UnityGameCamera create()
        {
            return new UnityGameCamera();
        }

        protected Camera _unityCamera;
        public Camera unityCamera => _unityCamera;

        override public void Init(string confname)
        {
            _unityCamera = _unityGameObject.GetComponent<Camera>();

            // TO DO : init
        }
    }
}
