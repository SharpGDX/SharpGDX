using SharpGDX.Shims;
using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Scenes.Scene2D.UI;

public abstract class Tooltip : InputListener
{
	internal readonly Container<Actor> container;
	internal Actor targetActor;
	internal bool always;
	internal bool instant;
	public virtual void hide()
	{
		manager.hide(this);
	}

	protected readonly TooltipManager manager;

	internal Tooltip(TooltipManager manager)
	{
		this.manager = manager;
	}
}

/** A listener that shows a tooltip actor when the mouse is over another actor.
 * @author Nathan Sweet */
public class Tooltip<T> : Tooltip
where T: Actor{
	static Vector2 tmp = new Vector2();
	internal new readonly Container<T> container;
	
	
	
		private bool touchIndependent;
	

	/** @param contents May be null. */
	public Tooltip ( T? contents) 
	: this(contents, TooltipManager.getInstance())
	{
		
	}

	/** @param contents May be null. */
	public Tooltip (T? contents, TooltipManager manager)
	:base(manager){
		container = new TooltipContainer(this, contents) ;
		container.setTouchable(Touchable.disabled);
	}

	private class TooltipContainer:Container<T>
	{
		private readonly Tooltip<T> _tooltip;

		public TooltipContainer(Tooltip<T> tooltip,T? contents)
		:base(contents)
		{
			_tooltip = tooltip;
		}
		public override void act(float delta)
		{
			base.act(delta);
			if (_tooltip.targetActor != null && _tooltip.targetActor.getStage() == null) remove();
		}
	}

	public TooltipManager getManager () {
		return manager;
	}

	public Container<T> getContainer () {
		return container;
	}

	public void setActor (T? contents) {
		container.setActor(contents);
	}

	public T? getActor () {
		return container.getActor();
	}

	/** If true, this tooltip is shown without delay when hovered. */
	public void setInstant (bool instant) {
		this.instant = instant;
	}

	/** If true, this tooltip is shown even when tooltips are not {@link TooltipManager#enabled}. */
	public void setAlways (bool always) {
		this.always = always;
	}

	/** If true, this tooltip will be shown even when screen is touched simultaneously with entering tooltip's targetActor */
	public void setTouchIndependent (bool touchIndependent) {
		this.touchIndependent = touchIndependent;
	}

	public override bool touchDown (InputEvent @event, float x, float y, int pointer, int button) {
		if (instant) {
			container.toFront();
			return false;
		}
		manager.touchDown(this);
		return false;
	}

	public override bool mouseMoved (InputEvent @event, float x, float y) {
		if (container.hasParent()) return false;
		setContainerPosition(@event.getListenerActor(), x, y);
		return true;
	}

	private void setContainerPosition (Actor actor, float x, float y) {
		this.targetActor = actor;
		Stage stage = actor.getStage();
		if (stage == null) return;

		container.setSize(manager.maxWidth, int.MaxValue);
		container.validate();
		container.width(container.getActor().getWidth());
		container.pack();

		float offsetX = manager.offsetX, offsetY = manager.offsetY, dist = manager.edgeDistance;
		Vector2 point = actor.localToStageCoordinates(tmp.set(x + offsetX, y - offsetY - container.getHeight()));
		if (point.y < dist) point = actor.localToStageCoordinates(tmp.set(x + offsetX, y + offsetY));
		if (point.x < dist) point.x = dist;
		if (point.x + container.getWidth() > stage.getWidth() - dist) point.x = stage.getWidth() - dist - container.getWidth();
		if (point.y + container.getHeight() > stage.getHeight() - dist) point.y = stage.getHeight() - dist - container.getHeight();
		container.setPosition(point.x, point.y);

		point = actor.localToStageCoordinates(tmp.set(actor.getWidth() / 2, actor.getHeight() / 2));
		point.sub(container.getX(), container.getY());
		container.setOrigin(point.x, point.y);
	}

	public override void enter (InputEvent @event, float x, float y, int pointer, Actor? fromActor) {
		if (pointer != -1) return;
		if (touchIndependent && Gdx.input.isTouched()) return;
		Actor actor = @event.getListenerActor();
		if (fromActor != null && fromActor.isDescendantOf(actor)) return;
		setContainerPosition(actor, x, y);
		manager.enter(this);
	}

	public override void exit (InputEvent @event, float x, float y, int pointer, Actor? toActor) {
		if (toActor != null && toActor.isDescendantOf(@event.getListenerActor())) return;
		hide();
	}

	public override void hide () {
		manager.hide(this);
	}
}
