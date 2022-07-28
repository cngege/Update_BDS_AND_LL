using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updata_BDS_AND_LL
{
    internal class Functions
    {
        /// <summary>
        /// 检测路径是否存在，不存在则创建
        /// </summary>
        /// <param name="path"></param>
        /// <returns>false 表示失败,最终这个路径是不存在的</returns>
        public static bool CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Directory.Exists(path);
        }
    }
}
