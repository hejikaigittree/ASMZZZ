using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace LFAOIRecipe
{
    class WireRegionsGroup : ViewModelBase
    {
        private int intdex;
        public int Index
        {
            get => intdex;
            set => OnPropertyChanged(ref intdex, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public WireRegionsGroup()
        {
            lineUserRegions = new ObservableCollection<UserRegion>();
        }

        private UserRegion bondOnFrameUserRegions;
        public UserRegion BondOnFrameUserRegions
        {
            get => bondOnFrameUserRegions;
            set => OnPropertyChanged(ref bondOnFrameUserRegions, value);
        }

        private UserRegion bondOnICUserRegions;
        public UserRegion BondOnICUserRegions
        {
            get => bondOnICUserRegions;
            set => OnPropertyChanged(ref bondOnICUserRegions, value);
        }

        private UserRegion refLineUserRegions;
        public UserRegion RefLineUserRegions
        {
            get => refLineUserRegions;
            set => OnPropertyChanged(ref refLineUserRegions, value);
        }

        private ObservableCollection<UserRegion>  lineUserRegions;
        public  ObservableCollection<UserRegion>  LineUserRegions
        {
            get => lineUserRegions;
            set => OnPropertyChanged(ref lineUserRegions, value);
        }

        private UserRegion leadWireOnFrameUserRegions;
        public UserRegion LeadWireOnFrameUserRegions
        {
            get => leadWireOnFrameUserRegions;
            set => OnPropertyChanged(ref leadWireOnFrameUserRegions, value);
        }

        private UserRegion leadWireOnICUserRegions;
        public UserRegion LeadWireOnICUserRegions
        {
            get => leadWireOnICUserRegions;
            set => OnPropertyChanged(ref leadWireOnICUserRegions, value);
        }


        //public BondWireParameter Parameter { get; set; } = new BondWireParameter();

    }
}
