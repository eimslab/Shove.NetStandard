using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Checksum;
using System.Collections.Generic;

namespace Shove
{
    /// <summary>
    /// 字符串相关。
    /// </summary>
    public class _String
    {
        /// <summary>
        /// 字符 ch 在 字符串 str 中出现的次数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static int StringAt(string str, char ch)
        {
            if (str == null)
                return 0;

            int Result = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ch)
                    Result++;
            }

            return Result;
        }

        /// <summary>
        /// 替换字符串中的某个字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ch"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static string ReplaceAt(string input, char ch, int pos)
		{
            return input.Substring(0, pos) + ch.ToString() + ((pos < input.Length) ? input.Substring(pos + 1) : "");
		}

        /// <summary>
        /// 翻转字符串
        /// </summary>
        /// <param name="sSourceStr"></param>
        /// <returns></returns>
        public static string Reverse(string sSourceStr)
        {
            StringBuilder sReversed = new StringBuilder();
            for (int i = sSourceStr.Length - 1; i >= 0; i--)
                sReversed.Append(sSourceStr[i]);
            return sReversed.ToString();
        }

        /// <summary>
        /// 字串截取(考虑汉字)
        /// </summary>
        /// <param name="Input">要截取的字符串</param>
        /// <param name="Length">要截取的字符串长度</param>
        /// <returns></returns>
        public static string Cut(string Input, int Length)
        {
            if (Length < 0)
            {
                Length = 0;
            }

            Length *= 2;

            if (GetLength(Input) <= Length)
            {
                return Input;
            }

            string Result = "";
            int i = 0;

            while ((GetLength(Result) < Length) && (i < Input.Length))
            {
                Result += Input[i].ToString();

                i++;
            }

            if (Result != Input)
            {
                Result += "..";
            }

            return Result;
        }

        /// <summary>
        /// HTML 格式字串截取
        /// </summary>
        /// <param name="Input">要截取的字符</param>
        /// <param name="Length">要截取的字符长度</param>
        /// <returns></returns>
        public static string HtmlTextCut(string Input, int Length)
        {
            if (Length < 0)
            {
                Length = 0;
            }

            Length *= 2;

            if (!Input.Contains("<body>"))
            {
                Input = "<body>" + Input;
            }

            Input = HTML.HTML.StandardizationHTML(Input, true, true, true);
            Input = HTML.HTML.GetText(Input, 0);

            return Cut(Input, Length);
        }

        /// <summary>
        /// 根据长度拆分字符串，不拆短汉字而产生乱码
        /// </summary>
        /// <param name="Input">输入的字符串</param>
        /// <param name="PartLength">每部分的长度</param>
        /// <returns>返回被拆分的多部分字符串数组</returns>
        public static string[] Split(string Input, int PartLength)
        {
            return Split(Input, PartLength, 0);
        }

        /// <summary>
        /// 根据长度拆分字符串，不拆短汉字而产生乱码
        /// </summary>
        /// <param name="Input">输入的字符串</param>
        /// <param name="PartLength">每部分的长度</param>
        /// <param name="MaxPartNum">最多只返回几个部分，右边多余的部分截取</param>
        /// <returns>返回被拆分的多部分字符串数组</returns>
        public static string[] Split(string Input, int PartLength, int MaxPartNum)
        {
            if (String.IsNullOrEmpty(Input))
            {
                return null;
            }

            IList<string> list = new List<string>();
            IList<int> list_length = new List<int>();

            list.Add("");
            list_length.Add(0);

            int locate = 0;

            for (int i = 0; i < Input.Length; i++)
            {
                string ch = Input[i].ToString();
                int len = System.Text.Encoding.Default.GetBytes(ch).Length;

                if (list_length[locate] + len > PartLength)
                {
                    locate++;

                    if ((MaxPartNum > 0) && (locate >= MaxPartNum))
                    {
                        break;
                    }

                    list.Add("");
                    list_length.Add(0);
                }

                list[locate] += ch;
                list_length[locate] += len;
            }

            string[] Result = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                Result[i] = list[i];
            }

            return Result;
        }

        /// <summary>
        /// Byte[] 转换成16进制字符串，结果带 0x 前缀。 _Byte 中有此方法，但结果不带 0x 前缀
        /// </summary>
        /// <param name="Input">input bytes[]</param>
        /// <returns>16进制字符串：0x......</returns>
        public static string BytesToHexString(byte[] Input)
        {
            string Result = "0x";

            if (Input.Length == 0)
            {
                return Result;
            }

            foreach (byte b in Input)
            {
                Result += b.ToString("X").PadLeft(2, '0');
            }

            return Result;
        }

        /// <summary> 
        /// 检测含有中文字符串的实际长度 ，一个汉字或全角字符算 2 个长度
        /// </summary> 
        /// <param name="str">字符串</param> 
        public static int GetLength(string str)
        {
            //System.Text.ASCIIEncoding n = new System.Text.ASCIIEncoding();
            byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
            int len = 0; // len 为字符串之实际长度 
            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                if (bytes[i] == 63) //判断是否为汉字或全角符号 
                {
                    len++;
                }

                len++;
            }

            return len;
        }

        /// <summary> 
        /// 检测含有中文字符串的实际长度，使用指定的字符集
        /// </summary> 
        /// <param name="str">字符串</param> 
        public static int GetBytesLength(string str)
        {
            return GetBytesLength(str, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 检测含有中文字符串的实际长度，使用指定的字符集
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static int GetBytesLength(string str, System.Text.Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);

            if (bytes != null)
            {
                return bytes.Length;
            }

            return 0;
        }

        /// <summary>
        /// 是否汉字
        /// </summary>
        public static bool isChineseCharacters(char ch)
        {
            int Unicode = (int)(ch) - 19968;
            return ((Unicode >= 0) && (Unicode <= 20900));
        }

        /// <summary>
        /// 是否全角字符
        /// </summary>
        public static bool isDBCCharacters(char ch)
        {
            int Unicode = (int)(ch);

            return ((Unicode == 12288) || ((Unicode > 65280) && (Unicode < 65375)));
        }

        /// <summary>
        /// 将字符串转化为标准的“标识符”
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StandardizationIdentifier(string str)
        {
            str = str.Trim();
            if (str == "")
                return "CHERY_ADD";

            if ("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_".IndexOf(str[0].ToString()) < 0)
                str = "CHERY_ADD_" + str;

            int i;
            string StandardizationChars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
            string d = "";
            for (i = 0; i < str.Length; i++)
            {
                if (StandardizationChars.IndexOf(str[i].ToString()) >= 0)
                    d += str[i].ToString();
            }

            return d;
        }

        /// <summary>
        /// 字符串压缩
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] Compress(string str)
        {
            byte[] data = System.Text.UnicodeEncoding.Unicode.GetBytes(str);
            Deflater f = new Deflater(Deflater.BEST_COMPRESSION);
            f.SetInput(data);
            f.Finish();

            MemoryStream o = new MemoryStream(data.Length);
            try
            {
                byte[] buf = new byte[1024];
                while (!f.IsFinished)
                {
                    int got = f.Deflate(buf);
                    o.Write(buf, 0, got);
                }
            }
            finally
            {
                o.Close();
            }

            byte[] Result = o.ToArray();
            if ((Result.Length % 2) == 0)
                return Result;

            byte[] Result2 = new byte[Result.Length + 1];
            for (int i = 0; i < Result.Length; i++)
                Result2[i] = Result[i];
            Result2[Result.Length] = 0;
            return Result2;
        }

        /// <summary>
        /// 字符串解压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decompress(byte[] data)
        {
            if (data == null)
                return "";
            if (data.Length == 0)
                return "";

            Inflater f = new Inflater();
            f.SetInput(data);

            MemoryStream o = new MemoryStream(data.Length);
            try
            {
                byte[] buf = new byte[1024];
                while (!f.IsFinished)
                {
                    int got = f.Inflate(buf);
                    o.Write(buf, 0, got);
                }
            }
            finally
            {
                o.Close();
            }
            return System.Text.UnicodeEncoding.Unicode.GetString(o.ToArray());
        }

        //下面是压缩\解压缩字符串2个函数的另外一种写法
        /*
        public static byte[] Compress(string s)
        {
            Byte[] pBytes = System.Text.UnicodeEncoding.Unicode.GetBytes(s);

            //创建支持内存存储的流
            MemoryStream mMemory = new MemoryStream();
            Deflater mDeflater = new Deflater(ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_COMPRESSION);
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream mStream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(mMemory,mDeflater,131072);

            mStream.Write(pBytes, 0, pBytes.Length);
            mStream.Close();

            return mMemory.ToArray();
        }

        public static string Decompress(byte[] data)
        {
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream mStream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(new MemoryStream(data));
            
            //创建支持内存存储的流
            MemoryStream mMemory = new MemoryStream();
            Int32 mSize;

            Byte[] mWriteData = new Byte[4096];
            while(true)
            {
                mSize = mStream.Read(mWriteData, 0, mWriteData.Length);
                if (mSize > 0)
                {
                    mMemory.Write(mWriteData, 0, mSize);
                }
                else
                {
                    break;
                }
            }

            mStream.Close();
            return System.Text.UnicodeEncoding.Unicode.GetString(mMemory.ToArray());
        }
        */

        /// <summary>
        /// 将字符串转为 Base64 编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeBase64(string str)
        {
            string strResult = "";

            if ((str != null) && (str != ""))
            {
                strResult = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(str));
            }

            return strResult;
        }

        /// <summary>
        /// 将 Base64 编码转为字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeBase64(string str)
        {
            string strResult = "";

            if ((str != null) && (str != ""))
            {
                strResult = System.Text.ASCIIEncoding.Default.GetString(Convert.FromBase64String(str));
            }

            return strResult;
        }

        /// <summary>
        /// Byte[] 转换成16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            string Result = "0x";

            if (bytes.Length == 0)
            {
                return Result;
            }

            foreach (byte b in bytes)
            {
                Result += b.ToString("X").PadLeft(2, '0');
            }

            return Result;
        }

        /// <summary>
        /// 字符串转换编码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="srcEncoding"></param>
        /// <param name="tarEncoding"></param>
        /// <returns></returns>
        public static string ConvertEncoding(string input, string srcEncoding, string tarEncoding)
        {
            Encoding _srcEncoding = Encoding.GetEncoding(srcEncoding);
            Encoding _tarEncoding = Encoding.GetEncoding(tarEncoding);

            return ConvertEncoding(input, _srcEncoding, _tarEncoding);
        }

        /// <summary>
        /// 字符串转换编码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="srcEncoding"></param>
        /// <param name="tarEncoding"></param>
        /// <returns></returns>
        public static string ConvertEncoding(string input, Encoding srcEncoding, Encoding tarEncoding)
        {
            if (srcEncoding == tarEncoding)
            {
                return input;
            }

            byte[] temp = srcEncoding.GetBytes(input);
            byte[] temp2 = Encoding.Convert(srcEncoding, tarEncoding, temp);

            return tarEncoding.GetString(temp2);
        }

        /// <summary>
        /// 校验相关
        /// </summary>
        public class Valid
        {
            /// <summary>
            /// 校验 Email 格式
            /// </summary>
            /// <param name="Email"></param>
            /// <returns></returns>
            public static bool isEmail(string Email)
            {
                return Regex.IsMatch(Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }

            /// <summary>
            /// 校验身份证格式_大陆
            /// </summary>
            /// <param name="IDCardNumber"></param>
            /// <returns></returns>
            public static bool isIDCardNumber(string IDCardNumber)
            {
                return Regex.IsMatch(IDCardNumber, @"(^\d{17}|^\d{14})(\d|x|X|y|Y)$");
            }

            /// <summary>
            /// 校验身份证格式_台湾
            /// </summary>
            /// <param name="IDCardNumber"></param>
            /// <returns></returns>
            public static bool isIDCardNumber_Taiwan(string IDCardNumber)
            {
                return Regex.IsMatch(IDCardNumber, @"[A-Za-z][12]\d{8}");
            }

            /// <summary>
            /// 校验身份证格式_香港
            /// </summary>
            /// <param name="IDCardNumber"></param>
            /// <returns></returns>
            public static bool isIDCardNumber_Hongkong(string IDCardNumber)
            {
                return Regex.IsMatch(IDCardNumber, @"[A-Za-z]{1,2}\d{6}\([Aa0-9]\)");
            }

            /// <summary>
            /// 校验身份证格式_新加坡
            /// </summary>
            /// <param name="IDCardNumber"></param>
            /// <returns></returns>
            public static bool isIDCardNumber_Singapore(string IDCardNumber)
            {
                return Regex.IsMatch(IDCardNumber, @"\d{7}[A-JZa-jz]");
            }

            /// <summary>
            /// 校验身份证格式_澳门
            /// </summary>
            /// <param name="IDCardNumber"></param>
            /// <returns></returns>
            public static bool isIDCardNumber_Macau(string IDCardNumber)
            {
                return Regex.IsMatch(IDCardNumber, @"\d{7}\([0-9]\)");
            }

            /// <summary>
            /// 校验银行卡格式
            /// </summary>
            /// <param name="BankCardNumber"></param>
            /// <returns></returns>
            public static bool isBankCardNumber(string BankCardNumber)
            {
                int Valid_BankCardNumberMinLength = _Convert.StrToInt(System.Configuration.ConfigurationManager.AppSettings["Valid_BankCardNumberMinLength"], 12);
                int Valid_BankCardNumberMaxLength = _Convert.StrToInt(System.Configuration.ConfigurationManager.AppSettings["Valid_BankCardNumberMaxLength"], 22);

                if (Valid_BankCardNumberMinLength < 1)
                {
                    Valid_BankCardNumberMinLength = 1;
                }

                if (Valid_BankCardNumberMaxLength < Valid_BankCardNumberMinLength)
                {
                    Valid_BankCardNumberMaxLength = Valid_BankCardNumberMinLength;
                }

                string Pattern = "^\\d{" + Valid_BankCardNumberMinLength.ToString() + "," + Valid_BankCardNumberMaxLength.ToString() + "}$";
                return Regex.IsMatch(BankCardNumber, Pattern);
            }

            /// <summary>
            /// 校验日期时间格式
            /// </summary>
            /// <param name="DateTimeString"></param>
            /// <returns></returns>
            public static bool isDateTime(string DateTimeString)
            {
                return Regex.IsMatch(DateTimeString, @"^((\d{2}(([02468][048])|([13579][26]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|([1-2][0-9])))))|(\d{2}(([02468][1235679])|([13579][01345789]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|(1[0-9])|(2[0-8]))))))(\s(((0?[1-9])|(1[0-2]))\:([0-5][0-9])((\s)|(\:([0-5][0-9])\s))([AM|PM|am|pm]{2,2})))?$");
            }

            /// <summary>
            /// 校验日期格式
            /// </summary>
            /// <param name="DateString"></param>
            /// <returns></returns>
            public static bool isDate(string DateString)
            {
                return Regex.IsMatch(DateString, @"^\d{4}[\-\/\s]?((((0[13578])|(1[02]))[\-\/\s]?(([0-2][0-9])|(3[01])))|(((0[469])|(11))[\-\/\s]?(([0-2][0-9])|(30)))|(02[\-\/\s]?[0-2][0-9]))$");
            }

            /// <summary>
            /// 校验 IP 地址格式
            /// </summary>
            /// <param name="Address"></param>
            /// <returns></returns>
            public static bool isIPAddress(string Address)
            {
                return Regex.IsMatch(Address, @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
            }

            /// <summary>
            /// 校验域名格式，格式要求域名带 http://  ftp://  https:// 等等前缀
            /// </summary>
            /// <param name="Url"></param>
            /// <returns></returns>
            public static bool isUrl(string Url)
            {
                return isUrl(Url, true);
            }

            /// <summary>
            /// 校验域名格式
            /// </summary>
            /// <param name="Url"></param>
            /// <param name="WithPreFix">是否需要代 http:// ftp:// https:// 等前缀</param>
            /// <returns></returns>
            public static bool isUrl(string Url, bool WithPreFix)
            {
                if (WithPreFix)
                {
                    return Regex.IsMatch(Url, @"^(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?$");
                }
                else
                {
                    return Regex.IsMatch(Url, @"^[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?$");
                }

            }

            /// <summary>
            /// 校验手机号码
            /// </summary>
            /// <param name="MobileNumber"></param>
            /// <returns></returns>
            public static bool isMobile(string MobileNumber)
            {
                return Regex.IsMatch(MobileNumber, @"^((13[0-9])|(14[5,7])|(15[0-3,5-9])|(17[0,3,5-8])|(18[0-9])|166|198|199|(147))\d{8}$");
            }
        }
    }
}
