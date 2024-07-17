namespace SharpGDX.Utils.Async
{
	/** Returned by {@link AsyncExecutor#submit(AsyncTask)}, allows to poll for the result of the asynch workload.
 * @author badlogic */
	public class AsyncResult<T>
	{
		private readonly Task<T> future;

		internal AsyncResult(Task<T> future)
		{
			this.future = future;
		}

		/** @return whether the {@link AsyncTask} is done */
		public bool isDone()
		{
			return future.IsCompleted;
		}

		/** @return waits if necessary for the computation to complete and then returns the result
		 * @throws GdxRuntimeException if there was an error */
		public T? get()
		{
			try
			{
				return future.Result;
			}
			// TODO: Not sure that this would be acceptable here.
			catch (ThreadInterruptedException ex)
			{
				return default;
			}
			// TODO: Is this too broad?
			catch (Exception ex)
			{
				// TODO: not sure if this goes deep enough for original: ex.getCause()
				throw new GdxRuntimeException(ex.InnerException);
			}
		}
	}
}
