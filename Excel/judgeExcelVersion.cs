using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Excel
{
    public class judgeExcelVersion
    {

        #region  该方法编程参照
        //string strVersionResult = "";
        //strVersionResult += "错误提示：";
        //strVersionResult += "\n1、请确保您已经正确安装Office1997-2003或者以上版本（简化版或者免安装版不属于正确安装）";
        //strVersionResult += "\n2、请确保您已经正确安装Excel1997-2003或者以上版本（简化版或者免安装版不属于正确安装）";
        //strVersionResult += "\n3、发生以上错误，建议重新下载完整版Office进行安装";

        //switch (i)
        //{
        //    case 8:
        //        strVersionResult = "office2000";
        //        break;
        //    case 10:
        //        strVersionResult = "officeXP";
        //        break;
        //    case 11:
        //        strVersionResult = "office2003";
        //        break;
        //    case 12:
        //        strVersionResult = "office2007";
        //        break;
        //    case 14:
        //        strVersionResult = "office2010";
        //        break;
        //    case 15:
        //        strVersionResult = "office2013";
        //        break;
        //}
        #endregion

        /// <summary>
        /// 返回Office当前版本号
        /// </summary>
        /// <returns></returns>
        public static int GetOfficeVersionNum()
        {
            int version = 0;
            Microsoft.Win32.RegistryKey regKey = null;
            //Microsoft.Win32.RegistryKey regSubKey1 = null;
            Microsoft.Win32.RegistryKey regSubKey2 = null;
            regKey = Microsoft.Win32.Registry.LocalMachine;
            for (int i = 0; i < 18; i++)//遍历获取当前电脑安装的版本号
            {
                //regSubKey1 = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\" + i + @".0\Common\InstallRoot", false);
                regSubKey2 = regKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\" + i + @".0", false);
                //if ((regSubKey1 != null && regSubKey1.GetValue("Path") != null) || (regSubKey2 != null && regSubKey2.GetValue("Path") != null))
                if (regSubKey2 != null)
                {    //&& regSubKey2.GetValue("Path") != null
                     //path1 = regSubKey1.GetValue("Path").ToString();
                     // path2 = regSubKey2.GetValue("Path").ToString();
                    version = i;
                }
            }
            return version;//如果返回0或者小于8说明该电脑中没有正确安装Office
        }
    }
}
