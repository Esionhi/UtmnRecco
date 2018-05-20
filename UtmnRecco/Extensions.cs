using System;
using System.Collections.Generic;
using System.Linq;

namespace UtmnRecco
{
	public static class Extensions
	{
		static readonly char[] splittingChars = new[] { ' ', '\r', '\n' };

		public static IEnumerable<string> Words(this string str) =>
			str.Split(splittingChars, StringSplitOptions.RemoveEmptyEntries)
				.Select(w => new string(w.Where(ch => Char.IsLetterOrDigit(ch)).ToArray()));
	}
}