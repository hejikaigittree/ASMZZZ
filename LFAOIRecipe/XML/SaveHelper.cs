using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using HalconDotNet;

namespace LFAOIRecipe
{
    class SaveHelper
    {
        public static void AddRegionsIsEnableTup(IEnumerable<UserRegion> userRegions, string name, string recipeDirectory, string modelIndex = "")//
        {
            HTuple SaveTemp = new HTuple();
            foreach (var userRegion in userRegions)
            {
                SaveTemp = SaveTemp.TupleConcat(userRegion.IsEnable ? 1 : 0);
            }
            if (modelIndex == "")
            {
                HOperatorSet.WriteTuple(SaveTemp, $"{recipeDirectory}" + name + "_" + "IsEnable.tup");
            }
            else if (modelIndex != "")
            {
                HOperatorSet.WriteTuple(SaveTemp, $"{recipeDirectory}" + name + modelIndex+ "_" + "IsEnable.tup");
            }
        }

        public static void WriteRegionIsEnable(IEnumerable<UserRegion> userRegions, string recipeDirectory,string name, string modelIndex = "")
        {
            if (userRegions.Count() == 0) return;
            if(modelIndex=="")
            {
                HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(userRegions), recipeDirectory + name + ".reg");
                SaveHelper.AddRegionsIsEnableTup(userRegions, name, recipeDirectory);
            }
            else if(modelIndex!= "")
            {
                HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(userRegions), $"{recipeDirectory}"+ name + modelIndex+ ".reg"); 
                SaveHelper.AddRegionsIsEnableTup(userRegions, name, recipeDirectory, modelIndex);
            }
        }

        public static void WriteRegion(IEnumerable<UserRegion> userRegions, string recipeDirectory, string name, string modelIndex = "")
        {
            if (userRegions.Count() == 0) return;
            if (modelIndex == "")
            {
                HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(userRegions), recipeDirectory + name + ".reg");
            }
            else if (modelIndex != "")
            {
                HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(userRegions), $"{recipeDirectory}" + name + modelIndex + ".reg");
            }
        }

        public static HObject BondRegionOffset(UserRegion userRegion, RegionType regionType, double dieImageRowOffset, double dieImageColumnOffset)
        {
            HOperatorSet.GenRectangle1(out HObject calculateRegion, userRegion.RegionParameters[0]+ dieImageRowOffset, userRegion.RegionParameters[1]+ dieImageColumnOffset, userRegion.RegionParameters[2]+ dieImageRowOffset, userRegion.RegionParameters[3]+ dieImageColumnOffset);
            return calculateRegion;
        }
    }
}
