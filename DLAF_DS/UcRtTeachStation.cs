using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using JFHub;
using DLAF;
using JFUI;

namespace DLAF_DS
{
    public partial class UcRtTeachStation : JFRealtimeUI, IJFStationMsgReceiver
    {
        static string CategoteProduct = "Product";
        public UcRtTeachStation()
        {
            InitializeComponent();
        }

        UcSingleVisionAssist _va = new UcSingleVisionAssist();
        FormInspectResult fmShowInspectResult = new FormInspectResult();
        private void UcRtTeachStation_Load(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Add(_va);
            _va.Show();
            fmShowInspectResult.HideWhenClose = true;



            AdjustStationView();
        }

        void AdjustStationView()
        {
            IJFRecipeManager mgr = JFHubCenter.Instance.RecipeManager;
            if(null == mgr)
            {
                Enabled = false;
                cbRecipeID.Text = "RecipeManager = null!";
                return;
            }

            if(!mgr.IsInitOK)
            {
                Enabled = false;
                cbRecipeID.Text = "RecipeManager not init!";
                return;
            }

            if(_station == null)
            {
                Enabled = false;
                return;
            }

            string _modelPicPath = _station.GetCfgParamValue(DLAFVisionTeachStation.SCItemName_ModelPicPath) as string;
            if (null == _modelPicPath)
                tbModelPicPath.Text = "";
            tbModelPicPath.Text = _modelPicPath;

            Enabled = true;
            string currRecipeID = cbRecipeID.Text; //当前已选的RecipeID
            cbRecipeID.Items.Clear();
            string[] allRecipeIDs = mgr.AllRecipeIDsInCategoty(CategoteProduct);
            if (null != allRecipeIDs && allRecipeIDs.Length > 0)
            {
                foreach (string s in allRecipeIDs)
                    cbRecipeID.Items.Add(s);
                if (!string.IsNullOrEmpty(currRecipeID))
                {
                    if (allRecipeIDs.Contains(currRecipeID))
                        cbRecipeID.Text = currRecipeID;
                    else
                        cbRecipeID.SelectedIndex = -1;
                }

            }
            else
                cbRecipeID.SelectedIndex = -1;

            string currVAName = cbVAName.Text;
            cbVAName.Items.Clear();
            JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
            string[] allVANames = vm.AllSVAssistNames();
            if (null != allVANames && allVANames.Length > 0)
            {
                foreach (string s in allVANames)
                    cbVAName.Items.Add(s);
                if (!allVANames.Contains(currVAName))
                    cbVAName.SelectedIndex = -1;
                else
                    cbVAName.Text = currVAName;
            }
            else
                cbVAName.SelectedIndex = -1;

            AdjustVisionCfg();
        }

        /// <summary>
        /// 更新各种视觉配置下拉框
        /// </summary>
        void AdjustVisionCfg()
        {
            string vaName = cbVAName.Text;
            if (string.IsNullOrEmpty(vaName))
                return;
            string[] allVcName = JFHubCenter.Instance.VisionMgr.SingleVisionCfgNameByOwner(vaName);
            if (null == allVcName || 0 == allVcName.Length)
                return;
            string markVcName1 = cbMarkVc1.Text;
            cbMarkVc1.Items.Clear();
            foreach (string s in allVcName)
                cbMarkVc1.Items.Add(s);
            if(!string.IsNullOrEmpty(markVcName1))
            {
                cbMarkVc1.Text = markVcName1;
                if (!allVcName.Contains(markVcName1))
                    cbMarkVc1.BackColor = Color.OrangeRed;
                else
                    cbMarkVc1.BackColor = SystemColors.Control;
            }


            string markVcName2 = cbMarkVc2.Text;
            cbMarkVc2.Items.Clear();
            foreach (string s in allVcName)
                cbMarkVc2.Items.Add(s);
            if (!string.IsNullOrEmpty(markVcName2))
            {
                cbMarkVc2.Text = markVcName2;
                if (!allVcName.Contains(markVcName2))
                    cbMarkVc2.BackColor = Color.OrangeRed;
                else
                    cbMarkVc2.BackColor = SystemColors.Control;
            }


            string TaskVcName = cbTaskVc.Text;
            cbTaskVc.Items.Clear();
            foreach (string s in allVcName)
                cbTaskVc.Items.Add(s);
            if (!string.IsNullOrEmpty(TaskVcName))
            {
                cbTaskVc.Text = TaskVcName;
                if (!allVcName.Contains(TaskVcName))
                    cbTaskVc.BackColor = Color.OrangeRed;
                else
                    cbTaskVc.BackColor = SystemColors.Control;
            }




        }

