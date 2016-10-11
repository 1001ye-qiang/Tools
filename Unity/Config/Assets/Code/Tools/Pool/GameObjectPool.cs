using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool : SingletonMono<GameObjectPool>
{

    private Dictionary<string, Queue<GameObject>> dic = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// 不能有同名，而不是同一个东西的调用
    /// </summary>
    /// <param name="typename">唯一类型标识</param>
    /// <param name="original">当不存在的时候用来克隆的Ojbect</param>
    /// <returns></returns>
    public GameObject GetItem(string typename, GameObject original)
    {
        if (dic.ContainsKey(typename))
        {
            Queue<GameObject> que = dic[typename];
            if (que.Count > 0)
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
            dic[typename] = new Queue<GameObject>();
            return CreateItem(original);
        }
    }

    public GameObject GetItemDynamic(string typename, GameObject original, int count = 200)
    {
        if(!dic.ContainsKey(typename) || dic[typename].Count == 0)
        {
            GameObject obj = null;
            Transform trans = transform;
            for(int i = 0; i < count; ++i)
            {
                obj = CreateItem(original);
                obj.transform.parent = trans;
                Recycle(typename, obj);
            }
        }

        return dic[typename].Dequeue();
    }



    private GameObject CreateItem(Object original)
    {
        return GameObject.Instantiate(original) as GameObject;
    }

    public void Recycle(string typename, GameObject item)
    {
        if (!dic.ContainsKey(typename))
        {
            dic[typename] = new Queue<GameObject>();
        }
        dic[typename].Enqueue(item);
        item.transform.parent = transform;
    }
}
