using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFAOIRecipe
{
    class FilePath
    {
        public static string ProductDirectory { get; set; } = $"{Application.StartupPath}\\TestProduct";

        public static string RecipeLoadProductDirectory { get; set; } = string.Empty;//

        public static string EnsureDirectoryExisted (string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }
    }
}
