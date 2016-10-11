
using System.Collections.Generic;

// GameObject 需要自己封装
public class Pool<T> : Singleton<Pool<T>> where T : class, new()
{
    private Queue<object> _que = new Queue<object>();

    /// <summary>
    /// 不能有同名，而不是同一个东西的调用
    /// </summary>
    /// <param name="typename">唯一类型标识</param>
    /// <param name="original">当不存在的时候用来克隆的Ojbect</param>
    /// <returns></returns>
    public object GetItem()
    {
        if(_que.Count > 0)
        {
            return _que.Dequeue();
        }
        else
        {
            return CreateItem();
        }
    }
    private object CreateItem()
    {
        return new T();
    }

    public void Recycle(object item)
    {
        _que.Enqueue(item);
    }

}
