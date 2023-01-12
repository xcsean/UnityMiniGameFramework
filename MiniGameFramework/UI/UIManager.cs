using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class UIManager
    {
        protected Dictionary<string, Func<IUIObject>> _uiObjectCreators;
        protected Dictionary<string, Func<IUIPanel>> _uiPanelCreators;
        protected Dictionary<string, IUIPanel> _uiPanels;
        protected Dictionary<string, UIPanelConf> _uiPanelConfs;

        protected UIConfig _uiConf;

        protected IUIPreloaderPanel _preloaderPanel;
        public IUIPreloaderPanel preloaderPanel => _preloaderPanel;

        public UIManager()
        {
            _uiObjectCreators = new Dictionary<string, Func<IUIObject>>();
            _uiPanelCreators = new Dictionary<string, Func<IUIPanel>>();
            _uiPanels = new Dictionary<string, IUIPanel>();
            _uiPanelConfs = new Dictionary<string, UIPanelConf>();
        }

        public void Init(string uiConfName)
        {
            _uiConf = new UIConfig();
            _uiConf.Init(uiConfName, "uiconf");
        }

        public void regUIObjectCreator(string objectType, Func<IUIObject> creator)
        {
            if (_uiObjectCreators.ContainsKey(objectType))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"regUIObjectCreator ({objectType}) already exist");
                return;
            }
            _uiObjectCreators[objectType] = creator;
        }

        public IUIObject createUIObject(string objectType)
        {
            if (_uiObjectCreators.ContainsKey(objectType))
            {
                return _uiObjectCreators[objectType]();
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"createUIObject ({objectType}) not exist");

            return null;
        }

        public void regUIPanelCreator(string panelType, Func<IUIPanel> creator)
        {
            if (_uiPanelCreators.ContainsKey(panelType))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"regUIPanelCreator ({panelType}) already exist");
                return;
            }
            _uiPanelCreators[panelType] = creator;
        }

        protected IUIPanel _createUIPanelByType(string panelType)
        {
            if (_uiPanelCreators.ContainsKey(panelType))
            {
                return _uiPanelCreators[panelType]();
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"_createUIPanelByType ({panelType}) not exist");

            return null;
        }

        public IUIPanel getUIPanel(string panelName)
        {
            if (panelName == "preloader")
            {
                return _preloaderPanel;
            }
            if (_uiPanels.ContainsKey(panelName))
            {
                return _uiPanels[panelName];
            }

            return null;
        }

        public IUIPanel createUIPanelByConf(UIPanelConf conf)
        {
            IUIPanel panel = _createUIPanelByType(conf.type);
            if (panel != null)
            {
                panel.Init(conf);

                _uiPanelConfs[conf.name] = conf;

                panel.onShowStartHandle = () =>
                {
                    if (conf.needMask)
                    {
                        showMask(conf);
                    }
                };
                panel.onHideEndHandle = () =>
                {
                    closeMask(conf);
                };

                if (conf.name == "preloader")
                {
                    _preloaderPanel = (IUIPreloaderPanel)panel;
                }
                else
                {
                    _uiPanels[conf.name] = panel;
                }
            }

            return panel;
        }

        /// <summary>
        /// 每次都新建一个界面
        /// </summary>
        public IUIPanel createNewUIPanel(string panelName)
        {
            if (!_uiConf.uiConf.UIPanels.ContainsKey(panelName))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"createUIPanel ({panelName}) config not exist");
                return null;
            }
            string panelFile = _uiConf.uiConf.UIPanels[panelName];

            UIPanelConfig panelConf = new UIPanelConfig();
            panelConf.Init(panelFile, panelName);

            return createUIPanelByConf(panelConf.uiPanelConf);
        }

        /// <summary>
        /// 显示一个界面 唯一的
        /// </summary>
        public IUIPanel createUIPanel(string panelName)
        {
            if (getUIPanel(panelName) != null)
            {
                // 显示之前创建界面
                return getUIPanel(panelName);
            }

            return createNewUIPanel(panelName);
        }

        public void DisposeUIPanel(IUIPanel p)
        {
            if (_uiPanels.ContainsKey(p.name))
            {
                _uiPanels.Remove(p.name);
            }
            p.Dispose();
        }

        public List<string> gatherUIPanelDependFiles(string panelName)
        {
            if (!_uiConf.uiConf.UIPanels.ContainsKey(panelName))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"gatherUIPanelDependFiles ({panelName}) config not exist");
                return null;
            }

            List<string> listFiles = new List<string>();
            listFiles.Add(_uiConf.uiConf.UIPanels[panelName]); // panel config file

            // TO DO : add other files, textures, etc.

            return listFiles;
        }

        protected List<string> maskList = new List<string>();
        public IUIPanel _maskPanel = null;
        public IUIPanel maskPanel => _maskPanel;
        public void showMask(UIPanelConf conf)
        {
            var ve = getMaskUI();
            ve.SetSortOrder(conf.sortOrder - 1);

            if (maskList.Contains(conf.name))
            {
                maskList.Remove(conf.name);
            }
            else
            {
                //
            }
            maskList.Add(conf.name);
        }

        public void closeMask(UIPanelConf conf)
        {
            if (!conf.needMask)
            {
                return;
            }
            if (maskList.Count == 0)
            {
                return;
            }
            if (maskList.Contains(conf.name))
            {
                maskList.Remove(conf.name);
            }
            if (maskList.Count != 0)
            {
                string lastName = maskList[maskList.Count - 1];
                showMask(_uiPanelConfs[lastName]);
            }
            else
            {
                if (_maskPanel != null)
                {
                    _maskPanel.hideUI();
                }
            }
        }

        public IUIPanel getMaskUI()
        {
            if (_maskPanel == null)
            {
                _maskPanel = createUIPanel("MaskUI");
            }
            _maskPanel.showUI();

            return _maskPanel;
        }

    }
}
