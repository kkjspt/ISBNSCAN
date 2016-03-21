using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Management;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;

namespace ISBNSCAN
{
    public class judgeHardWard
    {
        #region 获取客户端用户硬件信息
        ///   <summary> 
        ///   获取cpu序列号     
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetCpu()
        {
            string temp = "";
            try
            {
                ManagementClass cimobject = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = cimobject.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    temp = mo.Properties["ProcessorId"].Value.ToString().Trim();
                }
            }
            catch
            {
                //return "获取CUPID号失败";
                return "[/error_cpu]";
            }
            return temp.ToString();
        }
        /// <summary>
        /// 获得硬盘编号
        /// </summary>
        /// <returns></returns>
        public static string GetHardDiskID()
        {
            string result = "";
            try
            {
                ManagementClass mcHD = new ManagementClass("win32_logicaldisk");
                ManagementObjectCollection mocHD = mcHD.GetInstances();
                foreach (ManagementObject m in mocHD)
                {
                    if (m["DeviceID"].ToString() == "C:")
                    {
                        result = m["VolumeSerialNumber"].ToString().Trim();
                    }
                }
            }
            catch
            {
                //return "获取硬盘ID失败";
                return "[/error_disk]";
            }
            return result;
        }
        /// <summary>
        /// 获得网卡MAC
        /// </summary>
        /// <returns></returns>
        public static string GetMac()
        {
            string result = "";
            try
            {
                ManagementClass mcMAC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection mocMAC = mcMAC.GetInstances();
                foreach (ManagementObject m in mocMAC)
                {
                    if ((bool)m["IPEnabled"])
                    {
                        result = m["MacAddress"].ToString();
                    }
                }
            }
            catch
            {
                //return "获取MAC失败";
                return "[/error_mac]";
            }
            return result;
        }

