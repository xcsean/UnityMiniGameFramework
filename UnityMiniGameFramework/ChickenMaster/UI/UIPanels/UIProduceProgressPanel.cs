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
        protected VisualElement _sprStockGoods;
        protected Label _labProduceCnt;
        protected VisualElement _sprProduceGoods;
        protected ProgressBar _progressBar;

        protected CMFactory _CMFactory;
        protected int _lastUpdateProduceVer;
        protected PopupNumber leftPopupNumber;
        protected PopupNumber rightPopupNumber;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

            RefreshInfo(null);
        }

        protected void FindUI()
        {
            _labLeftPopup = this._uiObjects["labLeftPopup"].unityVisualElement as Label;
            _labRightPopup = this._uiObjects["labRightPopup"].unityVisualElement as Label;
            _labStockCnt = this._uiObjects["labStockCnt"].unityVisualElement as Label;
            _sprStockGoods = this._uiObjects["sprStockGoods"].unityVisualElement;
            _labProduceCnt = this._uiObjects["labProduceCnt"].unityVisualElement as Label;
            _sprProduceGoods = this._uiObjects["sprProduceGoods"].unityVisualElement;
            _progressBar = this._uiObjects["progressBar"].unityVisualElement as ProgressBar;

            _labLeftPopup.text = "";
            _labRightPopup.text = "";
        }

        public void RefreshInfo(CMFactory _cmFactory)
        {
            _CMFactory = _cmFactory;
            if (_CMFactory == null)
            {
                DoUpdateInputStore(0, 0);
                DoUpdatePruduceGoods(0, 0);
                return;
            }
            _lastUpdateProduceVer = _CMFactory.produceVer;

            // 产品图
            var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadProductIcon($"icon_{_cmFactory.factoryConf.outputProductName}");
            _sprProduceGoods.style.backgroundImage = tx;
            _sprProduceGoods.style.width = tx.width / (tx.width / 40 + 1);
            _sprProduceGoods.style.height = tx.width / (tx.height / 40 + 1);

            DoUpdateInputStore(_CMFactory.currentProductInputStore, 0);
            DoUpdatePruduceGoods(_CMFactory.currentProductOutputStore, 0);
        }

        /// <summary>
        /// 原料输入工厂或生产时扣除原料
        /// </summary>
        public void DoUpdateInputStore(int totalCnt, int changeCnt)
        {
            // 库存
            _labStockCnt.text = $"{totalCnt}";


            if (changeCnt != 0)
            {
                leftPopupNumber = new PopupNumber()
                {
                    Text = changeCnt > 0 ? $"+{changeCnt}" : $"{changeCnt}",
                    TextColor = changeCnt > 0 ? Color.green : Color.red,
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
                    TextColor = Color.green,
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
                    TextColor = Color.red,
                    LifeTime = 1f,
                    UpPos = Vector3.zero,
                };
            }
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
            if (_CMFactory == null)
            {
                return;
            }
            if (_CMFactory.currentProductInputStore <= 0)
            {
                _progressBar.value = 0.0f;
                return;
            }
            _progressBar.value = (1.0f - (_CMFactory.currentCD / _CMFactory.produceCD)) * 100;

            if (_lastUpdateProduceVer != _CMFactory.produceVer)
            {
                RefreshInfo(_CMFactory);
            }
        }

        public override void showUI()
        {
            base.showUI();

            UnityGameApp.Inst.addUpdateCall(this.OnUpdate);
            UnityGameApp.Inst.addUpdateCall(this.onUpdate_Popup);
        }

        public override void hideUI()
        {
            base.hideUI();

            UnityGameApp.Inst.removeUpdateCall(this.OnUpdate);
            UnityGameApp.Inst.removeUpdateCall(this.onUpdate_Popup);
        }
    }
}
