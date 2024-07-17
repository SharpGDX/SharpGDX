using SharpGDX.Shims;
using static SharpGDX.Scenes.Scene2D.Actions.Actions;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Mathematics;
using SharpGDX.Utils;
using SharpGDX.Scenes.Scene2D.UI;

namespace SharpGDX.Scenes.Scene2D.UI;

/** Displays a dialog, which is a window with a title, a content table, and a button table. Methods are provided to add a label to
 * the content table and buttons to the button table, but any widgets can be added. When a button is clicked,
 * {@link #result(Object)} is called and the dialog is removed from the stage.
 * @author Nathan Sweet */
public class Dialog : Window {
	Table contentTable, buttonTable;
	private Skin? skin;
	ObjectMap<Actor, Object> values = new ();
	bool cancelHide;
	Actor previousKeyboardFocus, previousScrollFocus;
	FocusListener focusListener;

	protected InputListener ignoreTouchDown = new IgnoreTouchDownInputListener() ;

	private class IgnoreTouchDownInputListener : InputListener
	{
			public override bool touchDown(InputEvent @event, float x, float y, int pointer, int button)
			{
				@event.cancel();
				return false;
			}
}

	public Dialog (String title, Skin skin) 
: base(title, skin.get<Window.WindowStyle>(typeof(Window.WindowStyle)))
	{
		
		setSkin(skin);
		this.skin = skin;
		initialize();
	}

	public Dialog (String title, Skin skin, String windowStyleName) 
	: base(title, skin.get<Window.WindowStyle>(windowStyleName, typeof(Window.WindowStyle))){
		
		setSkin(skin);
		this.skin = skin;
		initialize();
	}

	public Dialog (String title, Window.WindowStyle windowStyle) 
	: base(title, windowStyle)
	{
		
		initialize();
	}

	private class DialogChangeListener:ChangeListener
	{
		private readonly Dialog _dialog;

		public DialogChangeListener(Dialog dialog)
		{
			_dialog = dialog;
		}

		public override void changed(ChangeEvent @event, Actor actor) {
			if (!_dialog.values.containsKey(actor)) return;
			while (actor.getParent() != _dialog.buttonTable)
				actor = actor.getParent();
			_dialog.result(_dialog.values.get(actor));
			if (!_dialog.cancelHide) _dialog.hide();
			_dialog.cancelHide = false;
		}
	}

		private void initialize () {
		setModal(true);

		defaults().Space(6);
		add(contentTable = new Table(skin)).Expand().Fill();
		row();
		add(buttonTable = new Table(skin)).FillX();

		contentTable.defaults().Space(6);
		buttonTable.defaults().Space(6);

		buttonTable.addListener(new DialogChangeListener(this) );

		focusListener = new DialogFocusListener(this) ;
	}

	private class DialogFocusListener : FocusListener
	{
		private readonly Dialog _dialog;

		public DialogFocusListener(Dialog dialog)
		{
			_dialog = dialog;
		}

		public override void keyboardFocusChanged(FocusEvent @event, Actor actor, bool focused)
			{
				if (!focused) focusChanged(@event);
			}

		public override void scrollFocusChanged(FocusListener.FocusEvent @event, Actor actor, bool focused)
			{
				if (!focused) focusChanged(@event);
			}

			private void focusChanged(FocusListener.FocusEvent @event)
			{
				Stage stage = _dialog.getStage();
				if (_dialog._isModal && stage != null && stage.getRoot().getChildren().size > 0
				    && stage.getRoot().getChildren().peek() == _dialog) { // Dialog is top most actor.
					Actor newFocusedActor = @event.getRelatedActor();
					if (newFocusedActor != null && !newFocusedActor.isDescendantOf(_dialog)
					                                    && !(newFocusedActor.Equals(_dialog.previousKeyboardFocus) || newFocusedActor.Equals(_dialog.previousScrollFocus)))
						@event.cancel();
				}
			}
		}

	internal protected override void setStage (Stage stage) {
		if (stage == null)
			addListener(focusListener);
		else
			removeListener(focusListener);
		base.setStage(stage);
	}

	public Table getContentTable () {
		return contentTable;
	}

	public Table getButtonTable () {
		return buttonTable;
	}

	/** Adds a label to the content table. The dialog must have been constructed with a skin to use this method. */
	public Dialog text (String? text) {
		if (skin == null)
			throw new IllegalStateException("This method may only be used if the dialog was constructed with a Skin.");
		return this.text(text, skin.get<Label.LabelStyle>(typeof(Label.LabelStyle)));
	}

	/** Adds a label to the content table. */
	public Dialog text (String? text, Label.LabelStyle labelStyle) {
		return this.text(new Label(text, labelStyle));
	}

	/** Adds the given Label to the content table */
	public Dialog text (Label label) {
		contentTable.add(label);
		return this;
	}

	/** Adds a text button to the button table. Null will be passed to {@link #result(Object)} if this button is clicked. The
	 * dialog must have been constructed with a skin to use this method. */
	public Dialog button (String? text) {
		return button(text, null);
	}

