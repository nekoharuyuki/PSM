/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;


namespace Sce.PlayStation.Framework
{
	public class Actor
	{
		/// <summary>このアクターの名前。</summary>
		public string Name { get; set; }
		
		/// <summary>アクターの状態。</summary>
		public enum ActorStatus
		{
			/// <summary>行動中。</summary>
			Action,
			/// <summary>Update()のみ行う。Render()は行わない。</summary>
			UpdateOnly,
			/// <summary>Update()は行わず、Render()のみ行う。</summary>
			RenderOnly,
			/// <summary>休止中。Update()、Render()は行わない。</summary>
			Rest,
			/// <summary>現在未使用。アクターツリーから削除はされません。</summary>
			NoUse,
			/// <summary>死亡状態。この状態にすると、アクターツリーから削除されます。</summary>
			Dead,
		}
		
		ActorStatus status;
		public ActorStatus Status
		{
			get { return status; }
			set
			{
				status = value;
				StatusNextFrame = value;
			}
		}

		public ActorStatus StatusNextFrame;
		
		/// <summary>一番上の階層から数えた階層。</summary>
		protected Int32 level=0;
		
		protected List<Actor> children = new List<Actor>();
		
		public List<Actor> Children
		{
			get { return children;}	
		}
		
		public Actor()
		{
			this.Name = "no_name";
			this.Status = ActorStatus.Action;
		}
		
		
		public Actor(string name)
		{
			this.Name = name;
			this.Status = ActorStatus.Action;
		}
		
		public override string ToString ()
		{
			return this.Name;
		}
		
		/// <summary>更新。</summary>
		virtual public void Update()
		{ 
			foreach( Actor actorChild in children)
			{
				if(actorChild.Status == ActorStatus.Action || actorChild.Status == ActorStatus.UpdateOnly)
					actorChild.Update();	
			}
		}

		/// <summary>描画。</summary>
		virtual public void Render() 
		{ 
			foreach( Actor actorChild in children)
			{
				actorChild.Render();	
			}		}
		
		virtual public void AddChild(Actor actor)
		{
			children.Add(actor);
			actor.level = this.level+1;
		}
		
		
		
		/// <summary>nameのアクターを、自分を含めて子孫から探す。
		/// 最初に見つかったアクターを返す。
		/// </summary>
		virtual public Actor Search(string name)
		{
			if( this.Name == name)
				return this;

			Actor retActor;
			
			foreach (Actor actorChild in children)
			{
				if ((retActor = actorChild.GetActor(name)) != null)
					return retActor;
			}
			
			return null;
		}
		
		public Actor GetActor(string name)
		{
			if( this.Name == name)
				return this;

			Actor retActor;

			foreach (Actor actorChild in children)
			{
				if ((retActor = actorChild.GetActor(name)) != null)
					return retActor;
			}

			return null;
		}
		
		
		
		/// <summary>
		/// パス指定でActorインスタンスを取得する。
		/// aaa/bbb/cccの形式。先頭に/はつけない。
		/// 自分は検索対象外。
		/// </summary>
		public Actor SearchByPath(string path)
		{
			// pathを/で分断する。最初の/を取得。なかったら終了。
			int pos=path.IndexOf('/');
			if(pos!=-1)
			{
				string nameLeft = path.Substring( 0, pos );
				string nameRight = path.Substring( pos+1 );
				
				foreach( Actor actorChild in this.Children)
				{
					if(actorChild.Name == nameLeft)
						return actorChild.SearchByPath(nameRight);
				}
			}
			else
			{
				// '/'が見つからなかった。
				foreach( Actor actorChild in this.Children)
				{
					if(actorChild.Name == path)
						return actorChild;
				}
			}
			
			return null;
		}
		
		
		/// <summary>アクターのステータスをチェックし、処理をおこなう。各フレームの描画終了後に呼んでください。</summary>
		virtual public void CheckStatus()
		{
			if (this.status != this.StatusNextFrame)
				this.status = this.StatusNextFrame;
			
			
			//@e Set dead flags for all the children if the player is dead.
			//@j 自分が死亡していたら、子すべてに死亡フラグを立てる。
			if( this.Status == ActorStatus.Dead)
			{
				foreach(Actor actorChild in children)
				{
					actorChild.Status = ActorStatus.Dead;
				}
			}
			
			//@e Visit children with recursive call.
			//@j 再帰処理で子を巡回していく。
			foreach(Actor actorChild in children)
			{
				actorChild.CheckStatus();
			}
			
			//@e Delete a child where the dead flag is set from a list.
			//@j 死亡フラグのたっている子をリストから削除。
			children.RemoveAll(CheckDeadActor);
		}

		
		static bool CheckDeadActor(Actor actor)
		{
			//@e Delete the elements to be proper with this condition.
			//@j この条件で真になる要素を削除。
			return actor.Status == ActorStatus.Dead; 
		}
		
		/// <summary>生きている子孫の数を取得する。自分自身は数えない。</summary>
		public Int32 GetAliveChildren()
		{
			Int32 cnt = 0;

			foreach (Actor actorChild in children)
			{
				if (actorChild.Status != ActorStatus.Dead)
				{
					cnt++;
					cnt += actorChild.GetAliveChildren();
				}
			}

			return cnt;
		}
		

		/*
		/// <summary>デストラクタ。</summary>
		~Actor()
		{
			Console.WriteLine("~"+Name);
		}
		*/

		
	}
}
