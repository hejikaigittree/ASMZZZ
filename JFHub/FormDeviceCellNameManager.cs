using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;

namespace JFHub
{
    /// <summary>
    /// Dio/Aio/触发/轴设备命名管理窗口
    /// </summary>
    public partial class FormDeviceCellNameManager : Form
    {
        public enum DevNodeCategory
        {
            MotionDaqDev,
            Module, //运动控制器中各module的父节点
            MotionModule, //运动控制器中的轴模块
            DioModule,//运动控制器中的数字量IO模块
            AioModule,//运动控制器中的模拟量IO模块
            CmpTrigModule, //运动控制器中的位置比较触发模块
            TrigCtrlDev,//触发控制器设备
            LightCtrlTDev,//带触发功能的光源控制器设备
        }
        public class DevNodeInfo
        {
            public DevNodeInfo()
            {
                DevID = null;
                Categoty = DevNodeCategory.MotionDaqDev;
                ModuleIndex = 0;
            }

            public DevNodeInfo(string devID, DevNodeCategory category,int moduleIndex)
            {
                DevID = devID;
                Categoty = category;
                ModuleIndex = moduleIndex;
            }


            public string DevID;
            public DevNodeCategory Categoty;
            public int ModuleIndex;

        }

        public FormDeviceCellNameManager()
        {
            InitializeComponent();
            ucDioNames = new UcNamesEditTest_Dio();
            ucDioNames.ContextMenuStrip = contextMenuStripEditName;
            ucAioNames = new UcNamesEditTest_Aio();
            ucAioNames.ContextMenuStrip = contextMenuStripEditName;
            ucAxisNames = new UcNamesEditTest_Motion();
            ucAxisNames.ContextMenuStrip = contextMenuStripEditName;
            ucCmpTrigNames = new UcNamesEditTest_CmpTrig();
            ucCmpTrigNames.ContextMenuStrip = contextMenuStripEditName;
            ucTrigCtrlNames = new UcNamesEditTest_TrigCtrl();
            ucTrigCtrlNames.ContextMenuStrip = contextMenuStripEditName;

            ucLightCtrlNames = new UcNamesEditTest_LightCtrl();
            ucLightCtrlNames.ContextMenuStrip = contextMenuStripEditName;
        }

        private void FormDeviceNameManager_Load(object sender, EventArgs e)
        {
            if (null != Parent)
                Parent.VisibleChanged += FormDeviceCellNameManager_VisibleChanged;
            tvDevs.Nodes.Clear();
            tvDevs.Nodes.Add("设备名称列表");
            tvDevs.Nodes[0].ContextMenuStrip = contextMenuDevMgr;
            _LoadCfg();
        }

        UcNamesEditTest_Dio ucDioNames; //用于编辑DIO名称的控件
        UcNamesEditTest_Aio ucAioNames;
        UcNamesEditTest_Motion ucAxisNames;
        UcNamesEditTest_CmpTrig ucCmpTrigNames; //位置比较触发模块

        UcNamesEditTest_TrigCtrl ucTrigCtrlNames; //触发控制器设备面板

        UcNamesEditTest_LightCtrl ucLightCtrlNames; //光源控制器设备面板（兼容带触发功能的设备）


        //bool isCfgChanged = false;//

        void _LoadCfg()
        {
            JFHubCenter.Instance.MDCellNameMgr.Load();
            _UpdateCfg2UI();
        }

        /// <summary>
        /// 向树形控件添加一个运动控制设备节点
        /// </summary>
        /// <param name="devID"></param>
        void _AddDevNode(string devID,DevNodeCategory category)
        {
            TreeNode devNode = new TreeNode(devID);
            devNode.Tag = new DevNodeInfo(devID, /*DevNodeCategory.MotionDaqDev*/category, 0);
            devNode.ContextMenuStrip = contextMenuDev;
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            tvDevs.Nodes[0].Nodes.Add(devNode);
            _UpdateModuleNode(devNode);
        }

