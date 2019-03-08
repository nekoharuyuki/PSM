/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// @if LANG_EN
	/// <summary>
	/// The delegate type for functions added to the Scheduler has to match Node.Update's signature.
	/// We can't use anonymous functions because we want to be able to find and delete.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>デリゲート型の関数です。Scheduler は Node.Updateのシグニチャに一致するように追加してください。
	/// 関数名の検索と削除を行うため、匿名の関数は使用できません。
	/// </summary>
	/// @endif
	public delegate void DSchedulerFunc( float dt ); 

	/// @if LANG_EN
	/// <summary>
	/// The Scheduler is the singleton that manages Node related update functions registered by the user.
	/// Scheduled functions can be called everyframe, or at user special intervals in seconds.
	/// The Scheduler update loop is called inside Director.Update().
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Scheduler は、開発者が登録する update 関数に関連した Node を管理するシングルトンです。
	/// スケジュールされた関数は毎フレーム呼び出されます。または、開発者は特別な間隔で呼び出すこともできます。
	/// Scheduler の update ループは Director.Update() の内部で呼び出されます。
	/// </summary>
	/// @endif
	public class Scheduler
	{
		internal class Entry
		{
			internal Node m_node; // back pointer to the node, so we can query its SchedulerPaused
			internal DSchedulerFunc m_func;
			internal float m_interval; // period at which we call m_func, 0.0f means everyframe 
			internal float m_interval_counter;

			internal bool Valid
			{
				get { return m_func != null;}
			}

			public override string ToString()
			{
				return "m_interval=" + m_interval + " m_node=" + (m_node!=null?m_node.ToString():"") + " m_func=" + (m_func!=null?m_func.ToString():"") + " SchedulerPaused="+m_node.SchedulerPaused;
			}

			internal void invalidate()
			{
				m_node = null;
				m_func = null;
			}
		}

		Entry add_entry( Node node, DSchedulerFunc func, float interval )
		{
			Entry entry = new Entry() 
			{
				m_node = node,
				m_func = func,
				m_interval = interval,
			};

			if ( !m_nodes.Contains( node ) )
				m_nodes.Add( node );

			if ( node.m_scheduler_entries == null )
				node.m_scheduler_entries = new List< Entry >();

			node.m_scheduler_entries.Add( entry );

			return entry;
		}

		void invalidate_entry( Node node, DSchedulerFunc func )
		{
			if ( !m_nodes.Contains( node ) )
				return;	// no such node

			foreach ( Entry entry in node.m_scheduler_entries )
			{
				if ( entry.m_func == func )
					entry.invalidate();
			}
		}

		void invalidate_all_entries( Node node )
		{
			if ( !m_nodes.Contains( node ) )
				return;

			invalidate_all_entries( ref node.m_scheduler_entries );
		}

		void invalidate_all_entries( ref List< Entry > list )
		{
			foreach ( Entry entry in list )
				entry.invalidate();
		}

		const int max_priority = 3; // priority can be set in [-max_priority,+max_priority]
		public const int PriorityGroups = 2 * max_priority + 1;
		public const int DefaultPriority = 0;

		List< Entry >[] m_groups; // one list of scheduled entries per priority group
		List< Entry > m_cache; // temporary array
		HashSet< Node > m_nodes; // set of nodes that have scheduled functions  
		List< Node > m_nodes_to_remove;

		public Scheduler()
		{
			m_nodes = new HashSet< Node >();
			m_groups = new List< Entry >[PriorityGroups];
			m_cache = new List< Entry >();
			m_nodes_to_remove = new List< Node >();

			for ( int i = 0; i < PriorityGroups; ++i )
				m_groups[i] = new List< Entry >();
		}

		void schedule_internal( Node target, DSchedulerFunc func, float interval, int priority )
		{
			Entry entry = add_entry( target, func, interval );
			m_groups[ max_priority + priority ].Add( entry );
		}

		/// @if LANG_EN
		/// <summary>
		/// Register a Node function in the scheduler.
		/// </summary>
		/// <param name="target">The target node for the scheduled function.</param>
		/// <param name="func">The scheduled function.</param>
		/// <param name="interval">Period at which the function should be called (in seconds). Zero means "everyframe".</param>
		/// <param name="paused">Set the scheduler paused state for that node.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラに Node 関数を登録します。
		/// </summary>
		/// <param name="target">スケジュール関数を登録する対象ノード。</param>
		/// <param name="func">スケジュール関数。</param>
		/// <param name="interval">関数が呼び出される期間 (秒単位)。0なら毎フレーム呼び出されます。</param>
		/// <param name="paused">スケジューラを一時停止します。</param>
		/// @endif
		public void Schedule( Node target, DSchedulerFunc func, float interval, bool paused, int priority = DefaultPriority )
		{
			schedule_internal( target, func, interval, priority );
			target.SchedulerPaused = paused;
		}

		/// @if LANG_EN
		/// <summary>
		/// Remove a function from the scheduler.
		/// </summary>
		/// <param name="target">The target node for the removed function.</param>
		/// <param name="func">The function to remove.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラから関数を削除します。
		/// </summary>
		/// <param name="target">削除する関数をもった対象ノード。</param>
		/// <param name="func">削除する関数。</param>
		/// @endif
		public void Unschedule( Node target, DSchedulerFunc func )
		{
			invalidate_entry( target, func );
		}

		/// @if LANG_EN
		/// <summary>
		/// Remove all functions related to a given node from the scheduler.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラから指定されたノードに関連する全ての関数を削除します。
		/// </summary>
		/// @endif
		public void UnscheduleAll( Node target )
		{
			invalidate_all_entries( target );
		}

		/// @if LANG_EN
		/// <summary>
		/// Remove all functions from the scheduler.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラから全ての関数を削除します。
		/// </summary>
		/// @endif
		public void UnscheduleAll()
		{
			foreach ( Node node in m_nodes )
				invalidate_all_entries( node );
		}

		/// @if LANG_EN
		/// <summary>
		/// Schedule a Node's Update function to be called everyframe.
		/// </summary>
		/// <param name="target">The target node.</param>
		/// <param name="priority">Priority of the Update function in the scheduler.</param>
		/// <param name="paused">The scheduler paused stated for that node.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>ノードの Update 関数を毎フレーム呼び出すようにスケジュールします。
		/// </summary>
		/// <param name="target">対象ノード。</param>
		/// <param name="priority">スケジューラ内の Update 関数の優先順位。</param>
		/// <param name="paused">一時停止状態にします。</param>
		/// @endif
		public void ScheduleUpdateForTarget( Node target, int priority, bool paused )
		{
			schedule_internal( target, target.Update, 0.0f, priority );
			target.SchedulerPaused = paused;
		}

		/// @if LANG_EN
		/// <summary>
		/// Remove a Node's Update function from the scheduler.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラから Node の Update 関数を削除します。
		/// </summary>
		/// @endif
		public void UnscheduleUpdateForTarget( Node target )
		{
			Unschedule( target, target.Update );
		}
/* 
// use ActionsPaused directly 
		public void PauseTarget( Node target )
		{
			target.SchedulerPaused = true;
		}

		public void ResumeTarget( Node target )
		{
			target.SchedulerPaused = false;
		}

		public bool IsTargetPaused( Node target )
		{
			return target.SchedulerPaused;
		}
*/
		/// @if LANG_EN
		/// <summary>Print some debug information, content might vary in the future.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>デバッグ情報を出力します。(将来変更される可能性があります)
		/// </summary>
		/// @endif
		public void Dump()
		{
			string prefix = Common.FrameCount + " Scheduler: ";

			System.Console.WriteLine( prefix+"Node set" );

			foreach ( Node node in m_nodes )
			{
				foreach ( Entry entry in node.m_scheduler_entries )
				{
					Common.Assert( entry != null );
					System.Console.WriteLine( prefix + entry );
				}
			}

			int prio = 0;
			foreach ( List< Entry > group in m_groups )
			{
				if ( group.Count == 0 )	continue;

				System.Console.WriteLine( prefix + "group " + prio + " " + " entries:" + group.Count );

				foreach ( Entry entry in group )
				{
					Common.Assert( entry != null );
					System.Console.WriteLine( prefix + entry );
				}

				++prio;
			} 
		}

		internal void Update( float dt )
		{
			{
				m_nodes_to_remove.Clear();

				foreach ( Node node in m_nodes )
				{
					List< Entry > list = node.m_scheduler_entries;

					for ( int i=0; i < list.Count; )
					{
						if ( !list[i].Valid )
						{
							list[i] = list[ list.Count - 1 ];
							list.RemoveAt( list.Count - 1 );
						}
						else ++i;
					}

					if ( list.Count == 0 )
						m_nodes_to_remove.Add( node ); // no more scheduled functions
				}

				foreach ( Node node in m_nodes_to_remove )
					m_nodes.Remove( node );
			}

			foreach ( List< Entry > list in m_groups )
			{
				m_cache.Clear();

				// remove invalid entries from this group, and at the same 
				// time copy entries for this priority group in m_cache

				for ( int i=0; i < list.Count; )
				{
					if ( !list[i].Valid )
					{
						list[i] = list[ list.Count - 1 ];
						list.RemoveAt( list.Count - 1 );
					}
					else
					{
						m_cache.Add( list[i] );
						++i;
					}
				}

				// execute cached entries

				foreach ( Entry data in m_cache )
				{
					// Unschedule might have invalidated some entries within that loop aready, 
					// so we need to check for validity again
					if ( !data.Valid )
						continue;

//					if ( !data.Paused )
					if ( !data.m_node.SchedulerPaused )
						data.m_interval_counter += dt;

					if ( data.m_interval_counter > data.m_interval )
					{
						data.m_func( dt );
						data.m_interval_counter -= data.m_interval;
					}
				}
			}
		}

//		private static readonly Scheduler m_instance = new Scheduler();
		static internal Scheduler m_instance;

		/// @if LANG_EN
		/// <summary>The scheduler singleton.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラのシングルトン。
		/// </summary>
		/// @endif
		public static Scheduler Instance
		{
			get { return m_instance; }
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

