using System.Collections.Generic;

public delegate void UIEventFun(object param);

public class UIEventManager : Singleton<UIEventManager> {
    Dictionary<string, List<UIEventFun>> dic = new Dictionary<string, List<UIEventFun>>();

    public void Register(string key, UIEventFun fun)
    {
        if(dic.ContainsKey(key))
        {
            dic[key].Add(fun);
        }
        else
        {
            List<UIEventFun> lstFun = new List<UIEventFun>();
            lstFun.Add(fun);
            dic[key] = lstFun;
        }
    }

    public void UnRegister(string key, UIEventFun fun)
    {
        if(dic.ContainsKey(key))
        {
            dic[key].Remove(fun);
        }
    }

    public void Dispatch(string key, object param)
    {
        if(dic.ContainsKey(key))
        {
            for(int i= 0; i < dic[key].Count; ++i)
            {
                if (dic[key][i] != null)
                    dic[key][i](param);
            }
        }
    }
}
