using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    class EpoxyParameter : ViewModelBase, IParameter
    {
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

        private int imageCountChannels;
        /// <summary>
        /// 图像通道数量
        /// </summary>
        public int ImageCountChannels
        {
            get => imageCountChannels;
            set => OnPropertyChanged(ref imageCountChannels, value);
        }

        private int imageChannelIndex;
        public int ImageChannelIndex
        {
            get => imageChannelIndex;
            set => OnPropertyChanged(ref imageChannelIndex, value);
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

        private int? userRegionForCutOutIndex=null;
        public int? UserRegionForCutOutIndex
        {
            get => userRegionForCutOutIndex;
            set => OnPropertyChanged(ref userRegionForCutOutIndex, value);
        }

        private int currentTrainSet = 50;
        /// <summary>
        /// 最大训练图集数量
        /// </summary>
        public int CurrentTrainSet
        {
            get => currentTrainSet;
            set => OnPropertyChanged(ref currentTrainSet, value);
        }

        public string ModelIdPath { get; set; } = string.Empty;

        public double DieImageRowOffset { get; set; }

        public double DieImageColumnOffset { get; set; }
    }
}
