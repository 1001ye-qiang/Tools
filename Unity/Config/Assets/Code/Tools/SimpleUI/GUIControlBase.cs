using UnityEngine;
using System.Collections;

public abstract class GUIControlBase : MonoBehaviour {
    
    /// <summary>
    /// 注册UI需要的监听事件;
    /// 调用初始化指定子UI;
    /// </summary>
    public abstract void InitUI(LitJson.JsonData param = null, int vTabIndex = 0, int hTabIndex = 0);


    /// <summary>
    /// 更新数据
    /// 服务端收到请求以后，返回的时候可以进入该方法内，
    /// 来刷新UI的信息
    /// 参数可以随意增减
    /// </summary>
    public abstract void UpdateData(LitJson.JsonData param = null);
}
