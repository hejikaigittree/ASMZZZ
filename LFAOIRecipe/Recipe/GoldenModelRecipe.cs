using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using HalconDotNet;
using System.Threading;
using System.Windows.Threading;

namespace LFAOIRecipe
{
    abstract class GoldenModelRecipe : ProcedureControl, IRecipe
    {
        public abstract string DisplayName { get; }

        public object Content { get; protected set;}

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;
        
        public string ReferenceDirectory { get; set; }=$"{FilePath.ProductDirectory}\\Recipe\\Reference\\";

        private string ModelsFile => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\");

        private string RecipeFile => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\");

        protected abstract string RecipeDirectory { get;}

        protected abstract string ModelsRecipeDirectory { get; } 

        protected HTHalControlWPF htWindow;

        public abstract string XmlPath { get; }

        public abstract int RecipeIndex { get; }

        public abstract string RecipeName { get; }

        /// <summary>
        /// Die区域
        /// </summary>
        protected ObservableCollection<UserRegion> dieUserRegions = new ObservableCollection<UserRegion>();

        /// <summary>
        /// 定位模板区
        /// </summary>
        protected ObservableCollection<UserRegion> matchUserRegions = new ObservableCollection<UserRegion>();

        /// <summary>
        /// 检测区
        /// </summary>
        protected ObservableCollection<UserRegion> inspectUserRegions = new ObservableCollection<UserRegion>();

        /// <summary>
        /// 免检区
        /// </summary>
        protected ObservableCollection<UserRegion> rejectUserRegions = new ObservableCollection<UserRegion>();

        /// <summary>
        /// 重点检测区
        /// </summary>
        protected ObservableCollection<UserRegion> subUserRegions = new ObservableCollection<UserRegion>();

        //12-05
        protected ObservableCollection<MatchRegionsGroup> MatchRegionsGroups = new ObservableCollection<MatchRegionsGroup>();
        //

        public ObservableCollection<UserRegion> FrameUserRegions { get; private set; } = new ObservableCollection<UserRegion>();

        protected GoldenModelParameter goldenModelParameter = new GoldenModelParameter();

        protected FrameLocateInspectParameter frameLocateInspectParameter = new FrameLocateInspectParameter();

        protected GoldenModelInspectParameter goldenModelInspectParameter = new GoldenModelInspectParameter();

        protected GoldenModelObject goldenModelObject = new GoldenModelObject();

        #region 流程模块
        protected IProcedure cutOutDie;
        protected IProcedure cutOutDieFrame;
        protected IProcedure addMatchRegionFrame;
        protected IProcedure addMatchRegion;
        protected IProcedure addInspectRegion;
        protected IProcedure addRejectRegion;
        protected IProcedure addSubRegion;
        protected IProcedure createGoldenModel;
        protected IProcedure createPositionModel;
        protected IProcedure goldenModelInspectVerify;
        protected IProcedure frameLocateInspectVerify;
        #endregion

        public GoldenModelRecipe()
        {
            Content = new Page_CreateRecipe();
            htWindow = (Content as Page_CreateRecipe).htWindow;           
        }

        protected IEnumerable<HObject> DieRegions => dieUserRegions.Select(u => u.DisplayRegion);//修改

        protected IEnumerable<HObject> MatchRegions => matchUserRegions.Select(u => u.DisplayRegion);

        protected IEnumerable<HObject> InspectRegions => inspectUserRegions.Select(u => u.DisplayRegion);

        protected IEnumerable<HObject> RejectRegions => rejectUserRegions.Select(u => u.DisplayRegion);

        protected IEnumerable<HObject> SubRegions => subUserRegions.Select(u => u.DisplayRegion);

