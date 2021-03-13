using HalconDotNet;
using LFAOIRecipe.Algorithm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    class CreatePositionModel : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }    

        public GoldenModelParameter GoldenModelParameter { get; private set; }

        //public ObservableCollection<UserRegion> MatchUserRegions { get; private set; }

        //private IEnumerable<HObject> MatchRegions => MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public ObservableCollection<MatchRegionsGroup> Groups { get; private set; }


        private HTHalControlWPF htWindow;

        private GoldenModelObject goldenModelObject;

        private readonly string ModelsRecipeDirectory;

        //private string imageIndex= "System.Windows.Controls.ComboBoxItem: 原图";
        //public string ImageIndex
        //{
        //    get => imageIndex;
        //    set
        //    {
        //        if (imageIndex != value)
        //        {
        //            goldenModelObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex);
        //            htWindow.Display(goldenModelObject.DieChannelImage);
        //            imageIndex = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        private int imageIndex;
        /// <summary>
        /// 切换原图或通道图
        /// </summary>
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage0, GoldenModelParameter.ImageChannelIndex + 1);
                        goldenModelObject.ChannelImage = ChannelImage0;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 0)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage1, 1);
                        goldenModelObject.ChannelImage = ChannelImage1;
                        GoldenModelParameter.ImageChannelIndex = 0;

                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 1)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage2, 2);
                        goldenModelObject.ChannelImage = ChannelImage2;
                        GoldenModelParameter.ImageChannelIndex = 1;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 2)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage3, 3);
                        goldenModelObject.ChannelImage = ChannelImage3;
                        GoldenModelParameter.ImageChannelIndex = 2;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 3)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage4, 4);
                        goldenModelObject.ChannelImage = ChannelImage4;
                        GoldenModelParameter.ImageChannelIndex = 3;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImagen, value + 1);
                        goldenModelObject.ChannelImage = ChannelImagen;
                        GoldenModelParameter.ImageChannelIndex = value;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }


        public CommandBase CreatePosModelCommand { get; private set; }

        public CreatePositionModel(HTHalControlWPF htWindow,
                                 GoldenModelParameter goldenModelParameter,
                                 GoldenModelObject goldenModelObject,
                                 ObservableCollection<MatchRegionsGroup> groups,
                                 string modelsRecipeDirectory)
        {
            DisplayName = "创建定位模板";
            Content = new Page_CreatePositionModel { DataContext = this };
            this.htWindow = htWindow;
            this.GoldenModelParameter = goldenModelParameter;
            this.goldenModelObject = goldenModelObject;
            this.Groups = groups;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;
            CreatePosModelCommand = new CommandBase(ExecuteCreatePosModelCommand);
        }

        private void ExecuteCreatePosModelCommand(object parameter)
        {
            if (GoldenModelParameter.ImageCountChannels > 0 && GoldenModelParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            if (Groups==null || Groups.Count == 0)
            {
                MessageBox.Show("请先添加匹配区域！");
                return;
            }
            if (goldenModelObject.Image == null || !goldenModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }

            Window_Loading window_Loading = new Window_Loading("正在生成定位模板");
            try
            {
                window_Loading.Show();
                HOperatorSet.GenEmptyObj(out HObject matchRegions);
                foreach (var item in Groups)
                {
                    // 0115 -lw
                    HOperatorSet.ConcatObj(Region.Union1Region(item.MatchUserRegions.Where(r => r.IsEnable)), matchRegions, out matchRegions);
                }

                // 清除已有模板 0115 -lw
                if (goldenModelObject.PosModelID != null && goldenModelObject.PosModelID.TupleLength() > 0)
                {
                    HTuple ModelTypeStr = null;
                    ModelTypeStr =  GoldenModelParameter.ModelType == 0 ? "ncc" : "shape";
                    Algorithm.Model_RegionAlg.HTV_clear_model(goldenModelObject.PosModelID, ModelTypeStr);
                }

                //12-05 使用多定位模板
                Algorithm.Model_RegionAlg.HTV_CreateLocModel(Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, GoldenModelParameter.ImageChannelIndex),
                                                            matchRegions,
                                                            GoldenModelParameter.ModelType,//0
                                                            GoldenModelParameter.AngleStart,//传入接口是角度值
                                                            GoldenModelParameter.AngleExt,//传入接口是角度值
                                                            out HTuple PosModelID);

                //不使用多定位模板
                //Algorithm.Model_RegionAlg.HTV_CreateLocModel(Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, GoldenModelParameter.ImageChannelIndex),
                                                   //GoldenModelParameter.IsMultiModelPosMode? Algorithm.Region.ConcatRegion(MatchUserRegions): Algorithm.Region.Union1Region(MatchUserRegions),
                                                   //Algorithm.Region.Union1Region(MatchUserRegions),
                                                   //GoldenModelParameter.ModelType,//0
                                                   //out HTuple PosModelID);

                goldenModelObject.PosModelID = PosModelID;
                //保存模板
                if (goldenModelObject.PosModelID.TupleLength() == 1 )
                {
                    GoldenModelParameter.PosModelIdPath = $"{ModelsRecipeDirectory}PosModel.dat";
                    Algorithm.File.SaveModel(GoldenModelParameter.PosModelIdPath, GoldenModelParameter.ModelType, goldenModelObject.PosModelID);
                    MessageBox.Show("定位模板已生成!");
                }
                else if (goldenModelObject.PosModelID.TupleLength() > 1)
                {
                    String[] ModelIdPathArry = new string[goldenModelObject.PosModelID.TupleLength()];
                    for (int i = 0; i < goldenModelObject.PosModelID.TupleLength(); i++)
                    {
                        ModelIdPathArry[i] = $"{ModelsRecipeDirectory}PosModel_" + i + ".dat";
                    }
                    GoldenModelParameter.PosModelIdPath = String.Join(",", ModelIdPathArry);
                    Algorithm.File.SaveModel(ModelIdPathArry, GoldenModelParameter.ModelType, goldenModelObject.PosModelID);//
                    MessageBox.Show("定位模板已生成！");
                }
                window_Loading.Close();
            }
            catch (System.Exception ex)
            {
                window_Loading.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        private void DispalyAllGroupRegions(bool isDisplayImage = false)
        {
            if (Groups == null || Groups.Count == 0) return;
            HOperatorSet.GenEmptyObj(out HObject matchRegions);
            foreach (var item in Groups)
            {
                HOperatorSet.ConcatObj(Region.ConcatRegion(item.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion)), matchRegions, out matchRegions);
            }
            if (isDisplayImage)
            {
                htWindow.DisplaySingleRegion(matchRegions);
            }
            else
            {
                htWindow.DisplaySingleRegion(matchRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex));
            }
        }

        public bool CheckCompleted()
        {
            /*
            if (!isCompleted)
            {
                MessageBox.Show("请创建定位模板");
            }           
            return isCompleted;
            */
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();
            //htWindow.DisplayMultiRegion(MatchRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage,GoldenModelParameter.ImageChannelIndex));

            DispalyAllGroupRegions();

            //1121
            ChannelNames = new ObservableCollection<ChannelName>(GoldenModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = GoldenModelParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");
        }

        public void Dispose()
        {
            (Content as Page_CreatePositionModel).DataContext = null;
            (Content as Page_CreatePositionModel).Close();
            Content = null;
            this.htWindow = null;
            this.GoldenModelParameter = null;
            this.goldenModelObject = null;
            this.Groups = null;
            CreatePosModelCommand = null;
        }
    }
}
