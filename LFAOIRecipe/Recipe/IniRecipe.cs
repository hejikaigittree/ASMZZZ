using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using HalconDotNet;

namespace LFAOIRecipe
{
    class IniRecipe : ProcedureControl, IRecipe
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "IniRecipe.xml";

        public const string IdentifyString = "INIRECIPEXML";

        public string XmlPath => $"{IniDirectory}{XmlName}";

        public int RecipeIndex { get; } = 0;

        public string RecipeName { get; } = "IniRecipe";

        public string IniDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\Reference\\");

        public string ModelsIniDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\Reference\\");

        private HTHalControlWPF htWindow;

        private IniObjects iniObjects = new IniObjects();

        private IniParameters iniParameters = new IniParameters();

        private ObservableCollection<UserRegion> DieUserRegions = new ObservableCollection<UserRegion>();

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        #region 流程模块
        private IProcedure createReference;
        private IProcedure inspectNode;
        #endregion

        public IniRecipe()
        {
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = "创建全局数据";

            createReference = new CreateReference(htWindow,
                                                  iniObjects,
                                                  iniParameters,
                                                  IniDirectory,
                                                  DieUserRegions);

            inspectNode = new InspectNode(htWindow,
                                          iniParameters,
                                          ModelsFile, 
                                          RecipeFile);

            (createReference as CreateReference).OnSaveXML += SaveXML;

            // 0118;
            htWindow.useInspectNode(inspectNode as InspectNode);

            Procedures = new IProcedure[]
            {
                createReference,
                inspectNode
            };
            ProcedureChanged(0);
        }

        //保存
        public void SaveXML()
        {
            try
            {
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XMLHelper.AddParameters(root, iniParameters, IdentifyString);
                XMLHelper.AddRegion(root, DieUserRegions, "Die_Regions");
                root.Save(XmlPath);

                if (iniObjects.Image == null || !iniObjects.Image.IsInitialized())
                {
                    MessageBox.Show("请加载参考图像！");
                    return;
                }
                /*
                if (!Directory.Exists(iniParameters.TrainningImagesDirectory) )
                {
                    MessageBox.Show("提示：训练图集没有加载！");
                }
                */
                if (iniParameters.UserRegionForCutOutIndex<1)
                {
                    MessageBox.Show("请选择参考Die！");
                    return;
                }

                HOperatorSet.WriteImage(iniObjects.Image, "tiff", 0, IniDirectory + "ReferenceImage.tiff");
                HOperatorSet.WriteTuple(iniParameters.ImagePath, IniDirectory + "ImagePath.tup");
                HOperatorSet.WriteTuple(iniParameters.TrainningImagesDirectory, IniDirectory + "TrainningImagesDirectory.tup");
                HOperatorSet.WriteTuple((int)(iniParameters.UserRegionForCutOutIndex), IniDirectory + "UserRegionForCutOutIndex.tup");
                HOperatorSet.WriteTuple(iniParameters.DieImageRowOffset, IniDirectory + "DieImageRowOffset.tup");
                HOperatorSet.WriteTuple(iniParameters.DieImageColumnOffset, IniDirectory + "DieImageColumnOffset.tup");
                SaveHelper.WriteRegion(DieUserRegions, IniDirectory, "CoarseReference");
                HOperatorSet.WriteRegion((createReference as CreateReference).SelectedUserRegion?.CalculateRegion, IniDirectory + "DieReference.reg");

                //1121-lht
                //HOperatorSet.ReadTuple(IniDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
                HOperatorSet.WriteTuple(iniParameters.ImageCountChannels, IniDirectory + "ImageChannelnumber.tup");

                /* 功能拓展
                if (iniParameters.ImageCountChannels == 3)
                {
                    HOperatorSet.WriteImage(iniObjects.ImageR, "tiff", 0, ChannelImageDirectory + "ImageR.tiff");
                    HOperatorSet.WriteImage(iniObjects.ImageG, "tiff", 0, ChannelImageDirectory + "ImageG.tiff");
                    HOperatorSet.WriteImage(iniObjects.ImageB, "tiff", 0, ChannelImageDirectory + "ImageB.tiff");
                }
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"缺少全局数据！");
            }
        }

        //加载
        public async void LoadXML(string xmlPath) //async
        {
            await Task.Run(() =>
            {
                while (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff)
                {
                    Thread.Sleep(500);
                }
            });

            try
            {
                XElement root = XElement.Load(xmlPath);
                XMLHelper.ReadParameters(root, iniParameters);
                DieUserRegions.Clear();
                XMLHelper.ReadRegion(root, DieUserRegions, "Die_Regions", iniParameters.DieImageRowOffset, iniParameters.DieImageColumnOffset);
                if (iniParameters.ImageIndex == 0)
                {
                    iniParameters.ImageIndex = 1;
                }

                if (!File.Exists($"{IniDirectory}ReferenceImage.tiff"))
                {
                    MessageBox.Show("全局数据没有参考图像！");
                    return;
                }
                if (iniParameters.UserRegionForCutOutIndex == -1)
                {
                    MessageBox.Show("全局数据没有参考Die！");
                    return;
                }
                if (!File.Exists(IniDirectory + "TrainningImagesDirectory.tup"))
                {
                    MessageBox.Show("提示：训练图集文件不存在！");
                }

                HOperatorSet.ReadTuple(IniDirectory + "TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
                //GoldenModelParameter.TrainningImagesDirectory = TrainningImagesDirectoryTemp;
                if (!Directory.Exists(TrainningImagesDirectoryTemp))
                {
                    MessageBox.Show("提示：训练图集不存在！");
                }

                (createReference as CreateReference).LoadImage();
                (createReference as CreateReference).SelectedUserRegion = DieUserRegions.Where(u => u.Index == (int)iniParameters.UserRegionForCutOutIndex).FirstOrDefault();
                (createReference as CreateReference).DieCutOut(false);
                (createReference as CreateReference).Initial();

                //1120               
                iniParameters.ChannelNames.Clear();
                for (int i = 0; i < iniParameters.ImageCountChannels; i++)
                {
                    ChannelName tmp_name = new ChannelName();
                    tmp_name.Name = (i + 1).ToString();
                    iniParameters.ChannelNames.Add(tmp_name);
                }
                (createReference as CreateReference).SwitchImageChannelIndex = iniParameters.ImageIndex - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Dispose()
        {
            //htWindow.Dispose();
            (Content as Page_CreateRecipe).DataContext = null;
            (Content as Page_CreateRecipe).Close();
            Content = null;
            (createReference as CreateReference).OnSaveXML -= SaveXML;
            createReference.Dispose();
            //inspectNode.Dispose();
            Procedures = null;
        }
    }
}
