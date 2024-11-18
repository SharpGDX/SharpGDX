using SharpGDX.Shims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Input
{
    public class NativeInputConfiguration {

	private IInput.OnscreenKeyboardType type = IInput.OnscreenKeyboardType.Default;
	private bool preventCorrection = false;

	private TextInputWrapper textInputWrapper;
	private bool _isMultiLine = false;
	private int maxLength;
	private IInput.IInputStringValidator validator;
	private String placeholder = "";
	private bool showPasswordButton = false;
	private String[] autoComplete = null;

	public IInput.OnscreenKeyboardType getType () {
		return type;
	}

	/** @param type which type of keyboard we wish to display. */
	public NativeInputConfiguration setType (IInput.OnscreenKeyboardType type) {
		this.type = type;
		return this;
	}

	public bool isPreventCorrection () {
		return preventCorrection;
	}

	/** @param preventCorrection Disable autocomplete/correction */
	public NativeInputConfiguration setPreventCorrection (bool preventCorrection) {
		this.preventCorrection = preventCorrection;
		return this;
	}

	public TextInputWrapper getTextInputWrapper () {
		return textInputWrapper;
	}

	/** @param textInputWrapper Should provide access to the backed input field. */
	public NativeInputConfiguration setTextInputWrapper (TextInputWrapper textInputWrapper) {
		this.textInputWrapper = textInputWrapper;
		return this;
	}

	public bool isMultiLine () {
		return _isMultiLine;
	}

	/** @param multiLine whether the keyboard should accept multiple lines. */
	public NativeInputConfiguration setMultiLine (bool multiLine) {
		_isMultiLine = multiLine;
		return this;
	}

	public int getMaxLength () {
		return maxLength;
	}

	/** @param maxLength What the text length limit should be. */
	public NativeInputConfiguration setMaxLength (int maxLength) {
		this.maxLength = maxLength;
		return this;
	}

	public IInput.IInputStringValidator getValidator () {
		return validator;
	}

	/** @param validator Can validate the input from the keyboard and reject. */
	public NativeInputConfiguration setValidator (IInput.IInputStringValidator validator) {
		this.validator = validator;
		return this;
	}

	public String getPlaceholder () {
		return placeholder;
	}

	/** @param placeholder String to show, if nothing is inputted yet. */
	public NativeInputConfiguration setPlaceholder (String placeholder) {
		this.placeholder = placeholder;
		return this;
	}

	public bool isShowPasswordButton () {
		return showPasswordButton;
	}

	/** @param showPasswordButton Whether to show a button to show unhidden password */
	public NativeInputConfiguration setShowPasswordButton (bool showPasswordButton) {
		this.showPasswordButton = showPasswordButton;
		return this;
	}

	public String[] getAutoComplete () {
		return autoComplete;
	}

	public NativeInputConfiguration setAutoComplete (String[] autoComplete) {
		this.autoComplete = autoComplete;
		return this;
	}

	public void validate () {
		String message = null;
		if (type == null) message = "OnscreenKeyboardType needs to be non null";
		if (textInputWrapper == null) message = "TextInputWrapper needs to be non null";
		if (showPasswordButton && type != IInput.OnscreenKeyboardType.Password)
			message = "ShowPasswordButton only works with OnscreenKeyboardType.Password";
		if (placeholder == null) message = "Placeholder needs to be non null";
		if (autoComplete != null && type != IInput.OnscreenKeyboardType.Default)
			message = "AutoComplete should only be used with OnscreenKeyboardType.Default";
		if (autoComplete != null && _isMultiLine) message = "AutoComplete shouldn't be used with multiline";

		if (message != null) {
			throw new IllegalArgumentException("NativeInputConfiguration validation failed: " + message);
		}
	}
}
}
