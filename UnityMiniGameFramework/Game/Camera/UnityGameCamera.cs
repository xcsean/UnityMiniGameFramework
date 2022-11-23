﻿using System;
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

        protected Vector3 _distance;
        protected UnityEngine.GameObject _followObject;

        protected Camera _unityCamera;
        public Camera unityCamera => _unityCamera;

        override public void Init(string confname)
        {
            _unityCamera = _unityGameObject.GetComponent<Camera>();

            // TO DO : init
        }

        public void follow(IGameObject obj)
        {
            _followObject = (obj as MGGameObject).unityGameObject;
            _distance = _unityGameObject.transform.position - _followObject.transform.position;
        }

        override public void OnPostUpdate(uint timeElasped)
        {
            base.OnPostUpdate(timeElasped);

            if (_followObject != null)
            {
                _unityGameObject.transform.position = _followObject.transform.position + _distance;
            }
        }
    }
}
