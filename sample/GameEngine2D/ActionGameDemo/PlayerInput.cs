
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace SirAwesome
{
	public class PlayerInput
	{
    	static float FilterAnalogValue( float value, float deadzone )
    	{
            float sign = ( value > 0.0f ? 1.0f : -1.0f );
    		value *= sign;

    		if ( value < deadzone ) return 0.0f;
    		else return sign * ( value - deadzone ) / ( 1.0f - deadzone );
    	}

		public static float LeftRightAxis()
		{
			GamePadData data = GamePad.GetData(0);
			
			if (Input2.GamePad0.Left.Down)
				return -1.0f;
				
			if (Input2.GamePad0.Right.Down)
				return 1.0f;
				
			return FilterAnalogValue( data.AnalogLeftX, 0.08f );
		}
		
		public static bool JumpButton()
		{
			if (Input2.GamePad0.Cross.Press)
				return true;
				
			return false;
		}
		
		public static bool AttackButton()
		{
			if (Input2.GamePad0.Square.Press)
				return true;
				
			return false;
		}
		
		public static bool SpecialButton()
        {
			if (Input2.GamePad0.Circle.Press)
				return true;
				
			return false;
		}
		
		public static bool AnyButton()
		{
			return
				AttackButton() || 
				JumpButton() ||
				SpecialButton();
		}
	}
}
