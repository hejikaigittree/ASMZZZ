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
    public  class WireAutoRegionGroup : ViewModelBase
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        //选取制作Wire模板起始焊点索引号
        private int selectModelNumber = -1;
        public int SelectModelNumber
        {
            get => selectModelNumber;
            set => OnPropertyChanged(ref selectModelNumber, value);
        }

        public WireAutoRegionGroup()
        {
            lineModelRegions = new ObservableCollection<UserRegion>();
        }

        private ObservableCollection<UserRegion> lineModelRegions;
        public ObservableCollection<UserRegion> LineModelRegions
        {
            get => lineModelRegions;
            set => OnPropertyChanged(ref lineModelRegions, value);
        }

        private UserRegion modelStartUserRegions;
        public UserRegion ModelStartUserRegions
        {
            get => modelStartUserRegions;
            set => OnPropertyChanged(ref modelStartUserRegions, value);
        }

        private UserRegion modelStopUserRegions;
        public UserRegion ModelStopUserRegions
        {
            get => modelStopUserRegions;
            set => OnPropertyChanged(ref modelStopUserRegions, value);
        }

        private UserRegion refLineModelRegions;
        public UserRegion RefLineModelRegions
        {
            get => refLineModelRegions;
            set => OnPropertyChanged(ref refLineModelRegions, value);
        }
    }
}
