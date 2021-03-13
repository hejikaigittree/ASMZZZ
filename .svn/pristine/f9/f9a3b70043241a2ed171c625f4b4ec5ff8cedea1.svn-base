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
using JFToolKits;

namespace JFUI
{
    public partial class UcDioPanel : JFRealtimeUI//UserControl
    {
        public UcDioPanel()
        {
            InitializeComponent();
        }

        private void UcDioPanel_Load(object sender, EventArgs e)
        {
            int nativeRight = 0, nativeBottom = 0;
            foreach (Control ctrl in Controls)
            {
                if(!ucIOs.Contains(ctrl))
                {
                    if (ctrl.Location.X + ctrl.Size.Width > nativeRight)
                        nativeRight = ctrl.Location.X + ctrl.Size.Width;
                    if (ctrl.Location.Y + ctrl.Size.Height > nativeBottom)
                        nativeBottom = ctrl.Location.Y + ctrl.Size.Height;
                }
            }
            nativeZize = new Size(nativeRight, nativeBottom);
            AdjustIOView();
            isLoaded = true;
            UpdateIOStatus();
        }

        [Category("DIO"), Description("IO类型"), Browsable(true)]
        public bool IsDigitOut 
        {
            get { return _isDOut; } 
            set
            {
                _isDOut = value;
                lbTital.Text = _isDOut ? "DOUT Panel:" : "DIN Panel:";
                AdjustIOView();
            }
        }

        [Category("DIO"), Description("IO单元控件大小"), Browsable(true)]
        public Size DioItemSize 
        {
            get { return _dioItemSize; }
            set
            {
                if (value == _dioItemSize)
                    return;
                _dioItemSize = value;
                AdjustIOView();
            }
        }

        [Category("DIO"), Description("排列方向"), Browsable(true)]
        public AlignMode DioItemAlign
        {
            get
            {
                return _dioAlignMode;
            }
            set
            {
                if (_dioAlignMode == value)
                    return;
                _dioAlignMode = value;
                AdjustIOView();
            }
        }

        [Category("DIO"), Description("每(行)列最大个数"), Browsable(true)]
        public int MaxItemsInRow
        {
            get { return _maxItemsInRow; } 
            set
            {
                if (value < 0)
                    value = 0;
                if (_maxItemsInRow == value)
                    return;
                _maxItemsInRow = value;
                AdjustIOView();
            }
        }

        [Category("DIO"), Description("横向间距"), Browsable(true)]
        public int IntervalH 
        {
            get { return _intervalH; } 
            set
            {
                if (value < 0)
                    value = 0;
                if (value == _intervalH)
                    return;
                _intervalH = value;
                AdjustIOView();
            }
        }
        [Category("DIO"), Description("纵向间距"), Browsable(true)]
        public int IntervalV
        {
            get { return _intervalV; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value == _intervalV)
                    return;
                _intervalV = value;
                AdjustIOView();
            }
        }




        /// <summary>
        ///  添加一个IO ,IO类型由属性 IsDigitOut 决定
        /// </summary>
        /// <param name="module"></param>
        /// <param name="ioIndex"></param>
        /// <param name="name"></param>
        public void AddIO(IJFModule_DIO module,int ioIndex,string name = null,LampButton.LColor offColor = LampButton.LColor.Gray, LampButton.LColor onColor = LampButton.LColor.Green)
        {
            if (name == null)
                name = (IsDigitOut ? "Out_" : "In_") + ioIndex.ToString("D2");
            foreach(DIOInfo ii in ioInfos)
                if (module == ii.Module && ioIndex == ii.Index)
                    return;
            UcDioChn io = new UcDioChn();
            io.OffColor = offColor;
            io.OnColor = onColor;
            if (IsDigitOut)
                io.Click += new EventHandler(DOButtonClick);
            io.IOName = name;
            Controls.Add(io);
            ucIOs.Add(io);
            ioInfos.Add(new DIOInfo(module, ioIndex));
            //dictIOs.Add(io, new DIOInfo(module, ioIndex));
            if (isLoaded)
                AdjustIOView();
            
        }

