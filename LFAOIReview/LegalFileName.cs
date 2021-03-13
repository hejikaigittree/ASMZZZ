using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    /// <summary>
    /// 将字符串转为文件名合法的字符串
    /// </summary>
    class LegalFileName
    {
        public  static string Get(string str)
        {
            StringBuilder sb = new StringBuilder(str);
            Path.GetFullPath(str);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sb.Replace(c.ToString(), "-");
            }
            return sb.ToString();
        }
    }
}
