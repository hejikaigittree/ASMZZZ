using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondMeasureParameter : ViewModelBase, IParameter
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

        /// <summary>
        /// 加载图像
        /// </summary>
        private string imagePath = string.Empty;
        public string ImagePath
        {
            get => imagePath;
            set => OnPropertyChanged(ref imagePath, value);
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

        private string verifyImagesDirectory = string.Empty;
        /// <summary>
        /// 检测图集
        /// </summary>
        public string VerifyImagesDirectory
        {
            get => verifyImagesDirectory;
            set => OnPropertyChanged(ref verifyImagesDirectory, value);
        }

        private int? userRegionForCutOutIndex = null;
        public int? UserRegionForCutOutIndex
        {
            get => userRegionForCutOutIndex;
            set => OnPropertyChanged(ref userRegionForCutOutIndex, value);
        }

        public double DieImageRowOffset { get; set; }

        public double DieImageColumnOffset { get; set; }

        private bool isPickUp = false;
        public bool IsPickUp
        {
            get => isPickUp;
            set => OnPropertyChanged(ref isPickUp, value);
        }

        // 当前FovName
        public string CurFovName { get; set; } = string.Empty;


        //add by wj 2020-12-27
        private double[] genPadRegionsSize = { 20, 20 };
        /// <summary>
        /// 设置自动生成起始焊点、结束焊点半径
        /// </summary>
        public double[] GenPadRegionsSize
        {
            get => genPadRegionsSize;
            set => OnPropertyChanged(ref genPadRegionsSize, value);
        }


        //自动生成焊点检测区域 拾取功能 2021-01-05
        //private bool isVerifyRegionPickUp = false;
        //public bool IsVerifyRegionPickUp
        //{
        //    get => isVerifyRegionPickUp;
        //    set => OnPropertyChanged(ref isVerifyRegionPickUp, value);
        //}

        //加载编辑焊盘区域 拾取功能
        private bool isPadRegionPickUp = false;
        public bool IsPadRegionPickUp
        {
            get => isPadRegionPickUp;
            set => OnPropertyChanged(ref isPadRegionPickUp, value);
        }
        //-----------------------------

        //焊盘区域或检测区域方法选择索引

        private int algoParameterIndex = 1;
        public int AlgoParameterIndex
        {
            get => algoParameterIndex;
            set => OnPropertyChanged(ref algoParameterIndex, value);
        }

    }
}
