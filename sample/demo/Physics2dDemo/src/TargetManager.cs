/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

namespace Physics2dDemo
{
	
public class TargetManager : IDisposable
{
    private List<Target> updateList = new List<Target>();
    private List<Target> renderList = new List<Target>();
    private int updateIndex = 0;

    /// コンストラクタ
    public TargetManager()
    {
    }

    /// 破棄
    public void Dispose()
    {
        updateList.ForEach(target => {
            target.End(this);
            target.Dispose();
        });

        updateList.Clear();
        renderList.Clear();

        updateIndex = 0;
    }

    /// 登録
    public bool Regist(Target target, string parentName = null)
    {
        return Regist(target, FindTarget(parentName));
    }

    /// 登録
    public bool Regist(Target target, Target parent)
    {
        //同一名が既にいる場合は登録不可
        if (FindTarget(target.Name) != null) {
            return false;
        }

        // 挿入位置の決定
        if (parent != null) {
            // 親の後ろに挿入
            updateList.Insert(updateList.IndexOf(parent) + 1, target);
            renderList.Insert(renderList.IndexOf(parent) + 1, target);
        } else {
            updateList.Add(target);
            renderList.Add(target);
        }

        // 現在より前に挿入する場合は1つ進める
        if (updateList.IndexOf(target) <= updateIndex) {
            updateIndex++;
        }

        // 親をセット
        target.SetParent(parent);

        // 開始
        target.Start(this);

        return true;
    }

    // リストから削除
    public void Remove(Target target)
    {
        if (updateList.IndexOf(target) > 0) {
            // 現在より前のタスクを削除する場合は1つ戻す
            if (updateList.IndexOf(target) <= updateIndex) {
                updateIndex--;
            }

            updateList.Remove(target);
            renderList.Remove(target);
            target.End(this);
            target.Dispose();
        }
    }

    /// 更新
    public bool Update()
    {
        for (updateIndex = 0; updateIndex < updateList.Count; updateIndex++) {
            updateList[updateIndex].Update(this);
        }

        return true;
    }

    /// 描画
    public bool Render()
    {
        renderList.Sort((x, y) => {
                if (x.Priority > y.Priority) {
                    return -1;
                } else if (x.Priority < y.Priority) {
                    return 1;
                } else {
                    return 0;
                }
            });

        renderList.ForEach(target => target.Render(this));
			
        return true;
    }
	
	/// ターゲットをスクロール
	public void Scroll(int x)	
	{
		updateList.ForEach(target => target.Move( target.baseX + x, target.baseY));
	}

	/// 指定のターゲットを移動
	public void MoveTarget(string name, int x, int y)	
	{
		FindTarget(name).Move( x, y);
	}

    /// 検索
    public Target FindTarget(string name)
    {
        return updateList.Find(target => (name != null && target.Name == name));
    }

    /// 登録確認
    public bool IsRegist(Target target)
    {
        return updateList.Contains(target);
    }

    /// 更新用のリスト
    public List<Target> UpdateList
    {
        get {return updateList;}
    }
}
	
} // Physics2dDemo

