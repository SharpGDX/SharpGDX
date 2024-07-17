using System;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.UI;
using Fixed = SharpGDX.Scenes.Scene2D.UI.Value.Fixed;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Shims;
using SharpGDX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Scenes.Scene2D.UI
{
	/** Value placeholder, allowing the value to be computed on request. Values can be provided an actor for context to reduce the
 * number of value instances that need to be created and reduce verbosity in code that specifies values.
 * @author Nathan Sweet */
abstract public class Value {
	/** Calls {@link #get(Actor)} with null. */
	public float get () {
		return get(null);
	}

	/** @param context May be null. */
	abstract public float get (Actor? context);

	/** A value that is always zero. */
	static public readonly Fixed zero = new Fixed(0);

	/** A fixed value that is not computed each time it is used.
	 * @author Nathan Sweet */
	public class Fixed : Value {
		static readonly Fixed[] cache = new Fixed[111];

		private readonly float value;

		public Fixed (float value) {
			this.value = value;
		}

		public override float get (Actor? context) {
			return value;
		}

		public override String ToString () {
			return (value).ToString();
		}

		static public Fixed valueOf (float value) {
			if (value == 0) return zero;
			if (value is >= -10 and <= 100 && value == (int)value) {
				Fixed @fixed = cache[(int)value + 10];
				if (@fixed == null) cache[(int)value + 10] = @fixed = new Fixed(value);
				return @fixed;
			}
			return new Fixed(value);
		}
	}

	/** Value that is the minWidth of the actor in the cell. */
	static public Value minWidth = new MinWidthValue() {
		
	};

	private class MinWidthValue : Value
	{
		public override float get(Actor? context)
		{
			if (context is ILayout) return ((ILayout)context).getMinWidth();
			return context == null ? 0 : context.getWidth();
		}
		}

	/** Value that is the minHeight of the actor in the cell. */
	static public Value minHeight = new MinHeightValue() {
		
	};

	private class MinHeightValue : Value
	{
		public override float get(Actor? context)
		{
			if (context is ILayout) return ((ILayout)context).getMinHeight();
			return context == null ? 0 : context.getHeight();
		}
		}

	/** Value that is the prefWidth of the actor in the cell. */
	static public Value prefWidth = new PrefWidthValue() {
		
	};

	private class PrefWidthValue : Value
	{
		public override float get( Actor? context)
		{
			if (context is ILayout) return ((ILayout)context).getPrefWidth();
			return context == null ? 0 : context.getWidth();

		}
		}

	/** Value that is the prefHeight of the actor in the cell. */
	static public Value prefHeight = new PrefHeightValue() {
		
	};

	private class PrefHeightValue : Value
	{
		public override float get(Actor? context)
		{
			if (context is ILayout) return ((ILayout)context).getPrefHeight();
			return context == null ? 0 : context.getHeight();
		}
	}

	/** Value that is the maxWidth of the actor in the cell. */
	static public Value maxWidth = new MaxWidthValue() {
		
	};

	private class MaxWidthValue : Value
	{
		public override float get(Actor? context)
		{
			if (context is ILayout) return ((ILayout)context).getMaxWidth();
			return context == null ? 0 : context.getWidth();
		}
		}

	/** Value that is the maxHeight of the actor in the cell. */
	static public Value maxHeight = new MaxHeightValue() {
		
	};

	private class MaxHeightValue : Value
	{
		public override float get( Actor? context)
		{
			if (context is ILayout) return ((ILayout)context).getMaxHeight();
			return context == null ? 0 : context.getHeight();
		}
		}

	/** Returns a value that is a percentage of the actor's width. */
	static public Value percentWidth (float percent)
	{
		return new PercentWidthValue(percent);
	}

	private class PercentWidthValue : Value
	{
		private float percent;
			public PercentWidthValue(float percent)
			{
				this.percent = percent;
			}
		public override float get( Actor? actor)
		{
			return actor.getWidth() * percent;
		}
	
		}

	/** Returns a value that is a percentage of the actor's height. */
	static public Value percentHeight (float percent)
	{
		return new PercentHeightValue(percent);
	}

	private class PercentHeightValue : Value
	{
		private float percent;
		public PercentHeightValue(float percent)
		{
			this.percent = percent;
		}
			public override float get( Actor? actor)
		{
			return actor.getHeight() * percent;
		}
		}

	/** Returns a value that is a percentage of the specified actor's width. The context actor is ignored. */
	static public Value percentWidth ( float percent,  Actor actor) {
		if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
		return new ActorPercentWidthValue(percent, actor) {
			
		};
	}

	private class ActorPercentWidthValue : Value
	{
		private readonly float _percent;
		private readonly Actor _actor;

		public ActorPercentWidthValue(float percent, Actor actor)
		{
			_percent = percent;
			_actor = actor;
		}

		public override float get(Actor? context)
		{
			return _actor.getWidth() * _percent;
		}
		}

	/** Returns a value that is a percentage of the specified actor's height. The context actor is ignored. */
	static public Value percentHeight ( float percent,  Actor actor) {
		if (actor == null) throw new IllegalArgumentException("actor cannot be null.");
		return new ActorPercentHeightValue(percent, actor) {
			
		};
	}

	private class ActorPercentHeightValue : Value
	{
		private readonly float _percent;
		private readonly Actor _actor;

		public ActorPercentHeightValue(float percent, Actor actor)
		{
			_percent = percent;
			_actor = actor;
		}
			public override float get( Actor? context)
		{
			return _actor.getHeight() * _percent;
		}
		}
}
}
