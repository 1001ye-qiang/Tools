using UnityEngine;
using System.Collections.Generic;

public abstract class GUIControl : GUIControlBase
{
    public List<GUIControl> lstChildren;

    /// <summary>
    /// 关闭UI，清除注册的事件
    /// </summary>
    public virtual void Close()
    {
        //GUIManager.Instance.CloseRequestUI(gameObject);
    }

}