        public void InitialProcedures()
        {
            cutOutDie = new CutOutDie(htWindow,
                                      ReferenceDirectory,
                                      goldenModelParameter,
                                      goldenModelObject,
                                      dieUserRegions);
            (cutOutDie as CutOutDie).OnResetDieRegion += OnResetDieRegion;

            cutOutDieFrame = new CutOutDieFrame(htWindow,
                                                ReferenceDirectory,
                                                goldenModelParameter,
                                                goldenModelObject,
                                                dieUserRegions);
            (cutOutDieFrame as CutOutDieFrame).OnResetDieRegion += OnResetDieRegion;

            addMatchRegionFrame = new AddMatchRegionFrame(htWindow,
                                                          goldenModelParameter,
                                                          goldenModelObject,
                                                          MatchRegionsGroups,
                                                          FrameUserRegions);

            addMatchRegion = new AddMatchRegion(htWindow,
                                                goldenModelParameter,
                                                goldenModelObject,
                                                matchUserRegions);

            addInspectRegion = new AddInspectRegion(htWindow,
                                                    RecipeFile,
                                                    ModelsFile,
                                                    goldenModelParameter,
                                                    goldenModelObject,
                                                    inspectUserRegions);

            addRejectRegion = new AddRejectRegion(htWindow,
                                                  RecipeFile,
                                                  ModelsFile,
                                                  goldenModelParameter,
                                                  goldenModelObject,
                                                  rejectUserRegions,
                                                  inspectUserRegions);

            addSubRegion = new AddSubRegion(htWindow,
                                            RecipeFile,
                                            ModelsFile,
                                            goldenModelParameter,
                                            goldenModelObject,
                                            subUserRegions,
                                            inspectUserRegions);//

            createPositionModel = new CreatePositionModel(htWindow,
                                                          goldenModelParameter,
                                                          goldenModelObject,
                                                          MatchRegionsGroups,
                                                          RecipeDirectory);

            createGoldenModel = new CreateGoldenModel(htWindow,
                                            ModelsFile,
                                            RecipeFile,
                                            goldenModelParameter,
                                            goldenModelInspectParameter,
                                            goldenModelObject,
                                            dieUserRegions,
                                            matchUserRegions,
                                            inspectUserRegions,
                                            rejectUserRegions,
                                            subUserRegions,
                                            RecipeDirectory,
                                            ReferenceDirectory);

            goldenModelInspectVerify = new GoldenModelInspectVerify(htWindow,
                                                                    ModelsFile,
                                                                    RecipeFile,
                                                                    ReferenceDirectory,
                                                                    goldenModelObject,
                                                                    goldenModelParameter,
                                                                    goldenModelInspectParameter,
                                                                    dieUserRegions,
                                                                    matchUserRegions,
                                                                    inspectUserRegions,
                                                                    rejectUserRegions,
                                                                    subUserRegions,
                                                                    RecipeDirectory,
                                                                    ModelsRecipeDirectory);

            frameLocateInspectVerify = new FrameLocateInspectVerify(htWindow,
                                                                  goldenModelObject,
                                                                  goldenModelParameter,
                                                                  frameLocateInspectParameter,
                                                                  dieUserRegions,
                                                                  MatchRegionsGroups,
                                                                  FrameUserRegions,
                                                                  RecipeDirectory);

            // 0125
            htWindow.useGoldenModelInspectVerify(goldenModelInspectVerify as GoldenModelInspectVerify);

            //(goldenModelInspectVerify as GoldenModelInspectVerify).OnSaveXML += SaveXML;   //改
        }

        protected void OnResetDieRegion()
        {
            goldenModelObject.DieImageR = null;
            goldenModelObject.DieImageG = null;
            goldenModelObject.DieImageB = null;
            matchUserRegions.Clear();
            inspectUserRegions.Clear();
            rejectUserRegions.Clear();
            subUserRegions.Clear();
        }

        public abstract void SaveXML();

        public abstract void LoadXML(string xmlPath);

        public virtual void Dispose()
        {
            (cutOutDie as CutOutDie).OnResetDieRegion -= OnResetDieRegion;
            (goldenModelInspectVerify as GoldenModelInspectVerify).OnSaveXML -= SaveXML;
            htWindow.Dispose();
            (Content as Page_CreateRecipe).DataContext = null;
            (Content as Page_CreateRecipe).Close();
            Content = null;
            Procedures = null;
            cutOutDieFrame.Dispose();
            cutOutDie.Dispose();
            addMatchRegion.Dispose();
            addMatchRegionFrame.Dispose();
            addInspectRegion.Dispose();
            addRejectRegion.Dispose();
            addSubRegion.Dispose();
            createPositionModel.Dispose();
            createGoldenModel.Dispose();
            frameLocateInspectVerify.Dispose();
            goldenModelInspectVerify.Dispose();
        }
    }
}
