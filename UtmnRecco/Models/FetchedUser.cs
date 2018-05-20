using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace UtmnRecco.Models
{
	public class FetchedUser : IdModel, IWordsExpandable
	{
		public FetchedUniversity[] Universities { get; set; }

		[JsonProperty("followers_count")]
		public int? FollowersCount { get; set; }

		public string Interests { get; set; }

		public string Music { get; set; }

		public string Activities { get; set; }

		public string Movies { get; set; }

		public string TV { get; set; }

		public string Books { get; set; }

		public string Games { get; set; }

		public string About { get; set; }

		public CountedResult<FetchedGroup> Subscriptions { get; set; }

		public CountedResult<FetchedWallItem> Wall { get; set; }

		public IEnumerable<string> GetWords()
		{
			List<string> list = new List<string>();
			list.AddRange(Interests == null ? Enumerable.Empty<string>() : Interests.Words());
			list.AddRange(Music == null ? Enumerable.Empty<string>() : Music.Words());
			list.AddRange(Activities == null ? Enumerable.Empty<string>() : Activities.Words());
			list.AddRange(Movies == null ? Enumerable.Empty<string>() : Movies.Words());
			list.AddRange(TV == null ? Enumerable.Empty<string>() : TV.Words());
			list.AddRange(Books == null ? Enumerable.Empty<string>() : Books.Words());
			list.AddRange(Games == null ? Enumerable.Empty<string>() : Games.Words());
			list.AddRange(About == null ? Enumerable.Empty<string>() : About.Words());
			list.AddRange(Subscriptions.Items.SelectMany(s => s?.Name == null ? Enumerable.Empty<string>() : s.GetWords()));
			list.AddRange(Wall.Items.SelectMany(wi => wi.GetWords()));
			return list;
		}
	}
}