        /// <summary>
        ///获取主板物理网卡地址，唯一性
        /// </summary>
        /// <returns></returns>
        public static string GetNetwork()
        {

            string strMac = null;
            try
            {
                NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in fNetworkInterfaces)
                {
                    string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                    if (rk != null)
                    {
                        string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString(); ;
                        if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                        {
                            if (adapter.GetPhysicalAddress() != null)
                            {
                                strMac = adapter.GetPhysicalAddress().ToString();
                            }
                        }
                    }
                }
                if (strMac != null)
                {
                    return strMac.ToLower();
                }
                else
                {
                    //return "";
                    return "[/error_network]";
                }
            }
            catch
            {
                //return "无法获取到物理网卡地址";
                return "[/error_network]";
            }
            return strMac.ToLower();
        }
        #endregion
        /// <summary>
        /// 公钥
        /// </summary>
        public static string pubkeytxt = "<RSAKeyValue><Modulus>w2AGqLT6/LKMSmfpCcFYf1eA/8DzZYjfqLfBw/dhaMtZKxFnWPSW16SChJZiq977GefUoHdAtbkukReDNH0k26SVeI/hgp9KNo2otyJoi9sLZnIKZiRvRS7+es48V1TWb2W5iCDK1NmpHFR8k+qJ/qtXCIUs3Lb+UbZNx4vQjcE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        /// <summary>
        /// 返回是否注册成功
        /// </summary>
        /// <param name="pubkeytxt">公钥字符串</param>
        /// <param name="registercode">注册码</param>
        /// <param name="username">用户硬件信息</param>
        public static bool checkCode(string pubkeytxt, string registercode, string username)
        {
            bool registered = false;
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(pubkeytxt);
                    RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsa);
                    f.SetHashAlgorithm("SHA1");
                    byte[] key = Convert.FromBase64String(registercode);
                    SHA1Managed sha = new SHA1Managed();
                    byte[] name = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(username));
                    if (f.VerifySignature(name, key))
                    {
                        writeReigster_codestring(registercode);
                        registered = true;
                    }
                    else
                    {
                        registered = false;
                    }
                }
            }
            catch
            {
                registered = false;
            }
            return registered;
        }
        /// <summary>
        /// 存储注册码文件
        /// </summary>
        public static string reigster_codestring = "RegisterCode.txt";

        #region 加密字符串，用于加密硬件信息
        /// <summary> 
        /// 加密字符串 
        /// 注意:密钥必须为８位 
        /// </summary> 
        /// <param name="strText">字符串</param> 
        /// <param name="encryptKey">密钥</param> 
        /// <param name="encryptKey">返回加密后的字符串</param> 
        public static string DesEncrypt(string inputString, string encryptKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(inputString);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return inputString;
                //return error.Message; 
                //return null;
            }
        }
        /// <summary> 
        /// 解密字符串 
        /// </summary> 
        /// <param name="this.inputString">加了密的字符串</param> 
        /// <param name="decryptKey">密钥</param> 
        /// <param name="decryptKey">返回解密后的字符串</param> 
        public static string DesDecrypt(string inputString, string decryptKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] inputByteArray = new Byte[inputString.Length];
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(inputString);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetString(ms.ToArray());
            }
            catch (System.Exception error)
            {
                //return error.Message; 
                return null;
            }
        }
        #endregion


        /// <summary>
        /// 向txt文件中写入注册码字符串
        /// </summary>
        /// <param name="sr">需要写入的字符串</param>
        public static void writeReigster_codestring(string str)
        {
            StreamWriter sr;
            string report;
            File.Delete(reigster_codestring);
            if (File.Exists(reigster_codestring)) //如果文件存在,则创建File.AppendText对象
            {
                sr = File.AppendText(reigster_codestring);
                report = "appended";
            }
            else  //如果文件不存在,则创建File.CreateText对象
            {
                sr = File.CreateText(reigster_codestring);
                report = "created";
            }
            sr.WriteLine(str);
            //Console.WriteLine("{0} {1}", reigster_codestring, report);
            sr.Close();
        }

        #region 蜘蛛收集
        /// <summary>
        /// 采用WebRequest方法加载url地址
        /// </summary>
        public static string WebRequestloadurl(string url)
        {
            WebRequest request = WebRequest.Create(url); //WebRequest.Create方法，返回WebRequest的子类HttpWebRequest
            WebResponse response = request.GetResponse(); //WebRequest.GetResponse方法，返回对 Internet 请求的响应

            Stream resStream = response.GetResponseStream(); //WebResponse.GetResponseStream 方法，从 Internet 资源返回数据流。
            Encoding enc = Encoding.GetEncoding("GB2312"); // 如果是乱码就改成 utf-8 / GB2312
            StreamReader sr = new StreamReader(resStream, enc); //命名空间:System.IO。 StreamReader 类实现一个 TextReader (TextReader类，表示可读取连续字符系列的读取器)，使其以一种特定的编码从字节流中读取字符。
            string content = sr.ReadToEnd(); //输出(HTML代码)，ContentHtml为Multiline模式的TextBox控件

            string regex = "href=[\\\"\\\'](https://http://www.mogujie.com/cover/u/)\\w{6,10}[\\\"\\\']";//匹配网页中必须含有http://blog.csdn.net/的URL地址
            Regex re = new Regex(regex);
            MatchCollection matches = re.Matches(content);
            System.Collections.IEnumerator enu = matches.GetEnumerator();

            string allstr1 = "";
            string allstr2 = "";
            int start = 0;
            string newstring = "";
            while (enu.MoveNext() && enu.Current != null)//
            {
                Match match = (Match)(enu.Current);
                string newstr = match.Value.Substring(6, match.Value.Length - 7);//截取url地址
                newstr = newstr.Trim();
                //string newstr = match.Value;
                if (start % 2 == 1)
                {
                    allstr1 = newstr;
                }
                else
                {
                    allstr2 = newstr;
                }

                if (allstr1 != allstr2)
                {
                    newstring = newstr;
                    start++;
                }
            }
            return newstring;
        }
        #endregion
    }
}
