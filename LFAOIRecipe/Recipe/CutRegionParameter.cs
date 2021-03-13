using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    class CutRegionParameter : ViewModelBase, IParameter
    {
        public HTuple InspectItemNum { get; set; } = new HTuple();

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

        private double[] cutRegionParameters = new double[] { };
        public double[] CutRegionParameters
        {
            get => cutRegionParameters;
            set => OnPropertyChanged(ref cutRegionParameters, value);
        }

        private int? userRegionForCutOutIndex = null;
        public int? UserRegionForCutOutIndex
        {
            get => userRegionForCutOutIndex;
            set => OnPropertyChanged(ref userRegionForCutOutIndex, value);
        }

        public double DieImageRowOffset { get; set; }

        public double DieImageColumnOffset { get; set; }
    }
}
