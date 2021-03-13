using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;

namespace LFAOIRecipe.Algorithm
{
    class Region
    {
        public static HObject ConcatRegion(UserRegion regions)
        {
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);           
            
                if ( regions.IsEnable)
                {
                    if(regions.IsAccept)
                    {
                        HOperatorSet.ConcatObj(concatRegion, regions.CalculateRegion, out concatRegion);
                    }
                    else
                    {
                        HOperatorSet.Difference(concatRegion, regions.CalculateRegion, out concatRegion);
                    }                    
                }
            
            return concatRegion;
        }

        public static HObject ConcatRegion(UserRegion region1, UserRegion region2)
        {
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);

            if (region2.IsAccept==true)
            {
                HOperatorSet.ConcatObj(region1.CalculateRegion, region2.CalculateRegion, out concatRegion);
            }
            else if(region2.IsAccept == false)
            {
                HOperatorSet.Difference(region1.CalculateRegion, region2.CalculateRegion, out concatRegion);
            }   

            return concatRegion;
        }

        public static HObjectVector ConcatRegion(List<HObject> region)
        {
            HObjectVector hvec_Region = new HObjectVector(1);

            for(int i=0;i<region.Count()-1;i++)
            { 
               
                hvec_Region[i] = new HObjectVector(region[i].CopyObj(1, -1));
            }
            
            return hvec_Region;
        }

        public static HObject ConcatRegion(IEnumerable<HObject> regions)
        {
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);

            foreach (var region in regions)
            {
                if (region != null && region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                }
            }
            return concatRegion;
        }        

        public static HObject ConcatRegion(IEnumerable<UserRegion> regions)//
        {
            HObject concatRegionAccept = null;
            HObject concatRegionDifference = null;
            HOperatorSet.GenEmptyObj(out concatRegionAccept);
            HOperatorSet.GenEmptyObj(out concatRegionDifference);

            foreach (var region in regions)
            {
                if (region.IsEnable)
                {
                    if (region.IsAccept)
                    {
                        HOperatorSet.ConcatObj(concatRegionAccept, region.CalculateRegion, out concatRegionAccept);
                    }
                    else
                    {
                        HOperatorSet.ConcatObj(concatRegionDifference, region.CalculateRegion, out concatRegionDifference);
                    }
                }
            }
            HOperatorSet.Difference(concatRegionAccept, concatRegionDifference, out concatRegionAccept);
            return concatRegionAccept;
        }

        public static HObject ConcatDisplayRegion(IEnumerable<UserRegion> regions)
        {
            HObject concatRegionAccept = null;
            HObject concatRegionDifference = null;
            HOperatorSet.GenEmptyObj(out concatRegionAccept);
            HOperatorSet.GenEmptyObj(out concatRegionDifference);

            foreach (var region in regions)
            {
                if (region.IsEnable)
                {
                    if (region.IsAccept)
                    {
                        HOperatorSet.ConcatObj(concatRegionAccept, region.DisplayRegion, out concatRegionAccept);
                    }
                    else
                    {
                        HOperatorSet.ConcatObj(concatRegionDifference, region.DisplayRegion, out concatRegionDifference);
                    }
                }
            }
            HOperatorSet.Difference(concatRegionAccept, concatRegionDifference, out concatRegionAccept);
            return concatRegionAccept;
        }

        public static HObject Union1Region(IEnumerable<UserRegion> regions)//
        {            
            HOperatorSet.GenEmptyObj(out  HObject concatAcceptRegion);
            HOperatorSet.GenEmptyObj(out HObject concatDifferRegion);
            HOperatorSet.GenEmptyObj(out HObject UnionRegion);

            foreach (var region in regions)
            {
                if (region.IsEnable)
                {
                    if (region.IsAccept)
                    {
                        HOperatorSet.ConcatObj(concatAcceptRegion, region.CalculateRegion, out concatAcceptRegion);
                    }
                    else
                    {
                        HOperatorSet.ConcatObj(concatDifferRegion, region.CalculateRegion, out concatDifferRegion);
                    }
                }
            }
            HOperatorSet.Difference(concatAcceptRegion, concatDifferRegion, out HObject concatRegion);
            HOperatorSet.Union1(concatRegion, out UnionRegion);
            return UnionRegion;
        }

        public static HObject Union1DisplayRegion(HObject region, UserRegion userRegion)//DisplayRegion
        {
            HOperatorSet.GenEmptyObj(out HObject UnionRegion);
            HOperatorSet.GenEmptyObj(out HObject unionRegion);

            if (userRegion.IsEnable)
            {
                if (userRegion.IsAccept)
                {
                    HOperatorSet.ConcatObj(region, userRegion.DisplayRegion, out unionRegion);
                }
                else if (!userRegion.IsAccept)
                {
                    HOperatorSet.Difference(region, userRegion.DisplayRegion, out unionRegion);
                }
            }
            HOperatorSet.Union1(unionRegion, out UnionRegion);
            return UnionRegion;
        }

        public static HObject Union1DisplayRegion(IEnumerable<UserRegion> regions)//DisplayRegion
        {
            HOperatorSet.GenEmptyObj(out HObject concatAcceptRegion);
            HOperatorSet.GenEmptyObj(out HObject concatDifferRegion);
            HOperatorSet.GenEmptyObj(out HObject UnionRegion);

            foreach (var region in regions)
            {
                if (region.IsEnable)
                {
                    if (region.IsAccept)
                    {
                        HOperatorSet.ConcatObj(concatAcceptRegion, region.DisplayRegion, out concatAcceptRegion);
                    }
                    else
                    {
                        HOperatorSet.ConcatObj(concatDifferRegion, region.DisplayRegion, out concatDifferRegion);
                    }
                }
            }
            HOperatorSet.Difference(concatAcceptRegion, concatDifferRegion, out HObject concatRegion);
            HOperatorSet.Union1(concatRegion, out UnionRegion);
            return UnionRegion;
        }       

        /*
        public static HObject Union1DisplayRegion(IEnumerable<UserRegion> regions)
        {
            HOperatorSet.GenEmptyObj(out HObject concatAcceptRegion);
            HOperatorSet.GenEmptyObj(out HObject concatDifferRegion);
            HOperatorSet.GenEmptyObj(out HObject UnionRegion);

            foreach (var region in regions)
            {
                if (region.IsEnable)
                {
                    if (region.IsAccept)
                    {
                        HOperatorSet.ConcatObj(concatAcceptRegion, region.DisplayRegion, out concatAcceptRegion);
                    }
                    else
                    {
                        HOperatorSet.ConcatObj(concatDifferRegion, region.DisplayRegion, out concatDifferRegion);
                    }
                }
            }
            HOperatorSet.Difference(concatAcceptRegion, concatDifferRegion, out HObject concatRegion);
            HOperatorSet.Union1(concatRegion, out UnionRegion);
            return UnionRegion;
        }
        */

        /// <summary>
        /// 获取指定通道的图像
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ImageIndex"></param>
        /// <param name="ChannelIndexImage"></param>
        /// <returns></returns>
        public static HObject GetChannnelImage(HObject Image,int ImageIndex,out HObject ChannelIndexImage)//
        {
            Separat_Image(Image, out HObject concatChunnelImage);
            HOperatorSet.SelectObj(concatChunnelImage, out ChannelIndexImage, ImageIndex + 1);
            return ChannelIndexImage;
        }

        public static HObject GetChannnelImageUpdate(HObject Image, int ImageIndex)
        {
            if (Image == null || ImageIndex < 0) return null;
            HOperatorSet.CountChannels(Image, out HTuple channels);
            if (ImageIndex >= channels) return null;

            HOperatorSet.GenEmptyObj(out HObject ImageReturn);
            if (channels==1)
            {
                ImageReturn = Image;
            }
            else if(channels > 1)
            {
                //1122-lw
                HOperatorSet.AccessChannel(Image, out ImageReturn, ImageIndex + 1);
                //1121-lht
                //ImageReturn = Image;
                //HOperatorSet.GenEmptyObj(out HObject ImageR);
                //HOperatorSet.GenEmptyObj(out HObject ImageG);
                //HOperatorSet.GenEmptyObj(out HObject ImageB);
                //HOperatorSet.GenEmptyObj(out HObject ImageConcact);
                //HOperatorSet.Decompose3(Image, out ImageR, out ImageG, out ImageB);
                //HOperatorSet.ConcatObj(ImageConcact, Image, out ImageConcact);
                //HOperatorSet.ConcatObj(ImageConcact, ImageR, out ImageConcact);
                //HOperatorSet.ConcatObj(ImageConcact, ImageG, out ImageConcact);
                //HOperatorSet.ConcatObj(ImageConcact, ImageB, out ImageConcact);
                //HOperatorSet.SelectObj(ImageConcact, out HObject ChannelImage, ImageIndex+1);
                //ImageReturn = ChannelImage;
                //ImageR.Dispose();
                //ImageG.Dispose();
                //ImageB.Dispose();
                //ImageConcact.Dispose();
            }
            return ImageReturn;
        }

        public static HObject GetChannnelImageConcact(HObject Image)
        {
            if (Image == null) return null;
            HOperatorSet.CountChannels(Image, out HTuple channels);
            HOperatorSet.GenEmptyObj(out HObject ImageReturn);

            if (channels == 1)
            {
                ImageReturn = Image;
            }
            else if (channels > 1)
            {
                // 1122 -lw
                HOperatorSet.ImageToChannels(Image, out ImageReturn);

                //HOperatorSet.GenEmptyObj(out HObject ImageR);
                //HOperatorSet.GenEmptyObj(out HObject ImageG);
                //HOperatorSet.GenEmptyObj(out HObject ImageB);
                //HOperatorSet.GenEmptyObj(out HObject ImageConcact);
                //HOperatorSet.Decompose3(Image, out ImageR, out ImageG, out ImageB);
                //HOperatorSet.ConcatObj(ImageConcact, ImageR, out ImageConcact);
                //HOperatorSet.ConcatObj(ImageConcact, ImageG, out ImageConcact);
                //HOperatorSet.ConcatObj(ImageConcact, ImageB, out ImageConcact);
                //ImageReturn = ImageConcact;
                //ImageR.Dispose();
                //ImageG.Dispose();
                //ImageB.Dispose();
            }
            return ImageReturn;
        }


        /// <summary>
        /// 传入原图 传出分离通道后concat一起的图
        /// </summary>
        /// <param name="ho_i_Image"></param>
        /// <param name="ho_o_Image"></param>
        /// <returns></returns>
        public static HObject Separat_Image(HObject ho_i_Image, out HObject ho_o_Image)
        {

            HObject ho_ObjectSelected = null, ho_Image1 = null;
            HObject ho_Image2 = null, ho_Image3 = null, ho_ObjectsConcat = null;

            // Local control variables 

            HTuple hv_Number = null, hv_Index = null, hv_Channels = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_o_Image);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);

            HOperatorSet.CountObj(ho_i_Image, out hv_Number);
            ho_o_Image.Dispose();
            HOperatorSet.GenEmptyObj(out ho_o_Image);
            HTuple end_val3 = hv_Number;
            HTuple step_val3 = 1;
            for (hv_Index = 1; hv_Index.Continue(end_val3, step_val3); hv_Index = hv_Index.TupleAdd(step_val3))
            {
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_i_Image, out ho_ObjectSelected, hv_Index);
                HOperatorSet.CountChannels(ho_ObjectSelected, out hv_Channels);
                if ((int)(new HTuple(hv_Channels.TupleEqual(3))) != 0)
                {
                    ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                    HOperatorSet.Decompose3(ho_ObjectSelected, out ho_Image1, out ho_Image2,
                        out ho_Image3);
                    ho_ObjectsConcat.Dispose();
                    HOperatorSet.ConcatObj(ho_Image1, ho_Image2, out ho_ObjectsConcat);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_ObjectsConcat, ho_Image3, out ExpTmpOutVar_0);
                        ho_ObjectsConcat.Dispose();
                        ho_ObjectsConcat = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    ho_ObjectsConcat.Dispose();
                    ho_ObjectsConcat = ho_ObjectSelected.CopyObj(1, -1);
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_o_Image, ho_ObjectsConcat, out ExpTmpOutVar_0);
                    ho_o_Image.Dispose();
                    ho_o_Image = ExpTmpOutVar_0;
                }
            }

            ho_ObjectSelected.Dispose();
            ho_Image1.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            ho_ObjectsConcat.Dispose();

            return ho_o_Image;

        }

        public static HTuple MetrologyType(IEnumerable<UserRegion> regions)
        {
            HTuple _MetrologyType = new HTuple();
            HTuple MetrologyType = new HTuple();

            foreach (var region in regions)
            {
                switch (region.RegionType)
                {
                    case RegionType.Line:
                        _MetrologyType = 1;
                        break;
                    case RegionType.Rectangle2:
                        _MetrologyType = 2;
                        break;
                    case RegionType.Circle:
                        _MetrologyType = 3;
                        break;
                    case RegionType.Ellipse:
                        _MetrologyType = 4;
                        break;
                    default:
                        MessageBox.Show("请重新选择画区域工具！");
                        return null;
                }
                MetrologyType = MetrologyType.TupleConcat(_MetrologyType);
            }
            return MetrologyType;
        }
    }
}
