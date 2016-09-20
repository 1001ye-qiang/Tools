using UnityEngine;

public class BaseDefinition : SingletonMono<BaseDefinition> {

    public string url = "http://192.168.0.246:8080/zq/ConfigCompress/";
    public string strConfigName = "ConfigList.txt";
    
    private string strDstPath = "";
    // save config path, end with "/"
    public string StrDstPath { get { if (string.IsNullOrEmpty(strDstPath)) strDstPath = Application.persistentDataPath + "/"; return strDstPath; } }

    public string StrConfigURL { get { return url + strConfigName; } }
    public string StrConfigPath { get { return StrDstPath + strConfigName; } }

    void Awake()
    {
        // end with "/"
        url = url.Replace('\\', '/');
        if (!url.EndsWith("/")) url += "/";


    }
}
