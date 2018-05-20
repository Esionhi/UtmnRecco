using System.Collections.Generic;

namespace UtmnRecco.Models
{
	public class FetchedCopyHistory : IWordsExpandable
	{
		public int Id { get; set; }

		public string Text { get; set; }

		public IEnumerable<string> GetWords() => Text.Words();
	}
}