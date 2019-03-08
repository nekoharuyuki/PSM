/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core.Imaging;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace PuzzleGameDemo
{
	public class ScissorNode : Node
	{
		ImageRect desired;
		ImageRect restore;

		public ScissorNode(int x, int y, int w, int h)
		{
			desired = new ImageRect(x, y, w, h);
			restore = desired;
		}

		public override void PushTransform()
		{
			base.PushTransform();
			restore = Director.Instance.GL.Context.GetScissor();
			Director.Instance.GL.Context.SetScissor(desired);
		}

		public override void PopTransform()
		{
			Director.Instance.GL.Context.SetScissor(restore);
			base.PopTransform();
		}
	}
}

