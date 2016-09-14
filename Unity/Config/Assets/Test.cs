using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {
    public float fProcess = 0f;

    public string url = "http://192.168.0.246:8080/zq/ConfigCompress/";
    void OnEnable()
    {
        DownloadManager.Instance.download(url + "ConfigList.txt", delegate (string contents) {
            contents = contents.Replace("\r\n", "\n");
            Download(contents.Split('\n'));
        });
    }

    void Download(string [] lines)
    {
        List<string> lst = new List<string>();
        for(int i = 0; i < lines.Length; ++i)
        {
            lst.Add(url + lines[i].Split('\t')[1]);
        }

        Debug.LogError(Application.temporaryCachePath);
        DecompressManager.Instance.DownloadAndSave(lst, Application.temporaryCachePath, delegate (float p) {
            fProcess = p;
        });
    }
}
