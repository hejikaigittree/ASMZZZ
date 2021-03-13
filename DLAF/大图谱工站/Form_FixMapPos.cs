using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HTHalControl;
using HalconDotNet;
using ToolKits.RegionModify;
using VisionMethonDll;

namespace DLAF
{
    public partial class Form_FixMapPos : Form
    {
        public static Form_FixMapPos Instance = null;
        public HObject showRegion = null;
        Model LoctionModels = null;
        HTWindowControl htWindow = null;
        RegionModifyForm regionModifyForm = null;
        AutoMappingStation _station;
        public Form_FixMapPos()
        {
            InitializeComponent();
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Instance = this;
        }

        public void SetStation(AutoMappingStation station)
        {
            _station = station;
        }

        /// <summary>
        /// 窗体实例化方法
        /// </summary>
        /// <param name="htWindow">图像视窗</param>
        /// <param name="Models">操作的模板变量</param>
        /// <param name="regionMode">区域类型</param>
        public Form_FixMapPos(HTWindowControl htWindow, Model Models, RegionModifyForm.RegionMode regionMode)
        {
            InitializeComponent();
            numericUpDown1.Value = (decimal)0.5;
            trackBar1.Value = (int)(numericUpDown1.Value*100);
            this.LoctionModels = Models;
            this.htWindow = htWindow;
            this.htWindow.SetMenuStrip(false);
            Instance = this;
        }

        private void button_DragModelRegion_Click(object sender, EventArgs e)
        {
            if (showRegion != null)
                if (showRegion.IsInitialized())
                    showRegion.Dispose();
            showRegion = regionModifyForm.ModifyRegion;
            htWindow.Focus();
            HObject ho_show_region = null, ho_dragRect=null, ho_update_show_contour=null;
            HOperatorSet.GenRegionContourXld(LoctionModels.showContour, out ho_show_region,
                "filled");
            HOperatorSet.DragRegion1(ho_show_region, out ho_dragRect, htWindow.HTWindow.HalconWindow);
            HOperatorSet.GenContourRegionXld(ho_dragRect, out ho_update_show_contour,
                "border");
            HOperatorSet.ConcatObj(ho_update_show_contour, showRegion, out showRegion);
            if (regionModifyForm != null)
                if (!regionModifyForm.IsDisposed)
                    regionModifyForm.Dispose();
            regionModifyForm = new RegionModifyForm(htWindow, showRegion, RegionModifyForm.RegionMode.contour);
            regionModifyForm.Dock = DockStyle.Fill;
            regionModifyForm.Visible = true;
            panel1.Controls.Add(regionModifyForm);
            _station.operation.ShowImage(htWindow, htWindow.Image, showRegion);
        }

        private void button_GenClipPos_Click(object sender, EventArgs e)
        {

            try
            {
                HTuple hv_iFlag=null;
                VisionMethon.get_mapping_coords_ext(showRegion, _station.hv_xSnapPosLT, _station.hv_ySnapPosLT, _station.List_UV2XYResult[_station.SelectedIndex], _station.jFDLAFProductRecipe.ScaleFactor,
                    _station.RowNumber, _station.ColumnNumber * _station.BlockNumber,
                    out _station.clipMapX, out _station.clipMapY, out _station.clipMapRow, out _station.clipMapCol,
                    out _station.clipMapU, out _station.clipMapV, out _station.hv_dieWidth, out _station.hv_dieHeight, out hv_iFlag);
                if (hv_iFlag.S != "")
                {
                    MessageBox.Show("生成芯片点位失败." + hv_iFlag.S);
                    return;
                }
                HOperatorSet.WriteTuple(_station.clipMapX, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapX.dat");
                HOperatorSet.WriteTuple(_station.clipMapY, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapY.dat");
                HOperatorSet.WriteTuple(_station.clipMapRow, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapRow.dat");
                HOperatorSet.WriteTuple(_station.clipMapCol, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapCol.dat");
                //_station.scanIniConfig.Writedouble("ScanPoints", "dieWidth", _station.hv_dieWidth.D);
                //_station.scanIniConfig.Writedouble("ScanPoints", "dieHeight", _station.hv_dieHeight.D);
                //清除之前的点位
                if (_station.ClipMapPostions != null)
                {
                    _station.ClipMapPostions.Clear();
                }
                else
                {
                    _station.ClipMapPostions = new List<ImagePosition>();
                }
                ImagePosition imagePosition = new ImagePosition();
                imagePosition.z = _station.ZFocus;
                imagePosition.b = 0;
                _station.clipPosNum = _station.clipMapX.Length;
                for (int i = 0; i < _station.clipPosNum; i++)
                {
                    imagePosition.x = _station.clipMapX.TupleSelect(i);
                    imagePosition.y = _station.clipMapY.TupleSelect(i);
                    imagePosition.r = _station.clipMapRow.TupleSelect(i);
                    imagePosition.c = _station.clipMapCol.TupleSelect(i);
                    _station.ClipMapPostions.Add(imagePosition);
                }
                MessageBox.Show("生成芯片点位完成.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存芯片点位失败。\n" + ex.ToString());
            }
        }

        private void button_FindModel_Click(object sender, EventArgs e)
        {
            if(showRegion!=null)
                if(showRegion.IsInitialized()) showRegion.Dispose();
            _station.operation.ShowImage(htWindow, htWindow.Image, null);
            HTuple hv_found_row=null;HTuple hv_found_col=null;
            HTuple hv_found_angle=null; HTuple hv_found_score=null; HTuple hv_update_def_row=null;
            HTuple hv_update_def_col=null; HTuple hv_model_H_new=null; HTuple hv_iFlag=null;
            VisionMethon.find_model_ext(htWindow.Image, htWindow.Image, LoctionModels.showContour, out showRegion,
          LoctionModels.modelType, LoctionModels.modelID, -1, -1, ((double)trackBar1.Value)/100, 0, LoctionModels.defRows, LoctionModels.defCols,
          out hv_found_row, out hv_found_col, out hv_found_angle, out hv_found_score,
          out hv_update_def_row, out hv_update_def_col, out hv_model_H_new, out hv_iFlag);
            _station.operation.ShowImage(htWindow, htWindow.Image, showRegion);
            if (regionModifyForm != null)
                if (!regionModifyForm.IsDisposed)
                    regionModifyForm.Dispose();
            regionModifyForm = new RegionModifyForm(htWindow, showRegion, RegionModifyForm.RegionMode.contour);
            regionModifyForm.Dock = DockStyle.Fill;
            regionModifyForm.Visible = true;
            panel1.Controls.Add(regionModifyForm);

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = ((decimal)trackBar1.Value)/100;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)(numericUpDown1.Value*100) ;
        }

        private void Form_FixMapPos_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.htWindow.SetMenuStrip(true);
        }
    }
}
