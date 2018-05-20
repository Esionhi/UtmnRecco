using System.Collections.Generic;

namespace UtmnRecco.Models
{
	public class FetchedGroup : IdModel, IWordsExpandable
	{
		public string Name { get; set; }

		public IEnumerable<string> GetWords() => Name.Words();
	}
}