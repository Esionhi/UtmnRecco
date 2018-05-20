using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace UtmnRecco.Models
{
	public class FetchedWallItem : IWordsExpandable
	{
		public int Id { get; set; }

		[JsonProperty("copy_history", NullValueHandling = NullValueHandling.Ignore)]
		public FetchedCopyHistory[] CopyHistory { get; set; }

		public IEnumerable<string> GetWords() => CopyHistory == null ? Enumerable.Empty<string>() : CopyHistory.SelectMany(ch => ch.GetWords());
	}
}