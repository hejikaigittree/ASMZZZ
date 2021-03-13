using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondMeasureVerifyParameter : ViewModelBase, IParameter
    {
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

        private int imageChannelIndex = 0;
        /// <summary>
        /// 图像三通道索引值
        /// </summary>
        public int ImageChannelIndex
        {
            get => imageChannelIndex;
            set => OnPropertyChanged(ref imageChannelIndex, value);
        }
    }
}
