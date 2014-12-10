using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using gearit.xna;
using Microsoft.Xna.Framework.Media;

namespace gearit.src.xna.Sound
{
	public class AudioManager
	{
		private Dictionary<string, SoundEffect> _sounds;
		private Song _music;
		private float _volume; // Must be between 0.0f & 1.0f
		public static AudioManager Instance { get; private set; }
		//private SoundEffectInstance _music;

		public AudioManager(ScreenManager screen)
		{
			Instance = this;
			if (_sounds == null)
				LoadAudioContent(screen);
		}

		private void LoadAudioContent(ScreenManager screen)
		{
			_volume = 0.30f;
			_sounds = new Dictionary<string, SoundEffect>();
			_sounds["hover"] = screen.Content.Load<SoundEffect>("Audio\\Effects\\hover_menu");
			_sounds["click"] = screen.Content.Load<SoundEffect>("Audio\\Effects\\enter_menu");
			_sounds["collide_dynamic"] = screen.Content.Load<SoundEffect>("Audio\\Effects\\collide_dynamic");
            /* 
			 * Old method for sound playing, i leave it here if it turns out we need to use this instead of the MediaPlayer for the background music
			 * 
			 * _music = screen.Content.Load<SoundEffect>("Audio\\music_sc2").CreateInstance();
			 * _music.IsLooped = true;
			 * _music.Play();
			 */

            /*
			 * Start the background music. /!\ Windows Media Player MUST BE enabled on your system, otherwise it will raise en exception
			 * TODO : Add a collection of songs, and play them specifically in the menu, in game, in the editor, etc.
			 */
			try
			{
				_music = screen.Content.Load<Song>("Audio\\igi_song");
				MediaPlayer.Volume = 0.20f;
				MediaPlayer.Play(_music);
			}
			catch (InvalidOperationException ex)
			{
			}
		}

		public void PlaySound(string name)
		{
			_sounds[name].Play(0.15f, 0.0f, 0.0f);
		}

        public void SetVolume(float volume)
		{
			_volume = volume;
		}
	}
}
