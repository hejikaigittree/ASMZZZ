using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class Bond2ModelParameter : ViewModelBase, IParameter
    {
        //1122
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

        private int? userRegionForCutOutIndex = null;
        public int? UserRegionForCutOutIndex
        {
            get => userRegionForCutOutIndex;
            set => OnPropertyChanged(ref userRegionForCutOutIndex, value);
        }

        /// <summary>
        /// 加载图像
        /// </summary>
        private string imagePath = string.Empty;
        public string ImagePath
        {
            get => imagePath;
            set => OnPropertyChanged(ref imagePath, value);
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

        private int imageChannelIndex;
        public int ImageChannelIndex
        {
            get => imageChannelIndex;
            set => OnPropertyChanged(ref imageChannelIndex, value);
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

        public string ModelIdPath { get; set; } = string.Empty;

        private int modelType=0;
        /// <summary>
        /// 模板类型
        /// </summary>
        public int ModelType
        {
            get => modelType;
            set => OnPropertyChanged(ref modelType, value);
        }

        //private bool isPreProcess = true; 删除原有的
        private bool isPreProcess = false;
        /// <summary>
        /// 预处理
        /// </summary>
        public bool IsPreProcess
        {
            get => isPreProcess;
            set => OnPropertyChanged(ref isPreProcess, value);
        }

        private double gamma=0;
        /// <summary>
        /// 预处理时使用的Gamma变换系数
        /// </summary>
        public double Gamma
        {
            get => gamma;
            set => OnPropertyChanged(ref gamma, value);
        }

        private bool isPickUp = false;
        public bool IsPickUp
        {
            get => isPickUp;
            set => OnPropertyChanged(ref isPickUp, value);
        }


        public double DieImageRowOffset { get; set; }

        public double DieImageColumnOffset { get; set; }

        // 当前FovName
        public string CurFovName { get; set; } = string.Empty;
    }
}
