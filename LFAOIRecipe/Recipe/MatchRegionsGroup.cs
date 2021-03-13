using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace LFAOIRecipe
{
    class MatchRegionsGroup : ViewModelBase
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

        public MatchRegionsGroup()
        {
            MatchUserRegions = new ObservableCollection<UserRegion>();
        }

        private ObservableCollection<UserRegion> matchUserRegions;
        public ObservableCollection<UserRegion> MatchUserRegions
        {
            get => matchUserRegions;
            set => OnPropertyChanged(ref matchUserRegions, value);
        }
    }

}
