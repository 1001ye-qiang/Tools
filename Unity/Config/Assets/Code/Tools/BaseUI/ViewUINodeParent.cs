using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ViewUINodeParent : ViewUINode
{
    public List<ViewUINode> lstChildren;

    /// <summary>
    /// 注册UI需要的监听事件;
    /// 调用初始化指定子UI;
    /// </summary>
    public override void InitUI(object param = null, int vTabIndex = 0, int hTabIndex = 0)
    {
        StartCoroutine(delayInit());
    }

    IEnumerator delayInit()
    {
        for(int i = 0; i < lstChildren.Count; ++i)
        {
            yield return new WaitForFixedUpdate();
            if(this != null)
                lstChildren[i].InitUI();
        }
    }
}
