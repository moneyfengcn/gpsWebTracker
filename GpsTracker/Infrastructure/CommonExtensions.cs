using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Infrastructure
{
	public static class CommonExtensions
	{
		public static long? TryLong(this object o, long? nullVal = null)
		{
			if (o == null)
			{
				return nullVal;
			}
			if (long.TryParse(o.ToString(), out long result))
			{
				return result;
			}
			return nullVal;
		}

		public static int? TryInt(this object o, int? nullVal = null)
		{
			if (o == null)
			{
				return nullVal;
			}
			if (int.TryParse(o.ToString(), out int result))
			{
				return result;
			}
			return nullVal;
		}

		public static decimal? TryDecimal(this object o, decimal? nullVal = null)
		{
			if (o == null)
			{
				return nullVal;
			}
			if (decimal.TryParse(o.ToString(), out decimal result))
			{
				return result;
			}
			return nullVal;
		}

		public static bool? TryBoolean(this object o, string trueVal = "1", string falseVal = "0", bool? nullVal = null)
		{
			if (o == null)
			{
				return nullVal;
			}
			string text = o.ToString();
			if (bool.TryParse(text, out bool result))
			{
				return result;
			}
			if (trueVal == text)
			{
				return true;
			}
			if (falseVal == text)
			{
				return false;
			}
			return nullVal;
		}

		public static Guid? TryGuid(this object o, Guid? nullVal = null)
		{
			if (o == null)
			{
				return nullVal;
			}
			if (Guid.TryParse(o.ToString(), out Guid result))
			{
				return result;
			}
			return nullVal;
		}

		public static DateTime? TryDateTime(this object o, DateTime? nullValue = null)
		{
			if (o == null)
			{
				return nullValue;
			}
			if (DateTime.TryParse(o.ToString(), out DateTime result))
			{
				return result;
			}
			return nullValue;
		}

		public static string TryString(this object o, string nullValue = "")
		{
			if (o == null)
			{
				return nullValue;
			}
			return o.ToString();
		}

		public static string TryString(this object o, Func<string> fn, string nullValue = "")
		{
			if (o == null)
			{
				return nullValue;
			}
			return fn();
		}

		public static string TryTrimString(this object o, string nullValue = "")
		{
			if (o == null)
			{
				return nullValue;
			}
			return o.ToString().Trim();
		}
	}
}
