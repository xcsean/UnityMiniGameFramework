using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class UnityGameObjectBehaviour : MonoBehaviour
    {
        protected MGGameObject _mgGameObject;

        public string mgGameObjectType;
        public string mgGameObjectConfigName;

        public MGGameObject mgGameObject => _mgGameObject;

        public UnityGameObjectBehaviour()
        {
        }

        protected virtual void Awake()
        {
        }
        
        protected virtual void Start()
        {
            _mgGameObject = (MGGameObject)GameObjectManager.createGameObject(mgGameObjectType);
            _mgGameObject.setUnityGameObject(this.gameObject);
            _mgGameObject.Init(mgGameObjectConfigName);
        }

        protected virtual void Update()
        {
            _mgGameObject.OnUpdate((uint)(Time.deltaTime*1000));
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
            _mgGameObject.OnPostUpdate((uint)(Time.deltaTime * 1000));
        }
    }

    public class MGGameObject : MiniGameFramework.GameObject
    {
        override public string type => "MGGameObject";

        protected UnityEngine.GameObject _unityGameObject;

        public UnityEngine.GameObject unityGameObject => _unityGameObject;

        public void setUnityGameObject(UnityEngine.GameObject o)
        {
            _unityGameObject = o;
        }
    }
}
