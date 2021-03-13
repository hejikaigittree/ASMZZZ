using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LFAOIRecipe
{
    public class IniParameters : ViewModelBase,IParameter
    {
        //1120
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        private int imageIndex = 0;
        /// <summary>
        /// 图像通道索引值
        /// </summary>
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
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

        private string trainningImagesDirectory = string.Empty;
        /// <summary>
        /// 训练图集
        /// </summary>
        public string TrainningImagesDirectory
        {
            get => trainningImagesDirectory;
            set => OnPropertyChanged(ref trainningImagesDirectory, value);
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

        private int userRegionForCutOutIndex=-1;
        public int UserRegionForCutOutIndex
        {
            get => userRegionForCutOutIndex;
            set => OnPropertyChanged(ref userRegionForCutOutIndex, value);
        }

        public double DieImageRowOffset { get; set; }

        public double DieImageColumnOffset { get; set; }

        // 当前FovName
        public string CurFovName { get; set; } = string.Empty;

        public string IniDirectory { get; set; }

        public bool IsInspectNodeVerify = false;
    }


   public class ChannelName // : ViewModelBase
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                //OnPropertyChanged();
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                //OnPropertyChanged();
            }
        }

    }
}
