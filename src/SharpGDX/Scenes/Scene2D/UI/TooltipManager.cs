using SharpGDX.Shims;
using static  SharpGDX.Scenes.Scene2D.Actions.Actions;
using SharpGDX;
using SharpGDX.Scenes.Scene2D.UI;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using Timer = SharpGDX.Utils.Timer;
using Task =SharpGDX.Utils.Timer.Task;

namespace SharpGDX.Scenes.Scene2D.UI;

/** Keeps track of an application's tooltips.
 * @author Nathan Sweet */
public class TooltipManager {
	static private TooltipManager instance;
	static private IFiles files;

	/** Seconds from when an actor is hovered to when the tooltip is shown. Default is 2. Call {@link #hideAll()} after changing to
	 * reset internal state. */
	public float initialTime = 2;
	/** Once a tooltip is shown, this is used instead of {@link #initialTime}. Default is 0. */
	public float subsequentTime = 0;
	/** Seconds to use {@link #subsequentTime}. Default is 1.5. */
	public float resetTime = 1.5f;
	/** If false, tooltips will not be shown. Default is true. */
	public bool enabled = true;
	/** If false, tooltips will be shown without animations. Default is true. */
	public bool animations = true;
	/** The maximum width of a {@link TextTooltip}. The label will wrap if needed. Default is Integer.MAX_VALUE. */
	public float maxWidth = int.MaxValue;
	/** The distance from the mouse position to offset the tooltip actor. Default is 15,19. */
	public float offsetX = 15, offsetY = 19;
	/** The distance from the tooltip actor position to the edge of the screen where the actor will be shown on the other side of
	 * the mouse cursor. Default is 7. */
	public float edgeDistance = 7;

	readonly Array<Tooltip> shown = new ();

	float time ;
	private readonly Task resetTask;

	private class ResetTask : Task
		{
			private readonly TooltipManager _tooltipManager;

			public ResetTask(TooltipManager tooltipManager)
			{
				_tooltipManager = tooltipManager;
			}

			public override void run()
			{
				_tooltipManager.time = _tooltipManager.initialTime;
			}
		}

	Tooltip showTooltip;

	public TooltipManager()
	{
		time = initialTime;
		resetTask = new ResetTask(this);
		showTask = new ShowTask(this);
	}

	private readonly Task showTask;

	private class ShowTask : Task
	{
		private readonly TooltipManager _tooltipManager;

		public ShowTask(TooltipManager tooltipManager)
		{
			_tooltipManager = tooltipManager;
		}
		public override void run()
		{
			if (_tooltipManager.showTooltip == null || _tooltipManager.showTooltip.targetActor == null) return;

			Stage stage = _tooltipManager.showTooltip.targetActor.getStage();
			if (stage == null) return;
			stage.addActor(_tooltipManager.showTooltip.container);
			_tooltipManager.showTooltip.container.toFront();
			_tooltipManager.shown.add(_tooltipManager.showTooltip);

			_tooltipManager.showTooltip.container.clearActions();
			_tooltipManager.showAction(_tooltipManager.showTooltip);

			if (!_tooltipManager.showTooltip.instant)
			{
				_tooltipManager.time = _tooltipManager.subsequentTime;
				_tooltipManager.resetTask.cancel();
			}
		}
	}

	public void touchDown (Tooltip tooltip) {
		showTask.cancel();
		if (tooltip.container.remove()) resetTask.cancel();
		resetTask.run();
		if (enabled || tooltip.always) {
			showTooltip = tooltip;
			Timer.schedule(showTask, time);
		}
	}

	public void enter (Tooltip tooltip) {
		showTooltip = tooltip;
		showTask.cancel();
		if (enabled || tooltip.always) {
			if (time == 0 || tooltip.instant)
				showTask.run();
			else
				Timer.schedule(showTask, time);
		}
	}

	public void hide (Tooltip tooltip) {
		showTooltip = null;
		showTask.cancel();
		if (tooltip.container.hasParent()) {
			shown.removeValue(tooltip, true);
			hideAction(tooltip);
			resetTask.cancel();
			Timer.schedule(resetTask, resetTime);
		}
	}

	/** Called when tooltip is shown. Default implementation sets actions to animate showing. */
	protected void showAction (Tooltip tooltip) {
		float actionTime = animations ? (time > 0 ? 0.5f : 0.15f) : 0.1f;
		tooltip.container.setTransform(true);
		tooltip.container.getColor().a = 0.2f;
		tooltip.container.setScale(0.05f);
		tooltip.container.addAction(parallel(fadeIn(actionTime, Interpolation.fade), scaleTo(1, 1, actionTime, Interpolation.fade)));
	}

	/** Called when tooltip is hidden. Default implementation sets actions to animate hiding and to remove the actor from the stage
	 * when the actions are complete. A subclass must at least remove the actor. */
	protected void hideAction (Tooltip tooltip) {
		tooltip.container
			.addAction(sequence(parallel(alpha(0.2f, 0.2f, Interpolation.fade), scaleTo(0.05f, 0.05f, 0.2f, Interpolation.fade)), removeActor()));
	}

	public void hideAll () {
		resetTask.cancel();
		showTask.cancel();
		time = initialTime;
		showTooltip = null;

		foreach (Tooltip tooltip in shown)
			tooltip.hide();
		shown.clear();
	}

	/** Shows all tooltips on hover without a delay for {@link #resetTime} seconds. */
	public void instant () {
		time = 0;
		showTask.run();
		showTask.cancel();
	}

	static public TooltipManager getInstance () {
		if (files == null || files != Gdx.files) {
			files = Gdx.files;
			instance = new TooltipManager();
		}
		return instance;
	}
}