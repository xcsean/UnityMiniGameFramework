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
    public class PopupNumber
    {
        public string Text;
        public Color TextColor;
        public float LifeTime;
        public Vector3 UpPos;
    }

    /// <summary>
    /// 生产建筑头顶生产进度条
    /// </summary>
    public class UIProduceProgressPanel : UIPanel
    {
        override public string type => "UIProduceProgressPanel";
        public static UIProduceProgressPanel create()
        {
            return new UIProduceProgressPanel();
        }

        protected Label _labLeftPopup;
        protected Label _labRightPopup;
        protected Label _labStockCnt;
        protected Label _labProduceCnt;
        protected Label _labLockTip;
        protected VisualElement _sprStockGoods;
        protected VisualElement _sprProduceGoods;

        protected VisualElement _barValue;
        protected VisualElement _customerBar;
        protected VisualElement _wait;
        protected VisualElement _arrow;
        protected VisualElement _unlock;

        protected CMFactory _CMFactory;
        protected CMFactoryConf _factoryConf;
        protected int _lastUpdateProduceVer;
        protected PopupNumber leftPopupNumber;
        protected PopupNumber rightPopupNumber;

        protected Color _red = new Color(237f / 255f, 77f / 255f, 10f / 255f);
        protected Color _green = new Color(146f / 255f, 234f / 255f, 75f / 255f);

        protected Transform followTrans;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

            WaitAction();
        }

        protected void FindUI()
        {
            _labLockTip = this._uiObjects["labLockTip"].unityVisualElement as Label;
            _labLeftPopup = this._uiObjects["labLeftPopup"].unityVisualElement as Label;
            _labRightPopup = this._uiObjects["labRightPopup"].unityVisualElement as Label;
            _labStockCnt = this._uiObjects["labStockCnt"].unityVisualElement as Label;
            _sprStockGoods = this._uiObjects["sprStockGoods"].unityVisualElement;
            _labProduceCnt = this._uiObjects["labProduceCnt"].unityVisualElement as Label;
            _sprProduceGoods = this._uiObjects["sprProduceGoods"].unityVisualElement;
            _barValue = this._uiObjects["barValue"].unityVisualElement;
            _customerBar = this._uiObjects["customerBar"].unityVisualElement;
            _wait = this._uiObjects["wait"].unityVisualElement;
            _arrow = this._uiObjects["arrow"].unityVisualElement;
            _unlock = this._uiObjects["unlock"].unityVisualElement;

            _sprStockGoods.style.backgroundImage = null;
            _sprProduceGoods.style.backgroundImage = null;
            _labLeftPopup.text = "";
            _labRightPopup.text = "";
        }

        public void RefreshInfo(CMFactory _cmFactory, CMFactoryConf conf)
        {
            if (conf != null)
            {
                _factoryConf = conf;
            }
            _CMFactory = _cmFactory;
            if (_CMFactory == null || !_CMFactory.IsActive)
            {
                _labLockTip.text = $"Unlock at\r\nbattle level {_factoryConf.userLevelRequire}";
                _labLockTip.style.display = DisplayStyle.Flex;
                _unlock.style.display = DisplayStyle.None;
                DoUpdateInputStore(0, 0);
                DoUpdatePruduceGoods(0, 0);
                return;
            }
            _labLockTip.style.display = DisplayStyle.None;
            _unlock.style.display = DisplayStyle.Flex;

            _lastUpdateProduceVer = _CMFactory.produceVer;

            DoUpdateInputStore(_CMFactory.currentProductInputStore, 0);
            DoUpdatePruduceGoods(_CMFactory.currentProductOutputStore, 0);

            // 产品图
            if (_sprStockGoods.style.backgroundImage == StyleKeyword.Null)
            {
                var outTx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadProductIcon($"icon_{_cmFactory.factoryConf.inputProductName}");
                if (outTx != null)
                {
                    _sprStockGoods.style.backgroundImage = outTx;
                }
            }
            if (_sprProduceGoods.style.backgroundImage == StyleKeyword.Null)
            {
                var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadProductIcon($"icon_{_cmFactory.factoryConf.outputProductName}");
                if (tx != null)
                {
                    _sprProduceGoods.style.backgroundImage = tx;
                }
            }
        }

        /// <summary>
        /// 原料输入工厂或生产时扣除原料
        /// </summary>
        public void DoUpdateInputStore(int totalCnt, int changeCnt)
        {
            // 库存
            _labStockCnt.text = $"{totalCnt}";

            if (totalCnt == 0)
            {
                _wait.style.display = DisplayStyle.Flex;
                _arrow.style.display = DisplayStyle.None;
                _customerBar.style.display = DisplayStyle.None;
            }
            else
            {
                _wait.style.display = DisplayStyle.None;
                _arrow.style.display = DisplayStyle.Flex;
                _customerBar.style.display = DisplayStyle.Flex;
            }

            if (changeCnt != 0)
            {
                leftPopupNumber = new PopupNumber()
                {
                    Text = changeCnt > 0 ? $"+{changeCnt}" : $"{changeCnt}",
                    TextColor = changeCnt > 0 ? _green : _red,
                    LifeTime = 1f,
                    UpPos = Vector3.zero,
                };
            }
        }
        /// <summary>
        /// 原料生产为成品
        /// </summary>
        public void DoUpdatePruduceGoods(int totalCnt, int changeCnt)
        {
            // 产出
            _labProduceCnt.text = $"{totalCnt}";

            if (changeCnt != 0)
            {
                rightPopupNumber = new PopupNumber()
                {
                    Text = $"+{changeCnt}",
                    TextColor = _green,
                    LifeTime = 1f,
                    UpPos = Vector3.zero,
                };
            }
        }

        /// <summary>
        /// 成品从工厂输出到车站
        /// </summary>
        public void DoUpdateOutStore(int totalCnt, int changeCnt)
        {
            _labProduceCnt.text = $"{totalCnt}";

            if (changeCnt != 0)
            {
                rightPopupNumber = new PopupNumber()
                {
                    Text = $"{changeCnt}",
                    TextColor = _red,
                    LifeTime = 1f,
                    UpPos = Vector3.zero,
                };
            }
        }

        private Vector2 screenPos;
        public void SetFollowTarget(Transform trans)
        {
            followTrans = trans;
        }

        private float waitIconOpacity = 1f;
        private float waitIconOpacityChange = 1;
        private void WaitAction()
        {
            unityUIDocument.rootVisualElement.schedule.Execute(() => {
                if (_wait.style.display == DisplayStyle.Flex)
                {
                    if (waitIconOpacity > 1f)
                    {
                        waitIconOpacityChange = -1f;
                    }
                    else if (waitIconOpacity < 0f)
                    {
                        waitIconOpacityChange = 1f;
                    }
                    waitIconOpacity += (0.05f * waitIconOpacityChange);
                    _wait.style.opacity = waitIconOpacity;
                }
            }).Every(30);
        }

        // 飘字
        protected void onUpdate_Popup()
        {
            if (leftPopupNumber != null && leftPopupNumber.LifeTime > 0f)
            {
                leftPopupNumber.LifeTime -= Time.deltaTime;
                // 反向的
                leftPopupNumber.UpPos.y -= Time.deltaTime * 50;
                _labLeftPopup.transform.position = leftPopupNumber.UpPos;
                if (_labLeftPopup.text == "")
                {
                    _labLeftPopup.text = $"{leftPopupNumber.Text}";
                    _labLeftPopup.style.color = new StyleColor(leftPopupNumber.TextColor);
                }
            }
            else
            {
                _labLeftPopup.text = "";
                leftPopupNumber = null;
            }
            if (rightPopupNumber != null && rightPopupNumber.LifeTime > 0f)
            {
                rightPopupNumber.LifeTime -= Time.deltaTime;
                // 反向的
                rightPopupNumber.UpPos.y -= Time.deltaTime * 50;
                _labRightPopup.transform.position = rightPopupNumber.UpPos;
                if (_labRightPopup.text == "")
                {
                    _labRightPopup.text = $"{rightPopupNumber.Text}";
                    _labRightPopup.style.color = new StyleColor(rightPopupNumber.TextColor);
                }
            }
            else
            {
                _labRightPopup.text = "";
                rightPopupNumber = null;
            }
        }

        protected void OnUpdate()
        {
            onUpdate_Popup();

            if (followTrans != null && _factoryConf != null)
            {
                screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera).worldToScreenPos(followTrans.position));
                setPoisition(screenPos.x + _factoryConf.TopUIOffset.x, screenPos.y + _factoryConf.TopUIOffset.y);
            }

            if (_CMFactory == null || ! _CMFactory.IsActive)
            {
                return;
            }
            // 叹号闪闪
            if (_wait.style.display == DisplayStyle.Flex)
            {
                if (waitIconOpacity > 1f)
                {
                    waitIconOpacityChange = -1f;
                }
                else if (waitIconOpacity < 0f)
                {
                    waitIconOpacityChange = 1f;
                }
                waitIconOpacity += (0.05f * waitIconOpacityChange);
                _wait.style.opacity = waitIconOpacity;
            }
            if (_CMFactory.currentProductInputStore <= 0)
            {
                _barValue.style.width = new StyleLength(new Length(0f));
                return;
            }

            _barValue.style.width = new StyleLength(new Length(58 * (1f - _CMFactory.currentCD / _CMFactory.produceCD)));

            if (_lastUpdateProduceVer != _CMFactory.produceVer)
            {
                RefreshInfo(_CMFactory, _factoryConf);
            }
        }

        public override void showUI()
        {
            base.showUI();

            addUpdate(this.OnUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();
        }
    }
}
