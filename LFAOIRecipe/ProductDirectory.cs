using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class ProductDirectory
    {
        public static void Set(string productDirectory) => FilePath.ProductDirectory = productDirectory;
    }
}