        DLAFVisionTeachStation _station = null;
        public void SetStation(DLAFVisionTeachStation station)
        {
            _station = station;
            if (Created)
                AdjustStationView();
        }


        public override void UpdateSrc2UI()
        {


            base.UpdateSrc2UI();
        }

        /// <summary>
        /// 产品ID改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRecipeID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string errorInfo;
            if(cbRecipeID.SelectedIndex  < 0)
            {
                _station.SetRecipeID(null,out errorInfo);
                gbFixProduct.Enabled = false;
                gbFixIC.Enabled = false;
                gbFixFov.Enabled = false;
                gbTaskSave.Enabled = false;
                gbVisionGrab.Enabled = false;
                btChkCfg.Enabled = false;
                btFlushVc.Enabled = false;

                return;
            }

            if(!_station.SetRecipeID(cbRecipeID.Text,out errorInfo))
            {
                MessageBox.Show(errorInfo);
                cbRecipeID.Text = _station.CurrRecipeID();
                return;
            }
            string currRecipeID = cbRecipeID.Text;
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            string[] allRecipeIDs = rm.AllRecipeIDsInCategoty(CategoteProduct);
            if (string.IsNullOrEmpty(currRecipeID) ||
           allRecipeIDs == null ||
           !allRecipeIDs.Contains(currRecipeID))
            {
                gbFixProduct.Enabled = false;
                gbFixFov.Enabled = false;
                gbFixIC.Enabled = false;
                gbTaskSave.Enabled = false;
                gbVisionGrab.Enabled = false;
                btChkCfg.Enabled = false;
                btFlushVc.Enabled = false;
                return;
            }

            gbFixProduct.Enabled = true;
            gbFixFov.Enabled = true;
            gbFixIC.Enabled = true;
            gbTaskSave.Enabled = true;
            gbVisionGrab.Enabled = true;
            btChkCfg.Enabled = true;
            btFlushVc.Enabled = true;

            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, currRecipeID) as JFDLAFProductRecipe;
            _isMark1VcEdit = false; //Mark1视觉参数是否正在配置
            _isMark2VcEdit = false;
            //btFixMark1.Text = "Vc"; //Vision Config
            //btFixMark2.Text = "Vc";
            btMark1EditSave.Text = "Vc";
            btMark2EditSave.Text = "Vc";
           // cbMarkVc1.Text = recipe.Mark1VisionCfgName;
            cbMarkVc1.Enabled = false;
            cbMarkVc2.Enabled = false;
            //cbMarkVc2.Text = recipe.Mark2VisionCfgName;
            double x1, y1, x2, y2;
            recipe.GetMarkSnapPos1(out x1, out y1);
            recipe.GetMarkSnapPos2(out x2, out y2);
            lbMarPos.Text = string.Format("x1:{0:F3} y1:{1:F3} \nx2:{2:F3} y2:{3:F3}", x1, y1, x2, y2);
            string mark1VcName = recipe.GetMark1LightCfg();
            string mark2VcName = recipe.GetMark2LightCfg();
            cbMarkVc1.Text = mark1VcName;
            cbMarkVc2.Text = mark2VcName;

            string currRowSel = cbIcRow.Text;
            string currColSel = cbIcCol.Text;

            cbIcRow.Items.Clear();
            for (int i = 0; i < recipe.RowCount; i++)
                cbIcRow.Items.Add(i.ToString());
            if (!string.IsNullOrEmpty(currRowSel))
            {
                if (Convert.ToInt32(currRowSel) < recipe.RowCount)
                    cbIcRow.Text = currRowSel;
                else
                    cbIcRow.Text = "";
            }

