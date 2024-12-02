using SharpGDX.Utils;


namespace SharpGDX.Scenes.Scene2D
{
    /// <summary>
    /// Actions attach to an {@link Actor} and perform some task, often over time.
    /// </summary>
    abstract public class Action : IPoolable
	{
        /// <summary>
        /// The actor this action is attached to, or null if it is not attached.
        /// </summary>
        protected Actor? Actor;

        /// <summary>
        /// The actor this action targets, or null if a target has not been set.
        /// </summary>
        protected Actor? Target;

	private Pool? _pool;

	/** Updates the action based on time. Typically this is called each frame by {@link Actor#act(float)}.
	 * @param delta Time in seconds since the last frame.
	 * @return true if the action is done. This method may continue to be called after the action is done. */
	abstract public bool Act(float delta);

        /// <summary>
        /// Sets the state of the action so it can be run again.
        /// </summary>
        public virtual void Restart()
	{
	}

	/** Sets the actor this action is attached to. This also sets the {@link #setTarget(Actor) target} actor if it is null. This
	 * method is called automatically when an action is added to an actor. This method is also called with null when an action is
	 * removed from an actor.
	 * <p>
	 * When set to null, if the action has a {@link #setPool(Pool) pool} then the action is {@link Pool#free(Object) returned} to
	 * the pool (which calls {@link #reset()}) and the pool is set to null. If the action does not have a pool, {@link #reset()} is
	 * not called.
	 * <p>
	 * This method is not typically a good place for an action subclass to query the actor's state because the action may not be
	 * executed for some time, eg it may be {@link DelayAction delayed}. The actor's state is best queried in the first call to
	 * {@link #act(float)}. For a {@link TemporalAction}, use TemporalAction#begin(). */
	public virtual void SetActor(Actor? actor)
	{
		this.Actor = actor;
		
        if (Target == null) SetTarget(actor);
		
        if (actor == null)
		{
			if (_pool != null)
			{
				((Pool<Action>?)_pool)!.free(this);
				_pool = null;
			}
		}
	}

	/** @return null if the action is not attached to an actor. */
	public Actor? GetActor()
	{
		return Actor;
	}

	/** Sets the actor this action will manipulate. If no target actor is set, {@link #setActor(Actor)} will set the target actor
	 * when the action is added to an actor. */
	public virtual void SetTarget(Actor? target)
	{
		this.Target = target;
	}

	/** @return null if the action has no target. */
	public Actor? GetTarget()
	{
		return Target;
	}

	/** Resets the optional state of this action to as if it were newly created, allowing the action to be pooled and reused. State
	 * required to be set for every usage of this action or computed during the action does not need to be reset.
	 * <p>
	 * The default implementation calls {@link #restart()}.
	 * <p>
	 * If a subclass has optional state, it must override this method, call super, and reset the optional state. */
	public virtual void Reset()
	{
		Actor = null;
		Target = null;
		_pool = null;
		Restart();
	}

	public Pool<Action>? GetPool()
	{
		return (Pool<Action>?)_pool;
	}

	/** Sets the pool that the action will be returned to when removed from the actor.
	 * @param pool May be null.
	 * @see #setActor(Actor) */
	public void SetPool<T>(Pool<T>? pool)
	where T: Action
	{
		this._pool = pool;
	}

	public override String ToString()
		{
		String name = GetType().Name;
		int dotIndex = name.LastIndexOf('.');
		
        if (dotIndex != -1) name = name[(dotIndex + 1)..];
		
        if (name.EndsWith("Action")) name = name[..^6];

		return name;
	}
}
}
