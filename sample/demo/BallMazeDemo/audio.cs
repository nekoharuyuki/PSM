/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Input;

namespace Demo_BallMaze
{
    public class SoundSystem
    {
        Sound[] sound;
        SoundPlayer[] soundPlayer;

		public static Defs.eSound ballLoopSoundHandle;
		public static Defs.eSound musicPlaying, oldMusicPlaying;
		public static Defs.eSound musicPlayingHandle, oldMusicPlayingHandle;
		public static float masterMusicVolume, musicVolume, oldMusicVolume, masterSoundVolume;

        public SoundSystem()
        {
            sound = new Sound[]
			{
			    new Sound("/Application/data/wall_hit.wav"),
			    new Sound("/Application/data/penalty_hit.wav"),
			    new Sound("/Application/data/cube_drop.wav"),
			    new Sound("/Application/data/beep.wav"),
			    new Sound("/Application/data/bong.wav"),
			    new Sound("/Application/data/ball_roll.wav"),
			    new Sound("/Application/data/ball_slide.wav"),
				new Sound("/Application/data/beep.wav"),
			    new Sound("/Application/data/music_attract.wav"),
			    new Sound("/Application/data/music_gameplay.wav"),
			    new Sound("/Application/data/music_winner.wav"),
			    new Sound("/Application/data/music_gameover.wav"),
			    new Sound("/Application/data/music_loser.wav"),
		    };

            soundPlayer = new SoundPlayer[]
			{
			    sound[0].CreatePlayer(),
			    sound[1].CreatePlayer(),
			    sound[2].CreatePlayer(),
			    sound[3].CreatePlayer(),
			    sound[4].CreatePlayer(),
			    sound[5].CreatePlayer(),
			    sound[6].CreatePlayer(),
			    sound[7].CreatePlayer(),
			    sound[8].CreatePlayer(),
			    sound[9].CreatePlayer(),
			    sound[10].CreatePlayer(),
			    sound[11].CreatePlayer(),
			    sound[12].CreatePlayer(),
		    };
        }
		
		public Defs.eSound Play(Defs.eSound index, float volume, bool bLoop)
		{
			soundPlayer[(int)index].Play();
			soundPlayer[(int)index].Volume = volume * masterSoundVolume;
			soundPlayer[(int)index].Loop = bLoop;
			return index;
		}

		public void Stop(Defs.eSound index)
		{
			soundPlayer[(int)index].Stop();
		}

		public void SetVolume(Defs.eSound index, float volume)
		{
			soundPlayer[(int)index].Volume = volume * masterSoundVolume;
		}

		public void SetPitch(Defs.eSound index, float pitch)
		{
//			soundPlayer[(int)index].Pitch = pitch;
		}

		// crossfade the new music into the old music
		public void StartNewMusicCrossfade(Defs.eSound newMusic)
		{
			// make sure we're done with any old song
			if (oldMusicPlayingHandle != Defs.eSound.MUS_NO_MUSIC)
			{
				Stop(oldMusicPlayingHandle);
			}

			// move the current song data into the "old" data
			if (musicPlayingHandle != Defs.eSound.MUS_NO_MUSIC)
			{
				oldMusicVolume = musicVolume;
				oldMusicPlaying = musicPlaying;
				oldMusicPlayingHandle = musicPlayingHandle;
			}

			// start the new song silent, then ramp it up
			if (newMusic != Defs.eSound.MUS_NO_MUSIC)
			{
				musicVolume = 0.0f;
				musicPlaying = newMusic;
				musicPlayingHandle = Play(newMusic, masterMusicVolume * musicVolume, true);
			}
			else
			{
				musicVolume = 0.0f;
				musicPlaying = Defs.eSound.MUS_NO_MUSIC;
				musicPlayingHandle = Defs.eSound.MUS_NO_MUSIC;
			}
		}

		public void UpdateMusic(float dT)
		{
			// fade the old song out if needed
			if (oldMusicPlaying != Defs.eSound.MUS_NO_MUSIC)
			{
				soundPlayer[(int)oldMusicPlayingHandle].Volume = masterMusicVolume * oldMusicVolume;

				oldMusicVolume -= dT / Defs.MUSIC_CROSSFADE_TIME;
				if (oldMusicVolume <= 0.0f)
				{
					Stop(oldMusicPlayingHandle);
					oldMusicPlayingHandle = Defs.eSound.MUS_NO_MUSIC;
					oldMusicPlaying = Defs.eSound.MUS_NO_MUSIC;
					oldMusicVolume = 0.0f;
				}
			}

			// fade the new song in if needed
			if ((musicPlaying != Defs.eSound.MUS_NO_MUSIC) && (musicVolume < 1.0f))
			{
				musicVolume += dT / Defs.MUSIC_CROSSFADE_TIME;
				if (musicVolume >= 1.0f)
				{
					musicVolume = 1.0f;
				}

				soundPlayer[(int)musicPlayingHandle].Volume = masterMusicVolume * musicVolume;
			}

			// start any new songs
			if (BallMazeProgram.gameState == Defs.eGameState.STATE_ATTRACT)
			{
				if (musicPlaying != Defs.eSound.MUS_ATTRACT_LOOP)
				{
					StartNewMusicCrossfade(Defs.eSound.MUS_ATTRACT_LOOP);
				}
			}
			else if (BallMazeProgram.gameState == Defs.eGameState.STATE_READY)
			{
				if (musicPlaying != Defs.eSound.MUS_NO_MUSIC)
				{
					StartNewMusicCrossfade(Defs.eSound.MUS_NO_MUSIC);
				}
			}
			else if (BallMazeProgram.gameState == Defs.eGameState.STATE_PLAY)
			{
				if (musicPlaying != Defs.eSound.MUS_PLAY_LOOP)
				{
					StartNewMusicCrossfade(Defs.eSound.MUS_PLAY_LOOP);
				}
			}
			else if (BallMazeProgram.gameState == Defs.eGameState.STATE_FINISH)
			{
				if ((musicPlaying != Defs.eSound.MUS_WINNER_LOOP) && (musicPlaying != Defs.eSound.MUS_GAMEOVER_LOOP) && (musicPlaying != Defs.eSound.MUS_LOSER_LOOP))
				{
					if (BallMazeProgram.lastGameTime >= Defs.OUT_OF_TIME)
					{
						if (musicPlaying != Defs.eSound.MUS_LOSER_LOOP)
						{
							StartNewMusicCrossfade(Defs.eSound.MUS_LOSER_LOOP);
						}
					}
					else if (BallMazeProgram.bWinner)
					{
						if (musicPlaying != Defs.eSound.MUS_WINNER_LOOP)
						{
							StartNewMusicCrossfade(Defs.eSound.MUS_WINNER_LOOP);
						}
					}
					else
					{
						if (musicPlaying != Defs.eSound.MUS_GAMEOVER_LOOP)
						{
							StartNewMusicCrossfade(Defs.eSound.MUS_GAMEOVER_LOOP);
						}
					}
				}
			}
		}
   }
}
