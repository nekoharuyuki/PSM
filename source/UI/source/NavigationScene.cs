/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        internal class NavigationScene : Scene
        {
            private static readonly int navigationBarFontSize = 28;
            private static readonly float navigationBarHeight = 66.0f;
            private static readonly float backButtonWidth = 200.0f;
            private static readonly float backButtonHeight = 56.0f;
            private static readonly float stackAnimationTime = 500.0f;
            private static readonly float showHideAnimationDuration = 500.0f;

            private Panel navigationPanel;
            private ImageBox backgroundImage;
            private Button backButtonCurrent;
            private Button backButtonNext;
            private Label labelCurrent;
            private Label labelNext;

            private float leftLabelPosX;
            private float centerLabelPosX;
            private float rightLabelPosX;

            private MoveEffect panelShowMoveEffect;
            private MoveEffect panelHideMoveEffect;
            private MoveEffect labelCurrentMoveEffect;
            private FadeOutEffect labelCurrentFadeOutEffect;
            private MoveEffect labelNextMoveEffect;
            private FadeInEffect labelNextFadeInEffect;
            private FadeOutEffect backButtonCurrentFadeOutEffect;
            private FadeInEffect backButtonNextFadeInEffect;

            public NavigationScene()
            {
                Visible = false;

                // Initialize widgets

                // Base panel
                navigationPanel = new Panel()
                {
                    Width = UISystem.FramebufferWidth,
                    Height = navigationBarHeight,
                    X = 0.0f,
                    Y = -navigationBarHeight,
                    BackgroundColor = new UIColor(0.0f, 0.0f, 0.0f, 0.0f)
                };
                this.RootWidget.AddChildLast(navigationPanel);

                // Background image box
                backgroundImage = new ImageBox()
                {
                    Width = navigationPanel.Width,
                    Height = navigationPanel.Height,
                    X = 0,
                    Y = 0,
                    ImageScaleType = ImageScaleType.NinePatch,
                    Image = new ImageAsset(SystemImageAsset.NavigationBarBackground),
                    NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.NavigationBarBackground)
                };
                navigationPanel.AddChildLast(backgroundImage);

                // Back button
                CustomButtonImageSettings customImage = new CustomButtonImageSettings()
                {
                    BackgroundNormalImage = new ImageAsset(SystemImageAsset.BackButtonBackgroundNormal),
                    BackgroundPressedImage = new ImageAsset(SystemImageAsset.BackButtonBackgroundPressed),
                    BackgroundDisabledImage = new ImageAsset(SystemImageAsset.BackButtonBackgroundDisabled),
                    BackgroundNinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.BackButtonBackgroundNormal),
                };
                float backButtonMargin = (navigationBarHeight - backButtonHeight) / 2.0f;
                UIFont navigationBarFont = new UIFont(FontAlias.System, navigationBarFontSize, FontStyle.Regular);

                backButtonCurrent = new Button()
                {
                    Width = backButtonWidth,
                    Height = backButtonHeight,
                    X = backButtonMargin,
                    Y = backButtonMargin,
                    TextFont = navigationBarFont,
                    Style = ButtonStyle.Custom,
                    CustomImage = customImage,
                    Visible = false
                };
                backButtonCurrent.ButtonAction += new EventHandler<TouchEventArgs>(PopAction);
                navigationPanel.AddChildLast(backButtonCurrent);

                backButtonNext = new Button()
                {
                    Width = backButtonWidth,
                    Height = backButtonHeight,
                    X = backButtonMargin,
                    Y = backButtonMargin,
                    TextFont = navigationBarFont,
                    Style = ButtonStyle.Custom,
                    CustomImage = customImage,
                    Visible = false
                };
                backButtonNext.ButtonAction += new EventHandler<TouchEventArgs>(PopAction);
                navigationPanel.AddChildLast(backButtonNext);

                // Back key handler
                this.RootWidget.KeyEventReceived += RootWidget_KeyEventReceived;

                // Title label
                float labelWidth = navigationPanel.Width - backButtonWidth * 2 - backButtonMargin * 2;
                leftLabelPosX = -labelWidth;
                centerLabelPosX = backButtonWidth + backButtonMargin;
                rightLabelPosX = UISystem.FramebufferWidth;

                labelCurrent = new Label()
                {
                    Width = labelWidth,
                    Height = navigationPanel.Height,
                    X = centerLabelPosX,
                    Y = 0.0f,
                    Font = navigationBarFont,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Middle
                };
                navigationPanel.AddChildLast(labelCurrent);

                labelNext = new Label()
                {
                    Width = labelWidth,
                    Height = navigationPanel.Height,
                    X = rightLabelPosX,
                    Y = 0.0f,
                    Font = navigationBarFont,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Middle,
                    Visible = false
                };
                navigationPanel.AddChildLast(labelNext);

                // Initialize effects
                panelShowMoveEffect = new MoveEffect()
                {
                    Widget = navigationPanel,
                    Interpolator = MoveEffectInterpolator.Custom,
                    CustomInterpolator = new AnimationInterpolator(EaseOutQuartInterpolator),
                    Time = showHideAnimationDuration,
                    X = 0.0f,
                    Y = 0.0f
                };

                panelHideMoveEffect = new MoveEffect()
                {
                    Widget = navigationPanel,
                    Interpolator = MoveEffectInterpolator.Custom,
                    CustomInterpolator = new AnimationInterpolator(EaseOutQuartInterpolator),
                    Time = showHideAnimationDuration,
                    X = 0.0f,
                    Y = -navigationBarHeight
                };
                panelHideMoveEffect.EffectStopped += OnPanelHideMoveEffectStopped;

                labelCurrentMoveEffect = new MoveEffect()
                {
                    Interpolator = MoveEffectInterpolator.Custom,
                    CustomInterpolator = new AnimationInterpolator(EaseOutQuartInterpolator),
                    Time = stackAnimationTime
                };
                labelCurrentMoveEffect.EffectStopped += OnLabelMoveEffectStopped;

                labelCurrentFadeOutEffect = new FadeOutEffect()
                {
                    Interpolator = FadeOutEffectInterpolator.Linear,
                    Time = stackAnimationTime
                };

                labelNextMoveEffect = new MoveEffect()
                {
                    Interpolator = MoveEffectInterpolator.Custom,
                    CustomInterpolator = new AnimationInterpolator(EaseOutQuartInterpolator),
                    Time = stackAnimationTime
                };

                labelNextFadeInEffect = new FadeInEffect()
                {
                    Interpolator = FadeInEffectInterpolator.Linear,
                    Time = stackAnimationTime
                };

                backButtonCurrentFadeOutEffect = new FadeOutEffect()
                {
                    Interpolator = FadeOutEffectInterpolator.Linear,
                    Time = stackAnimationTime
                };

                backButtonNextFadeInEffect = new FadeInEffect()
                {
                    Interpolator = FadeInEffectInterpolator.Linear,
                    Time = stackAnimationTime
                };
            }

            void RootWidget_KeyEventReceived(object sender, KeyEventArgs e)
            {
                if (Visible &&
                    !panelShowMoveEffect.Playing &&
                    !panelHideMoveEffect.Playing &&
                    e.KeyType == KeyType.Back &&
                    e.KeyEventType == KeyEventType.Down)
                {
                    UISystem.PopScene();
                    UISystem.CancelKeyEvents();
                    e.Handled = true;
                }
            }

            internal void Show(bool animation, string title)
            {
                if (panelShowMoveEffect.Playing || 
                    (!panelHideMoveEffect.Playing && Visible))
                {
                    return;
                }

                if (panelHideMoveEffect.Playing)
                {
                    panelHideMoveEffect.Stop();
                }

                Visible = true;

                labelCurrent.Text = title;

                if (animation)
                {
                    panelShowMoveEffect.Start();
                }
            }

            internal void Hide(bool animation)
            {
                if (panelHideMoveEffect.Playing || !Visible)
                {
                    return;
                }

                if (panelShowMoveEffect.Playing)
                {
                    panelShowMoveEffect.Stop();
                }

                if (animation)
                {
                    Visible = true;
                    panelHideMoveEffect.Start();
                }
                else
                {
                    Visible = false;
                }
            }

            internal void StartPushAnimation(string backSceneTitle, string newSceneTitle)
            {
                if (labelCurrentMoveEffect.Playing)
                {
                    labelCurrentMoveEffect.Stop();
                }

                labelCurrentMoveEffect.Widget = labelCurrent;
                labelCurrentMoveEffect.X = leftLabelPosX;
                labelCurrentMoveEffect.Y = labelCurrent.Y;
                labelCurrentMoveEffect.Start();

                labelNext.X = rightLabelPosX;
                labelNext.Text = newSceneTitle;

                labelNextMoveEffect.Widget = labelNext;
                labelNextMoveEffect.X = centerLabelPosX;
                labelNextMoveEffect.Y = labelNext.Y;
                labelNextMoveEffect.Start();

                PrepareLabelFadeAnimation();
                PrepareButtonFadeAnimtion(backSceneTitle);
            }

            internal void StartPopAnimation(string backSceneTitle, string newSceneTitle)
            {
                if (labelCurrentMoveEffect.Playing)
                {
                    labelCurrentMoveEffect.Stop();
                }

                labelCurrentMoveEffect.Widget = labelCurrent;
                labelCurrentMoveEffect.X = rightLabelPosX;
                labelCurrentMoveEffect.Y = labelCurrent.Y;
                labelCurrentMoveEffect.Start();

                labelNext.X = leftLabelPosX;
                labelNext.Text = newSceneTitle;

                labelNextMoveEffect.Widget = labelNext;
                labelNextMoveEffect.X = centerLabelPosX;
                labelNextMoveEffect.Y = labelNext.Y;
                labelNextMoveEffect.Start();

                PrepareLabelFadeAnimation();
                PrepareButtonFadeAnimtion(backSceneTitle);
            }

            private void PrepareLabelFadeAnimation()
            {
                labelCurrentFadeOutEffect.Widget = labelCurrent;
                labelCurrentFadeOutEffect.Start();

                labelNextFadeInEffect.Widget = labelNext;
                labelNextFadeInEffect.Start();
            }

            private void PrepareButtonFadeAnimtion(string backSceneTitle)
            {
                if (backButtonCurrent.Visible)
                {
                    backButtonCurrentFadeOutEffect.Widget = backButtonCurrent;
                    backButtonCurrentFadeOutEffect.Start();
                }

                if (backSceneTitle != null)
                {
                    backButtonNextFadeInEffect.Widget = backButtonNext;
                    backButtonNextFadeInEffect.Start();
                    backButtonNext.Text = backSceneTitle;
                }
            }

            private static void PopAction(object sender, TouchEventArgs e)
            {
                UISystem.PopScene();
            }

            private static float EaseOutQuartInterpolator(float from, float to, float ratio)
            {
                return AnimationUtility.EaseOutQuartInterpolator(from, to, ratio);
            }

            private void OnPanelHideMoveEffectStopped(object sender, EventArgs e)
            {
                Visible = false;
            }

            private void OnLabelMoveEffectStopped(object sender, EventArgs e)
            {
                Label tmpLabel = labelCurrent;
                labelCurrent = labelNext;
                labelNext = tmpLabel;

                Button tmpButton = backButtonCurrent;
                backButtonCurrent = backButtonNext;
                backButtonNext = tmpButton;

                labelNext.Visible = false;
                backButtonNext.Visible = false;
            }
        }
    }
}
