using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    class MappingViewModel : ViewModelBase
    {
        private string _SelectionText;
        /// <summary>
        /// 选择项信息
        /// </summary>
        public string SelectionText
        {
            get { return _SelectionText; }
            set
            {
                if (_SelectionText != value)
                {
                    _SelectionText = value;
                    OnPropertyChanged();
                }
            }
        }

        public double BottomBarHeight { get; set; } = 20;

        public void ChangeSelectedText(int rowIndex, int columnIndex, InspectionResult inspectionResult)
        {
            SelectionText = string.Format("行{0} 列{1} {2}", rowIndex, columnIndex, InspectionResultsConverter.ToString(inspectionResult));
        }
    }
}
