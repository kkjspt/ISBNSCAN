using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISBNSCAN
{
    class Program
    {
        static void Main(string[] args)
        {
            MainStart();
        }
        /// <summary>
        /// 启动主程序
        /// </summary>
        private static void MainStart()
        {
            if (registerdoCheck())//如果软件已经注册
            {
                Console.Title = "ISBN数据查询V1.0.4";
                //jsonReadNetworkData();
            }
            else
            {

                Console.Title = string.Format("ISBN数据查询V1.0.4[试用版{0}/200次查询*]", ReadRegedit());
                Console.WriteLine("继续试用？按Y键后回车");
                Console.WriteLine("马上注册？按S键后回车");
                string keyboard = Console.ReadLine();
                if (keyboard.ToUpper() == "Y")
                {
                    //jsonReadNetworkData();
                    AddCount();

                }
                if (keyboard.ToUpper() == "S")
                {
                    System.Diagnostics.Process.Start("RegisterApplication.exe");
                }
            }
        }

        #region 注册限制方法
        /// <summary>
        /// 判断是否注册true表示已经注册，false表示未注册
        /// </summary>
        /// <returns></returns>
        public static bool registerdoCheck()
        {

            bool value = false;
            string username = "[cpu]" + HardWare.judgeHardWard.GetCpu() + "[disk]" + HardWare.judgeHardWard.GetHardDiskID() + "[mac]" + HardWare.judgeHardWard.GetMac() + "[network]" + HardWare.judgeHardWard.GetNetwork();
            string user = HardWare.judgeHardWard.DesEncrypt(username, "kukusoft.net");//加密字符串
            string zhucema = "";
            if (File.Exists(HardWare.judgeHardWard.reigster_codestring))
            {
                zhucema = File.ReadAllText(HardWare.judgeHardWard.reigster_codestring.Trim());

                if (HardWare.judgeHardWard.checkCode(HardWare.judgeHardWard.pubkeytxt, zhucema, user))
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
        private static void CreateRegedit()
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
        private static void AddCount()
        {
            RegistryKey key = Registry.LocalMachine;
            int newcount = ReadRegedit() + 1;//试用次数加1
            RegistryKey softwareKey = key.OpenSubKey(@"software\kukusoft", true); //该项必须已存在
            softwareKey.SetValue("registercount", newcount);
        }
        /// <summary>
        /// 读取键值:已经扫描了多少本书
        /// </summary>
        private static int ReadRegedit()
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
        private static bool IsRegeditItemExist()
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
        private static bool IsRegeditKeyExit()
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
