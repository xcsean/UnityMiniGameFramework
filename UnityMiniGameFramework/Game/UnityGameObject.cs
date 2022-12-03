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
        public string mgGameObjectType = "MGGameObject";
        public string mgGameObjectConfigName;

        protected MGGameObject _mgGameObject;
        public MGGameObject mgGameObject => _mgGameObject;

        public UnityGameObjectBehaviour()
        {
        }

        protected virtual void Awake()
        {
            _mgGameObject = (MGGameObject)GameObjectManager.createGameObject(mgGameObjectType);
            _mgGameObject.setUnityGameObject(this.gameObject);
            _mgGameObject.Init(mgGameObjectConfigName);
        }
        
        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            _mgGameObject.OnUpdate(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
            _mgGameObject.OnPostUpdate(Time.deltaTime);
        }
    }

    public class MGGameObject : MiniGameFramework.GameObject
    {
        override public string type => "MGGameObject";
        public static MGGameObject create()
        {
            return new MGGameObject();
        }

        protected UnityEngine.GameObject _unityGameObject;

        public UnityEngine.GameObject unityGameObject => _unityGameObject;

        public void setUnityGameObject(UnityEngine.GameObject o)
        {
            _unityGameObject = o;
        }


        override public void Hide()
        {
            _unityGameObject.SetActive(false);
        }

        override public void Show()
        {
            _unityGameObject.SetActive(true);
        }
    }
}
