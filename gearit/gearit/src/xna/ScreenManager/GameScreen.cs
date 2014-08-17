#region File Description

//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

using System;
using Microsoft.Xna.Framework;
using gearit.src.utility;

namespace gearit.xna
{
	/// <summary>
	/// Enum describes the screen transition state.
	/// </summary>
	public enum ScreenState
	{
		TransitionOn,
		Active,
		TransitionOff,
		Hidden,
	}

	/// <summary>
	/// A screen is a single layer that has update and draw logic, and which
	/// can be combined with other layers to build up a complex menu system.
	/// For instance the main menu, the options menu, the "are you sure you
	/// want to quit" message box, and the main game itself are all implemented
	/// as screens.
	/// </summary>
	public abstract class GameScreen
	{
		private bool _otherScreenHasFocus;
		public bool VisibleMenu = false;
        public bool is_initialized = false;
		
		public GameScreen()
		{
			DrawPriority = 0;
			ScreenState = ScreenState.TransitionOn;
			TransitionPosition = 1;
			TransitionOffTime = TimeSpan.Zero;
			TransitionOnTime = TimeSpan.Zero;
			HasCursor = false;
			HasVirtualStick = false;
		}

		public bool HasCursor { get; set; }

		public bool HasVirtualStick { get; set; }

		/// <summary>
		/// Normally when one screen is brought up over the top of another,
		/// the first screen will transition off to make room for the new
		/// one. This property indicates whether the screen is only a small
		/// popup, in which case screens underneath it do not need to bother
		/// transitioning off.
		/// </summary>
		public bool IsPopup { get; protected set; }

		/// <summary>
		/// Indicates how long the screen takes to
		/// transition on when it is activated.
		/// </summary>
		public TimeSpan TransitionOnTime { get; protected set; }

		/// <summary>
		/// Indicates how long the screen takes to
		/// transition off when it is deactivated.
		/// </summary>
		public TimeSpan TransitionOffTime { get; protected set; }

		/// <summary>
		/// Gets the current position of the screen transition, ranging
		/// from zero (fully active, no transition) to one (transitioned
		/// fully off to nothing).
		/// </summary>
		public float TransitionPosition { get; protected set; }

		public int DrawPriority { get; set; }

		/// <summary>
		/// Gets the current alpha of the screen transition, ranging
		/// from 1 (fully active, no transition) to 0 (transitioned
		/// fully off to nothing).
		/// </summary>
		public float TransitionAlpha
		{
			get { return 1f - TransitionPosition; }
		}

		/// <summary>
		/// Gets the current screen transition state.
		/// </summary>
		public ScreenState ScreenState { get; protected set; }

		/// <summary>
		/// There are two possible reasons why a screen might be transitioning
		/// off. It could be temporarily going away to make room for another
		/// screen that is on top of it, or it could be going away for good.
		/// This property indicates whether the screen is exiting for real:
		/// if set, the screen will automatically remove itself as soon as the
		/// transition finishes.
		/// </summary>
		public bool IsExiting { get; protected internal set; }

		/// <summary>
		/// Checks whether this screen is active and can respond to user input.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return !_otherScreenHasFocus &&
					   (ScreenState == ScreenState.TransitionOn ||
						ScreenState == ScreenState.Active);
			}
		}

		/// <summary>
		/// Gets the manager that this screen belongs to.
		/// </summary>
		public ScreenManager ScreenManager { get; internal set; }

		/// <summary>
		/// Load graphics content for the screen.
		/// </summary>
		public virtual void LoadContent()
		{
            is_initialized = true;
		}

		/// <summary>
		/// Unload content for the screen.
		/// </summary>
		public virtual void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the screen to run logic, such as updating the transition position.
		/// Unlike HandleInput, this method is called regardless of whether the screen
		/// is active, hidden, or in the middle of a transition.
		/// </summary>
		public virtual void Update(GameTime gameTime)
		{
			if (IsExiting)
				ScreenManager.RemoveScreen(this);
			else
				ScreenState = ScreenState.Active;
		}

		/// <summary>
		/// Allows the screen to handle user input. Unlike Update, this method
		/// is only called when the screen is active, and not when some other
		/// screen has taken the focus.
		/// </summary>
		public virtual void HandleInput(InputHelper input, GameTime gameTime)
		{
		}

		/// <summary>
		/// This is called when the screen should draw itself.
		/// </summary>
		public virtual void Draw(GameTime gameTime)
		{
		}

		/// <summary>
		/// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
		/// instantly kills the screen, this method respects the transition timings
		/// and will give the screen a chance to gradually transition off.
		/// </summary>
		public void ExitScreen()
		{
			if (TransitionOffTime == TimeSpan.Zero)
			{
				// If the screen has a zero transition time, remove it immediately.
				ScreenManager.RemoveScreen(this);
			}
			else
			{
				// Otherwise flag that it should transition off and then exit.
				IsExiting = true;
			}
		}
	}
}