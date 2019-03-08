/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

namespace TwitterStreamingSample
{
	public class Dispatcher
	{
		private object m_SyncObject = new object();
		private List<Action> m_Actions = new List<Action>();
		
		public void BeginInvoke(Action action)
		{
			lock(this.m_SyncObject)
			{
				this.m_Actions.Add(action);
			}
		}
		
		public void CancelInvoke(Action action)
		{
			lock(this.m_SyncObject)
			{
				this.m_Actions.Contains(action);
				this.m_Actions.Remove(action);
			}
		}
		
		public void DoEvents()
		{
			var funcGetCount =
				new Func<int>(
					() =>
					{
						lock(this.m_SyncObject)
						{
							return this.m_Actions.Count;
						}
					});
			var funcGetAction =
				new Func<Action>(
					() =>
				{
					lock(this.m_SyncObject)
					{
						if(0 >= this.m_Actions.Count){ return null;}
						
						var action = this.m_Actions[0];
						this.m_Actions.RemoveAt(0);
						return action;
					}
				});
			while(0 < funcGetCount())
			{
				var action = funcGetAction();
				if(null == action) { continue; }
				
				action.Invoke();
			}
		}
	}
}

