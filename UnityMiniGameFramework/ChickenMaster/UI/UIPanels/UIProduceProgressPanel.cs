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
        protected int _lastUpdateProduceVer;

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
                DoUpdateInputStore(0, 0);
                DoUpdatePruduceGoods(0, 0);
                return;
            }
            _lastUpdateProduceVer = _CMFactory.produceVer;

            DoUpdateInputStore(_CMFactory.currentProductInputStore, 0);
            DoUpdatePruduceGoods(_CMFactory.currentProductOutputStore, 0);
        }

        /// <summary>
        /// 原料输入工厂
        /// </summary>
        public void DoUpdateInputStore(int totalCnt, int changeCnt)
        {
            // 库存
            _labStockCnt.text = $"{totalCnt}";
        }
        /// <summary>
        /// 原料生产为成品
        /// </summary>
        public void DoUpdatePruduceGoods(int totalCnt, int changeCnt)
        {
            // 产出
            _labProduceCnt.text = $"{totalCnt}";
        }
        
        /// <summary>
        /// 成品从工厂输出到车站
        /// </summary>
        public void DoUpdateOutStore(int totalCnt, int changeCnt)
        {
            _labProduceCnt.text = $"{totalCnt}";
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
        }

        public override void hideUI()
        {
            base.hideUI();

            UnityGameApp.Inst.removeUpdateCall(this.OnUpdate);
        }
    }
}
