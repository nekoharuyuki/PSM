using System;

namespace Sample
{
	public class FrameRate
	{
	    private DateTime m_baseTime;   //測定基準時間
	    private ulong m_count;      //フレーム数
	    private float   framerate;  //フレームレート
	    
	    //コンストラクタ
	    public FrameRate()
		{
	        m_baseTime = DateTime.Now;
			m_count = 0;
	    }
	
	    //フレームレートを取得
	    public float getFrameRate
		{
			get
			{
		        return framerate;
			}
	    }
	
	    //描画時に呼ぶ
	    public void Count()
		{
	        ++m_count;        //フレーム数をインクリメント
	        DateTime now = DateTime.Now;      //現在時刻を取得
	        if (now.Ticks - m_baseTime.Ticks >= 2000000)
	        {       //0.2秒以上経過していれば
	            framerate = (float)(m_count * 10000000) / (float)(now.Ticks - m_baseTime.Ticks);        //フレームレートを計算
	            m_baseTime = now;     //現在時刻を基準時間に
	            m_count = 0;          //フレーム数をリセット
	        }
	    }
	}
}

