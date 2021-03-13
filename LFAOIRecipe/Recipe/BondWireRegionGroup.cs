using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace LFAOIRecipe
{
    public class BondWireRegionGroup : ViewModelBase
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        private UserRegion bond2UserRegion;
        public UserRegion Bond2UserRegion
        {
            get => bond2UserRegion;
            set => OnPropertyChanged(ref bond2UserRegion, value);
        }

        private UserRegion wireUserRegion;
        public UserRegion WireUserRegion
        {
            get => wireUserRegion;
            set => OnPropertyChanged(ref wireUserRegion, value);
        }

        private int bond2_BallNums = 1;
        /// <summary>
        /// 区域中焊点的数量
        /// </summary>
        public int Bond2_BallNums
        {
            get => bond2_BallNums;
            set => OnPropertyChanged(ref bond2_BallNums, value);
        }

        //public WireRegionWithPara Parameter { get; set; }  // 改
    }
}