            cbIcCol.Items.Clear();
            for (int i = 0; i < recipe.ColCount; i++)
                cbIcCol.Items.Add(i.ToString());
            if (!string.IsNullOrEmpty(currColSel))
            {
                if (Convert.ToInt32(currColSel) < recipe.ColCount)
                    cbIcCol.Text = currColSel;
                else
                    cbIcCol.Text = "";
            }


            string currFovSel = cbFovName.Text;
            string[] allFovNames = recipe.FovNames();
            cbFovName.Items.Clear();
            if(null != allFovNames && allFovNames.Length > 0)
            {
                foreach (string s in allFovNames)
                    cbFovName.Items.Add(s);
                
            }

            if (!string.IsNullOrEmpty(currFovSel))
            {
                cbFovName.Text = currFovSel;
                if (allFovNames != null && allFovNames.Contains(currFovSel))
                    cbFovName.BackColor = SystemColors.Control;
                else
                    cbFovName.BackColor = Color.OrangeRed;
            }

            string currTaskNameSel = cbTaskName.Text;
            string[] allTaskNames = null;
            cbTaskName.Items.Clear();
            if (!string.IsNullOrEmpty(currFovSel))
            {
                allTaskNames = recipe.TaskNames(currFovSel);
                if (null != allTaskNames && allFovNames.Length > 0)
                    foreach (string s in allTaskNames)
                        cbTaskName.Items.Add(s);
            }

