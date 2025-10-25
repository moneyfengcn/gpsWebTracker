using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GpsTracker.Infrastructure
{
    public static class StringExtensions
    {
        private static Regex _emailregex = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$", RegexOptions.IgnoreCase);

        private static Regex _mobileregex = new Regex("^1[0-9]{10}$");

        private static Regex _phoneregex = new Regex("^(\\d{3,4}-?)?\\d{7,8}$");

        private static Regex _ipregex = new Regex("^(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])$");

        private static Regex _dateregex = new Regex("(\\d{4})-(\\d{1,2})-(\\d{1,2})");

        private static Regex _numericregex = new Regex("^[-]?[0-9]+(\\.[0-9]+)?$");

        private static Regex _zipcoderegex = new Regex("^\\d{6}$");

        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch("^[\\u4e00-\\u9fa5]+$", str);
        }

        public static bool IsEmail(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            return _emailregex.IsMatch(s);
        }

        public static bool IsMobile(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            return _mobileregex.IsMatch(s);
        }

        public static bool IsPhone(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            return _phoneregex.IsMatch(s);
        }

        public static bool IsIP(this string s)
        {
            return _ipregex.IsMatch(s);
        }

        public static bool IsIdCard(this string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            if (id.Length == 18)
            {
                return CheckIDCard18(id);
            }
            if (id.Length == 15)
            {
                return CheckIDCard15(id);
            }
            return false;
        }

        private static bool CheckIDCard18(string Id)
        {
            long result = 0L;
            if (!long.TryParse(Id.Remove(17), out result) || (double)result < Math.Pow(10.0, 16.0) || !long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out result))
            {
                return false;
            }
            if ("11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91".IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string s = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime result2 = default(DateTime);
            if (!DateTime.TryParse(s, out result2))
            {
                return false;
            }
            string[] array = "1,0,x,9,8,7,6,5,4,3,2".Split(',');
            string[] array2 = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(',');
            char[] array3 = Id.Remove(17).ToCharArray();
            int num = 0;
            for (int i = 0; i < 17; i++)
            {
                num += int.Parse(array2[i]) * int.Parse(array3[i].ToString());
            }
            int result3 = -1;
            Math.DivRem(num, 11, out result3);
            if (array[result3] != Id.Substring(17, 1).ToLower())
            {
                return false;
            }
            return true;
        }

        private static bool CheckIDCard15(string Id)
        {
            long result = 0L;
            if (!long.TryParse(Id, out result) || (double)result < Math.Pow(10.0, 14.0))
            {
                return false;
            }
            if ("11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91".IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string s = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime result2 = default(DateTime);
            if (!DateTime.TryParse(s, out result2))
            {
                return false;
            }
            return true;
        }

        public static bool IsDate(this string s)
        {
            return _dateregex.IsMatch(s);
        }

        public static bool IsNumeric(this string numericStr)
        {
            return _numericregex.IsMatch(numericStr);
        }

        public static bool IsZipCode(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return true;
            }
            return _zipcoderegex.IsMatch(s);
        }

        public static bool IsImgFileName(this string fileName)
        {
            if (fileName.IndexOf(".") == -1)
            {
                return false;
            }
            string text = fileName.Trim().ToLower();
            string text2 = text.Substring(text.LastIndexOf("."));
            switch (text2)
            {
                default:
                    return text2 == ".gif";
                case ".png":
                case ".bmp":
                case ".jpg":
                case ".jpeg":
                    return true;
            }
        }

        public static int Count(this string str, string compare)
        {
            int num = str.IndexOf(compare);
            if (num != -1)
            {
                return 1 + Count(str.Substring(num + compare.Length), compare);
            }
            return 0;
        }

        public static string Fmt(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string Sub(this string str, int length)
        {
            if (str.Length <= length)
            {
                return str;
            }
            return str.Substring(0, length);
        }

        public static bool IsIPV4(this string str)
        {
            string[] array = str.Split('.');
            for (int i = 0; i < array.Length; i++)
            {
                if (!Regex.IsMatch("^\\d+$", array[i]))
                {
                    return false;
                }
                if (Convert.ToUInt16(array[i]) > 255)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsIPV6(this string str)
        {
            if (str.Split(':').Length > 8)
            {
                return false;
            }
            int num = Count(str, "::");
            if (num > 1)
            {
                return false;
            }
            if (num == 0)
            {
                return Regex.IsMatch("^([\\da-f]{1,4}:){7}[\\da-f]{1,4}$", str);
            }
            return Regex.IsMatch("^([\\da-f]{1,4}:){0,5}::([\\da-f]{1,4}:){0,5}[\\da-f]{1,4}$", str);
        }




        public static string FromNow(this string formatter)
        {
            return DateTime.Now.ToString(formatter);
        }

        public static string FromTime(this string formatter, DateTime time)
        {
            return time.ToString(formatter);
        }

        [Obsolete]
        public static DateTime ToDateTimeFormStamp(this string timeStamp)
        {
            DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long ticks = long.Parse(timeStamp + "0000000");
            TimeSpan value = new TimeSpan(ticks);
            return dateTime.Add(value);
        }

        [Obsolete]
        public static long ToTimeStamp(this DateTime time)
        {
            DateTime d = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - d).TotalSeconds;
        }
    }
}
