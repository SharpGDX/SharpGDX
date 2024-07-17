using SharpGDX.Utils;


namespace SharpGDX.Scenes.Scene2D
{
	/** Actions attach to an {@link Actor} and perform some task, often over time.
 * @author Nathan Sweet */
	abstract public class Action : IPoolable
	{
	/** The actor this action is attached to, or null if it is not attached. */
	protected Actor actor;

	/** The actor this action targets, or null if a target has not been set. */
	protected Actor target;

	private Pool? pool;

	/** Updates the action based on time. Typically this is called each frame by {@link Actor#act(float)}.
	 * @param delta Time in seconds since the last frame.
	 * @return true if the action is done. This method may continue to be called after the action is done. */
	abstract public bool act(float delta);

	/** Sets the state of the action so it can be run again. */
	public virtual void restart()
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
	public virtual void setActor(Actor actor)
	{
		this.actor = actor;
		if (target == null) setTarget(actor);
		if (actor == null)
		{
			if (pool != null)
			{
				((Pool<Action>?)pool).free(this);
				pool = null;
			}
		}
	}

	/** @return null if the action is not attached to an actor. */
	public Actor getActor()
	{
		return actor;
	}

	/** Sets the actor this action will manipulate. If no target actor is set, {@link #setActor(Actor)} will set the target actor
	 * when the action is added to an actor. */
	public virtual void setTarget(Actor target)
	{
		this.target = target;
	}

	/** @return null if the action has no target. */
	public Actor getTarget()
	{
		return target;
	}

	/** Resets the optional state of this action to as if it were newly created, allowing the action to be pooled and reused. State
	 * required to be set for every usage of this action or computed during the action does not need to be reset.
	 * <p>
	 * The default implementation calls {@link #restart()}.
	 * <p>
	 * If a subclass has optional state, it must override this method, call super, and reset the optional state. */
	public virtual void reset()
	{
		actor = null;
		target = null;
		pool = null;
		restart();
	}

	public Pool<Action>? getPool()
	{
		return (Pool<Action>?)pool;
	}

	/** Sets the pool that the action will be returned to when removed from the actor.
	 * @param pool May be null.
	 * @see #setActor(Actor) */
	public void setPool<T>(Pool<T>? pool)
	where T: Action
	{
		this.pool = pool;
	}

	public override String ToString()
		{
		String name = GetType().Name;
		int dotIndex = name.LastIndexOf('.');
		if (dotIndex != -1) name = name.Substring(dotIndex + 1);
		if (name.EndsWith("Action")) name = name.Substring(0, name.Length - 6);
		return name;
	}
}
}
