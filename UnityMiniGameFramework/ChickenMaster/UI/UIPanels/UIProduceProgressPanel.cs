using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
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

        protected Label _labStockCnt;
        protected Image _sprStockGoods;
        protected Label _labProduceCnt;
        protected Image _sprProduceGoods;
        protected ProgressBar _progressBar;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

            RefreshInfo(null);
        }

        protected void FindUI()
        {
            _labStockCnt = this._uiObjects["labStockCnt"].unityVisualElement as Label;
            _sprStockGoods = this._uiObjects["sprStockGoods"].unityVisualElement as Image;
            _labProduceCnt = this._uiObjects["labProduceCnt"].unityVisualElement as Label;
            _sprProduceGoods = this._uiObjects["sprProduceGoods"].unityVisualElement as Image;
            _progressBar = this._uiObjects["progressBar"].unityVisualElement as ProgressBar;

        }

        public void RefreshInfo(LocalFactoryInfo lFacInfo)
        {
            if (lFacInfo == null)
            {
                _labStockCnt.text = $"{0}";
                _labProduceCnt.text = $"{0}";
                return;
            }
            // 库存
            if (lFacInfo.buildingInputProduct == null)
            {
                _labStockCnt.text = $"{0}";
            }
            else
            {
                _labStockCnt.text = $"{lFacInfo.buildingInputProduct.count}";
            }
            // 产出
            if (lFacInfo.buildingOutputProduct == null)
            {
                _labProduceCnt.text = $"{0}";
            }
            else
            {
                _labProduceCnt.text = $"{lFacInfo.buildingOutputProduct.count}";
            }
        }

        private int pgNum = 0;
        protected void OnUpdate()
        {
            pgNum = Math.Min(100, pgNum + 1);
            if (pgNum == 100) pgNum = 0;
            _progressBar.value = pgNum;
        }

        public override void showUI()
        {
            base.showUI();

            UnityGameApp.Inst.addUpdateCall(this.OnUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();

            UnityGameApp.Inst.removeUpdateCall(this.OnUpdate);
        }
    }
}
