/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UIAnimationSample
{
    public partial class UIAnimationScene : Scene
    {
        private enum FileType
        {
            NONE,
            UIA,
            UIM
        }

        private FileType currentFileType;
        private UIAnimationPlayer player;
        private ImageBox imageBox;
        private UIMotion motion;
        private Dictionary<string, Vector2> uimPositions;
        private Matrix4 imageTransform3D;
        private Vector2 center;

        public UIAnimationScene()
        {
            InitializeWidget();

            center = new Vector2(contentsPanel.Width / 2, contentsPanel.Height / 2);
            uimPositions = new Dictionary<string, Vector2>();

            imageBox = new ImageBox();
            imageBox.Image = new ImageAsset("/Application/assets/logo.png");
            imageBox.PivotType = PivotType.MiddleCenter;
            imageBox.SetSize(128, 128);
            imageTransform3D = imageBox.Transform3D;

            currentFileType = FileType.NONE;

            playButton.ButtonAction += OnPlayButtonAction;
            pauseButton.ButtonAction += OnPauseButtonAction;
            fileSelectPopupList.SelectionChanged += OnFileChanged;
            repeatCheckBox.CheckedChanged += OnRepeatChanged;
            pauseButton.Enabled = false;
            OnFileChanged(null, null);
        }
		
		protected override void OnShown ()
		{
			base.OnShown ();
            center = new Vector2(contentsPanel.Width / 2, contentsPanel.Height / 2);
            uimPositions.Add ("UIMSample.uim", center);
			
			OnFileChanged(null, null);
		}

        private void OnPlayButtonAction(object sender, TouchEventArgs e)
        {
            if (Playing || Paused)
            {
                Stop();
            }
            else
            {
                Play();
            }
        }

        private void OnPauseButtonAction(object sender, TouchEventArgs e)
        {
            if (Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        private void OnRepeatChanged(object sender, EventArgs e)
        {
            if (currentFileType == FileType.UIA && player != null)
            {
                player.Repeating = repeatCheckBox.Checked;
            }
            else if (currentFileType == FileType.UIM && motion != null)
            {
                motion.Repeating = repeatCheckBox.Checked;
            }
        }

        private void OnFileChanged(object sender, EventArgs e)
        {
            Stop ();

            string filename = fileSelectPopupList.ListItems[fileSelectPopupList.SelectedIndex];
            string filepath = "/Application/assets/" + filename;
            currentFileType = filename.EndsWith(".uia") ? FileType.UIA : (filename.EndsWith(".uim") ? FileType.UIM : FileType.NONE);

            contentsPanel.RemoveChild(player);
            contentsPanel.RemoveChild(imageBox);
            player = null;
            motion = null;

            if (currentFileType == FileType.UIA)
            {
                player = new UIAnimationPlayer(filepath);
                player.PivotType = PivotType.MiddleCenter;
                player.SetPosition(center.X, center.Y);
                player.Repeating = repeatCheckBox.Checked;
                player.AnimationStopped += OnStop;
                contentsPanel.AddChildFirst(player);
            }
            else if (currentFileType == FileType.UIM)
            {
                motion = new UIMotion(imageBox, filepath);
                motion.EffectStopped += OnStop;
                motion.Repeating = repeatCheckBox.Checked;

                Vector2 pos = center;
                if (uimPositions.ContainsKey(filename))
                {
                    pos = uimPositions[filename];
                }

                imageBox.Transform3D = imageTransform3D;
                imageBox.SetPosition(pos.X, pos.Y);
                imageTransform3D = imageBox.Transform3D;
                contentsPanel.AddChildFirst(imageBox);
            }
        }

        private void OnStop(object sender, EventArgs e)
        {
            playButton.Text = "Start";
            pauseButton.Enabled = false;
        }

        private void Play()
        {
            if (currentFileType == FileType.UIA && player != null)
            {
                player.Play();
            }
            else if (currentFileType == FileType.UIM && motion != null)
            {
                imageBox.Transform3D = imageTransform3D;
                motion.Start();
            }

            playButton.Text = "Stop";
            pauseButton.Text = "Pause";
            pauseButton.Enabled = true;
        }

        private void Stop()
        {
            if (currentFileType == FileType.UIA && player != null)
            {
                player.Stop();
            }
            else if (currentFileType == FileType.UIM && motion != null)
            {
                motion.Stop();
            }

            playButton.Text = "Start";
            pauseButton.Text = "Pause";
            pauseButton.Enabled = false;
        }

        private void Resume()
        {
            if (currentFileType == FileType.UIA && player != null)
            {
                player.Resume();
            }
            else if (currentFileType == FileType.UIM && motion != null)
            {
                motion.Resume();
            }

            pauseButton.Text = "Pause";
        }

        private void Pause()
        {
            if (currentFileType == FileType.UIA && player != null)
            {
                player.Pause();
            }
            else if (currentFileType == FileType.UIM && motion != null)
            {
                motion.Pause();
            }

            pauseButton.Text = "Resume";
        }

        private bool Playing
        {
            get
            {
                if (currentFileType == FileType.UIA && player != null)
                {
                    return player.Playing;
                }
                else if (currentFileType == FileType.UIM && motion != null)
                {
                    return motion.Playing;
                }

                return false;
            }
        }

        private bool Paused
        {
            get
            {
                if (currentFileType == FileType.UIA && player != null)
                {
                    return player.Paused;
                }
                else if (currentFileType == FileType.UIM && motion != null)
                {
                    return motion.Paused;
                }

                return false;
            }
        }
    }
}
