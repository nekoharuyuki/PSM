
using Sce.PlayStation.Core;

namespace SirAwesome
{
	public class PhysicsGameEntity
		 : GameEntity
	{
		public Vector2 Velocity = Vector2.Zero;
		public Vector2 AirFriction = Vector2.One;
		public Vector2 GroundFriction = Vector2.One;
        public float AirborneTime = 0.15f;
		
		public override void Tick(float dt)
		{
			const float GravityForce = -0.45f;
			
            Velocity += Vector2.UnitY * GravityForce;
            Position += Velocity;
            
			if (AirborneTime <= 0.0f)
	            Velocity *= GroundFriction;
	        else
	            Velocity *= AirFriction;
            
            Position = new Vector2(
				FMath.Clamp(Position.X, -260.0f, 1030.0f),
				FMath.Max(Position.Y, Game.Instance.FloorHeight)
			);
            Position = new Vector2(Position.X, FMath.Max(Position.Y, Game.Instance.FloorHeight));
            
			// cleanup infinitesmal float values
			if (System.Math.Abs(Velocity.X) < 0.0001f)
				Velocity = new Vector2(0.0f, Velocity.Y);
			if (System.Math.Abs(Velocity.Y) < 0.0001f)
				Velocity = new Vector2(Velocity.X, 0.0f);
				
			// if on floor, stop fall
			if (Position.Y <= Game.Instance.FloorHeight)
            {
                AirborneTime = 0.0f;
				Velocity = new Vector2(Velocity.X, FMath.Max(Velocity.Y, 0.0f));
            }
            else
            {
                AirborneTime += dt;
            }

			base.Tick(dt);
			
			GetTransform();
		}
	};
}