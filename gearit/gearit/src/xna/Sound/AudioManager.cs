using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using gearit.xna;

namespace gearit.src.xna.Sound
{
	public class AudioManager
	{
		private Dictionary<string, SoundEffect> _sounds;
		private SoundEffectInstance _music;
		public static AudioManager Instance { get; private set; }

		public AudioManager(ScreenManager screen)
		{
			Instance = this;
			if (_sounds == null)
				LoadAudioContent(screen);
		}

		private void LoadAudioContent(ScreenManager screen)
		{
			_sounds = new Dictionary<string, SoundEffect>();
			_sounds["hover"] = screen.Content.Load<SoundEffect>("Audio\\Effects\\hover_menu");
			_sounds["click"] = screen.Content.Load<SoundEffect>("Audio\\Effects\\click_menu");
			_music = screen.Content.Load<SoundEffect>("Audio\\music_sc2").CreateInstance();
			_music.Volume = 0.75f;
			_music.Play();
		}

		public void PlaySound(string name)
		{
			_sounds[name].Play(0.15f, 0.0f, 0.0f);
		}
	}
}
