/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using System.Diagnostics; // for [Conditional("DEBUG")]
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// @if LANG_EN
	/// <summary>
	/// Node is the base class for all scenegraph nodes. It holds a standard 2D transform, 
	/// a list of children and a handle to its parent (a node can have at most 1 parent).
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ノードは全てのシーングラフノードの基底クラスです。標準的な2D変換行列、子のリスト、親へのハンドラを持ちます。ノードは多くとも1つの親をもつことができます。
	/// </summary>
	/// @endif
	public class Node
	{
		Vector2 m_position;
		Vector2 m_rotation;	// stored as (cos,sin), not angle
		Vector2 m_scale;
		Vector2 m_skew;
		Vector2 m_skew_tan;
		Vector2 m_pivot;
		int m_order; // nodes with m_order < 0 will be drawn before their Parent (same as cocos2d logic)
		bool m_cached_local_transform_info_is_identity;
		bool m_cached_local_transform_info_is_orthonormal;
		bool m_cached_local_transform_info_is_dirty;
		Matrix3 m_cached_local_transform;
		bool m_is_running;
		byte m_scheduler_and_action_manager_pause_flag;	// pause flag used by ActionManager.Instance and Director.Instance.Sheduler
		internal List< Scheduler.Entry > m_scheduler_entries; // used by Scheduler (null if unused)
		internal List< ActionBase > m_action_entries; // used by ActionManager (null if unused)

		/// @if LANG_EN
		/// <summary>See GetTransform() for details about how the transform matrix is constructed.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>変換行列の構築に関しての詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public Vector2 Position { get { return m_position ;} set { m_position = value; m_cached_local_transform_info_is_dirty = true;}}

		/// @if LANG_EN
		/// <summary>
		/// Rotation is directly stored as a (cos,sin) unit vector. 
		/// This the code to be potentially cos,sin calls free, and as a side
		/// effect we get the direction vector for free, and also avoid all the usual angle wrapping problems.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Rotation は (cos,sin)の単位ベクトルとして直接格納されます。
		/// 
		/// 変換行列の構築に関しての詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public Vector2 Rotation { get { return m_rotation ;} set { m_rotation = value; m_cached_local_transform_info_is_dirty = true;}}

		/// @if LANG_EN
		/// <summary>
		/// RotationNormalize is like Rotation, but it normalizes on set,
		/// to prevent the unit vector from drifting because of accumulated numerical imprecision.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>RotationNormalize は Rotation に似ていますが、正規化を行います。
		/// これにより、蓄積される計算誤差で単位ベクトルがドリフトするのを防ぎます。
		/// 変換行列の構築の詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public Vector2 RotationNormalize { get { return m_rotation ;} set { m_rotation = value.Normalize(); m_cached_local_transform_info_is_dirty = true;}}

		/// @if LANG_EN
		/// <summary>
		/// Rotate the object by an angle 'angle'.
		/// Note that this function simply affects the the Rotation/Angle property (it simply "increments" the angle, regardless of Pivot and Position; and all those are combined in GetTransform().)
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// 引数 'angle' でオブジェクトを回転します。
		/// この関数は単に Rotation / Angle プロパティに影響を与えることに注意してください。( Pivot や Positionに関わらず、角度をインクリメントします。それらのすべては GetTransform() で結合されます。)
		/// 変換行列の構築の詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public void Rotate( float angle ) { Rotation = Rotation.Rotate( angle );}

		/// @if LANG_EN
		/// <summary>
		/// Rotate the object by an angle, the angle is given as a unit vector 'rotation'
		/// This lets you precompute the cos,sin needed during rotation.
		/// Note that this function simply affects the the Rotation/Angle property (it simply "increments" the angle, regardless of Pivot and Position; and all those are combined in GetTransform().)
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>単位ベクトル 'rotation' として渡される角度でオブジェクトを回転します。
		/// これで、回転中に必要な cos,sinを事前に計算できます。
		/// この関数は Rotation/Angle プロパティに影響を与えることに注意してください。( Pivot や Positionに関わらず、角度をインクリメントします。それらのすべては GetTransform() で結合されます。)
		/// 変換行列の構築の詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public void Rotate( Vector2 rotation ) { Rotation = Rotation.Rotate( rotation );}

		/// @if LANG_EN
		/// <summary>
		/// This property lets you set/get rotation as a angle. This is expensive and brings the usual
		/// angle discontinuity problems. The angle is always stored and returned in the the range -pi,pi.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このプロパティを使うと、回転を角度として set/getすることができます。
		/// この処理は重く、角度の不連続の問題を引き起こします。角度は常に格納され、かつ -pi,piの範囲で値を返します。
		/// 変換行列の構築の詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public float Angle { get { return Math.Angle( Rotation );} set { Rotation = Vector2.Rotation( value );}}

		/// @if LANG_EN
		/// <summary>
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// 変換行列の構築の詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public Vector2 Scale { get { return m_scale;} set { m_scale = value; m_cached_local_transform_info_is_dirty = true;}}

		/// @if LANG_EN
		/// <summary>
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// 変換行列の構築の詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public Vector2 Skew { get { return m_skew;} 

			set
			{ 
				m_skew = value;
				// cache the tan too
				m_skew_tan = new Vector2( FMath.Tan( m_skew.X ), FMath.Tan( m_skew.Y ) );
				m_cached_local_transform_info_is_dirty = true;
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Pivot is the pivot used for scale and rotation, and is expressed in this Node's local 'normalized' space.
		/// Which means that (0.5,0.5) is always the center of the object, regardless of the Scale for example.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Pivot はスケールや回転に使用される中心軸です。そして Node のローカル '正規化' 空間として表現されます。
		/// 例えば、スケールに関わりなく、(0.5, 0.5)は常にオブジェクトの中心です。
		/// 変換行列の構築の詳細は、GetTransform() を参考にしてください。
		/// </summary>
		/// @endif
		public Vector2 Pivot { get { return m_pivot;} set { m_pivot = value; m_cached_local_transform_info_is_dirty = true;}}

		/// @if LANG_EN
		/// <summary>
		/// VertexZ is the value set as the z coordinate during drawing. Note that by default ortho view only
		/// shows the [-1,1] Z range, just set Camera.Znear and Camera.Zfar if you want more.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>VectorZ は描画中、Z座標としての値のセットです。デフォルトでは、ortho ビューは Z範囲[-1, 1]を示し、任意で Camera.Znear と Camera.Zfar をセットします。
		/// </summary>
		/// @endif
		public float VertexZ; 

		/// @if LANG_EN
		/// <summary>If Visible is false, this node and its children are not drawn.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Visible が false の場合、このノードとその子は描画されません。
		/// </summary>
		/// @endif
		public bool Visible;

		protected Node m_parent;
		/// @if LANG_EN
		/// <summary>The parent node in the scenegraph. A node can only be the child of at most one parent node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シーングラフ内の親ノードです。ひとつのノードは、多くともひとつの親ノードの子になります。
		/// </summary>
		/// @endif
		public Node Parent { get { return m_parent; } }

		protected List< Node > m_children;
		/// @if LANG_EN
		/// <summary>The list of children nodes.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>子ノードのリスト。
		/// </summary>
		/// @endif
		public List< Node > Children { get { return m_children; } }

		/// @if LANG_EN
		/// <summary>
		/// The delegate type used by AdHocDraw property.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>AdHocDraw で使用されるデリゲート型。
		/// </summary>
		/// @endif
		public delegate void DDraw();

		/// @if LANG_EN
		/// <summary>
		/// If set, AdHocDraw gets called in the base Draw function. This is used mostly so we can setup simple scenes 
		/// without always having to derive just so we can define a Draw function.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>セットされた場合、baseの描画関数内でAdHocDrawが呼び出されます。
		/// 描画関数の定義のためにクラスを派生させずに、シンプルなシーンをセットアップするために使用されます。
		/// </summary>
		/// @endif
		public event DDraw AdHocDraw;

		/// @if LANG_EN
		/// <summary>
		/// You can use Node.Camera as a workaround to the fact there is normally only one camera in the scene.
		/// If Node.Camera is set, all transforms up to this node are ignored, and Node.Camera is push/pop
		/// everytime we draw this node.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シーン内には通常ひとつのカメラしか配置されていないことの対応策として、 Node.Camera を使うことができます。
		/// Node.Camera が設定されている場合、このノードまでの全ての変換行列は無視され、Node.Camera は、このノードを描画するたびに、プッシュ/ポップされます。
		/// </summary>
		/// @endif
		public ICamera Camera = null; 

		/// @if LANG_EN
		/// <summary>Shortcut to get the camera as a Camera2D.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Camera2D としてのカメラを取得するためのショートカット。
		/// </summary>
		/// @endif
		public Camera2D Camera2D { get { return(Camera2D)Camera;}}

		/// @if LANG_EN
		/// <summary>Shortcut to get the camera as a Camera3D.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Camera3D としてのカメラを取得するためのショートカット。
		/// </summary>
		/// @endif
		public Camera3D Camera3D { get { return(Camera3D)Camera;}}

		/// @if LANG_EN
		/// <summary>This property is true when this node is between its OnEnter()/OnExit() calls.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードが OnEnter()/OnExit() 呼び出しの合間である時、このプロパティは true になります。
		/// </summary>
		/// @endif
		public bool IsRunning { get { return m_is_running;}}

		/// @if LANG_EN
		/// <summary>The draw order value that was set in ReorderChild() or AddChild().</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ReorderChild() または AddChild() でセットされた描画順序の値。
		/// </summary>
		/// @endif
		public int Order { get { return m_order; }}

		/// @if LANG_EN
		/// <summary>Identifier for user.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>開発者用の識別子。
		/// </summary>
		/// @endif
		public string Name;

		/// @if LANG_EN
		/// <summary>Node constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public Node()
		{
			Position = GameEngine2D.Base.Math._00;
			Rotation = Math._10;
			Scale = Math._11;
			Skew = GameEngine2D.Base.Math._00;
			Pivot = GameEngine2D.Base.Math._00;
			VertexZ = 0.0f;
			m_order = 0;
			m_children = new List< Node >();
			Visible = true;
			m_parent = null;
			m_cached_local_transform_info_is_identity = true; // note: matrix might be identity even if this is false 
			m_cached_local_transform_info_is_orthonormal = true;
			m_cached_local_transform_info_is_dirty = false;
			m_cached_local_transform = Matrix3.Identity;
			m_is_running = false;
			m_scheduler_and_action_manager_pause_flag = 0; // we make the Scheduler and ActionManager pause flags 
														   // intrusive to Node, so that we don't need to hash (hope 
														   // it's the correct behavior)

//			Director.Instance.DebugLog( " Node construtor " + DebugInfo() );
		}

//		public virtual void OnParentChanged()
//		{
//		}

		~Node()
		{
//			Director.Instance.DebugLog( " Node destructor " + DebugInfo() );

			RemoveAllChildren( true );
			Cleanup();
		}

		/// @if LANG_EN
		/// <summary>
		/// This is called before drawing the node and its children.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ノードとその子を描画する前にこの関数が呼び出されます。
		/// </summary>
		/// @endif
		public virtual void PushTransform()
		{
			if ( Camera != null ) 
				Camera.Push();

			Director.Instance.GL.ModelMatrix.Push();

			if ( m_cached_local_transform_info_is_orthonormal )
				Director.Instance.GL.ModelMatrix.Mul1( GetTransform().Matrix4() );
			else Director.Instance.GL.ModelMatrix.Mul( GetTransform().Matrix4() );

			if ( VertexZ != 0.0f )
				Director.Instance.GL.ModelMatrix.Translate( new Vector3( 0.0f, 0.0f, VertexZ ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// This is called after drawing the node and its children.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ノードとその子が描画されたあとにこの関数が呼び出されます。
		/// </summary>
		/// @endif
		public virtual void PopTransform()
		{
			Director.Instance.GL.ModelMatrix.Pop();

			if ( Camera != null ) 
				Camera.Pop();
		}

		/// @if LANG_EN
		/// <summary>
		/// This function gets called when the scene is started by the Director.Instance.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シーンが Director.Instance によって開始されたとき、この関数が呼び出されます。
		/// </summary>
		/// @endif
		public virtual void OnEnter()
		{
			Director.Instance.DebugLog( " OnEnter " + DebugInfo() );

			foreach ( Node child in Children )
				child.OnEnter();

			ResumeSchedulerAndActions();
			m_is_running = true;
		}

		/// @if LANG_EN
		/// <summary>Delegate for OnExit() events.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>OnExit() イベントのデリゲート。
		/// </summary>
		/// @endif
		public delegate void DOnExitEvent();

		/// @if LANG_EN
		/// <summary>
		/// List of events to perform when OnExit gets called.
		/// The list gets cleared after it is executed.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>OnExit が呼び出された時、実行するイベントのリスト。
		/// 実行されたあと、リストはクリアされます。
		/// </summary>
		/// @endif
		public event DOnExitEvent OnExitEvents;

		/// @if LANG_EN
		/// <summary>
		/// This function gets called when we exit the Scene or when a child is explicitely removed 
		/// with RemoveChild() or RemoveAllChildren().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シーンを終了したり、子が RemoveChild() や RemoveAllChildren() で明示的に削除されたとき、この関数が呼び出されます。
		/// </summary>
		/// @endif
		public virtual void OnExit()
		{
			Director.Instance.DebugLog( " OnExit " + DebugInfo() );

			PauseSchedulerAndActions();
			m_is_running = false;

			foreach ( Node child in Children )
				child.OnExit();

			if ( OnExitEvents != null )
			{
//				System.Console.WriteLine( DebugInfo()+ " OnExitEvents got called" );
				OnExitEvents();
				OnExitEvents = null;
			}
		}

//		void ListIDisposable( ref List< System.IDisposable > list )
//		{
//		}

		// list up all disposable objects in the subtree 
		// starting at this (including this)
		void ListIDisposable( ref List< Node > list )
		{
			System.Type[] types = this.GetType().GetInterfaces();
			foreach ( System.Type type in types )
			{
				if ( type == typeof(System.IDisposable) ) 
				{
					list.Add( this );
					break;
				}
			}

			foreach ( Node child in Children )
				child.ListIDisposable( ref list );
		}

		/// @if LANG_EN
		/// <summary>
		/// Recurse through all the subtree (including this node)
		/// and register Dispose() functions for all the disposable
		/// objects. Cleanup is called first to make sure we
		/// don't Dispose() of running objects.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>すべてのサブツリー(このノードを含む)を再帰し、すべての破棄可能なオブジェクトに Dispose() 関数を登録します。
		/// 実行しているオブジェクトが Dispose() してないことを最初に確認し、Cleanupを呼び出します。
		/// </summary>
		/// @endif
		public void RegisterDisposeOnExitRecursive()
		{
			OnExitEvents += () => 
				{ 
					Cleanup();
				};

			List< Node > list = new List< Node >();
			ListIDisposable( ref list );
			foreach ( Node disposable in list )
			{
//				System.Console.WriteLine( disposable.DebugInfo() + "..." );
				RegisterDisposeOnExit( (System.IDisposable)disposable );
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Register a call to Dispose() in the OnExit() function of this node.
		/// For example, when you want to Dispose() of several objects (TextureInfo,
		/// FontMap, etc) when you exit a Scene node.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードの OnExit()関数内に、Dispose() の呼び出しを登録します。
		/// 例えば、いくつかのオブジェクト(TextureInfo, FontMap, など)で Dispose() を行いたいときや、 Scene ノードを終了するときです。
		/// </summary>
		/// @endif
		public void RegisterDisposeOnExit( System.IDisposable disposable )
		{
			OnExitEvents += () => 
				{ 
//					System.Console.WriteLine( "registered dispose called!" );
					disposable.Dispose(); 
				};

//			System.Console.WriteLine( DebugInfo()+ " RegisterDisposeOnExit registered a disposable object" );
		}


/// 	<summary>
//		/// DisposeTraverse calls Dispose( flags ) on this node and all its subtree.
//		/// </summary>
/// 	<summary>
/// 	</summary>
//		public void DisposeTraverse()
//		{
//			// if any dispose is happening, make sure nothing is keeping references 
//			// in Scheduler and ActionManager, since are going to Dispose/break some
//			// resources: cleanup all subtree
//
//			Cleanup();
//
//			// the recurse to call Dispose() on each subnode
//			Traverse( ( node, depth ) =>{ Common.DisposeAndNullifyIfIDisposable< Node >( node ); return true;}, 0 );
//		}

		void insert_child( Node child, int order )
		{
			Node last = null;
			if ( Children.Count != 0 )
				last = Children[ Children.Count - 1 ];

			if ( last == null || last.m_order <= order )
				Children.Add( child );
			else
			{
				int index = 0;
				foreach ( Node c in Children )
				{
					Common.Assert( c != null );

					if ( c.m_order > order )
					{
						Children.Insert( index, child );
						break;
					}
					index++;
				}
			}

			child.m_order = order;
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a child with draw priority. 
		/// </summary>
		/// <param name="child">The child to add.</param>
		/// <param name="order">The added node's draw priority. Draw order follows order numerical order, 
		/// negative priorities mean this child node will be drawn before its parent, and children 
		/// with positive priorities get drawn after their parent.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>子に描画プロパティを追加します。
		/// </summary>
		/// <param name="child">追加する子ノード。</param>
		/// <param name="order">追加するノードの描画プロパティ。描画順序は数値順に行われます。
		/// 負の数のプロパティはこの子が親より前に描画されることを意味します。
		/// 正の数のプロパティは親より後に描画されます。</param>
		/// @endif
		public void AddChild( Node child, int order )
		{
			Common.Assert( child != this, "Trying to add " + child + " as child of itself." );
			Common.Assert( child != null, "Trying to add a null child.");
			Common.Assert( child.Parent == null, "Child " + child + " alreay has a parent, it can't be added somewhere else." );

//			Director.Instance.DebugLog( " AddChild " + child.DebugInfo() + " to " + DebugInfo() );

			insert_child( child, order );

			child.m_parent = this;
//			OnParentChanged();

			if ( m_is_running )
				child.OnEnter();
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a child to this node, using its current order.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在の順序を使って、このノードに子を追加します。
		/// </summary>
		/// @endif
		public void AddChild( Node child )
		{
			AddChild( child, child.m_order );
		}

		/// @if LANG_EN
		/// <summary>
		/// Remove a child from this node.
		/// </summary>
		/// <param name="child">The child to remove.</param>
		/// <param name="do_cleanup">Do we call Cleanup for the removed node.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードから子を削除します。
		/// </summary>
		/// <param name="child">削除する子。</param>
		/// <param name="do_cleanup">削除するノードで Cleanup を呼び出す。</param>
		/// @endif
		public void RemoveChild( Node child, bool do_cleanup/*, bool dispose */ )
		{
//			Director.Instance.DebugLog( " RemoveChild " + child.DebugInfo() + " from " + DebugInfo() );

			if ( child == null )
				return;

			if ( Children.Contains( child ) )
			{
				child.on_remove( do_cleanup/*, dispose*/ );
				Children.Remove( child );
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// This is equivalent to calling RemoveChild( dispose_flags ) for all children.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>この関数は、全ての子に対して、RemoveChild( dispose_flags )を呼び出すのと、等価です。
		/// </summary>
		/// @endif
		public void RemoveAllChildren( bool do_cleanup/*, bool dispose */ )
		{
//			Director.Instance.DebugLog( " RemoveAllChildren(" + do_cleanup + ") from " + DebugInfo() );

			foreach ( Node child in Children )
			{
				child.on_remove( do_cleanup/*, dispose*/ );
				child.m_parent = null;
//				OnParentChanged();
			}

			Children.Clear();
		}

		void on_remove( bool do_cleanup/*, bool dispose */ )
		{
			if ( m_is_running )
				OnExit();

//			if ( dispose )
//				DisposeTraverse();

			if ( do_cleanup )
				Cleanup();

			m_parent = null;
		}

		/// @if LANG_EN
		/// <summary>
		/// Change order of a child within the Children list.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内での、子の描画順序を変更します。
		/// </summary>
		/// @endif
		void ReorderChild( Node child, int order )
		{
			// fixme: wasteful
			Children.Remove( child );
			insert_child( child, order );
		}

		/// @if LANG_EN
		/// <summary>
		/// Change the draw order value for this node (see AddChild for details about the draw order).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードの描画順序の値を変更します。描画順序の詳細は AddChild を参考にしてください。
		/// </summary>
		/// @endif
		void Reorder( int order )
		{
			if ( Parent != null )
				Parent.ReorderChild( this, order );
		}

		/// @if LANG_EN
		/// <summary>
		/// Function type to pass to the .Traverse method.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>巡回メソッドに渡す関数の型。
		/// </summary>
		/// @endif
		public delegate bool DVisitor( Node node, int depth );

		/// @if LANG_EN
		/// <summary>
		/// Call the 'visitor' function for this node and all its children, recursively.
		/// Interrupt traversing if visitor returns false.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードの引数 'visitor'関数を呼び出し、かつこのノードの全ての子を再帰的に呼び出します。
		/// visitor が false を返した場合、巡回を中断します。
		/// </summary>
		/// @endif
		public virtual void Traverse( DVisitor visitor, int depth )
		{
			if ( !visitor( this, depth ) )
				return;

			foreach ( Node child in Children )
				child.Traverse( visitor, depth + 1 );
		}

		/// @if LANG_EN
		/// <summary>
		/// This called by Director only, but PushTransform, Draw,
		/// and PopTransform can be overriden.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>この関数は、 Director によってのみ呼び出されます。しかし、PushTransform, Draw, PopTransform は上書きすることができます。
		/// </summary>
		/// @endif
		virtual public void DrawHierarchy()
		{
			if ( !Visible )
				return;

			////Common.Profiler.Push("DrawHierarchy's PushTransform");
			PushTransform();
			////Common.Profiler.Pop();

			int index=0;
			for ( ; index < Children.Count; ++index )
			{
				if ( Children[index].Order >= 0 )	break;
				Children[index].DrawHierarchy();
			}

			////Common.Profiler.Push("DrawHierarchy's PostDraw");
			Draw();
			////Common.Profiler.Pop();

			for ( ; index < Children.Count; ++index )
				Children[index].DrawHierarchy();

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawPivot ) != 0 )
				DebugDrawPivot();

			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawContentLocalBounds ) != 0 )
				DebugDrawContentLocalBounds();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PopTransform");
			PopTransform();
			////Common.Profiler.Pop();

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawTransform ) != 0 )
				DebugDrawTransform();
//			#endif
		}

		/// @if LANG_EN
		/// <summary>
		/// Renders what's *inside* the PushTransform / PopTransform.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>PushTransform / PopTransform 内の描画を実行します。
		/// </summary>
		/// @endif
		public virtual void Draw() 
		{
			if ( AdHocDraw != null )
				AdHocDraw();
		}

		/// @if LANG_EN
		/// <summary>
		/// The update function.
		/// The Director decides how many times a frame this function should be called, and with which delta time. 
		/// At the moment, Update functions are called once using the frame delta time as it is.
		/// </summary>
		/// <param name="dt">Delta time in seconds.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update 関数。 Director は1フレームあたりと経過時間あたりで、この関数を何度呼び出すか決定します。
		/// 現時点では、Update 関数は、フレーム経過時間をそのまま使って、一度呼び出されています。
		/// </summary>
		/// <param name="dt">1秒あたりの経過時間。</param>
		/// @endif
		public virtual void Update( float dt )
		{
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw bounds of local content and pivot, in Node local space.
		/// Normally you don't have to override this function, you just 
		/// override GetlContentLocalBounds() and this function shows it
		/// when DebugFlags.DrawContentLocalBounds is set for example. 
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ノードのローカル空間内に、ローカルコンテントの境界と中心軸を描画します。
		/// 普通は、この関数を上書きする必要はありません。
		/// GetlContentLocalBounds()を上書きし、例えば DebugFlags.DrawContentLocalBounds が設定されたとき、この関数はそれを表示します。
		/// </summary>
		/// @endif
		public virtual void DebugDrawContentLocalBounds()
		{
			var content_local_bounds = new Bounds2();
			GetlContentLocalBounds( ref content_local_bounds );

			Director.Instance.DrawHelpers.SetColor( Colors.Yellow );
			Director.Instance.DrawHelpers.DrawBounds2( content_local_bounds );
		}

		public void DebugDrawPivot()
		{
			Director.Instance.DrawHelpers.SetColor( Colors.White );
			Director.Instance.DrawHelpers.DrawDisk( Pivot, 0.1f, 12 );
		}

		/// @if LANG_EN
		/// <summary>
		/// A scale factor used by DebugDrawTransform to draw arrows.
		/// By default this is 1.0f, which means that unit length arrows are of length 1 on screen. 
		/// he game world showed on screen is too big, arrows of length one might be less then 1 pixel,
		/// and you won't be able to see them even through they are being drawn. In that case you can 
		/// scale them with DebugDrawTransform.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>矢印を描画する DebugDrawTransform によって使用されるスケール係数。
		/// デフォルトではこの値は1.0fで、矢印の単位の長さは、スクリーン上で長さ1にあたります。
		/// スクリーン上で表示されるゲームの世界では大きすぎるので、長さ1の矢印は、1ピクセル以下であるかもしれず、それらが描画されていても見ることができないかもしれません。
		/// その場合、DebugDrawTransform でそれらをスケールすることができます。
		/// </summary>
		/// @endif
		public static float DebugDrawTransformScale = 1.0f;

		/// @if LANG_EN
		/// <summary>
		/// Draw the local coordinate system, as arrows, in Parent Node's local space.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>親ノードのローカル空間内での、ローカル座標系を描画します。</summary>
		/// @endif
		public void DebugDrawTransform()
		{
			Matrix3 mat = GetTransform();

			mat.X.Xy *= DebugDrawTransformScale;
			mat.Y.Xy *= DebugDrawTransformScale;

			Director.Instance.DrawHelpers.DrawCoordinateSystem2D( mat, new DrawHelpers.ArrowParams( DebugDrawTransformScale ) );
		}
		
		/// @if LANG_EN
		/// <summary>Start an action on this node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードのアクションを開始します。</summary>
		/// @endif
		public void RunAction( ActionBase action )
		{
			ActionManager.Instance.AddAction( action, this/*, !m_is_running*/ );
			action.Run();
		}

		/// @if LANG_EN
		/// <summary>Stop all actions on this node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードの全てのアクションを停止します。</summary>
		/// @endif
		public void StopAllActions()
		{
			if ( ActionManager.Instance != null ) {
				ActionManager.Instance.RemoveAllActionsFromTarget( this );
			}
		}

		/// @if LANG_EN
		/// <summary>Stop an action on this node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードのアクションを停止します。</summary>
		/// @endif
		public void StopAction( ActionBase action )
		{
			Common.Assert( action.Target == this || action.Target == null );
			ActionManager.Instance.RemoveAction( action );
		}

		/// @if LANG_EN
		/// <summary>Search for the first action acting on this node with tag value 'tag' and stop/remove it.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>tag の値で、このノード内で実行している最初のアクションを検索し、停止/削除します。</summary>
		/// @endif
		public void StopActionByTag( int tag )
		{
			ActionManager.Instance.RemoveActionByTag( tag, this );
		}

		/// @if LANG_EN
		/// <summary>Return the 'ith' action with tag 'tag' acting on this node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノード内で、実行している 'tag' の 'ith' アクションを返します。</summary>
		/// @endif
		public ActionBase GetActionByTag( int tag, int ith = 0 )
		{
			return ActionManager.Instance.GetActionByTag( tag, this, ith );
		}

		/// @if LANG_EN
		/// <summary>Get the number of action acting on this node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノード内で、実行しているアクションの数を取得します。
		/// </summary>
		/// @endif
		public int NumRunningActions()
		{
			return ActionManager.Instance.NumRunningActions( this );
		}

		/// @if LANG_EN
		/// <summary>Recursively stop all actions and scheduled functions on this node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノード内の、全てのアクションとスケジュールされた関数を再帰的に停止します。
		/// </summary>
		/// @endif
		public virtual void Cleanup()
		{
			StopAllActions();
			UnscheduleAll();

			foreach ( Node child in Children )
				child.Cleanup();
		}

		/// @if LANG_EN
		/// <summary>Register this node's update function to the scheduler, it will get called everyframe.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラにこのノードのUpdate関数を登録します。update関数は全てのフレームで呼び出されます。
		/// </summary>
		/// @endif
		public void ScheduleUpdate( int priority = Scheduler.DefaultPriority )
		{
			Scheduler.Instance.ScheduleUpdateForTarget( this, priority, !m_is_running );
		}

		/// @if LANG_EN
		/// <summary>Remove the update function from the scheduler.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケジューラからUpdate関数を削除します。
		/// </summary>
		/// @endif
		public void UnscheduleUpdate()
		{
			Scheduler.Instance.UnscheduleUpdateForTarget( this );
		}

		/// @if LANG_EN
		/// <summary>Schedule node function 'func', it will get called everyframe.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数のノード関数 'func' をスケジュールします。関数は全てのフレームで呼び出されます。
		/// </summary>
		/// @endif
		public void Schedule( DSchedulerFunc func, int priority = Scheduler.DefaultPriority )
		{
			Scheduler.Instance.Schedule( this, func, 0.0f, !m_is_running, priority );
		}

		/// @if LANG_EN
		/// <summary>Schedule node function 'func' so it gets called every 'interval' seconds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数のノード関数 'func' を、引数 'interval'の間隔で呼び出されるようにスケジュールします。
		/// </summary>
		/// @endif
		public void ScheduleInterval( DSchedulerFunc func, float interval, int priority = Scheduler.DefaultPriority )
		{
			Scheduler.Instance.Schedule( this, func, interval, !m_is_running, priority );
		}

		/// @if LANG_EN
		/// <summary>Unschedule node function 'func'.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数のノード関数 'func'をアンスケジュールします。
		/// </summary>
		/// @endif
		public void Unschedule( DSchedulerFunc func )
		{
			Scheduler.Instance.Unschedule( this, func );
		}

		/// @if LANG_EN
		/// <summary>Unschedule all functions related to this node.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードに関連する全ての関数をアンスケジュールします。
		/// </summary>
		/// @endif
		public void UnscheduleAll()
		{
			if ( Scheduler.Instance != null ) {
				Scheduler.Instance.UnscheduleAll( this );
			}
		}

		/// @if LANG_EN
		/// <summary>All actions related to this node can be paused on and off with this flag.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードに関連する全てのアクションを、このフラグのon/offで一時停止させることができます。
		/// </summary>
		/// @endif
		public bool ActionsPaused
		{
			set {
				if ( value ) m_scheduler_and_action_manager_pause_flag |= 1; // pause off
				else m_scheduler_and_action_manager_pause_flag &= unchecked((byte)~1); // pause on
			}

			get { return( m_scheduler_and_action_manager_pause_flag & 1 ) != 0;}
		}

		/// @if LANG_EN
		/// <summary>All scheduled functions related to this node can be paused on and off with this flag.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードに関連するスケジュールされた全ての関数を、このフラグのon/offで一時停止させることができます。
		/// </summary>
		/// @endif
		public bool SchedulerPaused
		{
			set {
				if ( value ) m_scheduler_and_action_manager_pause_flag |= 2; // pause off
				else m_scheduler_and_action_manager_pause_flag &= unchecked((byte)~2); // pause on
			}

			get { return( m_scheduler_and_action_manager_pause_flag & 2 ) != 0;}
		}

		/// @if LANG_EN
		/// <summary>Sets SchedulerPaused and ActionsPaused to true.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>SchedulerPaused と ActionsPaused に true をセットします。
		/// </summary>
		/// @endif
		public void ResumeSchedulerAndActions()
		{
			SchedulerPaused = false; // Scheduler.Instance.ResumeTarget( this );
			ActionsPaused = false; // ActionManager.Instance.ResumeTarget( this);
		}

		/// @if LANG_EN
		/// <summary>Sets SchedulerPaused and ActionsPaused to false.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>SchedulerPaused と ActionsPaused に false をセットします、。
		/// </summary>
		/// @endif
		public void PauseSchedulerAndActions()
		{
			SchedulerPaused = true;	// Scheduler.Instance.PauseTarget( this );
			ActionsPaused = true; // ActionManager.Instance.PauseTarget( this);
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the transform matrix of this node, expressed in its parent space.
		/// 
		/// The node transform matrix is formed using the data accessed with the Position, Scale, Skew,
		/// Rotation/Angle/RotationNormalize, Pivot properties. The transform matrix is equivalent to:
		/// 
		/// 	  Matrix3.Translation( Position )
		/// 	* Matrix3.Translation( Pivot )
		/// 	* Matrix3.Rotation( Rotation )
		/// 	* Matrix3.Scale( Scale )
		/// 	* Matrix3.Skew( Skew )
		/// 	* Matrix3.Translation( -Pivot )
		/// 
		/// Node that the transform matrix returned is a pure 2D transform. 
		/// VertexZ is applied separately in the PushTransform function.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>親空間で表現される、このノードの変換行列を返します。
		/// 
		/// ノードの変換行列は、位置、スケール、歪み、回転/角度/ RotationNormalize、ピボットプロパティでアクセスされるデータを用いて形成されます。変換行列は次のものと等価です:
		/// 
		/// 	  Matrix3.Translation( Position )
		/// 	* Matrix3.Translation( Pivot )
		/// 	* Matrix3.Rotation( Rotation )
		/// 	* Matrix3.Scale( Scale )
		/// 	* Matrix3.Skew( Skew )
		/// 	* Matrix3.Translation( -Pivot )
		/// 
		/// 変換行列で返されるのは、純粋な2D変換であることに注意してください。
		/// VertexZ は PushTransform 関数で別々に適用されます。
		/// </summary>
		/// @endif
		public Matrix3 GetTransform()
		{
			if ( m_cached_local_transform_info_is_dirty )
			{
				// Note that the Pivot is in LOCAL space

//				m_cached_local_transform = Matrix3.Translation( Position ) 
//										   * Matrix3.Translation( Pivot ) 
//										   * Matrix3.Rotation( Rotation ) 
//										   * Matrix3.Scale( Scale )
//										   * Matrix3.Skew( m_skew_tan )
//										   * Matrix3.Translation( -Pivot )
//										   ;

				// this should be the exact same as above
				Math.TranslationRotationScale( ref m_cached_local_transform, Position + Pivot, Rotation, Scale );

//				m_cached_local_transform = m_cached_local_transform
//										   * Matrix3.Skew( m_skew_tan )
//										   * Matrix3.Translation( -Pivot );

				m_cached_local_transform = m_cached_local_transform 
										   // form the Matrix3.Skew( m_skew_tan ) * Matrix3.Translation( -Pivot ) matrix directly:
										   * new Matrix3( new Vector3( 1.0f, m_skew_tan.X, 0.0f ) ,
														  new Vector3( m_skew_tan.Y, 1.0f, 0.0f ) ,
														  new Vector3( -Pivot * ( Math._11 + m_skew_tan.Yx ), 1.0f ) );

				m_cached_local_transform_info_is_identity = false; // we don't know, so false
				m_cached_local_transform_info_is_orthonormal = ( Scale == Math._11 && Skew == GameEngine2D.Base.Math._00 );
				m_cached_local_transform_info_is_dirty = false;
			}
			return m_cached_local_transform;
		}

		/// @if LANG_EN
		/// <summary>
		/// Get the inverse of this node 's transform matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードの変換行列の逆行列を取得します。
		/// </summary>
		/// @endif
		public Matrix3 GetTransformInverse()
		{
			if ( m_cached_local_transform_info_is_orthonormal )
				return GetTransform().InverseOrthonormal();
			return GetTransform().Inverse();
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the transform matrix of this node, expressed in its world/parent Scene space.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ワールド/親 シーン空間で表される、このノードの変換行列を返します。
		/// </summary>
		/// @endif
		public Matrix3 GetWorldTransform()
		{
			Matrix3 ret = GetTransform();

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
				ret = parent.GetTransform() * ret;

			return ret;
		}

		/// @if LANG_EN
		/// <summary>
		/// Get the inverse of this node's world transform matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このノードのワールド変換行列の逆行列を取得します。
		/// </summary>
		/// @endif
		public Matrix3 CalcWorldTransformInverse()
		{
			Matrix3 ret = GetTransformInverse();

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
				ret = ret * parent.GetTransformInverse();

			return ret;
		}

		/// @if LANG_EN
		/// <summary>
		/// LocalToWorld Should return the same as ( GetWorldTransform() * local_point.Xy1 ).Xy.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>LocalToWorld は ( GetWorldTransform() * local_point.Xy1 ).Xy と同じものを返します。
		/// </summary>
		/// @endif
		public Vector2 LocalToWorld( Vector2 local_point )
		{
#if false
			Vector3 p = GetTransform() * local_point.Xy1;

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
			{
				if ( parent.m_cached_local_transform_info_is_identity == false )
					p = parent.GetTransform() * p;
			}

			return p.Xy;
#else
			// Same as above.
			// Because it is written for the explanation that I do it this way.
			return ( GetWorldTransform() * local_point.Xy1 ).Xy;
#endif
		}

		/// @if LANG_EN
		/// <summary>
		/// Should return the same as ( CalcWorldTransformInverse() * world_point.Xy1 ).Xy.
		/// The local space of the node is the space in which its geometry is defined, 
		/// i.e one level below GetWorldTransform().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>( CalcWorldTransformInverse() * world_point.Xy1 ).Xy と同じものを返します。
		/// ノードのローカル空間はその幾何情報を定義した空間です。
		/// すなわち、1レベル下の GetWorldTransform() 。
		/// </summary>
		/// @endif
		public Vector2 WorldToLocal( Vector2 world_point )
		{
#if false
			Matrix3 m = GetTransformInverse();

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
			{
				if ( parent.m_cached_local_transform_info_is_identity == false )
					m = m * parent.GetTransformInverse();
			}

			return( m * world_point.Xy1 ).Xy;
#else
			// Same as above.
			// Because it is written for the explanation that I do it this way.
			return ( CalcWorldTransformInverse() * world_point.Xy1 ).Xy;
#endif
		}

		/// @if LANG_EN
		/// <summary>
		/// Get the bounds for the content/geometry of this node (only), in node space (no recursion).
		/// Nodes that don't have any content just return false.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ノード空間内(再帰なし)で、このノードのみのコンテント/幾何情報の境界を取得します。
		/// 任意のコンテントを持たないノードは単に false を返します。
		/// </summary>
		/// @endif
		public virtual bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			return false;
		}

		/// @if LANG_EN
		/// <summary>
		/// Get the bounds for the content of this node (only), in world space (no recursion).
		/// Nodes that don't have any content just return false and don't touch bounds.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ワールド空間(再帰なし)で、このノードのみのコンテントの境界を取得します。
		/// 任意のコンテントを持たないノードは単に false を返します。その場合、境界は扱えません。
		/// </summary>
		/// @endif
		public virtual bool GetContentWorldBounds( ref Bounds2 bounds )
		{
			Bounds2 lbounds = new Bounds2();

			if ( !GetlContentLocalBounds( ref lbounds ) )
				return false; // this node had no content

			Matrix3 m = GetWorldTransform();

			bounds = new Bounds2( ( m * lbounds.Point00.Xy1 ).Xy );
			bounds.Add( ( m * lbounds.Point10.Xy1 ).Xy );
			bounds.Add( ( m * lbounds.Point01.Xy1 ).Xy );
			bounds.Add( ( m * lbounds.Point11.Xy1 ).Xy );

			return true;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return true if 'world_position' is inside the content oriented bounding box.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数'world_position'が、向きをもったコンテントの境界ボックス内にある場合、trueを返します。
		/// </summary>
		/// @endif
		public bool IsWorldPointInsideContentLocalBounds( Vector2 world_position )
		{
			Bounds2 bounds = new Bounds2();

			if ( !GetlContentLocalBounds( ref bounds ) )
				return false;

			return bounds.IsInside( WorldToLocal( world_position ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// Follow parent hierarchy until we find a Plane3D node,
		/// and set 'mat' to the Plane3D's plane matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Plane3D ノードが見つかるまで、親の階層構造に従い、
		/// Plane3D の平面行列に 'mat' をセットします。
		/// </summary>
		/// @endif
		public virtual void FindParentPlane( ref Matrix4 mat )
		{
			if ( Parent != null ) 
				Parent.FindParentPlane( ref mat );
		}

		/// @if LANG_EN
		/// <summary>
		/// Like Director.Instance.CurrentScene.Camera.NormalizedToWorld, but deals with
		/// the case when there is a Plane3D among ancestors in the scenegraph.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Director.Instance.CurrentScene.Camera.NormalizedToWorld に似ていますが、シーングラフ内の祖先に Plane3D が存在したときのケースを扱います。
		/// </summary>
		/// @endif
		Vector2 NormalizedToWorld( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos )
		{
			if ( Camera != null )  return Camera.NormalizedToWorld( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );
			Matrix4 plane_mat = Matrix4.Identity;
			FindParentPlane( ref plane_mat );
			Director.Instance.CurrentScene.Camera.SetTouchPlaneMatrix( plane_mat );
			return Director.Instance.CurrentScene.Camera.NormalizedToWorld( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );
		}

		/// @if LANG_EN
		/// <summary>
		/// Like Director.Instance.CurrentScene.Camera.GetTouchPos, but deals with
		/// the case when there is a Plane3D among ancestors in the scenegraph.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Like Director.Instance.CurrentScene.Camera.GetTouchPos に似ていますが、シーングラフ内の祖先に Plane3D が存在したときのケースを扱います。
		/// </summary>
		/// @endif
		public Vector2 GetTouchPos( int nth = 0, bool prev = false )
		{
			if ( Camera != null )  return Camera.GetTouchPos( nth, prev );
			Matrix4 plane_mat = Matrix4.Identity;
			FindParentPlane( ref plane_mat );
			Director.Instance.CurrentScene.Camera.SetTouchPlaneMatrix( plane_mat );
			return Director.Instance.CurrentScene.Camera.GetTouchPos( nth, prev );
		}

		public virtual string DebugInfo()
		{
			return "{" + GetType().Name + ":" + Name + "}";
//			return "{" + GetType().Name + ":" + Name + "} " + GetHashCode();
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// The Plane3D node allows 3d orientations in the scenegraph, to some extent.
	/// That somewhat complicates the way we deal with touch coordinates, so if you have a
	/// Plane3D in your scene hierarchy, make sure you use Node.GetTouchPos() to get points
	/// in the Plane3D space (were all subnodes are), instead of using directly
	/// Director.Instance.CurrentScene.Camera.GetTouchPos().
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Plane3D ノードは、シーングラフ内で、ある程度まで3Dの方向性を許可します。
	/// それは、タッチ座標を扱う方法をいくぶんか複雑にします。なのでシーン階層に Plane3D を含んでいる場合、直接 Director.Instance.CurrentScene.Camera.GetTouchPos() を使う代わりに、Plane3D 空間(すべてサブノード)で点を得るのに Node.GetTouchPos() を使うことを確認してください。
	/// </summary>
	/// @endif
	public class Plane3D
	: Node
	{
		/// @if LANG_EN
		/// <summary>
		/// The "transform matrix" for this plane. A plane is defined by a base point and a normal, but since we also use 
		/// it as a coordinate system (a matrix that we can push on the stack), we store it as a matrix.
		/// 
		/// The 3D plane is defined as the 'identity' plane z=0 transformed by ModelMatrix, which means that ModelMatrix.ColumnZ is 
		/// the normal vector of the plane, and ModelMatrix.ColumnW is any point on the plane. 
		/// 
		/// It is assumed that ModelMatrix is set to a right handled, orthonormal coordinate system (no check is performed).
		/// 
		/// That means that ModelMatrix.ColumnX, ModelMatrix.ColumnY, ModelMatrix.ColumnZ are all perpendicular with each other, 
		/// each of unit length, and the cross product of ModelMatrix.ColumnX and ModelMatrix.ColumnY is in the same direction as
		/// ModelMatrix.ColumnZ. The default value for ModelMatrix is Matrix4.Identity (z=0 plane).
		/// This matrix is used as it is in the transform stack.
		/// <summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>この平面のための"変換行列"です。
		/// 平面は基点と法線で定義されます。しかし我々は平面を座標系(スタックにプッシュできる行列)としても使用するため、平面を行列として格納します。
		/// 
		/// 3D平面は、 ModelMatrix によって変換される z=0の '単位行列' 平面として定義されます。それは、ModelMatrix.ColumnZ が 平面の法線ベクトルであり、ModelMatrix.ColumnWは平面の任意の点として表されます。
		/// 
		/// ModelMatrix は右手座標系、直交座標系(チェックはありません)に設定されていることを前提にしています。
		/// 
		/// ModelMatrix.ColumnX、ModelMatrix.ColumnY、ModelMatrix.ColumnZは、単位長さの各々は、お互いにすべて垂直であることを意味します。
		/// ModelMatrix.ColumnXとModelMatrix.ColumnYの外積はModelMatrix.ColumnZと同じ方向です。
		/// ModelMatrix のデフォルト値は Matrix4.Identity (z=0 平面) です。
		/// この行列は変換スタックにあるかのように使用されます。
		/// </summary>
		/// @endif
		public Matrix4 ModelMatrix = Matrix4.Identity;

		/// @if LANG_EN
		/// <summary>
		/// Plane3D constructor.
		/// Defaults to z=0 plane (identity).
		/// Please refer to ModelMatrix's comment for more details.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。デフォルトはz=0 平面(単位行列)。
		/// 詳細は  ModelMatrix のコメントを参照してください。
		/// </summary>
		/// @endif
		public Plane3D()
		{
		}

		/// @if LANG_EN
		/// <summary>Plane3D constructor.</summary>
		/// <param name="modelmatrix">The value to set ModelMatrix to. 
		/// Please refer to ModelMatrix's comment for more details.
		/// </param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// <param name="modelmatrix">平面にセットする ModelMatrix の値。
		/// 詳細は ModelMatrix のコメントを参照してください。
		/// </param>
		/// @endif
		public Plane3D( Matrix4 modelmatrix )
		{
			ModelMatrix = modelmatrix;
		}

		public override void PushTransform()
		{
			Director.Instance.GL.ModelMatrix.Push();
			Director.Instance.GL.ModelMatrix.Mul1( ModelMatrix );
		}

		/// @if LANG_EN
		/// <summary>
		/// Note: FindParentPlane stops at the first encounterd Plane3D.
		/// We assume we can't have several Plane3D nodes along a tree branch.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>注意: FindParentPlane は最初に見つかった Plane3D で停止します。
		/// ツリーの枝で、複数の Plane3D ノードを持っていないという前提です。
		/// </summary>
		/// @endif
		public override void FindParentPlane( ref Matrix4 mat )
		{
			mat = ModelMatrix;
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