        public void RemoveIO(IJFModule_DIO module, int ioIndex)
        {
            for (int i=0;i < ioInfos.Count;i++)
            {
                DIOInfo ii = ioInfos[i];
                if (module == ii.Module && ioIndex == ii.Index)
                {
                    ioInfos.RemoveAt(i);
                    Control ioCtrl = ucIOs[i];
                    ucIOs.RemoveAt(i);
                    Controls.Remove(ioCtrl);
                    AdjustIOView();
                    return;
                }
            }
        }
        public void RemoveAllDIO()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(RemoveAllDIO));
                return;
            }
            ioInfos.Clear();
            for (int i = 0; i < ucIOs.Count; i++)
                Controls.Remove(ucIOs[i]);
            
            ucIOs.Clear();
            AdjustIOView();
        }

        
        
        /// <summary>
        /// 将IO状态更新到界面上
        /// </summary>
        public void UpdateIOStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateIOStatus));
                return;
            }
            if (ucIOs.Count == 0)
                return;
            StringBuilder sb = new StringBuilder("Update Err:");
            bool isUpdateOK = true;
            do
            {
                bool[] updateds = new bool[ucIOs.Count]; //更新过的IO列表
                int updatedCount = 0;//已经更新过的数量
                for (int i = 0; i < ucIOs.Count; i++)
                {
                    if (updatedCount == ucIOs.Count) //全部更新完成
                        break;
                    if (updateds[i]) //这个IO更新过
                        continue;
                    UcDioChn ucio = ucIOs[i] as UcDioChn;
                    DIOInfo ii = ioInfos[i];

                    if (ii.Module == null)
                    {
                        updateds[i] = true;
                        updatedCount++;
                        sb.Append(ucio.IOName + " ");
                        isUpdateOK = false;
                        continue;
                    }

                    if (ii.Index < 0 || ii.Index >= (IsDigitOut ? ii.Module.DOCount : ii.Module.DICount))
                    {
                        updateds[i] = true;
                        updatedCount++;
                        sb.Append(ucio.IOName + " ");
                        isUpdateOK = false;
                        continue;
                    }


                    bool isOn = false;
                    int err = 0;
                    err = IsDigitOut? ii.Module.GetDO(ii.Index, out isOn):ii.Module.GetDI(ii.Index, out isOn);
                    if (err != 0)
                    {
                        updateds[i] = true;
                        updatedCount++;
                        sb.Append(ucio.IOName + " ");
                        isUpdateOK = false;
                        continue;
                    }
                    else
                        ucio.IsTurnOn = isOn;
                    
                    updateds[i] = true;
                    updatedCount++;
                    //尝试更新后继IO
                    bool isSameModuleExisted = false;
                    for(int j = updatedCount;j<ucIOs.Count;j++)
                        if(ioInfos[j].Module == ii.Module)
                        {
                            isSameModuleExisted = true;
                            break;
                        }
                    if (!isSameModuleExisted)
                        continue;
                    bool[] isOns = null;
                    err = IsDigitOut ? ii.Module.GetAllDOs(out isOns) : ii.Module.GetAllDIs(out isOns);
                    if (err != 0)
                        continue;
                    for (int j = updatedCount; j < ucIOs.Count; j++)
                    {
                        if (ioInfos[j].Module == ii.Module && ioInfos[j].Index >0 && ioInfos[j].Index <= (IsDigitOut ? ioInfos[j].Module.DOCount: ioInfos[j].Module.DICount))
                        {
                            UcDioChn ui = ucIOs[j] as UcDioChn;
                            ui.IsTurnOn = isOns[j];
                            updateds[j] = true;
                            updatedCount++;
                            i++;
                        }
                    }
                }
            } while (false);
            if (!isUpdateOK)
                lbInfo.Text = sb.ToString();
                
        }

        public bool DIOEditting
        {
            get { return _isDioEditting; }
            set 
            {
                if (value == _isDioEditting)
                    return;
                _isDioEditting = value;
                foreach (Control ctr in ucIOs)
                    (ctr as UcDioChn).IsEditting = _isDioEditting;
            }

        }

        public string[] DioNames
        {
            get
            {
                List<string> ret = new List<string>();
                foreach(Control ct in ucIOs)
                    ret.Add((ct as UcDioChn).IOName);
                return ret.ToArray();
            }
        }

        public string[] DioNamesEditted
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (Control ct in ucIOs)
                    ret.Add((ct as UcDioChn).IONameEditting);
                return ret.ToArray();
            }
        }




        /// <summary>
        /// 根据单元格大小调整界面
        /// </summary>
        void AdjustIOView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustIOView));
                return;
            }
            int i = 0;
            foreach(Control ucIO in ucIOs)
            {
                int x = 0;
                int y = 0;
                if(DioItemAlign == AlignMode.horizontal) //横排
                {
                    if (MaxItemsInRow == 0) //一行排到头
                    {
                        x = IntervalH + i  * (DioItemSize.Width + IntervalH);
                        y = nativeZize.Height + IntervalV;
                    }
                    else
                    {
                        x = IntervalH + (i % MaxItemsInRow) * (DioItemSize.Width + IntervalH);
                        y = nativeZize.Height + IntervalV + (i / MaxItemsInRow) * (DioItemSize.Height + IntervalV);
                    }
                }
                else //竖排
                {
                    if (MaxItemsInRow == 0) //一列排到底
                    {
                        x = IntervalH;
                        y = nativeZize.Height + IntervalV + i  * (DioItemSize.Height + IntervalV);
                    }
                    else
                    {
                        x = IntervalH + (i / MaxItemsInRow) * (DioItemSize.Width + IntervalH);
                        y = nativeZize.Height + IntervalV + (i % MaxItemsInRow) * (DioItemSize.Height + IntervalV);
                    }

                }
                ucIO.Location = new Point(x,y);
                ucIO.Size = DioItemSize;
                i++;
            }
            

            //UpdateIOStatus();

        }

        /// <summary>
        /// DO按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DOButtonClick(object sender,EventArgs e)
        {
            if (!IsDigitOut)
                return;
            for(int i = 0; i < ucIOs.Count;i++ )
                if(sender == ucIOs[i] || sender == (ucIOs[i] as UcDioChn).LButton)
                {
                    UcDioChn ucIO = ucIOs[i] as UcDioChn;
                    DIOInfo ii = ioInfos[i];
                    if (ii.Module == null)
                    {
                        lbInfo.Text = "Failed By:" + ucIO.IOName + "'s Module is null";
                        return;
                    }

                    if(ii.Index < 0 || ii.Index >= ii.Module.DOCount)
                    {
                        lbInfo.Text = "Failed By:" + ucIO.IOName + "'s index is out of rang 0~" + (ii.Module.DOCount-1);
                        return;
                    }
                    bool isOn = false;
                    int err = ii.Module.GetDO(ii.Index, out isOn);
                    if(err != 0)
                    {
                        lbInfo.Text = "Failed By: Get DO=\"" + ucIO.IOName + "\"failed ,errorcode = " + err;
                        return;
                    }
                    err = ii.Module.SetDO(ii.Index, !isOn);
                    if (err != 0)
                    {
                        lbInfo.Text = "Failed By: Set DO=\"" + ucIO.IOName + "\"failed ,errorcode = " + err;
                        return;
                    }
                    lbInfo.Text = "Success! Set DO=\"" + ucIO.IOName + "\""+ (isOn? "Off":"On");
                    return;
                }
        }

        public override void UpdateSrc2UI()
        {
            UpdateIOStatus();
        }

        bool isLoaded = false;
        bool _isDOut = false;
        Size _dioItemSize = new Size(240, 33);
        AlignMode _dioAlignMode = AlignMode.vertical;
        int _intervalH = 2;
        int _intervalV = 2;
        int _maxItemsInRow = 0;
        bool _isDioEditting = false; //处于编辑DIO名称状态

        //SortedDictionary<Control, DIOInfo> dictIOs = new SortedDictionary<Control, DIOInfo>();
        List<Control> ucIOs = new List<Control>();
        List<DIOInfo> ioInfos = new List<DIOInfo>();
        Size nativeZize;//其他控件所占区域
        


        public enum AlignMode
        {
            vertical = 0,
            horizontal ,
        }

         class DIOInfo
        {
            internal DIOInfo(IJFModule_DIO module, int ioIndex)
            {
                Module = module;
                Index = ioIndex;
            }

            internal IJFModule_DIO Module { get; set; }
            internal int Index { get; set; }
                 
        }

    }
}
