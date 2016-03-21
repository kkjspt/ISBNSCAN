using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ISBNSCAN
{
    class judgeNetWork
    {
        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>
        /// 判断本机网络情况
        /// </summary>
        /// <returns></returns>
        public static bool ComputerNetwork()
        {
            bool network = false;
            //判断是否联网
            int i = 0;
            if (InternetGetConnectedState(out i, 0))
            {
                //联网
                network = true;
            }
            else
            {
                //断网
                network = false;
            }
            return network;
        }
    }
}
