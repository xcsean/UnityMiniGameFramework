using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class GMLogInfo
    {
        public string time;
        public string condition;
        public string stackTrace;
        public LogType type;
    }

    /// <summary>
    /// GM工具 Log面板
    /// </summary>
    public class UIGMLogPanel : UIPopupPanel
    {
        override public string type => "UIGMLogPanel";
        public static UIGMLogPanel create()
        {
            return new UIGMLogPanel();
        }

        protected ListView _logListView;
        protected ListView _logDetailListView;
        protected Button _btnInfo;
        protected Button _btnWarn;
        protected Button _btnError;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

        }

        protected void FindUI()
        {
            _logListView = this._uiObjects["logListView"].unityVisualElement as ListView;
            _logDetailListView = this._uiObjects["logDetailListView"].unityVisualElement as ListView;
            _btnInfo = this._uiObjects["btnInfo"].unityVisualElement as Button;
            _btnWarn = this._uiObjects["btnWarn"].unityVisualElement as Button;
            _btnError = this._uiObjects["btnError"].unityVisualElement as Button;

            btnStatuDic[LogType.Log] = 0;
            btnStatuDic[LogType.Warning] = 0;
            btnStatuDic[LogType.Error] = 0;

            _btnInfo.clicked += onClickBtnInfo;
            _btnWarn.clicked += onClickBtnWarn;
            _btnError.clicked += onClickBtnError;
        }

        public override void showUI()
        {
            base.showUI();

            onShowLogByType();
        }

        StyleColor btnColor1 = new StyleColor(new Color(50f / 255f, 50f / 255f, 50f / 255f));
        StyleColor btnColor2 = new StyleColor(new Color(70f / 255f, 70f / 255f, 70f / 255f));
        protected void onClickBtnInfo()
        {
            if (btnStatuDic[LogType.Log] == 1)
            {
                btnStatuDic[LogType.Log] = 0;
                _btnInfo.style.backgroundColor = btnColor1;
            }
            else
            {
                btnStatuDic[LogType.Log] = 1;
                _btnInfo.style.backgroundColor = btnColor2;
            }
            onShowLogByType();
        }
        protected void onClickBtnWarn()
        {
            if (btnStatuDic[LogType.Warning] == 1)
            {
                btnStatuDic[LogType.Warning] = 0;
                _btnWarn.style.backgroundColor = btnColor1;
            }
            else
            {
                btnStatuDic[LogType.Warning] = 1;
                _btnWarn.style.backgroundColor = btnColor2;
            }
            onShowLogByType();
        }
        protected void onClickBtnError()
        {
            if (btnStatuDic[LogType.Error] == 1)
            {
                btnStatuDic[LogType.Error] = 0;
                _btnError.style.backgroundColor = btnColor1;
            }
            else
            {
                btnStatuDic[LogType.Error] = 1;
                _btnError.style.backgroundColor = btnColor2;
            }
            onShowLogByType();
        }

        protected Dictionary<LogType, int> btnStatuDic = new Dictionary<LogType, int>();
        protected List<GMLogInfo> infos = new List<GMLogInfo>();
        protected void onShowLogByType()
        {
            infos.Clear();
            for (int i = 0; i < UnityGameApp.Inst.logs.Count; i++)
            {
                if (btnStatuDic[UnityGameApp.Inst.logs[i].type] == 1)
                {
                    infos.Add(UnityGameApp.Inst.logs[i]);
                }
            }
            showLogListInfo(infos);
        }

        protected HelpBoxMessageType logTypeToHelpBoxMessageType(LogType _type)
        {
            switch (_type)
            {
                case LogType.Log:
                    return HelpBoxMessageType.Info;
                case LogType.Warning:
                    return HelpBoxMessageType.Warning;
                case LogType.Error:
                    return HelpBoxMessageType.Error;
                default:
                    break;
            }
            return HelpBoxMessageType.Info;
        }

        private bool isInitLogList = false;
        protected void showLogListInfo(List<GMLogInfo> logInfos)
        {
            if (logInfos.Count == 0)
            {
                _logListView.RefreshItems();
                return;
            }
            if (isInitLogList)
            {
                //return;
            }
            isInitLogList = true;
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                if (i >= logInfos.Count)
                {
                    return;
                }
                var _GMLogInfo = logInfos[i];
                (e as HelpBox).text = $"{_GMLogInfo.time} {_GMLogInfo.condition}";
                (e as HelpBox).messageType = logTypeToHelpBoxMessageType(_GMLogInfo.type);

                (e as HelpBox).style.borderBottomWidth = 0;
                (e as HelpBox).style.borderTopWidth = 0;
                (e as HelpBox).style.fontSize = 18;
                (e as HelpBox).style.whiteSpace = WhiteSpace.Normal;
                (e as HelpBox).style.width = new StyleLength(new Length(680));
                (e as HelpBox).style.color = new StyleColor(new Color(240f / 255f, 240f / 255f, 240f / 255f));
                (e as HelpBox).style.backgroundColor = i % 2 == 0 ? new StyleColor(new Color(30f / 255f, 30f / 255f, 30f / 255f)) : new StyleColor(new Color(38f / 255f, 38f / 255f, 38f / 255f));
            };
            _logListView.fixedItemHeight = 60;
            _logListView.makeItem = () => new HelpBox();
            _logListView.bindItem = bindItem;
            _logListView.itemsSource = logInfos;
            _logListView.selectionType = SelectionType.Single;
            _logListView.onSelectionChange += onClickLogItem;

            //_logListView.SetSelection(6);
            //UnityEngine.Debug.LogError("delay SetSelection6---->");

            //_logListView.schedule.Execute(() =>
            //{
            //    _logListView.SetSelection(6);
            //    UnityEngine.Debug.LogError("delay SetSelection6---->");
            //}).StartingIn(20);

            // todo 初始自动滑动到底部
            _logListView.itemsSourceChanged += () =>
            {
                UnityEngine.Debug.LogError("itemsSourceChanged---->");
                //_logListView.SetSelection(logInfos.Count - 1);
            };
        }

        List<string> singleList = new List<string>();
        Label logDetailLab;
        private void showLogDetailInfo(GMLogInfo _GMLogInfo)
        {
            singleList.Clear();
            singleList.Add($"{_GMLogInfo.condition}\r\n{_GMLogInfo.stackTrace}");

            if (singleList.Count > 0)
            {
                if (logDetailLab == null)
                {
                    Action<VisualElement, int> bindItem = (e, i) =>
                    {
                        logDetailLab = e as Label;
                        (e as Label).text = singleList[i];
                        (e as Label).style.fontSize = 18;
                        (e as Label).style.whiteSpace = WhiteSpace.Normal;
                        (e as Label).style.width = new StyleLength(new Length(680));
                        (e as Label).style.color = new StyleColor(new Color(240f / 255f, 240f / 255f, 240f / 255f));
                        (e as Label).style.backgroundColor = new StyleColor(new Color(30f / 255f, 30f / 255f, 30f / 255f, 0f));
                    };
                    _logDetailListView.makeItem = () => new Label();
                    _logDetailListView.bindItem = bindItem;
                    _logDetailListView.itemsSource = singleList;
                    _logDetailListView.selectionType = SelectionType.None;
                }
                else
                {
                    logDetailLab.text = singleList[0];
                }
            }
        }

        private void onClickLogItem(IEnumerable<object> obj)
        {
            foreach (var item in obj)
            {
                if (item != null && (item is GMLogInfo))
                {
                    UnityEngine.Debug.Log("obj2=" + (DateTime.Now.ToString("hh:mm:ss")));
                    showLogDetailInfo(item as GMLogInfo);
                    break;
                }
            }
        }

    }
}