	/** Adds a text button to the button table. The dialog must have been constructed with a skin to use this method.
	 * @param object The object that will be passed to {@link #result(Object)} if this button is clicked. May be null. */
	public Dialog button (String? text, Object? obj) {
		if (skin == null)
			throw new IllegalStateException("This method may only be used if the dialog was constructed with a Skin.");
		return button(text, obj, skin.get<TextButton.TextButtonStyle>(typeof(TextButton.TextButtonStyle)));
	}

	/** Adds a text button to the button table.
	 * @param object The object that will be passed to {@link #result(Object)} if this button is clicked. May be null. */
	public Dialog button (String? text,  Object? obj, TextButton.TextButtonStyle buttonStyle) {
		return button(new TextButton(text, buttonStyle), obj);
	}

	/** Adds the given button to the button table. */
	public Dialog button (Button button) {
		return this.button(button, null);
	}

	/** Adds the given button to the button table.
	 * @param object The object that will be passed to {@link #result(Object)} if this button is clicked. May be null. */
	public Dialog button (Button button, Object? obj) {
		buttonTable.add(button);
		setObject(button, obj);
		return this;
	}

	/** {@link #pack() Packs} the dialog (but doesn't set the position), adds it to the stage, sets it as the keyboard and scroll
	 * focus, clears any actions on the dialog, and adds the specified action to it. The previous keyboard and scroll focus are
	 * remembered so they can be restored when the dialog is hidden.
	 * @param action May be null. */
	public Dialog show (Stage stage, Action? action) {
		clearActions();
		removeCaptureListener(ignoreTouchDown);

		previousKeyboardFocus = null;
		Actor actor = stage.getKeyboardFocus();
		if (actor != null && !actor.isDescendantOf(this)) previousKeyboardFocus = actor;

		previousScrollFocus = null;
		actor = stage.getScrollFocus();
		if (actor != null && !actor.isDescendantOf(this)) previousScrollFocus = actor;

		stage.addActor(this);
		pack();
		stage.cancelTouchFocus();
		stage.setKeyboardFocus(this);
		stage.setScrollFocus(this);
		if (action != null) addAction(action);

		return this;
	}

	/** Centers the dialog in the stage and calls {@link #show(Stage, Action)} with a {@link Actions#fadeIn(float, Interpolation)}
	 * action. */
	public Dialog show (Stage stage) {
		show(stage, sequence(alpha(0), fadeIn(0.4f, Interpolation.fade)));
		setPosition((float)Math.Round((stage.getWidth() - getWidth()) / 2), (float)Math.Round((stage.getHeight() - getHeight()) / 2));
		return this;
	}

	/** Removes the dialog from the stage, restoring the previous keyboard and scroll focus, and adds the specified action to the
	 * dialog.
	 * @param action If null, the dialog is removed immediately. Otherwise, the dialog is removed when the action completes. The
	 *           dialog will not respond to touch down events during the action. */
	public void hide (Action? action) {
		throw new NotImplementedException();
		//Stage stage = getStage();
		//if (stage != null) {
		//	removeListener(focusListener);
		//	if (previousKeyboardFocus != null && previousKeyboardFocus.getStage() == null) previousKeyboardFocus = null;
		//	Actor actor = stage.getKeyboardFocus();
		//	if (actor == null || actor.isDescendantOf(this)) stage.setKeyboardFocus(previousKeyboardFocus);

		//	if (previousScrollFocus != null && previousScrollFocus.getStage() == null) previousScrollFocus = null;
		//	actor = stage.getScrollFocus();
		//	if (actor == null || actor.isDescendantOf(this)) stage.setScrollFocus(previousScrollFocus);
		//}
		//if (action != null) {
		//	addCaptureListener(ignoreTouchDown);
		//	addAction(sequence(action, removeListener(ignoreTouchDown, true), removeActor()));
		//} else
		//	remove();
	}

	/** Hides the dialog. Called automatically when a button is clicked. The default implementation fades out the dialog over 400
	 * milliseconds. */
	public void hide () {
		hide(fadeOut(0.4f, Interpolation.fade));
	}

	public void setObject (Actor actor,  Object? obj) {
		values.put(actor, obj);
	}

	/** If this key is pressed, {@link #result(Object)} is called with the specified object.
	 * @see Keys */
	public Dialog key ( int keycode,   Object? obj) {
		addListener(new KeyDownListener(this, keycode, obj) );
		return this;
	}

	private class KeyDownListener:InputListener
	{
		private readonly Dialog _dialog;
		private readonly int keycode;
		private readonly object? obj;
		public KeyDownListener(Dialog dialog,int keycode, object? obj)
		{
			_dialog = dialog;
			this.keycode = keycode;
			this.obj = obj;
		}

		public override bool keyDown(InputEvent @event, int keycode2)
		{
			if (keycode == keycode2)
			{
				// Delay a frame to eat the keyTyped event.
				Gdx.app.postRunnable(() =>
				{

					_dialog.result(obj);
					if (!_dialog.cancelHide) _dialog.hide();
					_dialog.cancelHide = false;
				});
			}
			return false;
		}
	}

/** Called when a button is clicked. The dialog will be hidden after this method returns unless {@link #cancel()} is called.
 * @param object The object specified when the button was added. */
protected virtual void result ( Object? obj) {
	}

	public void cancel () {
		cancelHide = true;
	}
}
