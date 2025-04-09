using System;
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

		public Trigger(bool prime) => _flag = prime ? 1 : 0;

		public Trigger() => _flag = 0;

		[Obsolete("Use `Trigger.TryFire` instead.")]
		public bool Fire => Interlocked.Exchange(ref _flag, 0) == 1;

		public bool TryFire => Interlocked.Exchange(ref _flag, 0) == 1;

		public void Prime() => _flag = 1;
	}
}