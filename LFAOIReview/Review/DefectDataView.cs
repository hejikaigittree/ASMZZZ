using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    internal class DefectDataView : DefectDataDb, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _DisplayIndex;
        public int DisplayIndex
        {
            get { return _DisplayIndex; }
            set
            {
                if (_DisplayIndex != value)
                {
                    _DisplayIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _DefectType;
        public string DefectType
        {
            get { return _DefectType; }
            set
            {
                if (_DefectType != value)
                {
                    _DefectType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _DieBelongTo;
        public string DieBelongTo
        {
            get { return _DieBelongTo; }
            set
            {
                if (_DieBelongTo != value)
                {
                    _DieBelongTo = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _Result;
        public new int Result
        {
            get { return _Result; }
            set
            {
                if (_Result != value)
                {
                    _Result = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _DisplayErrorDetail;
        public string DisplayErrorDetail
        {
            get { return _DisplayErrorDetail; }
            set
            {
                if (_DisplayErrorDetail != value)
                {
                    _DisplayErrorDetail = value;
                    OnPropertyChanged();
                }
            }
        }

        public int InspectionDataListIndex { get; set; }
    }
}
