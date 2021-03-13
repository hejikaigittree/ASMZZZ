using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;

namespace JFRecipe
{
    interface IJFRecipe
    {
        string[] AllItemNames { get; }
        Type GetItemType(string itemName);
        object GetItemValue(string itemValue);

        void SaveToCfg(JFXCfg cfg);
        void LoadFromCfg(JFXCfg cfg);

        void SaveToFile(string filePath);
        void LoadFromFile(string filePath);

    }
}
