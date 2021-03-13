using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFAOIRecipe
{
    public class GoldenModelParameter : ViewModelBase, IParameter
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        private int onRecipesIndex = -1;
        public int OnRecipesIndex
        {
            get => onRecipesIndex;
            set => OnPropertyChanged(ref onRecipesIndex, value);
        }

        private string[] onRecipesIndexs = new string[] { };
        public string[] OnRecipesIndexs
        {
            get => onRecipesIndexs;
            set => OnPropertyChanged(ref onRecipesIndexs, value);
        }

        private int onRecipesIndex1 = -1;
        public int OnRecipesIndex1
        {
            get => onRecipesIndex1;
            set => OnPropertyChanged(ref onRecipesIndex1, value);
        }

        private string[] onRecipesIndexs1 = new string[] { };
        public string[] OnRecipesIndexs1
        {
            get => onRecipesIndexs1;
            set => OnPropertyChanged(ref onRecipesIndexs1, value);
        }

        private string imagePath = string.Empty;
        /// <summary>
        /// 载入图像
        /// </summary>
        public string ImagePath
        {
            get => imagePath;
            set => OnPropertyChanged(ref imagePath, value);
        }

        private int imageCountChannels;
        /// <summary>
        /// 图像通道数量
        /// </summary>
        public int ImageCountChannels
        {
            get => imageCountChannels;
            set => OnPropertyChanged(ref imageCountChannels, value);
        }
        
        private string trainningImagesDirectory = string.Empty;
        /// <summary>
        /// 训练图集
        /// </summary>
        public string TrainningImagesDirectory
        {
            get => trainningImagesDirectory;
            set => OnPropertyChanged(ref trainningImagesDirectory, value);
        }

        private string mdelsReferencePath = string.Empty;
        /// <summary>
        /// 参考数据路径
        /// </summary>
        public string ReferencePath
        {
            get => mdelsReferencePath;
            set => OnPropertyChanged(ref mdelsReferencePath, value);
        }

        private int? userRegionForCutOutIndex=null;
        public int? UserRegionForCutOutIndex
        {
            get => userRegionForCutOutIndex;
            set => OnPropertyChanged(ref userRegionForCutOutIndex, value);
        }

        private int modelType;
        /// <summary>
        /// 模板类型
        /// </summary>
        public int ModelType
        {
            get => modelType;
            set => OnPropertyChanged(ref modelType, value);
        }

        private bool isMultiModelMode = false;
        /// <summary>
        /// 创建黄金模板是否多模板模式
        /// </summary>
        public bool IsMultiModelMode
        {
            get => isMultiModelMode;
            set => OnPropertyChanged(ref isMultiModelMode, value);
        }

        private bool isMultiModelPosMode=false;//
        /// <summary>
        /// 创建定位模板是否多模板模式
        /// </summary>
        public bool IsMultiModelPosMode//
        {
            get => isMultiModelPosMode;
            set => OnPropertyChanged(ref isMultiModelPosMode, value);
        }

        private double minMatchScore=0.8;//
        /// <summary>
        /// 模板匹配最小分数的设置
        /// </summary>
        public double MinMatchScore//
        {
            get => minMatchScore;
            set => OnPropertyChanged(ref minMatchScore, value);
        }
        
        private int imageChannelIndex = 0;
        /// <summary>
        /// 图像三通道索引值
        /// </summary>
        public int ImageChannelIndex
        {
            get => imageChannelIndex;
            set => OnPropertyChanged(ref imageChannelIndex, value);
        }

        private int imageGoldChannelIndex = 0;
        /// <summary>
        /// 黄金模板图像三通道索引值
        /// </summary>
        public int ImageGoldChannelIndex
        {
            get => imageGoldChannelIndex;
            set => OnPropertyChanged(ref imageGoldChannelIndex, value);
        }

        private int minTrainSet = 5;
        /// <summary>
        /// 最小训练图集数量
        /// </summary>
        public int MinTrainSet
        {
            get => minTrainSet;
            set => OnPropertyChanged(ref minTrainSet, value);
        }

        private int currentTrainSet = 0;
        /// <summary>
        /// 最大训练图集数量
        /// </summary>
        public int CurrentTrainSet
        {
            get => currentTrainSet;
            set => OnPropertyChanged(ref currentTrainSet, value);
        }

        private bool isRefine = true;
        /// <summary>
        /// 是否精炼图像
        /// </summary>
        public bool IsRefine
        {
            get => isRefine;
            set => OnPropertyChanged(ref isRefine, value);
        }

        private double refineThresh=3;
        /// <summary>
        /// 精炼阈值
        /// </summary>
        public double RefineThresh
        {
            get => refineThresh;
            set => OnPropertyChanged(ref refineThresh, value);
        }

        private bool isGoldenMatchModel = false;
        /// <summary>
        /// 是否用黄金比对模板重新生成目标定位匹配模板
        /// </summary>
        public bool IsGoldenMatchModel
        {
            get => isGoldenMatchModel;
            set => OnPropertyChanged(ref isGoldenMatchModel, value);
        }

        public string ModelIdPath { get; set; } = string.Empty;

        /// <summary>
        /// 定位模板路径
        /// </summary>
        public string PosModelIdPath { get; set; } = string.Empty;//

        public string MeanImagePath { get; set; } = string.Empty;

        public string StdImagePath { get; set; } = string.Empty;

        public string LightImagePath { get; set; } = string.Empty;

        public string DarkImagePath { get; set; } = string.Empty;

        public double DieImageRowOffset { get; set; }

        public double DieImageColumnOffset { get; set; }

        /// <summary>
        /// 选择图像在训练文件夹中的索引值，以1开始
        /// </summary>
        /*
        private int imageListIndexBegin;//
        public int ImageListIndexBegin//
        {
            get => imageListIndexBegin;
            set => OnPropertyChanged(ref imageListIndexBegin, value);
        }
        */

        private double angleStart = -22.5;//
        /// <summary>
        /// 图像最小旋转  传入的是角度 
        /// </summary>
        public double AngleStart//
        {
            get => angleStart;
            set => OnPropertyChanged(ref angleStart, value);
        }

        private double angleExt = 45;// ncc 默认45度
        /// <summary>
        /// 图像旋转角度范围 大于等于0  传入的是角度 
        /// </summary>
        public double AngleExt//
        {
            get => angleExt;
            set => OnPropertyChanged(ref angleExt, value);
        }

        private int matchNum = 1;//
        /// <summary>
        /// 
        /// </summary>
        public int MatchNum//
        {
            get => matchNum;
            set => OnPropertyChanged(ref matchNum, value);
        }

        private string verifyImagesDirectory = string.Empty;
        /// <summary>
        /// 检测图集
        /// </summary>
        public string VerifyImagesDirectory
        {
            get => verifyImagesDirectory;
            set => OnPropertyChanged(ref verifyImagesDirectory, value);
        }

        private int currentVerifySet;
        /// <summary>
        /// 当前检测图集
        /// </summary>
        public int CurrentVerifySet
        {
            get => currentVerifySet;
            set => OnPropertyChanged(ref currentVerifySet, value);
        }

        // 当前FovName
        public string CurFovName { get; set; } = string.Empty;

        public bool IsInspectVerify = false;
    }
}
