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

        protected Vector3 _distance;
        protected UnityEngine.GameObject _followObject;

        protected Camera _unityCamera;
        public Camera unityCamera => _unityCamera;

        override public void Init(string confname)
        {
            _unityCamera = _unityGameObject.GetComponent<Camera>();

            // TO DO : init from config
            _distance = new UnityEngine.Vector3(0, 8f, 8f);

            //_distance = 0.45 0.01 0.39 - 2.14 6.44 9.26 
        }

        public void follow(IGameObject obj)
        {
            _followObject = (obj as MGGameObject).unityGameObject;
            //_distance = _unityGameObject.transform.position - _followObject.transform.position;
        }

        public UnityEngine.Vector2 worldToScreenPos(UnityEngine.Vector3 pos)
        {
            var vec = _unityCamera.WorldToScreenPoint(pos);
            return new UnityEngine.Vector2(vec.x, Screen.height - vec.y);
        }
        public UnityEngine.Vector3 screenToWorldPos(UnityEngine.Vector2 screenPos)
        {
            return _unityCamera.ScreenToWorldPoint(screenPos);
        }
        public UnityEngine.Ray screenToWorldRay(UnityEngine.Vector2 screenPos)
        {
            return _unityCamera.ScreenPointToRay(screenPos);
        }

        override public void OnPostUpdate(float timeElasped)
        {
            base.OnPostUpdate(timeElasped);

            if (_followObject != null)
            {
                _unityGameObject.transform.position = _followObject.transform.position + _distance;
            }
        }
    }
}
