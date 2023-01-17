using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIGMLogPanel : UIPopupPanel
    {
        override public string type => "UIGMLogPanel";
        public static UIGMLogPanel create()
        {
            return new UIGMLogPanel();
        }

        protected ListView _logListView;
        protected ListView _logDetailListView;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

            showLogInfo();
        }

        protected void FindUI()
        {
            //BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            _logListView = this._uiObjects["logListView"].unityVisualElement as ListView;
            _logDetailListView = this._uiObjects["logDetailListView"].unityVisualElement as ListView;
        }

        List<Label> labNodes = new List<Label>();
        protected void showLogInfo(int logType = 0)
        {
            List<string> logs = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                logs.Add($"Test i={i}\r\nTest hahaha");
            }
            //for (int i = 0; i < logs.Count; i++)
            //{
            //    if (i < labNodes.Count)
            //    {
            //        labNodes[i].style.display = DisplayStyle.Flex;
            //    }
            //    else
            //    {
            //        //Label lab = new Label();
            //        //lab.text = logs[i];
            //        //_logListView.Add(lab);
            //        //labNodes.Add(lab);
            //    }
            //}

            //if (labNodes.Count > logs.Count)
            //{
            //    for (int i = logs.Count; i < labNodes.Count; i++)
            //    {
            //        labNodes[i].style.display = DisplayStyle.None;
            //    }
            //}

            Func<VisualElement> makeItem = () => new Label();
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as Label).AddToClassList("unity-gm-log-label");
                (e as Label).text = logs[i];
            };
            _logListView.makeItem = makeItem;
            _logListView.bindItem = bindItem;
            _logListView.itemsSource = logs;
            _logListView.selectionType = SelectionType.Multiple;
            _logListView.onItemsChosen += onClickLog;
        }

        private void onClickLog(object obj)
        {
            UnityEngine.Debug.Log("obj=" + obj);
        }
    }
}
