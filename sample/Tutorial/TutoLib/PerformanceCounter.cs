using System;
using System.Diagnostics;


namespace Tutorial.Utility
{
	public class PerformanceCounter
	{
		protected Stopwatch stopwatch;
		
		int pinSize;
		int currentPin=0;
		int count=0;
		int[,] time;
		int index=0;
		float[] timePercent;
		string [] pinName;
		
		
		public PerformanceCounter (int pinSize, Stopwatch stopwatch)
		{
			this.pinSize=pinSize;
			time= new int[2,pinSize];
			timePercent= new float[pinSize];
			pinName=new string[pinSize];
			
			this.stopwatch = stopwatch;
		}
		
		public void StartLapTime()
		{
			count=currentPin;
			currentPin=0;
			time[index,currentPin] = (int)stopwatch.ElapsedTicks;// start
		}
		
		//@e Setting text to pin.
		//@j pinに文字列を設定。
		public void SetLapTime(string name)
		{
			if(currentPin-1<pinSize)
			{
				pinName[currentPin] = name;
				++currentPin;
				time[index,currentPin] = (int)stopwatch.ElapsedTicks;// start
			}
		}
		
		
		/// <summary>処理にかかった時間を計算する。</summary>
		public void CalculateProcessTime(DebugString debugString)
		{
			float ticksPerFrame = Stopwatch.Frequency / 60.0f;
			
			int preIndex=(index==0) ? 1: 0;
			
			for(int i=0; i<count; ++i)
			{
				timePercent[i]=(time[preIndex, i+1]-time[preIndex,i])/ticksPerFrame;
				debugString.WriteLine(string.Format("{0}={1,6:N}%", pinName[i], timePercent[i] * 100));
			}
		}
		
		public void ChangeIndex()
		{
			index=(index==0) ? 1: 0;
		}
	}
}