        /// <summary>
        /// 更新控制卡的模块数量信息
        /// </summary>
        /// <param name="nodeModule"></param>
        void _UpdateModuleNode(TreeNode nodeModule)
        {
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            DevNodeInfo nodeInfo = nodeModule.Tag as DevNodeInfo;
            if (nodeInfo.Categoty == DevNodeCategory.MotionDaqDev) //更新运动控制设备节点
            {
                nodeModule.Nodes.Clear();
                int motionModuleCount = mgr.GetAxisModuleCount(nodeInfo.DevID);//轴模块
                if (motionModuleCount > 0)
                {
                    TreeNode motionsNode = new TreeNode("轴模块");
                    motionsNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.Module, 0);
                    nodeModule.Nodes.Add(motionsNode);
                    for (int i = 0; i < motionModuleCount; i++)
                    {
                        TreeNode motionNode = new TreeNode("序号_" + i + ",轴数:" + mgr.GetAxisCount(nodeInfo.DevID, i));
                        motionNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.MotionModule, i);
                        motionsNode.Nodes.Add(motionNode);
                        motionNode.ContextMenuStrip = contextMenuModule;
                    }
                }
                int dioModuleCount = mgr.GetDioModuleCount(nodeInfo.DevID);//数字IO模块
                if (dioModuleCount > 0)
                {
                    TreeNode diosNode = new TreeNode("Dio模块");
                    diosNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.Module, 0);
                    nodeModule.Nodes.Add(diosNode);
                    for (int i = 0; i < dioModuleCount; i++)
                    {
                        TreeNode dioNode = new TreeNode("序号_" + i + ",DI数:" + mgr.GetDiChannelCount(nodeInfo.DevID, i) + ",DO数:" + mgr.GetDoChannelCount(nodeInfo.DevID, i));
                        dioNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.DioModule, i);
                        diosNode.Nodes.Add(dioNode);
                        dioNode.ContextMenuStrip = contextMenuModule;
                    }
                }
                int trigModuleCount = mgr.GetCmpTrigModuleCount(nodeInfo.DevID);//位置比较触发
                if (trigModuleCount > 0)
                {
                    TreeNode trigsNode = new TreeNode("位置比较触发模块");
                    trigsNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.Module, 0);
                    nodeModule.Nodes.Add(trigsNode);
                    for (int i = 0; i < trigModuleCount; i++)
                    {
                        TreeNode trigNode = new TreeNode("序号_" + i + ",通道数:" + mgr.GetCmpTrigCount(nodeInfo.DevID, i));
                        trigNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.CmpTrigModule, i);
                        trigsNode.Nodes.Add(trigNode);
                        trigNode.ContextMenuStrip = contextMenuModule;
                    }
                }
                int aioModuleCount = mgr.GetAioModuleCount(nodeInfo.DevID);
                if (aioModuleCount > 0)
                {
                    TreeNode aiosNode = new TreeNode("Aio模块");
                    aiosNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.Module, 0);
                    nodeModule.Nodes.Add(aiosNode);
                    for (int i = 0; i < aioModuleCount; i++)
                    {
                        TreeNode aioNode = new TreeNode("序号_" + i + ",AI数:" + mgr.GetAiChannelCount(nodeInfo.DevID, i) + ",AO数:" + mgr.GetAoChannelCount(nodeInfo.DevID, i));
                        aioNode.Tag = new DevNodeInfo(nodeInfo.DevID, DevNodeCategory.AioModule, i);
                        aiosNode.Nodes.Add(aioNode);
                        aioNode.ContextMenuStrip = contextMenuModule;
                    }
                }
            }
            else if(nodeInfo.Categoty == DevNodeCategory.LightCtrlTDev) //光源控制器设备节点
            {
                ucLightCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
            }
            else if(nodeInfo.Categoty == DevNodeCategory.TrigCtrlDev) //触发控制器设备节点
            {
                ucTrigCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
            }
        }

        /// <summary>
        /// 更新模块的通道信息
        /// </summary>
        /// <param name="nodeChannel"></param>
        void _UpdateChannelNode(TreeNode nodeChannel)
        {
            DevNodeInfo nodeInfo = nodeChannel.Tag as DevNodeInfo;
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            if (nodeInfo.Categoty == DevNodeCategory.DioModule)
            {
                nodeChannel.Text = "序号_" + nodeInfo.ModuleIndex + ",DI数:" + mgr.GetDiChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex) + ",DO数:" + mgr.GetDoChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                ucDioNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
            else if (nodeInfo.Categoty == DevNodeCategory.MotionModule)
            {
                nodeChannel.Text = "序号_" + nodeInfo.ModuleIndex + ",轴数:" + mgr.GetAxisCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                ucAxisNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
            else if (nodeInfo.Categoty == DevNodeCategory.CmpTrigModule)
            {
                nodeChannel.Text = "序号_" + nodeInfo.ModuleIndex + ",通道数:" + mgr.GetCmpTrigCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                ucCmpTrigNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
            else if (nodeInfo.Categoty == DevNodeCategory.AioModule)
            {
                nodeChannel.Text = "序号_" + nodeInfo.ModuleIndex + ",AI数:" + mgr.GetAiChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex) + ",AO数:" + mgr.GetAoChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                ucAioNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
        }
    
        void _UpdateCfg2UI()
        {
            tvDevs.Nodes[0].Nodes.Clear();
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            string[] devIDs = mgr.AllMotionDaqDevices();
            if (null != devIDs)
                foreach (string devID in devIDs)
                    _AddDevNode(devID,DevNodeCategory.MotionDaqDev);

            string[] lightDevIDs = mgr.AllLightCtrlDevs();
            if (null != lightDevIDs)
                foreach (string lightDevID in lightDevIDs)
                    _AddDevNode(lightDevID, DevNodeCategory.LightCtrlTDev);


            string[] trigDevIDs = mgr.AllTrigCtrlDevs(); //触发控制器
            if(null != trigDevIDs)
                foreach (string trigDevID in trigDevIDs)
                    if(!typeof(IJFDevice_LightController).IsAssignableFrom(JFHubCenter.Instance.InitorManager.GetInitor(trigDevID).GetType())) //去除光源控制器设备
                    _AddDevNode(trigDevID, DevNodeCategory.TrigCtrlDev);

            
        }

        void _SaveCfg()
        {
            JFHubCenter.Instance.MDCellNameMgr.Save();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemLoadCfg_Click(object sender, EventArgs e)
        {
            _LoadCfg();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemSaveCfg_Click(object sender, EventArgs e)
        {
            _SaveCfg();
        }

    


        private void ToolStripMenuItemAddMotionDaq_Click(object sender, EventArgs e)
        {
            FormDevModuleInfo fm = new FormDevModuleInfo();
            fm.Text = "添加MotionDaq设备命名表";
            fm.SettingMode = DevModuleSettingMode.Add;
            if (DialogResult.OK != fm.ShowDialog())
                return;
            JFHubCenter.Instance.MDCellNameMgr.AddMotionDaqDevice(fm.DevID);
            JFHubCenter.Instance.MDCellNameMgr.SetAxisModuleCount(fm.DevID, fm.MotionCount);
            JFHubCenter.Instance.MDCellNameMgr.SetDioModuleCount(fm.DevID, fm.DioCount);
            JFHubCenter.Instance.MDCellNameMgr.SetAioModuleCount(fm.DevID, fm.AioCount);
            JFHubCenter.Instance.MDCellNameMgr.SetCmpTrigModuleCount(fm.DevID, fm.TrigCount);
            //_UpdateCfg2UI();
            _AddDevNode(fm.DevID,DevNodeCategory.MotionDaqDev);
        }

        private void FormDeviceCellNameManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(DialogResult.Yes == MessageBox.Show("窗口即将关闭，是否保存当前参数到配置文件中?","提示信息",MessageBoxButtons.YesNo,MessageBoxIcon.Warning))
                _SaveCfg();
        }

        /// <summary>
        /// 修改设备所属的模块数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemResetDevsModuleCount_Click(object sender, EventArgs e)
        {
            TreeNode currNode = tvDevs.SelectedNode;
            DevNodeInfo nodeInfo = currNode.Tag as DevNodeInfo;
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            if (nodeInfo.Categoty == DevNodeCategory.MotionDaqDev) //更改运动控制器模块数量
            {
                FormDevModuleInfo fm = new FormDevModuleInfo();
                fm.DevID = nodeInfo.DevID;
                fm.MotionCount = mgr.GetAxisModuleCount(nodeInfo.DevID);
                fm.DioCount = mgr.GetDioModuleCount(nodeInfo.DevID);
                fm.AioCount = mgr.GetAioModuleCount(nodeInfo.DevID);
                fm.TrigCount = mgr.GetCmpTrigModuleCount(nodeInfo.DevID);
                fm.SettingMode = DevModuleSettingMode.Set;
                if (DialogResult.OK == fm.ShowDialog())
                {
                    mgr.SetAxisModuleCount(nodeInfo.DevID, fm.MotionCount);
                    mgr.SetDioModuleCount(nodeInfo.DevID, fm.DioCount);
                    mgr.SetAioModuleCount(nodeInfo.DevID, fm.AioCount);
                    mgr.SetCmpTrigModuleCount(nodeInfo.DevID, fm.TrigCount);
                    _UpdateModuleNode(currNode);//_UpdateCfg2UI();
                }
            }
            else if(nodeInfo.Categoty == DevNodeCategory.TrigCtrlDev)//更改触发控制器通道数量
            {
                FormAddMChnDev fm = new FormAddMChnDev();
                fm.Text = "修改触发器通道数量";
                string[] devIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_TrigController)); //所有触发控制设备ID，过滤掉其中的光源控制器
                List<string> trigDevIDs = new List<string>();
                fm.SetDeviceID(nodeInfo.DevID);
                fm.SettingMode = DevModuleSettingMode.Set;
                fm.SetChannelTypes(new string[] { "触发通道数量:" });
                int nCurrChns = JFHubCenter.Instance.MDCellNameMgr.GetTrigCtrlChannelCount(nodeInfo.DevID);
                fm.ChannelCount = new int[] { nCurrChns };
                if (DialogResult.OK != fm.ShowDialog())
                    return;
                JFHubCenter.Instance.MDCellNameMgr.SetTrigCtrlChannelCount(fm.DeviceID, fm.ChannelCount[0]);
                _UpdateModuleNode(currNode);
            }
            else if(nodeInfo.Categoty == DevNodeCategory.LightCtrlTDev) //更改光源控制器通道数量
            {
                FormAddMChnDev fm = new FormAddMChnDev();
                fm.Text = "修改光源通道数量";
               // string[] devIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_TrigController)); //所有触发控制设备ID，过滤掉其中的光源控制器
                //List<string> trigDevIDs = new List<string>();
                fm.SetDeviceID(nodeInfo.DevID);
                fm.SettingMode = DevModuleSettingMode.Set;
                fm.SetChannelTypes(new string[] {"开关通道数量", "触发通道数量:" });
                int lightChnCount = JFHubCenter.Instance.MDCellNameMgr.GetLightCtrlChannelCount(nodeInfo.DevID);
                int trigChnCount = JFHubCenter.Instance.MDCellNameMgr.GetTrigCtrlChannelCount(nodeInfo.DevID);
                fm.ChannelCount = new int[] { lightChnCount, trigChnCount };
                
                if (DialogResult.OK != fm.ShowDialog())
                    return;
                JFHubCenter.Instance.MDCellNameMgr.SetLightCtrlChannelCount(fm.DeviceID, fm.ChannelCount[0]);
                JFHubCenter.Instance.MDCellNameMgr.SetTrigCtrlChannelCount(fm.DeviceID, fm.ChannelCount[1]);
                _UpdateModuleNode(currNode);
            }
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemDelectDev_Click(object sender, EventArgs e)
        {
            TreeNode currNode = tvDevs.SelectedNode;
            DevNodeInfo nodeInfo = currNode.Tag as DevNodeInfo;
            if(DialogResult.OK == MessageBox.Show("是否从命名表配置中删除设备:" + nodeInfo.DevID ,"警告",MessageBoxButtons.OKCancel))
            {
                JFHubCenter.Instance.MDCellNameMgr.RemoveMotionDaqDevice(nodeInfo.DevID);
                //_UpdateCfg2UI();
                currNode.Remove();
                MessageBox.Show("设备：" + nodeInfo.DevID + "  已从命名表配置中删除!");
            }
        }

        /// <summary>
        /// 打开/关闭设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemOpenCloseDev_Click(object sender, EventArgs e)
        {
            DevNodeInfo nodeInfo = tvDevs.SelectedNode.Tag as DevNodeInfo;
            IJFDevice dev = JFHubCenter.Instance.InitorManager.GetInitor(nodeInfo.DevID) as IJFDevice;
            if (dev.IsDeviceOpen)
            {
                int nRet = dev.CloseDevice();
                if (nRet != 0)
                    MessageBox.Show("关闭设备失败！错误信息 ：" + dev.GetErrorInfo(nRet));
                else
                    MessageBox.Show("设备已关闭");
            }
            else
            {
                int nRet = dev.OpenDevice();
                if (nRet != 0)
                    MessageBox.Show("打开设备失败！错误信息 ：" + dev.GetErrorInfo(nRet));
                else
                    MessageBox.Show("设备已打开！");
            }
        
        }

        /// <summary>
        /// 修改Module所属的通道数量 轴/Dio/Aio/Trig等等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemResetChannelCount_Click(object sender, EventArgs e)
        {
            TreeNode currNode = tvDevs.SelectedNode;
            DevNodeInfo nodeInfo = currNode.Tag as DevNodeInfo;
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            FormSetModuleChnCount fm = new FormSetModuleChnCount();
            if(nodeInfo.Categoty == DevNodeCategory.MotionModule )
            {
                fm.Category = FormSetModuleChnCount.ModuleCategory.Motion;
                fm.ChannelCount1 = mgr.GetAxisCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    mgr.SetAxisCount(nodeInfo.DevID, nodeInfo.ModuleIndex, fm.ChannelCount1);
                    _UpdateChannelNode(currNode);//_UpdateCfg2UI();
                }
            }
            else if(nodeInfo.Categoty == DevNodeCategory.CmpTrigModule)
            {
                fm.Category = FormSetModuleChnCount.ModuleCategory.Trig;
                fm.ChannelCount1 = mgr.GetCmpTrigCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    mgr.SetCmpTrigCount(nodeInfo.DevID, nodeInfo.ModuleIndex, fm.ChannelCount1);
                    _UpdateChannelNode(currNode);//_UpdateCfg2UI();
                }
            }
            else if(nodeInfo.Categoty == DevNodeCategory.DioModule)
            {
                fm.Category = FormSetModuleChnCount.ModuleCategory.Dio;
                fm.ChannelCount1 = mgr.GetDiChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                fm.ChannelCount2 = mgr.GetDoChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    mgr.SetDiChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex, fm.ChannelCount1);
                    mgr.SetDoChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex, fm.ChannelCount2);
                    _UpdateChannelNode(currNode);////_UpdateCfg2UI();
                }
            }
            else if(nodeInfo.Categoty == DevNodeCategory.AioModule)
            {
                fm.Category = FormSetModuleChnCount.ModuleCategory.Aio;
                fm.ChannelCount1 = mgr.GetAiChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                fm.ChannelCount2 = mgr.GetAoChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex);
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    mgr.SetAiChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex, fm.ChannelCount1);
                    mgr.SetAoChannelCount(nodeInfo.DevID, nodeInfo.ModuleIndex, fm.ChannelCount2);
                    _UpdateChannelNode(currNode);// _UpdateCfg2UI();
                }
            }
        }

        DevNodeCategory _currNodeCategory = DevNodeCategory.MotionDaqDev; //当前点击的节点类型
        private void tvDevs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//判断右键
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode nodeClicked = tvDevs.GetNodeAt(ClickPoint);
                if (null == nodeClicked)
                    return;
                tvDevs.SelectedNode = nodeClicked;
                DevNodeInfo nodeInfo = nodeClicked.Tag as DevNodeInfo;
                if (null == nodeInfo)
                    return;
                if(nodeInfo.Categoty == DevNodeCategory.MotionDaqDev)
                {
                    contextMenuDev.Items["ToolStripMenuItemResetDevsModuleCount"].Text = "修改模块数量";
                    if (JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_MotionDaq)).Contains(nodeInfo.DevID))
                    {
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Enabled = true;
                        IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(nodeInfo.DevID) as IJFDevice_MotionDaq;
                        if (dev.IsDeviceOpen)
                            contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "关闭设备";
                        else
                            contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "打开设备";
                    }
                    else
                    {
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Enabled = false;
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "无效设备";
                    }
                }
                else if(nodeInfo.Categoty == DevNodeCategory.LightCtrlTDev || nodeInfo.Categoty == DevNodeCategory.TrigCtrlDev)
                {
                    contextMenuDev.Items["ToolStripMenuItemResetDevsModuleCount"].Text = "修改通道数量";
                    if (JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_TrigController)).Contains(nodeInfo.DevID))
                    {
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Enabled = true;
                        IJFDevice_TrigController dev = JFHubCenter.Instance.InitorManager.GetInitor(nodeInfo.DevID) as IJFDevice_TrigController;
                        if (dev.IsDeviceOpen)
                            contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "关闭设备";
                        else
                            contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "打开设备";
                    }
                    else
                    {
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Enabled = false;
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "无效设备";
                    }
                }
                else if (nodeInfo.Categoty == DevNodeCategory.LightCtrlTDev )
                {
                    contextMenuDev.Items["ToolStripMenuItemResetDevsModuleCount"].Text = "修改通道数量";
                    if (JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_LightControllerWithTrig)).Contains(nodeInfo.DevID))
                    {
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Enabled = true;
                        IJFDevice_LightControllerWithTrig dev = JFHubCenter.Instance.InitorManager.GetInitor(nodeInfo.DevID) as IJFDevice_LightControllerWithTrig;
                        if (dev.IsDeviceOpen)
                            contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "关闭设备";
                        else
                            contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "打开设备";
                    }
                    else
                    {
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Enabled = false;
                        contextMenuDev.Items["ToolStripMenuItemOpenCloseDev"].Text = "无效设备";
                    }
                }

            }
            else if(e.Button == MouseButtons.Left) //左键
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode nodeClicked = tvDevs.GetNodeAt(ClickPoint);
                if (null == nodeClicked)
                    return;
                DevNodeInfo nodeInfo = nodeClicked.Tag as DevNodeInfo;
                if (null == nodeInfo)
                    return;
                panel1.Controls.Clear();
                _currNodeCategory = nodeInfo.Categoty;
                switch (nodeInfo.Categoty)
                {
                    case DevNodeCategory.MotionDaqDev:
                        timer1.Enabled = false;
                        break;
                    case DevNodeCategory.DioModule:
                        ucDioNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
                        panel1.Controls.Add(ucDioNames);
                        ucDioNames.Dock = DockStyle.Fill;
                        timer1.Enabled = true;
                        break;
                    case DevNodeCategory.MotionModule:
                        timer1.Enabled = true;
                        ucAxisNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
                        panel1.Controls.Add(ucAxisNames);
                        ucAxisNames.Dock = DockStyle.Fill;
                        timer1.Enabled = true;
                        break;
                    case DevNodeCategory.AioModule:
                        ucAioNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
                        panel1.Controls.Add(ucAioNames);
                        ucAioNames.Dock = DockStyle.Fill;
                        timer1.Enabled = true;
                        break;
                    case DevNodeCategory.CmpTrigModule:
                        ucCmpTrigNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
                        panel1.Controls.Add(ucCmpTrigNames);
                        ucCmpTrigNames.Dock = DockStyle.Fill;
                        timer1.Enabled = true;
                        break;
                    case DevNodeCategory.TrigCtrlDev:
                        ucTrigCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
                        panel1.Controls.Add(ucTrigCtrlNames);
                        ucTrigCtrlNames.Dock = DockStyle.Fill;
                        timer1.Enabled = true;
                        break;
                    case DevNodeCategory.LightCtrlTDev:
                        ucLightCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
                        panel1.Controls.Add(ucLightCtrlNames);
                        ucLightCtrlNames.Dock = DockStyle.Fill;
                        timer1.Enabled = true;
                        break;
                }
                

            }
        }

        bool isChannelNameEditting = false;
        /// <summary>
        /// 开始编辑/保存通道名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemEditSave_Click(object sender, EventArgs e)
        {
            DevNodeInfo nodeInfo = tvDevs.SelectedNode.Tag as DevNodeInfo;
            if (!isChannelNameEditting) //开始编辑
            {
                isChannelNameEditting = true;
                if (nodeInfo.Categoty == DevNodeCategory.DioModule)
                {
                    ucDioNames.BeginEdit();
                }
                else if (nodeInfo.Categoty == DevNodeCategory.MotionModule)
                {
                    ucAxisNames.BeginEdit();
                }
                else if (nodeInfo.Categoty == DevNodeCategory.CmpTrigModule)
                {
                    ucCmpTrigNames.BeginEdit();
                }
                else if (nodeInfo.Categoty == DevNodeCategory.AioModule)
                {
                    ucAioNames.BeginEdit();
                }
                else if (nodeInfo.Categoty == DevNodeCategory.TrigCtrlDev) //编辑触发控制器通道名称
                {
                    ucTrigCtrlNames.BeginEdit();
                }
                else if (nodeInfo.Categoty == DevNodeCategory.LightCtrlTDev)
                    ucLightCtrlNames.BeginEdit();
                ToolStripMenuItemEditSave.Text = "保存名称";
                ToolStripMenuItemEditCancel.Enabled = true;
                tvDevs.Enabled = false;
            }
            else //尝试保存并结束编辑状态
            {
                JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
                string errInfo = "";
                if (nodeInfo.Categoty == DevNodeCategory.DioModule)
                {
                    string[] diNames = ucDioNames.DiNames;
                    string[] doNames = ucDioNames.DoNames;
                    if(!mgr.SetDiNames(nodeInfo.DevID,nodeInfo.ModuleIndex, ucDioNames.DiNames, out errInfo))
                    {
                        MessageBox.Show("设置DI通道名称失败,错误信息:\n" + errInfo);
                        return;
                    }
                    if(!mgr.SetDoNames(nodeInfo.DevID, nodeInfo.ModuleIndex, ucDioNames.DoNames, out errInfo))
                    {
                        MessageBox.Show("设置DO通道名称失败,错误信息:\n" + errInfo);
                        return;
                    }
                    ucDioNames.EndEdit();
                    MessageBox.Show("设置DIO通道名称OK");
                    
                }
                else if (nodeInfo.Categoty == DevNodeCategory.MotionModule)
                {
                    if (!mgr.SetAxisNames(nodeInfo.DevID, nodeInfo.ModuleIndex, ucAxisNames.AxisNames, out errInfo))
                    {
                        MessageBox.Show("设置轴通道名称失败,错误信息:\n" + errInfo);
                        return;
                    }
                    ucAxisNames.EndEdit();
                    MessageBox.Show("设置轴通道名称OK");
                }
                else if (nodeInfo.Categoty == DevNodeCategory.CmpTrigModule)
                {
                    if(!mgr.SetCmpTrigNames(nodeInfo.DevID,nodeInfo.ModuleIndex, ucCmpTrigNames.TrigNames,out errInfo))
                    {
                        MessageBox.Show("设置比较触发通道名称失败,错误信息:\n" + errInfo);
                        return;
                    }
                    ucCmpTrigNames.EndEdit();
                    MessageBox.Show("设置比较触发通道名称OK");
                }
                else if (nodeInfo.Categoty == DevNodeCategory.AioModule)
                {
                    if (!mgr.SetAiNames(nodeInfo.DevID, nodeInfo.ModuleIndex, ucAioNames.AiNames, out errInfo))
                    {
                        MessageBox.Show("设置AI通道名称失败,错误信息:\n" + errInfo);
                        return;
                    }
                    if (!mgr.SetAoNames(nodeInfo.DevID, nodeInfo.ModuleIndex, ucAioNames.AoNames, out errInfo))
                    {
                        MessageBox.Show("设置AO通道名称失败,错误信息:\n" + errInfo);
                        return;
                    }
                    ucAioNames.EndEdit();
                    MessageBox.Show("设置AIO通道名称OK");
                }
                else if(nodeInfo.Categoty == DevNodeCategory.TrigCtrlDev)
                {
                    //ucTrigCtrlNames.
                    if(!mgr.SetTrigCtrlChannelNames(nodeInfo.DevID,ucTrigCtrlNames.ChannelNames(),out errInfo))
                    {
                        MessageBox.Show("设置触发控制器通道名称失败,错误信息:\n" + errInfo);
                        ucTrigCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
                        return;
                    }
                    ucTrigCtrlNames.EndEdit();
                }
                else if(nodeInfo.Categoty == DevNodeCategory.LightCtrlTDev)
                {
                    if(!mgr.SetLightCtrlChannelNames(nodeInfo.DevID,ucLightCtrlNames.LightChannelNames(),out errInfo))
                    {
                        MessageBox.Show("设置触发控制器通道名称失败,错误信息:\n" + errInfo);
                        ucLightCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
                            return;
                    }
                    string[] trigChannelNames = ucLightCtrlNames.TrigChannelNames();
                    if (null == trigChannelNames || 0 == trigChannelNames.Length)
                        mgr.RemoveLightCtrlDev(nodeInfo.DevID);
                    else
                    {
                        if (!mgr.SetTrigCtrlChannelNames(nodeInfo.DevID, ucLightCtrlNames.TrigChannelNames(), out errInfo))
                        {
                            MessageBox.Show("设置触发控制器通道名称失败,错误信息:\n" + errInfo);
                            ucLightCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
                            return;
                        }
                    }

                    ucLightCtrlNames.EndEdit();
                    //if (!mgr.SetLightCtrlChannelNames(nodeInfo.DevID, ucTrigCtrlNames.ChannelNames(), out errInfo))
                    //{
                    //    MessageBox.Show("设置触发控制器通道名称失败,错误信息:\n" + errInfo);
                    //    ucTrigCtrlNames.UpdateChannelsInfo(nodeInfo.DevID);
                    //    return;
                    //}

                }
                ToolStripMenuItemEditSave.Text = "编辑名称";
                ToolStripMenuItemEditCancel.Enabled = true;
                isChannelNameEditting = false;
                tvDevs.Enabled = true;

            }
        }

        /// <summary>
        /// 取消编辑通道名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemEditCancel_Click(object sender, EventArgs e)
        {
            DevNodeInfo nodeInfo = tvDevs.SelectedNode.Tag as DevNodeInfo;
            isChannelNameEditting = false;
            if (nodeInfo.Categoty == DevNodeCategory.DioModule)
            {
                ucDioNames.EndEdit();
                ucDioNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
            else if (nodeInfo.Categoty == DevNodeCategory.MotionModule)
            {
                ucAxisNames.EndEdit();
                ucAxisNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
            else if (nodeInfo.Categoty == DevNodeCategory.CmpTrigModule)
            {
                ucCmpTrigNames.EndEdit();
                ucCmpTrigNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
            else if (nodeInfo.Categoty == DevNodeCategory.AioModule)
            {
                ucAioNames.EndEdit();
                ucAioNames.UpdateChannelsInfo(nodeInfo.DevID, nodeInfo.ModuleIndex);
            }
            ToolStripMenuItemEditSave.Text = "编辑名称";
            ToolStripMenuItemEditCancel.Enabled = false;
            tvDevs.Enabled = true;

        }

        /// <summary>
        /// 添加一个触发控制器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAddTrigCtrl_Click(object sender, EventArgs e)
        {
            FormAddMChnDev fm = new FormAddMChnDev();
            fm.Text = "添加触发控制器设备";
            string[] devIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_TrigController)); //所有触发控制设备ID，过滤掉其中的光源控制器
            if(null == devIDs || 0 == devIDs.Length)
            {
                MessageBox.Show("设备列表中不存在触发控制器设备");
                return;
            }
            List<string> trigDevIDs = new List<string>();
            foreach (string devID in devIDs)
                if (!typeof(IJFDevice_LightController).IsAssignableFrom(JFHubCenter.Instance.InitorManager.GetInitor(devID).GetType()))
                    trigDevIDs.Add(devID);
            if(trigDevIDs.Count == 0)
            {
                MessageBox.Show("设备列表中不存在触发控制器设备...");
                return;
            }
            fm.SetOptionalDeviceIDs(trigDevIDs.ToArray());
            fm.SetChannelTypes(new string[] { "触发通道数量:" });
            if (DialogResult.OK != fm.ShowDialog())
                return;
            
            JFHubCenter.Instance.MDCellNameMgr.AddTrigCtrlDev(fm.DeviceID);
            JFHubCenter.Instance.MDCellNameMgr.SetTrigCtrlChannelCount(fm.DeviceID, fm.ChannelCount[0]);
            _AddDevNode(fm.DeviceID, DevNodeCategory.TrigCtrlDev);
        }

        /// <summary>
        /// 添加一个带触发功能的光源控制器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAddLightTrig_Click(object sender, EventArgs e)
        {
            FormAddMChnDev fm = new FormAddMChnDev();
            fm.Text = "添加光源控制器设备_T";
            string[] devIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_LightControllerWithTrig)); //所有触发控制设备ID，过滤掉其中的光源控制器
            if (null == devIDs || 0 == devIDs.Length)
            {
                MessageBox.Show("设备列表中不存在光源控制器设备_T");
                return;
            }
            fm.SetOptionalDeviceIDs(devIDs);
            fm.SetChannelTypes(new string[] { "开关通道数量:","触发通道数量:" });
            if (DialogResult.OK != fm.ShowDialog())
                return;

            JFHubCenter.Instance.MDCellNameMgr.AddLightCtrlDev(fm.DeviceID);
            JFHubCenter.Instance.MDCellNameMgr.SetLightCtrlChannelCount(fm.DeviceID, fm.ChannelCount[0]);

            JFHubCenter.Instance.MDCellNameMgr.AddTrigCtrlDev(fm.DeviceID);
            JFHubCenter.Instance.MDCellNameMgr.SetTrigCtrlChannelCount(fm.DeviceID, fm.ChannelCount[1]);
            _AddDevNode(fm.DeviceID, DevNodeCategory.LightCtrlTDev);
        }

        private void FormDeviceCellNameManager_VisibleChanged(object sender, EventArgs e) //已测试能到这一步
        {
            if (Visible)
            {
                if (_currNodeCategory == DevNodeCategory.MotionDaqDev || _currNodeCategory == DevNodeCategory.Module)
                    timer1.Enabled = false;
                else
                    timer1.Enabled = true;
            }
            else
                timer1.Enabled = false;
        }

        /// <summary>
        /// 刷新界面（IO状态等）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch(_currNodeCategory)
            {
                case DevNodeCategory.AioModule:
                    ucAioNames.UpdateIOStatus2UI();
                    break;
                case DevNodeCategory.CmpTrigModule:
                    timer1.Enabled = false;
                    break;// ucCmpTrigNames.UpdateChannelsInfo
                case DevNodeCategory.DioModule:
                    ucDioNames.UpdateIO2UI();
                    break;
                case DevNodeCategory.LightCtrlTDev:
                    timer1.Enabled = false;
                    //ucLightCtrlNames.UpdateChannelsInfo()
                    break;
                case DevNodeCategory.MotionModule:
                    ucAxisNames.UpdateAxis2UI();
                    break;
                case DevNodeCategory.TrigCtrlDev:
                    timer1.Enabled = false;//ucTrigCtrlNames.UpdateChannelsInfo
                    break;
                default:
                    timer1.Enabled = false;
                    break;
            }
        }
    }
}
