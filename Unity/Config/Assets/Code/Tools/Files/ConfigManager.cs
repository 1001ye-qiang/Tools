using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager> {
    Dictionary<string, JsonData> dic;
    public ConfigManager()
    {
        dic = new Dictionary<string, JsonData>();
    }

    private bool CheckDic(string name)
    {
        if(!dic.ContainsKey(name))
        {
            string contents = FilesManager.Instance.ReadAllText(BaseDefinition.Instance.StrDstPath + name);
            if (string.IsNullOrEmpty(contents))
            {
                Debug.LogError("config file not found! " + name);
                return false;
            }
            try
            {
                dic.Add(name, JsonMapper.ToObject(contents));
            }
            catch (System.Exception e) {
                Debug.LogError("config file format not correct! " + name);
                return false;
            }
        }
        return true;
    }

    public void ClearDic()
    {
        dic.Clear();
    }

    public string GetData(string name, string index, string column_name)
    {
        if (!CheckDic(name)) return null;
        try
        {
            JsonData data = dic[name];
            return (string)data[index][(int)data["Title"][column_name]];
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("Not Found Key: " + name + "-" + index + "-" + column_name);
            return null;
        }
    }


    /// <summary>
    /// 如果想遍历表，最好表的id是连续的，起始是默认的或者已知的。
    /// 不把表的内容直接暴露，防止程序员把内容缓存住，这样可以在不重启App的情况下更新配置表。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetDataLength(string name)
    {
        if (!CheckDic(name)) return 0;
        try
        {
            JsonData data = dic[name];
            return data.Count - 1; // 表头占一行
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("Not Found Key: " + name);
            return 0;
        }
    }
}
