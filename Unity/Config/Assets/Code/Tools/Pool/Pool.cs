using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// GameObject 需要自己封装
public class Pool<T> : Singleton<Pool<T>> where T : Object
{
    private Dictionary<string, Queue<Object>> dic = new Dictionary<string, Queue<Object>>();

    /// <summary>
    /// 不能有同名，而不是同一个东西的调用
    /// </summary>
    /// <param name="typename">唯一类型标识</param>
    /// <param name="original">当不存在的时候用来克隆的Ojbect</param>
    /// <returns></returns>
    public Object GetItem(string typename, Object original)
    {
        if(dic.ContainsKey(typename))
        {
            Queue<Object> que = dic[typename];
            if(que.Count > 0)
            {
                return que.Dequeue();
            }
            else
            {
                return CreateItem(original);
            }
        }
        else
        {
            dic[typename] = new Queue<Object>();
            return CreateItem(original);
        }
    }
    private Object CreateItem(Object original)
    {
        return GameObject.Instantiate(original);
    }

    public void Recycle(string typename, Object item)
    {
        if(!dic.ContainsKey(typename))
        {
            dic[typename] = new Queue<Object>();
        }
        dic[typename].Enqueue(item);
    }

}
