using Newtonsoft.Json;

namespace UtmnRecco.Models
{
	public class FetchedUniversity
	{
		[JsonProperty("faculty")]
		public int FacultyId { get; set; }
	}
}