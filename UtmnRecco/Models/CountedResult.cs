namespace UtmnRecco.Models
{
	public class CountedResult<T>
	{
		public int Count { get; set; }

		public T[] Items { get; set; }
	}
}