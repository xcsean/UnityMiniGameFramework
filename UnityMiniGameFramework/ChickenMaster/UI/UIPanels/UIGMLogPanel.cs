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
        protected Button _btnClear;
        /// <summary>
        /// 页签选中1
        /// </summary>
        protected Dictionary<LogType, int> btnStatuDic = new Dictionary<LogType, int>();
        /// <summary>
        /// 所有log日志
        /// </summary>
        protected List<GMLogInfo> infos = new List<GMLogInfo>();
        protected StyleColor btnBgColor1 = new StyleColor(new Color(50f / 255f, 50f / 255f, 50f / 255f));
        protected StyleColor btnBgColor2 = new StyleColor(new Color(70f / 255f, 70f / 255f, 70f / 255f));
        protected int logAllCnt = 0;

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
            _btnClear = this._uiObjects["btnClear"].unityVisualElement as Button;

            _btnInfo.style.color = new StyleColor(Color.white);
            _btnWarn.style.color = new StyleColor(Color.yellow);
            _btnError.style.color = new StyleColor(Color.red);

            btnStatuDic[LogType.Log] = 1;
            btnStatuDic[LogType.Warning] = 0;
            btnStatuDic[LogType.Error] = 0;

            _btnInfo.clicked += onClickBtnInfo;
            _btnWarn.clicked += onClickBtnWarn;
            _btnError.clicked += onClickBtnError;
            _btnClear.clicked += onClickBtnClear;
        }

        public override void showUI()
        {
            base.showUI();

            addUpdate(onUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();
        }

        protected void onUpdate()
        {
            if (logAllCnt != UnityGameApp.Inst.logs.Count && UnityGameApp.Inst.logs.Count > 0)
            {
                logAllCnt = UnityGameApp.Inst.logs.Count;
                onUpdateAllLog();
            }
        }

        protected void onClickBtnClear()
        {
            logAllCnt = 0;
            UnityGameApp.Inst.logs.Clear();
            onUpdateAllLog();
            showLogDetailInfo(null);
        }

        protected int _logInfoCnt = 0;
        protected int _logWarnCnt = 0;
        protected int _logErrorCnt = 0;
        protected int logInfoCnt
        {
            get { return _logInfoCnt; }
            set {
                _logInfoCnt = value;
                _btnInfo.text = _logInfoCnt > 999 ? "      999+" : $"      {_logInfoCnt}";
            } 
        }
        protected int logWarnCnt
        {
            get { return _logWarnCnt; }
            set
            {
                _logWarnCnt = value;
                _btnWarn.text = _logWarnCnt > 999 ? "      999+" : $"      {_logWarnCnt}";
            }
        }
        protected int logErrorCnt
        {
            get { return _logErrorCnt; }
            set
            {
                _logErrorCnt = value;
                _btnError.text = _logErrorCnt > 999 ? "      999+" : $"      {_logErrorCnt}";
            }
        }

        protected void onClickBtnInfo()
        {
            if (btnStatuDic[LogType.Log] == 1)
            {
                btnStatuDic[LogType.Log] = 0;
                _btnInfo.style.backgroundColor = btnBgColor1;
            }
            else
            {
                btnStatuDic[LogType.Log] = 1;
                _btnInfo.style.backgroundColor = btnBgColor2;
            }
            onUpdateAllLog();
        }
        protected void onClickBtnWarn()
        {
            if (btnStatuDic[LogType.Warning] == 1)
            {
                btnStatuDic[LogType.Warning] = 0;
                _btnWarn.style.backgroundColor = btnBgColor1;
            }
            else
            {
                btnStatuDic[LogType.Warning] = 1;
                _btnWarn.style.backgroundColor = btnBgColor2;
            }
            onUpdateAllLog();
        }
        protected void onClickBtnError()
        {
            if (btnStatuDic[LogType.Error] == 1)
            {
                btnStatuDic[LogType.Error] = 0;
                _btnError.style.backgroundColor = btnBgColor1;
            }
            else
            {
                btnStatuDic[LogType.Error] = 1;
                _btnError.style.backgroundColor = btnBgColor2;
            }
            onUpdateAllLog();
        }

        /// <summary>
        /// 刷新Log
        /// </summary>
        public void onUpdateAllLog()
        {
            logInfoCnt = 0;
            logWarnCnt = 0;
            logErrorCnt = 0;
            infos.Clear();
            for (int i = 0; i < UnityGameApp.Inst.logs.Count; i++)
            {
                switch (UnityGameApp.Inst.logs[i].type)
                {
                    case LogType.Log:
                        logInfoCnt += 1;
                        break;
                    case LogType.Warning:
                        logWarnCnt += 1;
                        break;
                    case LogType.Error:
                        logErrorCnt += 1;
                        break;
                }
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
            if (isInitLogList)
            {
                _logListView.itemsSource = logInfos;
                _logListView.RefreshItems();
                return;
            }
            if (logInfos.Count <= 0)
            {
                return;
            }
            isInitLogList = true;
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                if (i >= logInfos.Count)
                {
                    return;
                }
                var _GMLogInfo = logInfos[i];

                if (!(e as HelpBox).ClassListContains("unity-gm-font"))
                {
                    (e as HelpBox).AddToClassList("unity-gm-font");
                }
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
            _logListView.selectionType = SelectionType.Single;
            _logListView.onSelectionChange += onClickLogItem;
            _logListView.itemsSource = logInfos;

            // todo 初始自动滑动到底部

            //_logListView.SetSelection(6);
            //UnityEngine.Debug.LogError("delay SetSelection6---->");

            //_logListView.schedule.Execute(() =>
            //{
            //    _logListView.SetSelection(6);
            //    UnityEngine.Debug.LogError("delay SetSelection6---->");
            //}).StartingIn(20);
        }

        private List<string> singleList = new List<string>();
        private Label logDetailLab;

        /// <summary>
        /// 点击Log显示堆栈详情
        /// </summary>
        private void showLogDetailInfo(GMLogInfo _GMLogInfo)
        {
            singleList.Clear();
            if (_GMLogInfo == null)
            {
                _logDetailListView.itemsSource = singleList;
                _logDetailListView.RefreshItems();
                return;
            }

            singleList.Add($"{_GMLogInfo.condition}\r\n{_GMLogInfo.stackTrace}");
          
            if (logDetailLab == null)
            {
                Action<VisualElement, int> bindItem = (e, i) =>
                {
                    if (!(e as Label).ClassListContains("unity-gm-font"))
                    {
                        (e as Label).AddToClassList("unity-gm-font");
                    }
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

        private void onClickLogItem(IEnumerable<object> obj)
        {
            foreach (var item in obj)
            {
                if (item != null && (item is GMLogInfo))
                {
                    showLogDetailInfo(item as GMLogInfo);
                    break;
                }
            }
        }

    }
}
