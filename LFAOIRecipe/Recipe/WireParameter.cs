using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class WireParameter : ViewModelBase, IParameter
    {
        //1122
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

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

        /// <summary>
        /// 加载图像
        /// </summary>
        private string imagePath = string.Empty;
        public string ImagePath
        {
            get => imagePath;
            set => OnPropertyChanged(ref imagePath, value);
        }

        private bool isTailInspect = true;
        public bool IsTailInspect
        {
            get => isTailInspect;
            set => OnPropertyChanged(ref isTailInspect, value);
        }

        private bool isWireBatchParasSet;
        public bool IsWireBatchParasSet
        {
            get => isWireBatchParasSet;
            set => OnPropertyChanged(ref isWireBatchParasSet, value);
        }

        private bool isGroupBatchParasSet;
        public bool IsGroupBatchParasSet
        {
            get => isGroupBatchParasSet;
            set => OnPropertyChanged(ref isGroupBatchParasSet, value);
        }

        private bool isEnableStartVirtualBond = false;
        public bool IsEnableStartVirtualBond
        {
            get => isEnableStartVirtualBond;
            set => OnPropertyChanged(ref isEnableStartVirtualBond, value);
        }

        private bool isEnableEndVirtualBond = false;
        public bool IsEnableEndVirtualBond
        {
            get => isEnableEndVirtualBond;
            set => OnPropertyChanged(ref isEnableEndVirtualBond, value);
        }

        private int onStartRecipesIndex = -1;
        public int OnStartRecipesIndex
        {
            get => onStartRecipesIndex;
            set => OnPropertyChanged(ref onStartRecipesIndex, value);
        }

        private int onStopRecipesIndex = -1;
        public int OnStopRecipesIndex
        {
            get => onStopRecipesIndex;
            set => OnPropertyChanged(ref onStopRecipesIndex, value);
        }

        private string[] startBondonRecipesIndexs = new string[] { };
        public string[] StartBondonRecipesIndexs
        {
            get => startBondonRecipesIndexs;
            set => OnPropertyChanged(ref startBondonRecipesIndexs, value);
        }
        //add by wj
        private string[] stopBondonRecipesIndexs = new string[] { };
        public string[] StopBondonRecipesIndexs
        {
            get => stopBondonRecipesIndexs;
            set => OnPropertyChanged(ref stopBondonRecipesIndexs, value);
        }

        //add by wj 2020-10-22
        //private string[] startBondonRecipes = new string[] { };
        //public string[] StartBondonRecipes
        //{
        //    get => startBondonRecipes;
        //    set => OnPropertyChanged(ref startBondonRecipes, value);
        //}
        ////add by wj
        //private string[] stopBondonRecipes = new string[] { };
        //public string[] StopBondonRecipes
        //{
        //    get => stopBondonRecipes;
        //    set => OnPropertyChanged(ref stopBondonRecipes, value);
        //}


        //2020-10-19 add by lht 
        private int[] wireRegModelType = new int[] { };
        public int[] WireRegModelType
        {
            get => wireRegModelType;
            set => OnPropertyChanged(ref wireRegModelType, value);
        }

        //1109 add for pickup sort function
        private int[] index_initial = new int[] { };
        public int[] Index_initial
        {
            get => index_initial;
            set => OnPropertyChanged(ref index_initial, value);
        }

        //1022
        private int[] wireAutoIndex_sorted_start = new int[] { };
        public int[] WireAutoIndex_sorted_start
        {
            get => wireAutoIndex_sorted_start;
            set => OnPropertyChanged(ref wireAutoIndex_sorted_start, value);
        }
        //1022
        private int[] wireAutoIndex_sorted_stop = new int[] { };
        public int[] WireAutoIndex_sorted_stop
        {
            get => wireAutoIndex_sorted_stop;
            set => OnPropertyChanged(ref wireAutoIndex_sorted_stop, value);
        }

        private int? userRegionForCutOutIndex = null;
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

        private int imageIndex = 0;
        /// <summary>
        /// 图像通道索引值
        /// </summary>
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
        }

        /*
        private int verifyImageIndex = 0;
        /// <summary>
        /// 图像通道索引值
        /// </summary>
        public int VerifyImageIndex
        {
            get => verifyImageIndex;
            set => OnPropertyChanged(ref verifyImageIndex, value);
        }
        */

        private int imageCountChannels;
        /// <summary>
        /// 图像通道数量
        /// </summary>
        public int ImageCountChannels
        {
            get => imageCountChannels;
            set => OnPropertyChanged(ref imageCountChannels, value);
        }

        public string ModelIdPath { get; set; } = string.Empty;

        public double DieImageRowOffset { get; set; } = 0;

        public double DieImageColumnOffset { get; set; } = 0;

        private int rectangle2Width = 10;
        /// <summary>
        /// 设置可旋转矩形的宽
        /// </summary>
        public int Rectangle2Width
        {
            get => rectangle2Width;
            set => OnPropertyChanged(ref rectangle2Width, value);
        }

        // 当前FovName  lw
        public string CurFovName { get; set; } = string.Empty;

        //add by wj
        #region 自动生成金线检测区域起始焊点参数
        private int genStartBallSize = 12;
        /// <summary>
        /// 设置自动生成起始焊点、结束焊点半径
        /// </summary>
        public int GenStartBallSize
        {
            get => genStartBallSize;
            set => OnPropertyChanged(ref genStartBallSize, value);
        }
        //顺时针排序
        private int startSortMethod = 0;
        public int StartSortMethod
        {
            get => startSortMethod;
            set => OnPropertyChanged(ref startSortMethod, value);
        }

        private int startFirstSortNumber = 0;
        public int StartFirstSortNumber
        {
            get => startFirstSortNumber;
            set => OnPropertyChanged(ref startFirstSortNumber, value);
        }
        //自动生成焊点区域 拾取功能
        private bool isStartPickUp = false;
        public bool IsStartPickUp
        {
            get => isStartPickUp;
            set => OnPropertyChanged(ref isStartPickUp, value);
        }

        private bool isDrawStartVirtualBall = false;
        public bool IsDrawStartVirtualBall
        {
            get => isDrawStartVirtualBall;
            set => OnPropertyChanged(ref isDrawStartVirtualBall, value);
        }

        /// <summary>
        /// 顺时针逆时针排序重心
        /// </summary>
        private double coreRow = 0.0;
        /// <summary>
        /// 重心行坐标
        /// </summary>
        public double CoreRow
        {
            get => coreRow;
            set => OnPropertyChanged(ref coreRow, value);
        }
        private double coreColumn = 0.0;
        /// <summary>
        /// 重心列坐标
        /// </summary>
        public double CoreColumn
        {
            get => coreColumn;
            set => OnPropertyChanged(ref coreColumn, value);
        }
        #endregion

        //**************************************************
        #region 自动生成金线检测区域结束焊点参数设置
        private int genStopBallSize = 12;
        /// <summary>
        /// 设置自动生成起始焊点、结束焊点半径
        /// </summary>
        public int GenStopBallSize
        {
            get => genStopBallSize;
            set => OnPropertyChanged(ref genStopBallSize, value);
        }
        //顺时针排序
        private int stopSortMethod = 0;
        public int StopSortMethod
        {
            get => stopSortMethod;
            set => OnPropertyChanged(ref stopSortMethod, value);
        }

        private int stopFirstSortNumber = 0;
        public int StopFirstSortNumber
        {
            get => stopFirstSortNumber;
            set => OnPropertyChanged(ref stopFirstSortNumber, value);
        }
        //自动生成结束焊点区域 拾取功能
        private bool isStopPickUp = false;
        public bool IsStopPickUp
        {
            get => isStopPickUp;
            set => OnPropertyChanged(ref isStopPickUp, value);
        }

        private bool isDrawStopVirtualBall = false;
        public bool IsDrawStopVirtualBall
        {
            get => isDrawStopVirtualBall;
            set => OnPropertyChanged(ref isDrawStopVirtualBall, value);
        }
        #endregion

        #region  创建自动生成wire检测区域模板参数

        //焊点连线区域 拾取功能
        private bool isWireRegionPickUp = false;
        public bool IsWireRegionPickUp
        {
            get => isWireRegionPickUp;
            set => OnPropertyChanged(ref isWireRegionPickUp, value);
        }
        //创建wire检测区域拾取功能
        private bool isWirePickUp = false;
        public bool IsWirePickUp
        {
            get => isWirePickUp;
            set => OnPropertyChanged(ref isWirePickUp, value);
        }

        #endregion


    }
}
