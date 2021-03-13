using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe.Algorithm
{
    class File
    {
        public static void list_image_files(HTuple hv_ImageDirectory,
                                            HTuple hv_Extensions,
                                            HTuple hv_Options,
                                        out HTuple hv_ImageFiles)
        {
            // Local iconic variables 
            // Local control variables 
            HTuple hv_HalconImages = null, hv_OS = null;
            HTuple hv_Directories = null, hv_Index = null, hv_Length = null;
            HTuple hv_network_drive = null, hv_Substring = new HTuple();
            HTuple hv_FileExists = new HTuple(), hv_AllFiles = new HTuple();
            HTuple hv_i = new HTuple(), hv_Selection = new HTuple();
            HTuple hv_Extensions_COPY_INP_TMP = hv_Extensions.Clone();
            HTuple hv_ImageDirectory_COPY_INP_TMP = hv_ImageDirectory.Clone();

            // Initialize local and output iconic variables 
            //This procedure returns all files in a given directory
            //with one of the suffixes specified in Extensions.
            //
            //input parameters:
            //ImageDirectory: as the name says
            //   If a tuple of directories is given, only the images in the first
            //   existing directory are returned.
            //   If a local directory is not found, the directory is searched
            //   under %HALCONIMAGES%/ImageDirectory. If %HALCONIMAGES% is not set,
            //   %HALCONROOT%/images is used instead.
            //Extensions: A string tuple containing the extensions to be found
            //   e.g. ['png','tif',jpg'] or others
            //If Extensions is set to 'default' or the empty string '',
            //   all image suffixes supported by HALCON are used.
            //Options: as in the operator list_files, except that the 'files'
            //   option is always used. Note that the 'directories' option
            //   has no effect but increases runtime, because only files are
            //   returned.
            //
            //output parameter:
            //ImageFiles: A tuple of all found image file names
            //
            if ((int)((new HTuple((new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(""))))).TupleOr(new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(
                "default")))) != 0)
            {
                hv_Extensions_COPY_INP_TMP = new HTuple();
                hv_Extensions_COPY_INP_TMP[0] = "ima";
                hv_Extensions_COPY_INP_TMP[1] = "tif";
                hv_Extensions_COPY_INP_TMP[2] = "tiff";
                hv_Extensions_COPY_INP_TMP[3] = "gif";
                hv_Extensions_COPY_INP_TMP[4] = "bmp";
                hv_Extensions_COPY_INP_TMP[5] = "jpg";
                hv_Extensions_COPY_INP_TMP[6] = "jpeg";
                hv_Extensions_COPY_INP_TMP[7] = "jp2";
                hv_Extensions_COPY_INP_TMP[8] = "jxr";
                hv_Extensions_COPY_INP_TMP[9] = "png";
                hv_Extensions_COPY_INP_TMP[10] = "pcx";
                hv_Extensions_COPY_INP_TMP[11] = "ras";
                hv_Extensions_COPY_INP_TMP[12] = "xwd";
                hv_Extensions_COPY_INP_TMP[13] = "pbm";
                hv_Extensions_COPY_INP_TMP[14] = "pnm";
                hv_Extensions_COPY_INP_TMP[15] = "pgm";
                hv_Extensions_COPY_INP_TMP[16] = "ppm";
                //
            }
            if ((int)(new HTuple(hv_ImageDirectory_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                hv_ImageDirectory_COPY_INP_TMP = ".";
            }
            HOperatorSet.GetSystem("image_dir", out hv_HalconImages);
            HOperatorSet.GetSystem("operating_system", out hv_OS);
            if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
            {
                hv_HalconImages = hv_HalconImages.TupleSplit(";");
            }
            else
            {
                hv_HalconImages = hv_HalconImages.TupleSplit(":");
            }
            hv_Directories = hv_ImageDirectory_COPY_INP_TMP.Clone();
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_HalconImages.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_Directories = hv_Directories.TupleConcat(((hv_HalconImages.TupleSelect(hv_Index)) + "/") + hv_ImageDirectory_COPY_INP_TMP);
            }
            HOperatorSet.TupleStrlen(hv_Directories, out hv_Length);
            HOperatorSet.TupleGenConst(new HTuple(hv_Length.TupleLength()), 0, out hv_network_drive);
            if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
            {
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((((hv_Directories.TupleSelect(hv_Index))).TupleStrlen()
                        )).TupleGreater(1))) != 0)
                    {
                        HOperatorSet.TupleStrFirstN(hv_Directories.TupleSelect(hv_Index), 1, out hv_Substring);
                        if ((int)(new HTuple(hv_Substring.TupleEqual("//"))) != 0)
                        {
                            if (hv_network_drive == null)
                                hv_network_drive = new HTuple();
                            hv_network_drive[hv_Index] = 1;
                        }
                    }
                }
            }
            hv_ImageFiles = new HTuple();
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Directories.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                HOperatorSet.FileExists(hv_Directories.TupleSelect(hv_Index), out hv_FileExists);
                if ((int)(hv_FileExists) != 0)
                {
                    HOperatorSet.ListFiles(hv_Directories.TupleSelect(hv_Index), (new HTuple("files")).TupleConcat(
                        hv_Options), out hv_AllFiles);
                    hv_ImageFiles = new HTuple();
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Extensions_COPY_INP_TMP.TupleLength()
                        )) - 1); hv_i = (int)hv_i + 1)
                    {
                        HOperatorSet.TupleRegexpSelect(hv_AllFiles, (((".*" + (hv_Extensions_COPY_INP_TMP.TupleSelect(
                            hv_i))) + "$")).TupleConcat("ignore_case"), out hv_Selection);
                        hv_ImageFiles = hv_ImageFiles.TupleConcat(hv_Selection);
                    }
                    HOperatorSet.TupleRegexpReplace(hv_ImageFiles, (new HTuple("\\\\")).TupleConcat(
                        "replace_all"), "/", out hv_ImageFiles);
                    if ((int)(hv_network_drive.TupleSelect(hv_Index)) != 0)
                    {
                        HOperatorSet.TupleRegexpReplace(hv_ImageFiles, (new HTuple("//")).TupleConcat(
                            "replace_all"), "/", out hv_ImageFiles);
                        hv_ImageFiles = "/" + hv_ImageFiles;
                    }
                    else
                    {
                        HOperatorSet.TupleRegexpReplace(hv_ImageFiles, (new HTuple("//")).TupleConcat(
                            "replace_all"), "/", out hv_ImageFiles);
                    }
                    return;
                }
            }
        }

        public static void RotateImage(HObject sourceImage, out HObject destImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2)
        {
            HTuple linePhi = null;
            HOperatorSet.LineOrientation(row1, col1, row2, col2, out linePhi);
            HTuple lineCenterRow = (row1 + row2) / 2.0;
            HTuple lineCenterCol = (col1 + col2) / 2.0;
            HTuple hom2d = null, hom2dRotate = null;
            HOperatorSet.HomMat2dIdentity(out hom2d);
            HOperatorSet.HomMat2dRotate(hom2d, -linePhi, lineCenterRow, lineCenterCol, out hom2dRotate);
            destImage = null;
            HOperatorSet.AffineTransImage(sourceImage, out destImage, hom2dRotate, "bilinear", "false");
        }

        public static void SaveModel(string fileName, int modelType, HTuple modelId)
        {
            if (modelType == 0)
            {
                HOperatorSet.WriteNccModel(modelId, fileName);
            }
            else if (modelType == 1)
            {
                HOperatorSet.WriteShapeModel(modelId, fileName);
            }
            else
            {
                throw new HalconException("Wrong argument [modelType]=" + modelType);
            }
        }

        //重载 多个模板分开保存
        public static void SaveModel(string[] fileName, int modelType, HTuple modelId)
        {
            for (int i=0;i<modelId.TupleLength();i++)
            {
                if (modelType == 0)
                {
                    HOperatorSet.WriteNccModel(modelId[i], fileName[i]);
                }
                else if (modelType == 1)
                {
                    HOperatorSet.WriteShapeModel(modelId[i], fileName[i]);
                }
                else
                {
                    throw new HalconException("Wrong argument [modelType]=" + modelType);
                }
            }                 
        }

        //重载 多个模板分开保存
        public static void SaveModel(string[] fileName, int[] modelType, HTuple modelId)//
        {
            for (int i = 0; i < modelId.TupleLength(); i++)
            {
                if (modelType[i] == 0)
                {
                    HOperatorSet.WriteNccModel(modelId[i], fileName[i]);
                }
                else if (modelType[i] == 1)
                {
                    HOperatorSet.WriteShapeModel(modelId[i], fileName[i]);
                }
                else
                {
                    throw new HalconException("Wrong argument [modelType]=" + modelType);
                }
            }
        }

        public static HTuple ReadModel(string fileName, int modelType)
        {
            if (modelType == 0)
            {
                HOperatorSet.ReadNccModel(fileName, out HTuple modelId);
                return modelId;
            }
            else if (modelType == 1)
            {
                HOperatorSet.ReadShapeModel(fileName, out HTuple modelId);
                return modelId;
            }
            else
            {
                throw new HalconException("Wrong argument [modelType]=" + modelType);
            }
        }

        /// <summary>
        /// 重载 多个模板读取
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static HTuple ReadModel(string[] fileName, int modelType)
        {
            HTuple _modelId = new HTuple();
            HTuple modelId = new HTuple();
            for (int i = 0; i < fileName.Length; i++)
            {
                if (modelType == 0)
                {
                    HOperatorSet.ReadNccModel(fileName[i], out  _modelId);
                    
                }
                else if (modelType == 1)
                {
                    HOperatorSet.ReadShapeModel(fileName[i], out  _modelId);
                    
                }
                else
                {
                    
                    throw new HalconException("Wrong argument [modelType]=" + modelType);
                    
                }

                modelId = modelId.TupleConcat(_modelId);
            }
            return modelId;
        }


        /// <summary>
        /// 重载 多个模板读取
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static HTuple ReadModel(string[] fileName, int[] modelType)//
        {
            HTuple _modelId = new HTuple();
            HTuple modelId = new HTuple();
            for (int i = 0; i < fileName.Length; i++)
            {
                if (modelType[i] == 0)
                {
                    HOperatorSet.ReadNccModel(fileName[i], out _modelId);
                }
                else if (modelType[i] == 1)
                {
                    HOperatorSet.ReadShapeModel(fileName[i], out _modelId);
                }
                else
                {
                    throw new HalconException("Wrong argument [modelType]=" + modelType);
                }
                modelId = modelId.TupleConcat(_modelId);
            }
            return modelId;
        }
    }
}
