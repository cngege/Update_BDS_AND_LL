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

        /// <summary>
        /// 检查路径是否以/ 或者\\结尾,返回以/结尾的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CheckPathEnd(string path)
        {
            if (path.LastIndexOf("/") == path.Length - 1 || path.LastIndexOf("\\") == path.Length - 1)
            {
                return path;
            }
            if(path.IndexOf("/") != -1)
            {
                return path + "/";
            }
            return path + "\\";
        }

    }
}
