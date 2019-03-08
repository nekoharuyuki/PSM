/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Input;

namespace Demo_BrickSmash
{
    class SoundSystem
    {
        Sound[] sound;
        SoundPlayer[] soundPlayer;

        public SoundSystem()
        {
            sound = new Sound[]
			{
			    new Sound("/Application/data/paddlehit.wav"),
			    new Sound("/Application/data/brickhit.wav"),
			    new Sound("/Application/data/die.wav"),
		    };

            soundPlayer = new SoundPlayer[]
			{
			    sound[0].CreatePlayer(),
			    sound[1].CreatePlayer(),
			    sound[2].CreatePlayer(),
		    };
        }
		
		public void Play(int index, float volume, bool bLoop)
		{
			soundPlayer[index].Play();
			soundPlayer[index].Volume = volume;
			soundPlayer[index].Loop = bLoop;
		}

		public void Stop(int index)
		{
			soundPlayer[index].Stop();
		}
    }
}
