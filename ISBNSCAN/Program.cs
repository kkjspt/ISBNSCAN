
using Microsoft.Win32;
using MongoDB.Driver;
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
        /// <summary>
        /// 网络连接失败提示
        /// </summary>
        private static string info_networkcheckfail = "网络连接异常，请检查您的电脑是否已经正确接入互联网。";
        /// <summary>
        /// 实际是下载远端ISBN查询服务器网页失败提示
        /// </summary>
        private static string info_isbnservercheckfail = "没有找到这本书的信息！\n1、请确保您的计算机已经接入互联网\n2、请确保您的书籍为正版书籍\n3、请换本书籍试试";
        /// <summary>
        /// 没有注册或者是试用期结束
        /// </summary>
        private static string info_notregedit = "您还没有注册软件，是否注册？按Enter键立刻注册软件";
        /// <summary>
        /// 软件可供试用的次数
        /// </summary>
        private static int softwareOntrail = 100;

        static void Main(string[] args)
        {
            MainStart();
        }


        /// <summary>
        /// 启动主程序
        /// </summary>
        private static void MainStart()
        {
            if (Comm.registerdoCheck())//如果软件已经注册
            {
                Console.Title = "ISBN数据查询V1.0.4";
                jsonReadNetworkData();
            }
            else
            {
                Console.Title = string.Format("ISBN数据查询V1.0.4[试用版{0}/{1}次查询*]", Comm.ReadRegedit(), softwareOntrail);
                Console.WriteLine("继续试用？按Y键后回车");
                Console.WriteLine("马上注册？按S键后回车");
                string keyboard = Console.ReadLine();
                if (keyboard.ToUpper() == "Y")
                {
                    jsonReadNetworkData();
                }
                if (keyboard.ToUpper() == "S")
                {
                    System.Diagnostics.Process.Start("RegisterApplication.exe");
                }
            }
        }
        #region 主线上查询程序
        private static void jsonReadNetworkData()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;  //设置字体颜色为白色
            string newsurl = "http://www.kkjspt.cn/kukusoft/isbnsearch/V1.0.3/news/";
            //Console.WriteLine(downLoadPage(newsurl));
            Console.ForegroundColor = ConsoleColor.White;  //设置字体颜色为白色
            Console.WriteLine("请输入ISBN：");
            Console.ForegroundColor = ConsoleColor.Green;  //设置字体颜色为绿色
            string isbn = Console.ReadLine();



            if (judgeNetWork.ComputerNetwork())//网络连接情况判定
            {
                if (ISBNcheck.CheckISBN(isbn))//ISBN本地校验
                {
                    string b_id = "";//豆瓣编的ID号码
                    string b_isbn13 = "";//isbn13位
                    string b_isbn10 = "";//isbn10位
                    string b_title = "";//图书书名
                    string b_origin_title = "";
                    string b_alt_title = "";
                    string b_subtitle = "";
                    string b_url = "";
                    string b_images_large = "";//大图片地址
                    string b_images_medium = "";//中图片地址
                    string b_images_small = "";//小图片地址
                    string b_alt = "";//alt表示 http://book.douban.com/subject/1003078 中的1003078
                    string b_author = "";//作者名称
                    string b_publisher = "";//出版社
                    string b_translator = "";//翻译人
                    string b_pubdate = "";//出版日期
                    string b_price = "";//价格
                    string b_pages = "";//书页
                    string b_author_intro = "";//作者简介
                    string b_summary = "";//图书简介
                    string b_tags = "";//分类
                    string b_binding = "";//精装还是平装
                    string b_catalog = "";//序言目录之类


                    if (xmlControl.xmlHaveRecord(xmlControl.localPath, isbn))
                    {
                        XElement rootNode = XElement.Load(xmlControl.localPath);
                        IEnumerable<XElement> myTargetNodes = from myTarget in rootNode.Descendants("first")
                                                                  //where (myTarget.Element("b_id").Value.Trim().Equals(isbn) || myTarget.Element("b_isbn10").Value.Trim().Equals(isbn) || myTarget.Element("b_isbn13").Value.Trim().Equals(isbn)) && myTarget.HasElements
                                                              where (myTarget.Element("b_isbn10").Value.Trim().Equals(isbn) || myTarget.Element("b_isbn13").Value.Trim().Equals(isbn))
                                                              select myTarget;

                        foreach (XElement node in myTargetNodes)
                        {
                            b_id = node.Element("b_id").Value.ToString().Trim();
                            b_isbn10 = node.Element("b_isbn10").Value.ToString().Trim();
                            b_isbn13 = node.Element("b_isbn13").Value.ToString().Trim();
                            b_title = node.Element("b_title").Value.ToString().Trim();
                            b_origin_title = node.Element("b_origin_title").Value.ToString().Trim();
                            b_alt_title = node.Element("b_alt_title").Value.ToString().Trim();
                            b_subtitle = node.Element("b_subtitle").Value.ToString().Trim();
                            b_url = node.Element("b_url").Value.ToString().Trim();
                            b_alt = node.Element("b_alt").Value.ToString().Trim();
                            b_images_large = node.Element("b_images_large").Value.ToString().Trim();
                            b_author = node.Element("b_author").Value.ToString().Trim();
                            b_publisher = node.Element("b_publisher").Value.ToString().Trim();
                            b_translator = node.Element("b_translator").Value.ToString().Trim();
                            b_pubdate = node.Element("b_pubdate").Value.ToString().Trim();
                            b_price = node.Element("b_price").Value.ToString().Trim();
                            b_pages = node.Element("b_pages").Value.ToString().Trim();
                            b_author_intro = node.Element("b_author_intro").Value.ToString().Trim();
                            b_summary = node.Element("b_summary").Value.ToString().Trim();
                            b_tags = node.Element("b_tags").Value.ToString().Trim();
                            b_binding = node.Element("b_binding").Value.ToString().Trim();
                            b_catalog = node.Element("b_catalog").Value.ToString().Trim();
                        }
                        Console.WriteLine("本地查询");
                    }
                    else //如果本地的XML文件查找不到数据，就执行网络查找
                    {
                            string url = "https://api.douban.com/v2/book/isbn/:" + isbn;//豆瓣图书API接口获取JSON数据

                            Console.WriteLine("查找ISBN号为{0}的书籍…", isbn);//下载网页前，显示提示信息！
                            string jsonstr = downLoadPage(url);
                            JObject jo = JObject.Parse(jsonstr);

                            b_id = jo["id"] != null ? jo["id"].ToString() : "";
                            b_isbn13 = jo["isbn13"] != null ? jo["isbn13"].ToString() : "";
                            b_isbn10 = jo["isbn10"] != null ? jo["isbn10"].ToString() : "";

                            b_title = jo["title"].ToString() == "" ? "" : jo["title"].ToString();
                            b_origin_title = jo["origin_title"].ToString() == "" ? "" : jo["origin_title"].ToString();
                            b_alt_title = jo["alt_title"].ToString() == "" ? "" : jo["alt_title"].ToString();
                            b_subtitle = jo["subtitle"].ToString() == "" ? "" : jo["subtitle"].ToString();
                            b_url = jo["url"].ToString() == "" ? "" : jo["url"].ToString();

                            b_images_large = jo["images"]["large"] != null ? jo["images"]["large"].ToString() : "";
                            b_images_medium = jo["images"]["medium"] != null ? jo["images"]["medium"].ToString() : "";
                            b_images_small = jo["images"]["small"] != null ? jo["images"]["small"].ToString() : "";
                            b_alt = jo["alt"] != null ? jo["alt"].ToString() : "";
                            try { b_author = jo["author"][0] != null ? jo["author"][0].ToString() : ""; }
                            catch (Exception e) { }
                            b_publisher = jo["publisher"] != null ? jo["publisher"].ToString() : "";
                            b_translator = jo["translator"] != null ? jo["translator"].ToString() : "";
                            b_pubdate = jo["pubdate"].ToString() == "" ? "" : jo["pubdate"].ToString();
                            b_price = jo["price"].ToString() == "" ? "" : jo["price"].ToString();
                            b_pages = jo["pages"].ToString() == "" ? "" : jo["pages"].ToString();
                            b_author_intro = jo["author_intro"].ToString() == "" ? "" : jo["author_intro"].ToString();
                            b_summary = jo["summary"] == null ? "" : jo["summary"].ToString();

                            string tags0 = "";
                            try { tags0 = jo["tags"][0]["name"].ToString() == "" ? "" : "|" + jo["tags"][0]["name"].ToString(); }
                            catch (Exception e) { tags0 = ""; }
                            string tags1 = "";
                            try { tags1 = jo["tags"][1]["name"].ToString() == "" ? "" : "|" + jo["tags"][1]["name"].ToString(); }
                            catch (Exception e) { tags1 = ""; }
                            string tags2 = "";
                            try { tags2 = jo["tags"][2]["name"].ToString() == "" ? "" : "|" + jo["tags"][2]["name"].ToString(); }
                            catch (Exception e) { tags2 = ""; }
                            string tags3 = "";
                            try { tags3 = jo["tags"][3]["name"].ToString() == "" ? "" : "|" + jo["tags"][3]["name"].ToString(); }
                            catch (Exception e) { tags3 = ""; }
                            string tags4 = "";
                            try { tags4 = jo["tags"][4]["name"].ToString() == "" ? "" : "|" + jo["tags"][4]["name"].ToString(); }
                            catch (Exception e) { tags4 = ""; }
                            string tags5 = "";
                            try { tags5 = jo["tags"][5]["name"].ToString() == "" ? "" : "|" + jo["tags"][5]["name"].ToString(); }
                            catch (Exception e) { tags5 = ""; }
                            string tags6 = "";
                            try { tags6 = jo["tags"][6]["name"].ToString() == "" ? "" : "|" + jo["tags"][6]["name"].ToString(); }
                            catch (Exception e) { tags6 = ""; }
                            string tags7 = "";
                            try { tags7 = jo["tags"][7]["name"].ToString() == "" ? "" : "|" + jo["tags"][7]["name"].ToString(); }
                            catch (Exception e) { tags7 = ""; }
                            b_tags = tags0 + tags1 + tags2 + tags3 + tags4 + tags5 + tags6 + tags7;
                            b_binding = jo["binding"] != null ? jo["binding"].ToString() : "";
                            b_catalog = jo["catalog"].ToString() == "" ? "" : jo["catalog"].ToString();


                            if (isbn.Length == 13)
                            {
                                b_isbn13 = isbn;
                            }
                            else if (isbn.Length == 10)
                            {
                                b_isbn10 = isbn;
                            }

                            //读取软件在注册表中已经试用的次数
                            int OntrailCount = Comm.ReadRegedit();
                            if (!Comm.registerdoCheck() && OntrailCount <= softwareOntrail && b_title != "")
                            {
                                //只要返回成功标题，就在试用软件计数器加1
                                Comm.AddCount();
                                Console.Title = string.Format("ISBN数据查询V1.0.4[试用版{0}/{1}次查询*]", OntrailCount, softwareOntrail);
                            }

                            //不存在记录即可插入到本地XML数据文件中
                            xmlControl.insterXml(
                                                b_id,
                                                b_isbn10,
                                                b_isbn13,
                                                b_title,
                                                b_origin_title,
                                                b_alt_title,
                                                b_subtitle,
                                                b_url,
                                                b_alt,
                                                b_images_large,
                                                b_author,
                                                b_publisher,
                                                b_translator,
                                                b_pubdate,
                                                b_price,
                                                b_pages,
                                                b_author_intro,
                                                b_summary,
                                                b_tags,
                                                b_binding,
                                               b_catalog
                                            );
                        Console.WriteLine("网络查询");

                    }

                    ///向客户端的Excel中插入数据
                    OledbInsertExcel(
                         isbn,
                         b_title,
                         b_author,
                         b_pubdate,
                         b_price,
                         b_publisher,
                         b_summary,
                         b_images_small,
                         b_images_large,
                         b_images_medium,
                         b_alt
                         );
                    ///向kkjspt.com服务器提交数据
                    submitDataToServer.loadUrl("http://www.kkjspt.cn/kukusoft/isbnsearch/V1.0.3/isbnsubmit/",
                     "b_id=" + b_id +
                     "&b_isbn13=" + b_isbn13 +
                     "&b_isbn10=" + b_isbn10 +
                     "&b_title=" + b_title +
                     "&b_origin_title=" + b_origin_title +
                     "&b_alt_title=" + b_alt_title +
                     "&b_subtitle=" + b_subtitle +
                     "&b_url=" + b_url +
                     "&b_alt=" + b_alt +
                     "&b_images_large=" + b_images_large +
                     "&b_author=" + b_author +
                     "&b_publisher=" + b_publisher +
                     "&b_translator=" + b_translator +
                     "&b_pubdate=" + b_pubdate +
                     "&b_price=" + b_price +
                     "&b_pages=" + b_pages +
                     "&b_author_intro=" + b_author_intro +
                     "&b_summary=" + b_summary +
                     "&b_tags=" + b_tags +
                     "&b_binding=" + b_binding +
                     "&b_catalog=" + b_catalog
                     , Encoding.UTF8
                );
                    Console.ForegroundColor = ConsoleColor.Cyan;  //设置字体颜色为粉天蓝色
                    Console.WriteLine("该书已经成功录入到本地数据表中！");
                    Console.WriteLine("═════════════════════════════════════");
                    jsonReadNetworkData();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;  //设置字体颜色为红色
                    Console.WriteLine("ISBN码格式错误，您输入的不是标准的ISBN码，请核对后重新输入！");
                    jsonReadNetworkData();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;  //设置字体颜色为红色
                Console.WriteLine(info_networkcheckfail);
                jsonReadNetworkData();
            }

        }
    
        
        #endregion


        private static string ExcelPath1 = System.AppDomain.CurrentDomain.BaseDirectory + @"\data\book.xls";
        private static string ExcelPath2 = System.AppDomain.CurrentDomain.BaseDirectory + @"\data\book.xlsx";
        private static string OlecbExcel1 = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ExcelPath1 + ";Extended Properties=Excel 8.0;";//Excel2007以下版本驱动
        private static string OlecbExcel2= @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ExcelPath2 + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=0;\"";//Excel2007以上版本驱动
        /// <summary>
        /// 通过OLEDB写入Excel文档
        /// </summary>
        /// <param name="Path">Excel路径</param>
        /// <param name="isbn"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="pubdate"></param>
        /// <param name="price"></param>
        /// <param name="publisher"></param>
        /// <param name="summary"></param>
        /// <param name="small"></param>
        /// <param name="large"></param>
        /// <param name="medium"></param>
        /// <param name="alt"></param>
        public static void OledbInsertExcel(
            string isbn, 
            string title,
            string author,
            string pubdate,
            string price,
            string publisher,
            string summary,
            string small,
            string large,
            string medium,
            string alt)
        {
            try
            {
                string ExcelOledb = "";
                string info = "";
         
                if (judgeExcelVersion.GetOfficeVersionNum() < 8)
                {
                    ExcelOledb = "";
                    // "没有正确安装Office和Excel,请正确安装Office后再使用本软件！";
                }
                else if (judgeExcelVersion.GetOfficeVersionNum() > 8 && judgeExcelVersion.GetOfficeVersionNum()<12)
                {
                    ExcelOledb = OlecbExcel1;
                }
                else if (judgeExcelVersion.GetOfficeVersionNum() > 11)
                {
                    ExcelOledb = OlecbExcel2;
                }
                else
                {
                     // "没有正确安装Office和Excel,请正确安装Office后再使用本软件！";
                    ExcelOledb = "";//预留的判断
                }



                if (ExcelOledb != "")
                {
                    if (summary.Length > 200)//如果不限制内容简介的字符长度，插入过程将会报错
                        summary = summary.Substring(0, 200);
                    OleDbConnection thisconnection = new OleDbConnection(ExcelOledb);
                    thisconnection.Open();
                    string Sql = string.Format("insert into [Sheet1$] (ISBN,书名,作者,出版日期,价格,出版社,内容简介,small,large,medium,alt) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", isbn, title, author, pubdate, price, publisher, summary, small, large, medium, alt);
                    OleDbCommand mycommand = new OleDbCommand(Sql, thisconnection);
                    mycommand.ExecuteNonQuery();
                    thisconnection.Close();
                    Console.WriteLine("查询成功！");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;  //设置字体颜色为红色
                    Console.WriteLine("错误提示：");
                    Console.WriteLine("1、您的电脑中没有安装Offcie2003以上版本");
                    Console.WriteLine("2、您的电脑中安装了简化版或者绿色版的Offcie产品");
                    Console.WriteLine("3、建议您卸载当前版本重新完整安装Offic产品");
                    Console.WriteLine("立刻下载Office完整版？按Y键后回车");
                    string reg = Console.ReadLine().ToUpper();
                    if (reg == "Y")
                    {
                        System.Diagnostics.Process.Start("http://www.kkjspt.cn/kukusoft/isbnsearch/V1.0.3/help/?aq=4");
                    }
                    Console.Read();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;  //设置字体颜色为红色
                //Console.WriteLine("将数据录入到Excel失败！请关闭Excel！" + ex.ToString());
                Console.WriteLine("写入错误：");
                Console.WriteLine("1、您的Excel文件正打开着，请将它关闭后再试");
                Console.WriteLine("2、您修改了Excel的列结构，请卸载软件后重新安装\n如果您的Excel中已经有很多数据了，请另存Excel的数据到桌面再重新安装！");
                Console.WriteLine("3、您安装了简化版的Office产品，请卸载后重新安装完整版的Office应用程序");
                Console.ReadKey();
                jsonReadNetworkData();
            }
        }


        #region 下载网页类
        /// <summary>
        /// 定义写入流操作 取代WebClient方法下载网页内容
        /// 定义了超时时间为8秒
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string downLoadPage(string url)
        {
            string strBuff = "";

            if (judgeNetWork.ComputerNetwork())
            {

                try
                {

                    Uri httpURL = new Uri(url);
                    char[] cbuffer = new char[256];
                    int byteRead = 0;
                    //string filename = @"c:\log.txt";

                    ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换 
                    HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
                    ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换
                    httpReq.Timeout = 100000;
                    HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
                    ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容

                    ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理 
                    Stream respStream = httpResp.GetResponseStream();

                    ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以

                    //StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8） 
                    StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);

                    byteRead = respStreamReader.Read(cbuffer, 0, 256);

                    while (byteRead != 0)
                    {
                        string strResp = new string(cbuffer, 0, byteRead);
                        strBuff = strBuff + strResp;
                        byteRead = respStreamReader.Read(cbuffer, 0, 256);
                    }

                    respStream.Close();

                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;  //设置字体颜色为红色
                    Console.WriteLine(info_isbnservercheckfail);
                    jsonReadNetworkData();
                    return null;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;  //设置字体颜色为红色
                Console.WriteLine(info_networkcheckfail);
                jsonReadNetworkData();
            }
            return strBuff;
        }
        #endregion



        #region mongodb 轻量级数据库操作方法：没有使用的原因是要重新转化到Excel中供用户重新操作，麻烦！
        #endregion

    }
}
