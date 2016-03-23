using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ISBNSCAN
{
    class xmlControl
    {
        /// <summary>
        /// 本地XML数据文件存放地址
        /// </summary>
        public static string localPath = System.AppDomain.CurrentDomain.BaseDirectory + @"data\historyData.xml";
        public static XElement rootNode = XElement.Load(localPath);

        #region 创建Datax.xml文档
        /// <summary>
        /// 创建xml文档结构
        /// 因为是判断数据文件Datax.xml不存在之后才调用此方法
        /// System.AppDomain.CurrentDomain.BaseDirectory  控制台程序取得程序根目录地址
        /// </summary>
        /// <param name="xmlPathDatax">保存为新xml文档的路径包括后缀名</param>
        public static void GenerateXmlFile(string xmlPathDatax)
        {
            try
            {
                // 定义一个XDocument结构
                XDocument myXDoc = new XDocument(
                            new XElement("kkjspt.com",
                            new XElement("first",
                            new XElement("b_id", ""),
                            new XElement("b_isbn10", ""),
                            new XElement("b_isbn13", ""),
                            new XElement("b_title", ""),
                            new XElement("b_origin_title", ""),
                            new XElement("b_alt_title", ""),
                            new XElement("b_subtitle", ""),
                            new XElement("b_url", ""),
                            new XElement("b_alt", ""),
                            new XElement("b_images_large", ""),
                            new XElement("b_author", ""),
                            new XElement("b_publisher", ""),
                            new XElement("b_translator", ""),
                            new XElement("b_pubdate", ""),
                            new XElement("b_price", ""),
                            new XElement("b_pages", ""),
                            new XElement("b_author_intro", ""),
                            new XElement("b_summary", ""),
                            new XElement("b_tags", ""),
                            new XElement("b_binding", ""),
                            new XElement("b_catalog", "")
                    )));
                myXDoc.Save(xmlPathDatax);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 查询本地XML数据文件是否已经存在数据记录
        /// </summary>
        /// <param name="localPath">XML数据文件所在路径</param>
        /// <param name="isbn">要查询的ISBN码</param>
        /// <returns>false 不存在数据 true已经存在数据</returns>
        public static bool xmlHaveRecord(string localPath, string isbn)
        {
            bool insertData = true;
            //判断xml文件是否存在
            if (File.Exists(localPath))
            {

                //通过传入客户输入或者是扫描枪录入的ISBN码查询本地XML数据文件中是否已经存在该记录

                IEnumerable<XElement> myTargetNodes = from myTarget in rootNode.Descendants("first")
                                                      //where myTarget.Element("b_id").Value.Trim().Equals(isbn) || myTarget.Element("b_isbn10").Value.Trim().Equals(isbn) || myTarget.Element("b_isbn13").Value.Trim().Equals(isbn) || myTarget.Element("b_title").Value.Trim().Equals("") || myTarget.Element("b_title").Value.Trim().Equals("null")
                                                      where myTarget.Element("b_isbn13").Value.Trim().Equals(isbn)
                                                      select myTarget;
                foreach (XElement node in myTargetNodes)
                {
                    if (node.Element("b_title").Value.Trim().Equals(""))
                    {
                        insertData = false;//不存在记录
                        string b_title = node.Element("b_title").Value.Trim();
                    }
                }
            }
            else
            {
                ///创建xml数据文件
                GenerateXmlFile(localPath);
                insertData = false;
            }
            return insertData;
        }


        public static void insterXml(
               string b_id,
               string b_isbn10,
               string b_isbn13,
               string b_title,
               string b_origin_title,
               string b_alt_title,
               string b_subtitle,
               string b_url,
               string b_alt,
               string b_images_large,
               string b_author,
               string b_publisher,
               string b_translator,
               string b_pubdate,
               string b_price,
               string b_pages,
               string b_author_intro,
               string b_summary,
               string b_tags,
               string b_binding,
              string b_catalog
            )
        {
            
                //定义一个新节点
                XElement newNode = new XElement("first",
                                new XElement("b_id", b_id),
                                new XElement("b_isbn10", b_isbn10),
                                new XElement("b_isbn13", b_isbn13),
                                new XElement("b_title", b_title),
                                new XElement("b_origin_title", b_origin_title),
                                new XElement("b_alt_title", b_alt_title),
                                new XElement("b_subtitle", b_subtitle),
                                new XElement("b_url", b_url),
                                new XElement("b_alt", b_alt),
                                new XElement("b_images_large", b_images_large),
                                new XElement("b_author", Comm.replaceStr(b_author)),
                                new XElement("b_publisher", Comm.replaceStr(b_publisher)),
                                new XElement("b_translator", Comm.replaceStr(b_translator)),
                                new XElement("b_pubdate", b_pubdate),
                                new XElement("b_price", b_price),
                                new XElement("b_pages", b_pages),
                                new XElement("b_author_intro", Comm.ASCII_OTHER(b_author_intro)),
                                new XElement("b_summary", Comm.ASCII_OTHER(b_summary)),
                                new XElement("b_tags", b_tags),
                                new XElement("b_binding", Comm.replaceStr(b_binding)),
                                new XElement("b_catalog", Comm.replaceStr(b_catalog))
                               );
                rootNode.AddFirst(newNode);
                rootNode.Save(localPath);
           
        }

    }
}
#endregion