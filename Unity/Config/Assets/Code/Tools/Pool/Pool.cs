
using System.Collections.Generic;

// GameObject 需要自己封装
public class Pool<T> : Singleton<Pool<T>> where T : class, new()
{
    private Queue<object> _que = new Queue<object>();

    public object Get()
    {
        if(_que.Count > 0)
        {
            return _que.Dequeue();
        }
        else
        {
            return Create();
        }
    }
    private object Create()
    {
        return new T();
    }

    public void Recycle(object item)
    {
        _que.Enqueue(item);
    }

}
