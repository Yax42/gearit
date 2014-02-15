using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace gearit.xna
{
	class SoundManager
	{
		private ScreenManager _screenManager;
		private SoundEffect _sound;
		private SoundEffectInstance _instance;

		public SoundManager(ScreenManager screen, string sound)
		{
			_screenManager = screen;
			_sound = _screenManager.Content.Load<SoundEffect>(sound);
			_instance = _sound.CreateInstance();
		}

		public void ActiveLoop()
		{
			if (!_instance.IsLooped)
				_instance.IsLooped = true;
		}

		public void StopLoop()
		{
			if (_instance.IsLooped)
				_instance.IsLooped = false;
		}

		public bool isPlaying()
		{
			if (_instance.State == SoundState.Playing)
				return (true);
			else
				return (false);
		}

		public void playSound()
		{
			if (_instance.State != SoundState.Playing)
				_instance.Play();
		}

		public void StopSound()
		{
			_instance.Stop();
		}
	}
}
