using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBNSCAN
{
    /// <summary>
    /// 公共处理方法
    /// </summary>
    class Comm
    {
        /// <summary>
        /// 过滤多余字符串
        /// 和Json中的URL中反斜杠转移符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string replaceStr(string str)
        {
            string newstr = str.Replace("\"", "");
            newstr = newstr.Replace("\'", "");
            newstr = newstr.Replace("[", "");
            newstr = newstr.Replace("]", "");
            newstr = newstr.Replace(" ", "");
            newstr = newstr.Replace("\n", "");
            newstr = newstr.Replace("\r", "");
            newstr = newstr.Replace("元", "");
            return newstr;
        }
        /// <summary>
        /// ASCII转码
        /// </summary>
        /// <param name="paramsx"></param>
        /// <returns></returns>
        public static string ASCII_OTHER(string paramsx)
        {
            string str = System.Text.RegularExpressions.Regex.Replace(paramsx, @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
            return str;
        }

        #region 功能注册表限制：注册表操作方法
        /// <summary>
        /// 判断是否注册true表示已经注册，false表示未注册
        /// </summary>
        /// <returns></returns>
        public static bool registerdoCheck()
        {

            bool value = false;
            string username = "[cpu]" + judgeHardWard.GetCpu() + "[disk]" + judgeHardWard.GetHardDiskID() + "[mac]" + judgeHardWard.GetMac() + "[network]" + judgeHardWard.GetNetwork();
            string user = judgeHardWard.DesEncrypt(username, "kukusoft.net");//加密字符串
            string zhucema = "";
            if (File.Exists(judgeHardWard.reigster_codestring))
            {
                zhucema = File.ReadAllText(judgeHardWard.reigster_codestring.Trim());

                if (judgeHardWard.checkCode(judgeHardWard.pubkeytxt, zhucema, user))
                {
                    value = true;//注册成功
                }
                else
                {
                    value = false;//注册失败
                }
            }
            else
            {
                value = false;
            }
            return value;
        }
        /// <summary>
        /// 创建注册表项和键值，初始化使用次数
        /// </summary>
        public static void CreateRegedit()
        {
            RegistryKey key = Registry.LocalMachine;
            if (!IsRegeditItemExist())//如果注册表项中找不到键值
            {
                RegistryKey software = key.CreateSubKey(@"software\kukusoft");
                if (!IsRegeditKeyExit())
                {
                    RegistryKey softwareKey = key.OpenSubKey(@"software\kukusoft", true); //该项必须已存在
                    softwareKey.SetValue("registercount", "1");
                }
            }
        }
        /// <summary>
        /// 注册表键值加1
        /// </summary>
        public static void AddCount()
        {
            RegistryKey key = Registry.LocalMachine;
            int newcount = ReadRegedit() + 1;//试用次数加1
            RegistryKey softwareKey = key.OpenSubKey(@"software\kukusoft", true); //该项必须已存在
            softwareKey.SetValue("registercount", newcount);
        }
        /// <summary>
        /// 读取键值:已经扫描了多少本书
        /// </summary>
        public static int ReadRegedit()
        {
            RegistryKey Key;
            Key = Registry.LocalMachine;
            RegistryKey myreg = Key.OpenSubKey(@"software\kukusoft");
            int value = Convert.ToInt32(myreg.GetValue("registercount").ToString().Trim());
            myreg.Close();
            return value;
        }
        /// <summary>
        /// 判断项是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IsRegeditItemExist()
        {
            string[] subkeyNames;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey(@"software");
            //RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);  
            subkeyNames = software.GetSubKeyNames();
            //取得该项下所有子项的名称的序列，并传递给预定的数组中  
            foreach (string keyName in subkeyNames)
            //遍历整个数组  
            {
                if (keyName == "kukusoft")
                //判断子项的名称  
                {
                    hkml.Close();
                    return true;
                }
            }
            hkml.Close();
            return false;
        }
        /// <summary>
        /// 判断键值是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IsRegeditKeyExit()
        {
            string[] subkeyNames;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey(@"software\kukusoft");
            //RegistryKey software = hkml.OpenSubKey("SOFTWARE\\test", true);
            subkeyNames = software.GetValueNames();
            //取得该项下所有键值的名称的序列，并传递给预定的数组中
            foreach (string keyName in subkeyNames)
            {
                if (keyName == "registercount") //判断键值的名称
                {
                    hkml.Close();
                    return true;
                }
            }
            hkml.Close();
            return false;
        }
        #endregion
    }
}
