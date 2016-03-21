using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBNSCAN
{
    class ISBNcheck
    {
        private String isbn;

        public String ISBN
        {
            get { return isbn; }

            set { isbn = value; }
        }


        public ISBNcheck(String iSBN)
        {
            this.isbn = iSBN;
        }

        // 获取某位的ISBN字符  
        private static int GetISBNAt(String isbn, int index, bool xEnable)
        {
            char c = Convert.ToChar(isbn.Substring(index, 1));

            int n = c - '0';

            if (n < 0 || n > 9)
            {
                if (xEnable && (c == 'X' || c == 'x'))
                {
                    n = 10;
                }
            }
            return n;
        }
        /// <summary>
        /// 功    能：检查ISBN号码  
        /// 使用样例：bool isTrueIsbn = ISBNClass.CheckISBN("9787121000522");  
        /// </summary>
        /// <param name="isbn">7687687787</param>
        /// <returns>true表示通过验证 flase不合格的ISBN号码</returns>
        public static bool CheckISBN(String isbn)
        {
            try
            {
                int checkNum = 0;

                if (isbn.Length == 10)
                {
                    int start = 10;

                    int total = 0;

                    for (int i = 0; i < isbn.Length - 1; ++i)
                    {
                        total += GetISBNAt(isbn, i, false) * start--;
                    }

                    checkNum = total % 11;

                    if (checkNum > 0)
                    {
                        checkNum = 11 - checkNum;
                    }
                }
                else
                {
                    int total = 0;

                    for (int i = 0; i < isbn.Length - 1; ++i)
                    {
                        total += GetISBNAt(isbn, i, false) * (i % 2 == 0 ? 1 : 3);
                    }

                    checkNum = total % 10;

                    if (checkNum > 0)
                    {
                        checkNum = 10 - checkNum;
                    }
                }

                return GetISBNAt(isbn, isbn.Length - 1, true) == checkNum;
            }
            catch
            {
                return false;
            }

        }
    }
}
