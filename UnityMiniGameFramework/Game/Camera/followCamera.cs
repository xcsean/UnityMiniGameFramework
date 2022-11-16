using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    class followCamera : MonoBehaviour
    {
        protected Vector3 _distance;

        public GameObject followObject = null;
        
        // Start is called before the first frame update
        void Start()
        {
            if(followObject != null)
            {
                _distance = this.gameObject.transform.position - followObject.transform.position;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        // LateUpdate is called once per frame
        void LateUpdate()
        {
            if (followObject != null)
            {
                this.gameObject.transform.position = followObject.transform.position + _distance;
            }
        }
    }
}
