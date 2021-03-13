﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace JFToolKits
{
	/// <summary>
	/// 提供 <see cref="Char"/> 类的扩展方法。
	/// </summary>
	public static class CharExt
	{
		/// <summary>
		/// 所有用于不同进制的字符。
		/// </summary>
		internal static readonly char[] BaseDigits =
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
			'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
		};

		#region IsHex

		/// <summary>
		/// 指示指定的 Unicode 字符是否属于十六进制数字类别。
		/// </summary>
		/// <param name="ch">要计算的 Unicode 字符。</param>
		/// <returns>如果 <paramref name="ch"/> 是十进制数字，则为 <c>true</c>；
		/// 否则，为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 指示指定的 Unicode 字符是否属于十六进制数字类别。
		/// </summary>
		/// </overloads>
		public static bool IsHex(this char ch)
		{
			if (ch > 'f' || ch < '0') { return false; }
			if (ch > 'F')
			{
				return ch >= 'a';
			}
			return ch <= '9' || ch >= 'A';
		}
		/// <summary>
		/// 指示指定字符串中位于指定位置处的字符是否属于十六进制数字类别。
		/// </summary>
		/// <param name="str">一个字符串。</param>
		/// <param name="index">要计算的字符在 <paramref name="str"/> 中的位置。</param>
		/// <returns>如果 <paramref name="str"/> 中位于 <paramref name="index"/> 处的字符是十进制数字，
		/// 则为 <c>true</c>；否则，为 <c>false</c>。</returns>
		/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> 大于等于字符串的长度或小于零。</exception>
		public static bool IsHex(string str, int index)
		{
			return IsHex(str[index]);
		}

		#endregion // IsHex

		#region Escape

		/// <summary>
		/// 返回当前字符的转义字符串。
		/// </summary>
		/// <param name="ch">要获取转义字符串的字符。</param>
		/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
		/// <overloads>
		/// <summary>
		/// 返回当前字符的转义字符串。
		/// </summary>
		/// </overloads>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v），会返回其转义形式。</para>
		/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string Escape(this char ch)
		{
			return Escape(ch, null);
		}
		/// <summary>
		/// 返回当前字符的转义字符串。
		/// </summary>
		/// <param name="ch">要获取转义字符串的字符。</param>
		/// <param name="customEscape">自定义的需要转义的字符。</param>
		/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v）以及自定义的字符，会返回其转义形式。</para>
		/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string Escape(this char ch, ISet<char> customEscape)
		{
			// 转换字符转义。
			switch (ch)
			{
				case '\0': return "\\0";
				case '\\': return "\\\\";
				case '\a': return "\\a";
				case '\b': return "\\b";
				case '\f': return "\\f";
				case '\n': return "\\n";
				case '\r': return "\\r";
				case '\t': return "\\t";
				case '\v': return "\\v";
			}
			if (customEscape != null && customEscape.Contains(ch))
			{
				return "\\" + ch;
			}
			return EscapeUnicode(ch);
		}
		/// <summary>
		/// 返回当前字符的 Unicode 转义字符串。
		/// </summary>
		/// <param name="ch">要获取转义字符串的字符。</param>
		/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
		/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string EscapeUnicode(this char ch)
		{
			if (ch >= ' ' && ch <= '~')
			{
				return ch.ToString(CultureInfo.InvariantCulture);
			}
			return string.Concat("\\u", BaseDigits[ch >> 12], BaseDigits[(ch >> 8) & 0xF],
				BaseDigits[(ch >> 4) & 0xF], BaseDigits[ch & 0xF]);
		}

		#endregion // Escape

		#region Width

		/// <summary>
		/// 返回指定字符的宽度。
		/// </summary>
		/// <param name="ch">要获取宽度的字符。</param>
		/// <returns>指定字符的宽度，结果可能是 <c>0</c>、<c>1</c> 或 <c>2</c>。</returns>
		/// <remarks>此方法基于 Unicode 标准 6.3 版。详情请参见 
		/// <see href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</see>。
		/// 以及 Markus Kuhn 的 C 语言实现 <see href="http://www.cl.cam.ac.uk/~mgk25/ucs/wcwidth.c">wcwidth</see>。
		/// 返回 <c>0</c> 表示控制字符、非间距字符或格式字符，<c>1</c> 表示半角字符，
		/// <c>2</c> 表示全角字符。</remarks>
		/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
		/// <overloads>
		/// <summary>
		/// 返回指定字符的宽度。
		/// </summary>
		/// </overloads>
		public static int Width(this char ch)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 2);
			return Width((int)ch);
		}
		/// <summary>
		/// 返回指定字符串中位于指定位置的字符的宽度。
		/// </summary>
		/// <param name="str">要获取宽度的字符字符串。</param>
		/// <param name="index"><paramref name="str"/> 中的字符位置。</param>
		/// <returns><paramref name="str"/> 中指定字符的宽度，结果可能是 <c>0</c>、<c>1</c> 或 <c>2</c>。</returns>
		/// <remarks>此方法基于 Unicode 标准 6.3 版。详情请参见 
		/// <see href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</see>。 
		/// 返回 <c>0</c> 表示控制字符、非间距字符或格式字符，<c>1</c> 表示半角字符，
		/// <c>2</c> 表示全角字符。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> 大于等于字符串的长度或小于零。</exception>
		/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
		public static int Width(string str, int index)
		{
			if (str == null)
			{
				throw CommonExceptions.ArgumentNull("str");
			}
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			if (index >= str.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("index", index);
			}
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 2);
			return Width(char.ConvertToUtf32(str, index));
		}
		/// <summary>
		/// 返回指定字符的宽度。
		/// </summary>
		/// <param name="ch">要获取宽度的字符。</param>
		/// <returns>指定字符的宽度，结果可能是 <c>0</c>、<c>1</c> 或 <c>2</c>。</returns>
		internal static int Width(int ch)
		{
			Contract.Requires(ch >= 0);
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 2);
			UnicodeCategory category;
			if (ch <= char.MaxValue)
			{
				category = CharUnicodeInfo.GetUnicodeCategory((char)ch);
			}
			else
			{
				category = CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(ch), 0);
			}
			switch (category)
			{
				case UnicodeCategory.Control:
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.EnclosingMark:
				case UnicodeCategory.Format:
					return 0;
			}
			// 宽字符范围：
			// 0x1100 ~ 0x115F：Hangul Jamo init. consonants
			// 0x2329, 0x232A：左右尖括号〈〉
			// 0x2E80 ~ 0xA4CF 除了 0x303F：CJK ... YI
			// 0xAC00 ~ 0xD7A3：Hangul Syllables
			// 0xF900 ~ 0xFAFF：CJK Compatibility Ideographs
			// 0xFE10 ~ 0xFE19：Vertical forms
			// 0xFE30 ~ 0xFE6F：CJK Compatibility Forms
			// 0xFF00 ~ 0xFF60：Fullwidth Forms
			// 0xFFE0 ~ 0xFFE6, 0x20000 ~ 0x2FFFD, 0x30000 ~ 0x3FFFD
			if (ch < 0x1100 || ch > 0x3FFFD)
			{
				return 1;
			}
			if (ch >= 0x20000)
			{
				return ch <= 0x2FFFD || ch >= 0x30000 ? 2 : 1;
			}
			if (ch > 0xFFE6)
			{
				return 1;
			}
			if (ch <= 0xA4CF)
			{
				if (ch >= 0x2E80)
				{
					return ch != 0x303F ? 2 : 1;
				}
				if (ch <= 0x115F)
				{
					return 2;
				}
				return ch == 0x2329 || ch == 0x232A ? 2 : 1;
			}
			if (ch <= 0xD7A3)
			{
				return ch >= 0xAC00 ? 2 : 1;
			}
			if (ch < 0xF900)
			{
				return 1;
			}
			if (ch <= 0xFAFF)
			{
				return 2;
			}
			if (ch < 0xFE10)
			{
				return 1;
			}
			if (ch < 0xFF00)
			{
				if (ch > 0xFE6F)
				{
					return 1;
				}
				if (ch >= 0xFE30 || ch <= 0xFE19)
				{
					return 2;
				}
				return 1;
			}
			if (ch <= 0xFF60 || ch >= 0xFFE0)
			{
				return 2;
			}
			return 1;
		}

		#endregion // Width

	}
}