            if (!string.IsNullOrEmpty(currTaskNameSel))
            {
                cbTaskName.Text = currTaskNameSel;
                
                if (allTaskNames != null && allTaskNames.Contains(currTaskNameSel))
                    cbTaskName.BackColor = SystemColors.Control;
                else
                    cbTaskName.BackColor = Color.OrangeRed;
            }







        }

        /// <summary>
        /// 示教助手名称改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVAName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbVAName.SelectedIndex < 0)
            {
                _va.SetAssist(null);
                return;
            }
            else
            {
                JFSingleVisionAssist va = JFHubCenter.Instance.VisionMgr.GetSVAssistByName(cbVAName.Text);
                //if(null == va)
                //{
                //    MessageBox.Show("系统视觉管理器中不存在示教助手:" + cbVAName.Text);
                //    return;
                //}
                _va.SetAssist(va);
                AdjustVisionCfg();

            }
        }

        /// <summary>
        /// 开始产品定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFixMark_Click(object sender, EventArgs e)
        {
            if(null == _station)
            {
                MessageBox.Show(" 无效操作，工站未设置/空值");
                return;
            }
            if(string.IsNullOrEmpty(cbRecipeID.Text))
            {
                MessageBox.Show("请先选择Recipe！");
                return;
            }

            string errorInfo;
            if(!_station.FixProduct(chkVisionFixProduct.Checked, out errorInfo))
            {
                MessageBox.Show(errorInfo);
                return;
            }

            JFTipsDelayClose.Show("开始定位产品！", 1);
        }

        /// <summary>
        /// 开始定位IC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFixIC_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show(" 无效操作，工站未设置/空值");
                return;
            }
            if (string.IsNullOrEmpty(cbRecipeID.Text))
            {
                MessageBox.Show("请先选择Recipe！");
                return;
            }

            if(cbIcRow.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择IC Row！");
                return;
            }

            if(cbIcCol.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择IC Col！");
                return;
            }

            string errorInfo;
            if(!_station.FixIC(cbIcRow.SelectedIndex,cbIcCol.SelectedIndex,out errorInfo))
            {
                MessageBox.Show(errorInfo);
                return;
            }

            JFTipsDelayClose.Show("开始定位IC！", 1);

        }

        /// <summary>
        /// 开始定位FOV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFixFov_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show(" 无效操作，工站未设置/空值");
                return;
            }
            if (string.IsNullOrEmpty(cbRecipeID.Text))
            {
                MessageBox.Show("请先选择Recipe！");
                return;
            }

            if(cbFovName.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择 Fov Name！");
                return;
            }

            string errorInfo;
            if (!_station.FixFOV(cbFovName.Text,out errorInfo))
            {
                MessageBox.Show(errorInfo);
                return;
            }

            JFTipsDelayClose.Show("开始定位FOV！", 1);
        }




        private void UcRtTeachStation_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                AdjustStationView();//主要用于更新Recipe列表
        }

        private void btFixMark1_Click(object sender, EventArgs e)
        {
            if(null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            string errorInfo;
            if(!_station.FixMark(0,out errorInfo))
            {
                MessageBox.Show("Mark1定位失败:" + errorInfo);
                return;
            }
            


            JFTipsDelayClose.Show("开始定位Mark1",1);
        }

        private void btFixMark2_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            string errorInfo;
            if (!_station.FixMark(1, out errorInfo))
            {
                MessageBox.Show("Mark2定位失败:" + errorInfo);
                return;
            }
            JFTipsDelayClose.Show("开始定位Mark2", 1);
        }

        bool _isMark1VcEdit = false; //Mark1视觉参数是否正在配置
        bool _isMark2VcEdit = false;
        private void btMark1EditSave_Click(object sender, EventArgs e)
        {
            if(!_isMark1VcEdit)
            {
                _isMark1VcEdit = true;
                cbMarkVc1.Enabled = true;
                btMark1EditSave.Text = "Sv";
            }
            else
            {
                string vcName = cbMarkVc1.Text;
                if(string.IsNullOrEmpty(vcName))
                {
                    MessageBox.Show("请选择视觉配置名称");
                    return;
                }
                JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
                JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
                recipe.SetMark1LightCfg(vcName);
                _isMark1VcEdit = false;
                cbMarkVc1.Enabled = false;
                btMark1EditSave.Text = "Vc";
            }
        }


        private void btMark2EditSave_Click(object sender, EventArgs e)
        {
            if (!_isMark2VcEdit)
            {
                _isMark2VcEdit = true;
                cbMarkVc2.Enabled = true;
                btMark2EditSave.Text = "Sv";
            }
            else
            {
                string vcName = cbMarkVc2.Text;
                if (string.IsNullOrEmpty(vcName))
                {
                    MessageBox.Show("请选择视觉配置名称");
                    return;
                }
                JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
                JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
                recipe.SetMark2LightCfg(vcName);
                _isMark2VcEdit = false;
                cbMarkVc2.Enabled = false;
                btMark2EditSave.Text = "Vc";
            }
        }

        /// <summary>
        /// 更新所有视觉配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFlushVc_Click(object sender, EventArgs e)
        {
            AdjustVisionCfg();
        }

        /// <summary>
        /// 检查参数列表(是否有未设置选项)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btChkCfg_Click(object sender, EventArgs e)
        {
            string recipeID = cbRecipeID.Text;
            if(string.IsNullOrEmpty(recipeID))
            {
                MessageBox.Show("RecipeID未选择");
                return;
            }

            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            if(null == recipe)
            {
                MessageBox.Show("RecipeID = \"" + recipeID + "\"产品配方不存在");
                return;
            }

            JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
            

            bool isCfgOK = true;
            StringBuilder errorInfo = new StringBuilder();
            string mark1Cfg = recipe.GetMark1LightCfg();
            if(string.IsNullOrEmpty(mark1Cfg))
            {
                isCfgOK = false;
                errorInfo.AppendLine("Mark1视觉配置未设置");
            }
            else
            {
                if(!vm.ContainSingleVisionCfgByName(mark1Cfg))
                {
                    isCfgOK = false;
                    errorInfo.AppendLine("Mark1视觉参数:\"" + mark1Cfg + "\" 在视觉配置表中不存在");
                }
            }


            string mark2Cfg = recipe.GetMark2LightCfg();
            if (string.IsNullOrEmpty(mark2Cfg))
            {
                isCfgOK = false;
                errorInfo.AppendLine("Mark2视觉配置未设置");
            }
            else
            {
                if (!vm.ContainSingleVisionCfgByName(mark2Cfg))
                {
                    isCfgOK = false;
                    errorInfo.AppendLine("Mark2视觉参数:\"" + mark2Cfg + "\" 在视觉配置表中不存在");
                }
            }

            string[] allFovNames = recipe.FovNames();
            if(null != allFovNames)
                foreach(string fovName in allFovNames)
                {
                    string[] taskNames = recipe.TaskNames(fovName);
                    if(null != taskNames)
                        foreach(string taskName in taskNames)
                        {
                            string vcName = recipe.VisionCfgName(fovName, taskName);
                            if(string.IsNullOrEmpty(vcName))
                            {
                                isCfgOK = false;
                                errorInfo.AppendLine("Fov:\"" + fovName + "\" Task:\"" + taskName + "\" 视觉参数未设置");
                            }
                            else
                            {
                                if(!vm.ContainSingleVisionCfgByName(vcName))
                                {
                                    isCfgOK = false;
                                    errorInfo.AppendLine("Fov:\"" + fovName + "\" Task:\"" + taskName + "\" 视觉参数:\"" + vcName + "\"在视觉配置中不存在");

                                }
                            }
                        }
                }
            if(isCfgOK)
            {
                MessageBox.Show("所有视觉参数已配置完成！");
                return;
            }
            else
            {
                MessageBox.Show(errorInfo.ToString());
                return;
            }
        }

        /// <summary>
        /// 调整视觉配置为当前Task所选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFixTask_Click(object sender, EventArgs e)
        {
            string recipeID = cbRecipeID.Text;
            string fovName = cbFovName.Text;
            string taskName = cbTaskName.Text;
            if(string.IsNullOrEmpty(recipeID))
            {
                MessageBox.Show("请选择RecipeID");
                return;
            }
            if(string.IsNullOrEmpty(fovName))
            {
                MessageBox.Show("请选择Fov Name");
                return;
            }



            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            if(null == recipe)
            {
                MessageBox.Show("RecipeID:" + recipeID + " 产品配方不存在");
                return;
            }

            string taskVc = recipe.VisionCfgName(fovName, taskName);
            if(string.IsNullOrEmpty(taskVc))
            {
                MessageBox.Show("请先设置视觉配置");
                return;
            }

            string errorInfo = "";
            if(!_station.FixTaskVisionCfg(taskName,out errorInfo))
            {
                MessageBox.Show("操作失败:" + errorInfo);
                return;
            }


        }

        /// <summary>
        /// 设置/保存 Task视觉参数配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btTaskVcEditSave_Click(object sender, EventArgs e)
        {
            if(!_isTaskVcEditting)
            {
                string taskName = cbTaskName.Text;
                if(string.IsNullOrEmpty(taskName))
                {
                    MessageBox.Show("请选择需要设置的Task");
                    return;
                }

                _isTaskVcEditting = true;
                cbTaskName.Enabled = false;
                cbTaskVc.Enabled = true;
                btTaskVcEditSave.Text = "保存";
                btTaskVcEditCancel.Enabled = true;
            }
            else
            {
                string taskVcName = cbTaskVc.Text;
                if(string.IsNullOrEmpty(taskVcName))
                {
                    MessageBox.Show("请选择需要保存的视觉配置名称");
                    return;
                }
                _isTaskVcEditting = false;
                cbTaskName.Enabled = true;
                cbTaskVc.Enabled = false;
                btTaskVcEditSave.Text = "编辑";
                btTaskVcEditCancel.Enabled = false;
                JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
                JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
                recipe.SetVisionCfgName(cbFovName.Text, cbTaskName.Text, taskVcName);
                JFTipsDelayClose.Show("已保存\"taskVcName\" 到:" + cbTaskName.Text,1);
            }
        }

        /// <summary>
        /// 取消 Task参数编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btTaskVcEditCancel_Click(object sender, EventArgs e)
        {
            _isTaskVcEditting = false;
            cbTaskName.Enabled = true;
            cbTaskVc.Enabled = false;
            btTaskVcEditSave.Text = "编辑";
            btTaskVcEditCancel.Enabled = false;
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;

            string taskVcName = recipe.VisionCfgName(cbFovName.Text, cbTaskName.Text);
            cbTaskVc.Text = taskVcName;
        }

        /// <summary>
        /// Fov 选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFovName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbFovName.SelectedIndex < 0)
            {
                cbTaskName.Text = "";
                btFixFov.Enabled = false;
                return;
            }
            string currFovName = cbFovName.Text;
            string currTaskName = cbTaskName.Text;
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            string[] allFovNames = recipe.FovNames();
            if(!allFovNames.Contains(currFovName))
            {
                MessageBox.Show("当前Recipe未包含FovName:" + currFovName);
                cbTaskName.Text = "";
                btFixFov.Enabled = false;
                return;
            }
            
            btFixFov.Enabled = true;

            double x, y;
            recipe.GetFovOffset(currFovName, out x, out y);
            lbFovOffset.Text = string.Format("x:{0:F3} y:{1:F3}", x, y);


            string[] allTaskNames = recipe.TaskNames(currFovName);
            cbTaskName.Items.Clear();
            if (null != allTaskNames)
                foreach (string s in allTaskNames)
                    cbTaskName.Items.Add(s);
            if (!string.IsNullOrEmpty(currTaskName))
            {
                if(_isTaskVcEditting)
                {
                    _isTaskVcEditting = false;
                    cbTaskVc.Enabled = false;
                    btTaskVcEditCancel.Enabled = false;
                    btTaskVcEditSave.Text = "设置";
                }
                cbTaskName.Text = currTaskName;

                if (allTaskNames != null && allTaskNames.Contains(currTaskName))
                    cbTaskName.BackColor = SystemColors.Control;
                else
                    cbTaskName.BackColor = Color.OrangeRed;


            }

        }

        bool _isTaskVcEditting = false;

        private void cbTaskName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbTaskName.SelectedIndex < 0)
            {
                return;
            }

            string currFovName = cbFovName.Text;
            string currTaskName = cbTaskName.Text;
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            string taskVcName = recipe.VisionCfgName(currFovName, currTaskName);
            
            if(string.IsNullOrEmpty(taskVcName))
            {
                cbTaskVc.SelectedIndex = -1;
                return;
            }
       
            cbTaskVc.Text = taskVcName;

            string[] allVcNames = JFHubCenter.Instance.VisionMgr.SingleVisionCfgNameByOwner(cbRecipeID.Text);
            if (allVcNames != null && allVcNames.Contains(taskVcName))
                cbTaskVc.BackColor = SystemColors.Control;
            else
                cbTaskVc.BackColor = Color.OrangeRed;




        }


        #region IJFStationMsgReceiver's API
        public void OnWorkStatusChanged(JFWorkStatus currWorkStatus) //工作状态变化
        {

        }

        public void OnCustomStatusChanged(int currCustomStatus) //业务逻辑变化
        {

        }

        public void OnTxtMsg(string txt) //接受一条文本消息
        {

        }

        public void OnProductFinished(int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo) //产品加工完成
        {

        }

        public void OnCustomizeMsg(string msgCategory, object[] msgParams) //其他自定义消息
        {
            if (string.IsNullOrEmpty(msgCategory))
                return;
            if(msgCategory == "ShowJFImage")
            {
                _va.ShowImage(msgParams[0] as IJFImage);
            }
            else if(msgCategory == "SnapShow")
            {
                _va.SnapOneAndShow();
            }
            else if(msgCategory == "ShowInspectResult")
            {
                fmShowInspectResult.Show();
                fmShowInspectResult.ShowInspectResult(msgParams);
            }
        }
        #endregion

        private void cbMarkVc1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbMarkVc1.SelectedIndex < 0)
            {
                return;
            }
            string vcName = cbMarkVc1.Text;
            if (string.IsNullOrEmpty(vcName))
            {
                return;
            }
            //JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            //JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            _va.SetVcName(vcName);
        }

        private void cbMarkVc2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMarkVc2.SelectedIndex < 0)
            {
                return;
            }
            string vcName = cbMarkVc2.Text;
            if (string.IsNullOrEmpty(vcName))
            {
                return;
            }
            //JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            //JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            _va.SetVcName(vcName);
        }

        private void cbTaskVc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTaskVc.SelectedIndex < 0)
            {
                return;
            }
            string vcName = cbTaskVc.Text;
            if (string.IsNullOrEmpty(vcName))
            {
                return;
            }
            //JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            //JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            _va.SetVcName(vcName);
        }

        private void cbIcRow_SelectedIndexChanged(object sender, EventArgs e)
        {
            string recipeID = cbRecipeID.Text;
            if (string.IsNullOrEmpty(recipeID))
                return;
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            if (null == recipe)
                return;
            if (cbIcRow.SelectedIndex < 0)
                return;
            if (cbIcCol.SelectedIndex < 0)
                return;


            double x, y;
            recipe.GetICSnapCenter(Convert.ToInt32(cbIcRow.SelectedItem), Convert.ToInt32(cbIcCol.SelectedItem), out x, out y);
            lbICPos.Text = string.Format("x:{0:F3} y:{1:F3}", x, y);




        }

        private void cbIcCol_SelectedIndexChanged(object sender, EventArgs e)
        {
            string recipeID = cbRecipeID.Text;
            if (string.IsNullOrEmpty(recipeID))
                return;
            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            JFDLAFProductRecipe recipe = rm.GetRecipe(CategoteProduct, cbRecipeID.Text) as JFDLAFProductRecipe;
            if (null == recipe)
                return;
            if (cbIcRow.SelectedIndex < 0)
                return;
            if (cbIcCol.SelectedIndex < 0)
                return;


            double x, y;
            recipe.GetICSnapCenter(Convert.ToInt32(cbIcRow.SelectedItem), Convert.ToInt32(cbIcCol.SelectedItem), out x, out y);
            lbICPos.Text = string.Format("x:{0:F3} y:{1:F3}", x, y);

        }


        /// <summary>
        /// 采集产品模板图像 ，每个FOV的Task都采图/保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btGrabModelPics_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show(" 无效操作，工站未设置/空值");
                return;
            }
            if (string.IsNullOrEmpty(cbRecipeID.Text))
            {
                MessageBox.Show("请先选择Recipe！");
                return;
            }

            string errorInfo;
            if (!_station.GrabModelPicture( out errorInfo))
            {
                MessageBox.Show(errorInfo);
                return;
            }

            JFTipsDelayClose.Show("开始采集产品模板图像！", 1);

        }

        /// <summary>
        /// 设置模板图像保存路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetMdelSavePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = _station.GetCfgParamValue(DLAFVisionTeachStation.SCItemName_ModelPicPath) as string;
            if (dialog.ShowDialog().Equals(DialogResult.OK))
            {
                _station.SetCfgParamValue(DLAFVisionTeachStation.SCItemName_ModelPicPath, dialog.SelectedPath);
                _station.SaveCfg();
                tbModelPicPath.Text = dialog.SelectedPath;
            }
        }


        /// <summary>
        /// 检测所选FOV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInspectCurrFov_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show(" 无效操作，工站未设置/空值");
                return;
            }
            if (string.IsNullOrEmpty(cbRecipeID.Text))
            {
                MessageBox.Show("请先选择Recipe！");
                return;
            }

            if (cbIcRow.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择 IC Row！");
                return;
            }


            if (cbIcCol.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择 IC Col！");
                return;
            }


            if (cbFovName.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择 Fov Name！");
                return;
            }
            string errorInfo;
            int row = Convert.ToInt32(cbIcRow.Text);
            int col = Convert.ToInt32(cbIcCol.Text);
            string fovName = cbFovName.Text;
            if(!_station.InspectFov(row,col,fovName,out errorInfo))
            {
                MessageBox.Show("Fov视觉检测失败:" + errorInfo);
                return;
            }
            JFTipsDelayClose.Show("开始检测" + fovName, 1);
        }


    }
}
