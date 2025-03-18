using System.Threading;

namespace InfiniteCanvas.Utilities
{
	public class Trigger
	{
		/* I could do one-shot bools with implicit operators for nice syntax like
		 ```
		 if (trigger) // do stuff
		 ```
		 but I like the explicit check better for readability; it's also inherently threadsafe like this
		*/
		private int _flag;

		public bool Fire => IsPrimed();

		public void Prime() => _flag = 1;

		private bool IsPrimed() => Interlocked.Exchange(ref _flag, 0) == 1;
	}
}