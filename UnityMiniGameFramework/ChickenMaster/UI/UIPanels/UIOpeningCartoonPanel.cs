using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class BlackItem
    {
        public VisualElement Node;
        public Action InitFunc;
        public float DelayTime;
    }

    /// <summary>
    /// 开场漫画
    /// </summary>
    public class UIOpeningCartoonPanel : UIPanel
    {
        override public string type => "UIOpeningCartoonPanel";
        public static UIOpeningCartoonPanel create()
        {
            return new UIOpeningCartoonPanel();
        }

        protected VisualElement _page1;
        protected VisualElement _page2;

        protected List<BlackItem> blackList = new List<BlackItem>();

        protected float _timeInterval = 1.0f;
        protected float _time = 0;
        protected BlackItem _curFadeItem = null;
        private float _curFadeOpacity = 100;
        private bool isEnd = false;


        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUi();

            InitOpeningQueue();
        }

        protected void FindUi()
        {
            _page1 = this._uiObjects["page1"].unityVisualElement;
            _page2 = this._uiObjects["page2"].unityVisualElement;
        }

        protected void InitOpeningQueue()
        {
            blackList.Clear();
            isEnd = false;

            List<VisualElement> pageList = new List<VisualElement>();
            pageList.Add(_page1);
            pageList.Add(_page2);

            foreach (var page in pageList)
            {
                var group = page.Q<VisualElement>("blackGroup");
                group.style.display = DisplayStyle.Flex;
                page.style.display = DisplayStyle.None;

                blackList.Add(new BlackItem()
                {
                    InitFunc = () =>
                    {
                        page.style.display = DisplayStyle.Flex;
                    },
                    DelayTime = 0
                });

                foreach (var black in group.Children())
                {
                    blackList.Add(new BlackItem()
                    {
                        Node = black,
                        InitFunc = () =>
                        {
                            //black.style.opacity = 100;
                        },
                        DelayTime = 2
                    });
                }
            }
        }

        /// <summary>
        /// 开始开场漫画播放
        /// </summary>
        protected void OnUpdatePlay()
        {
            // 黑色遮罩慢慢消失
            if (_curFadeItem != null && _curFadeItem.Node != null && _curFadeOpacity > 0)
            {
                _curFadeOpacity = Mathf.Lerp(0, _curFadeOpacity, Time.deltaTime * 1f);
                _curFadeItem.Node.style.opacity = (int)_curFadeOpacity;

                if (_curFadeOpacity == 0)
                {
                    _curFadeItem = null;
                }
            }
            if (isEnd)
            {
                return;
            }
            _time += Time.deltaTime;
            if (_time < _timeInterval)
            {
                return;
            }
            if (blackList.Count < 1)
            {
                isEnd = true;
                EndPlay();
                return;
            }
            // 避免超出DelayTime时间 透明度变化还没执行完毕
            if (_curFadeItem != null && _curFadeItem.Node != null) _curFadeItem.Node.style.opacity = 0;

            _curFadeItem = blackList.First();
            _curFadeItem.InitFunc();
            blackList.RemoveAt(0);

            _timeInterval = _curFadeItem.DelayTime;
            _time = 0;
            _curFadeOpacity = 100;
        }

        /// <summary>
        /// 结束开场漫画播放
        /// </summary>
        public void EndPlay()
        {
            OnEnterGame();
            //hideUI();
        }

        /// <summary>
        /// 进入游戏战斗场景
        /// </summary>
        private void OnEnterGame()
        {
            UnityGameApp.Inst.LoadMainScene();
        }

        public override void showUI()
        {
            base.showUI();
            unityUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            UnityGameApp.Inst.addUpdateCall(this.OnUpdatePlay);
        }

        public override void hideUI()
        {
            base.hideUI();
            unityUIDocument.rootVisualElement.style.display = DisplayStyle.None;
            // to do: update方法里移除会导致Hashset的foreach遍历报错
            //UnityGameApp.Inst.removeUpdateCall(this.OnUpdatePlay);
        }

    }
}
