using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LFAOIRecipe
{
    public class AroundBondRegionModelInspectParameter : ViewModelBase, IParameter
    {
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public HTuple InspectItemNum { get; set; } = new HTuple();

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

        private int? userRegionForCutOutIndex = null;
        public int? UserRegionForCutOutIndex
        {
            get => userRegionForCutOutIndex;
            set => OnPropertyChanged(ref userRegionForCutOutIndex, value);
        }

        public double DieImageRowOffset { get; set; }

        public double DieImageColumnOffset { get; set; }

        private int imageAroundBondRegVerifyChannelIndex = 0;
        public int ImageAroundBondRegVerifyChannelIndex
        {
            get => imageAroundBondRegVerifyChannelIndex;
            set => OnPropertyChanged(ref imageAroundBondRegVerifyChannelIndex, value);
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
    }
}
