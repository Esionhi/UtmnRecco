using System.Collections.Generic;

namespace UtmnRecco.Models
{
	public interface IWordsExpandable
	{
		IEnumerable<string> GetWords();
	}
}