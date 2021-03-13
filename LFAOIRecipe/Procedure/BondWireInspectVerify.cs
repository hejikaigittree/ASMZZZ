using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LFAOIRecipe
{
    class BondWireInspectVerify : ViewModelBase, IProcedure
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        private BondWireRegionGroup currentGroup;
        public BondWireRegionGroup CurrentGroup
        {
            get => currentGroup;
            set
            {
                if (currentGroup != value)
                {
                    currentGroup = value;
                    OnPropertyChanged();
                    DispalyGroupRegion();
                    if (CurrentGroup?.WireUserRegion == null)
                    {
                        (Content as Page_BondWireInspectVerify).dockPanel_IsCurve.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        (Content as Page_BondWireInspectVerify).dockPanel_IsCurve.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private string switchImageComboBoxIndex;
        /// <summary>
        /// 切换整图和Die区域
        /// </summary>
        public string SwitchImageComboBoxIndex
        {
            get => switchImageComboBoxIndex;
            set
            {
                if (switchImageComboBoxIndex != value)
                {
                    if (value == "System.Windows.Controls.ComboBoxItem: 原图")
                    {
                        htWindow.Display(bond2ModelObject.DieImage);
                    }
                    else if (value == "System.Windows.Controls.ComboBoxItem: R通道")
                    {
                        htWindow.Display(bond2ModelObject.DieImageR);
                    }
                    else if (value == "System.Windows.Controls.ComboBoxItem: G通道")
                    {
                        htWindow.Display(bond2ModelObject.DieImageG);
                    }
                    else if (value == "System.Windows.Controls.ComboBoxItem: B通道")
                    {
                        htWindow.Display(bond2ModelObject.DieImageB);
                    }
                    switchImageComboBoxIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 验证的焊点一的横坐标
        /// </summary>
        private double bond1Row;
        public double Bond1Row
        {
            get => bond1Row;
            set => OnPropertyChanged(ref bond1Row, value);
        }

        /// <summary>
        /// 验证的焊点一的纵坐标
        /// </summary>
        private double bond1Column;
        public double Bond1Column
        {
            get => bond1Column;
            set => OnPropertyChanged(ref bond1Column, value);
        }

        //验证的第一焊点的半径在5-30的范围外，显示-1
        private double bond1Radius;
        public double Bond1Radius
        {
            get => bond1Radius;
            set
            {
                if ((value > 5) && (value < 30))
                {
                    OnPropertyChanged(ref bond1Radius, value);
                }
                else
                {
                    OnPropertyChanged(ref bond1Radius, -1);
                }
            }
        }

        /// <summary>
        /// 验证的焊点二的横坐标
        /// </summary>
        private double bond2Row;
        public double Bond2Row
        {
            get => bond2Row;
            set => OnPropertyChanged(ref bond2Row, value);
        }

        /// <summary>
        /// 验证的焊点二的纵坐标
        /// </summary>
        private double bond2Column;
        public double Bond2Column
        {
            get => bond2Column;
            set => OnPropertyChanged(ref bond2Column, value);
        }

        //验证的第二焊点的半径在5-30的范围外，显示-1
        private double bond2Radius;
        public double Bond2Radius//
        {
            get => bond2Radius;
            set
            {
                if ((value > 5) && (value < 30))
                {
                    OnPropertyChanged(ref bond2Radius, value);
                }
                else
                {
                    OnPropertyChanged(ref bond2Radius, -1);
                }
            }
        }

        //新田间中继点参数设置
        private bool bendFlag = true;//
        public bool BendFlag
        {
            get { return bendFlag; }
            set { OnPropertyChanged(ref bendFlag, value); }
        }

        public ObservableCollection<BondWireRegionGroup> Groups { get; private set; } = new ObservableCollection<BondWireRegionGroup>();

        public event Action OnSaveXML;

        private HTHalControlWPF htWindow;

        private Bond2ModelObject bond2ModelObject;

        //private Bond2ModelParameter bond2ModelParameter;
        private Bond2ModelParameter bond2ModelParameter;//
        public Bond2ModelParameter Bond2ModelParameter//
        {
            get => bond2ModelParameter;
            set => OnPropertyChanged(ref bond2ModelParameter, value);
        }
        
        public CommandBase LoadImageCommand { get; private set; }
        public CommandBase VerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }

        public BondWireInspectVerify(HTHalControlWPF htWindow,
                                     ObservableCollection<BondWireRegionGroup> groups,
                                     Bond2ModelObject bond2ModelObject,
                                     Bond2ModelParameter bond2ModelParameter)
        {
            DisplayName = "检测验证";
            Content = new Page_BondWireInspectVerify { DataContext = this };

            this.htWindow = htWindow;
            this.Groups = groups;
            this.bond2ModelObject = bond2ModelObject;
            this.Bond2ModelParameter = bond2ModelParameter;
            LoadImageCommand = new CommandBase(ExecuteLoadImageCommand);
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
        }


        private void ExecuteLoadImageCommand(object parameter)//
        {

        }

        //检测验证
        private void ExecuteVerifyCommand(object parameter)
        {
            /*
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点金线组合");
                return;
            }
            HTuple modelId;
            string[] ModelPathArr = bond2ModelParameter.ModelIdPath.Split(',');
            if (bond2ModelObject.ModelID != null)
            {
                modelId = bond2ModelObject.ModelID;
            }
            else if (File.Exists(ModelPathArr[0]))
            {
                modelId = Algorithm.File.ReadModel(ModelPathArr, bond2ModelParameter.ModelType);
            }
            else
            {
                MessageBox.Show("请先创建第二焊点模板");
                return;
            }
            Window_Loading window_Loading = new Window_Loading("正在检验");
            window_Loading.Show();
            try
            {
                HTupleVector bond1MeasurePara = new HTupleVector(1);
                bond1MeasurePara = (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple())));
                bond1MeasurePara[0] = new HTupleVector(new HTuple(CurrentGroup.Parameter.Bond1MinRadius));
                bond1MeasurePara[1] = new HTupleVector(new HTuple(CurrentGroup.Parameter.Bond1MaxRadius));
                bond1MeasurePara[2] = new HTupleVector(new HTuple(CurrentGroup.Parameter.Bond1MeasureTh));
                bond1MeasurePara[3] = new HTupleVector(new HTuple(CurrentGroup.Parameter.IsBond1MeasureTransPositive ? "positive" : "negative"));

                HTupleVector bond2MeasurePara = new HTupleVector(1);
                bond2MeasurePara[0] = new HTupleVector(new HTuple(CurrentGroup.Parameter.IsBond2Model ? 1 : 0));

                bond2MeasurePara[1] = new HTupleVector(new HTuple(bond2ModelParameter.ModelType));
                bond2MeasurePara[2] = new HTupleVector(modelId);
                bond2MeasurePara[3] = new HTupleVector(new HTuple(bond2ModelParameter.IsPreProcess ? 1 : 0)); 
                bond2MeasurePara[4] = new HTupleVector(new HTuple(bond2ModelParameter.Gamma));

                bond2MeasurePara[5] = new HTupleVector(new HTuple(CurrentGroup.Parameter.Bond2MinRadius));
                bond2MeasurePara[6] = new HTupleVector(new HTuple(CurrentGroup.Parameter.Bond2MaxRadius));
                bond2MeasurePara[7] = new HTupleVector(new HTuple(CurrentGroup.Parameter.Bond2GrayTh));
                bond2MeasurePara[8] = new HTupleVector(new HTuple(CurrentGroup.Parameter.Bond2MeasureTh));
                bond2MeasurePara[9] = new HTupleVector(new HTuple(CurrentGroup.Parameter.IsBond2MeasureTransPositive ? "positive" : "negative"));

                HTupleVector wireMeasurePara = new HTupleVector(1);
                wireMeasurePara[0] = new HTupleVector(new HTuple(CurrentGroup.Parameter.IsCurve ? 1 : 0));
                wireMeasurePara[1] = new HTupleVector(new HTuple(CurrentGroup.Parameter.WireSearchLen));
                wireMeasurePara[2] = new HTupleVector(new HTuple(CurrentGroup.Parameter.WireClipLen));
                wireMeasurePara[3] = new HTupleVector(new HTuple(CurrentGroup.Parameter.WireWidth));
                wireMeasurePara[4] = new HTupleVector(new HTuple(CurrentGroup.Parameter.WireContrast));
                wireMeasurePara[5] = new HTupleVector(new HTuple(CurrentGroup.Parameter.WireMinSegLen));
                wireMeasurePara[6] = new HTupleVector(new HTuple(CurrentGroup.Parameter.WireAngleExt));
                wireMeasurePara[7] = new HTupleVector(new HTuple(CurrentGroup.Parameter.WireMaxGap));

                HTuple imageIndex = new HTuple();
                imageIndex[0] = CurrentGroup.Parameter.WireImageIndex+1 ;
                imageIndex[1] = CurrentGroup.Parameter.PCBImageIndex+1 ;

                HOperatorSet.GenEmptyObj(out HObject channelImage);
                if (Bond2ModelParameter.ImageCountChannels==1)
                {
                    channelImage = bond2ModelObject.Image;
                } 
                else if (Bond2ModelParameter.ImageCountChannels == 3)
                {
                    Algorithm.Region.Separat_Image(bond2ModelObject.Image, out channelImage);
                }
                //接口更新             [1,3]   
                Algorithm.Model.HTV_Inspect_WireBondGroup_Recipe(channelImage,
                                                                 CurrentGroup.Bond1UserRegion?.CalculateRegion,
                                                                 CurrentGroup.Bond2UserRegion?.CalculateRegion,
                                                                 CurrentGroup.WireUserRegion?.CalculateRegion,
                                                             out HObject wire,
                                                                 imageIndex,
                                                                 bond1MeasurePara,
                                                                 bond2MeasurePara,
                                                                 wireMeasurePara,
                                                             out HTuple bond1Pos,
                                                             out HTuple bond2Pos);
                
                HOperatorSet.GenEmptyObj(out HObject bond1);
                HOperatorSet.GenEmptyObj(out HObject bond2);
                if ((int)(new HTuple(((bond1Pos.TupleSelect(2))).TupleEqual(-1))) == 0)
                {
                    HOperatorSet.GenCircle(out bond1,
                                           bond1Pos.TupleSelect(0)- bond2ModelParameter.DieImageRowOffset,
                                           bond1Pos.TupleSelect(1)- bond2ModelParameter.DieImageColumnOffset,
                                           bond1Pos.TupleSelect(2));
                    Bond1Row = bond1Pos.TupleSelect(0);
                    Bond1Column = bond1Pos.TupleSelect(1);
                    Bond1Radius = bond1Pos.TupleSelect(2);
                }
                else
                {
                    Bond1Row = 0;
                    Bond1Column = 0;
                    Bond1Radius = 0;
                }
                if ((int)(new HTuple(((bond2Pos.TupleSelect(2))).TupleEqual(-1))) == 0)
                {
                    HOperatorSet.GenCircle(out bond2,
                                           bond2Pos.TupleSelect(0) - bond2ModelParameter.DieImageRowOffset,
                                           bond2Pos.TupleSelect(1) - bond2ModelParameter.DieImageColumnOffset,
                                           bond2Pos.TupleSelect(2));
                    Bond2Row = bond2Pos.TupleSelect(0);
                    Bond2Column = bond2Pos.TupleSelect(1);
                    Bond2Radius = bond2Pos.TupleSelect(2);
                }
                else
                {
                    Bond2Row = 0;
                    Bond2Column = 0;
                    Bond2Radius = 0;
                }
                htWindow.DisplayMultiRegion(bond1, bond2, wire);
                window_Loading.Close();

                if (currentGroup.WireUserRegion == null) return;
                if (currentGroup.WireUserRegion.RegionType != RegionType.Point) return;
                Algorithm.Model.HTV_Node_Inf(bond2Pos.TupleSelect(0),
                                             bond2Pos.TupleSelect(1),
                                             bond1Pos.TupleSelect(0),
                                             bond1Pos.TupleSelect(1),
                                             currentGroup.WireUserRegion.RegionParameters[0],
                                             currentGroup.WireUserRegion.RegionParameters[1],
                                        new HTuple(BendFlag),
                                        out HTuple wirePara,
                                        out HTuple errorCode,
                                        out HTuple errorString);
                if (errorCode.I != 0)
                {
                    int errorStringCount = errorString.TupleLength();
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < errorStringCount; i++)
                    {
                        sb.AppendLine(errorString.TupleSelect(i));
                    }
                    MessageBox.Show(sb.ToString());
                    return;
                }
                currentGroup.WireBendPara = wirePara;
            }
            catch (Exception ex)
            {
                window_Loading.Close();
                MessageBox.Show(ex.ToString(), "检测验证出错");
            }
            */
        }
        
        //保存参数
        private void ExecuteSaveCommand(object parameter)
        {
            try
            {
                OnSaveXML?.Invoke();
                MessageBox.Show("参数保存完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "参数保存失败");
            }
        }

        private void DispalyGroupRegion()
        {
            if (CurrentGroup == null)
            {
                htWindow.Display(bond2ModelObject.DieImage, true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            if (CurrentGroup.Bond2UserRegion != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.Bond2UserRegion.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (CurrentGroup.WireUserRegion != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.WireUserRegion.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }
            htWindow.DisplaySingleRegion(concatGroupRegion, bond2ModelObject.DieImage);
        }

        public bool CheckCompleted()
        {
            return true;
        }

        public void Initial()
        {
            if (Bond2ModelParameter.ImageCountChannels == 1)
            {
                (Content as Page_BondWireInspectVerify).stackPanel_channelDisplay.Visibility = Visibility.Collapsed;
            }
            else if (Bond2ModelParameter.ImageCountChannels > 1)
            {
                (Content as Page_BondWireInspectVerify).stackPanel_channelDisplay.Visibility = Visibility.Visible;
            }
            htWindow.ClearSelection();
            DispalyGroupRegion();
            htWindow.Display(bond2ModelObject.DieImage);
        }

        public void Dispose()
        {
            (Content as Page_BondWireInspectVerify).DataContext = null;
            (Content as Page_BondWireInspectVerify).Close();
            Content = null;
            htWindow = null;
            Groups = null;
            bond2ModelObject = null;
            bond2ModelParameter = null;
            VerifyCommand = null;
            SaveCommand = null;
        }
    }
}
