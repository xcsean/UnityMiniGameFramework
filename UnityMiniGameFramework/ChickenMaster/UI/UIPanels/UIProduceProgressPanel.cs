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

        protected CMFactory _CMFactory;

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

        public void RefreshInfo(CMFactory _cmFactory)
        {
            _CMFactory = _cmFactory;
            if (_CMFactory == null)
            {
                _labStockCnt.text = $"{0}";
                _labProduceCnt.text = $"{0}";
                return;
            }
            // 库存
            _labStockCnt.text = $"{_CMFactory.currentProductInputStore}";
            // 产出
            _labProduceCnt.text = $"{_CMFactory.currentProductOutputStore}";
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

            //if (_lastUpdateProduceVer != _factory.produceVer)
            //{
            //    _refreshInfo();
            //}
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
