using System.Collections;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class UIImageNumText : UIImageText
    {
        public override string text
        {
            get=>base.text;
            set => base.text = StringUtil.StringNumFormat(value);
        }

        public void setFont(string fontName)
        {
            
        }

        public void OnPlayEnd()
        {
            Destroy(gameObject, 0.1f);
        }
        
        
        
    }
}