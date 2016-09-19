using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Test : MonoBehaviour {

    public float fProcess = 0f;

    public string url = "http://192.168.0.246:8080/zq/ConfigCompress/";
    // end with "/"
    private string Url { get { if (!url.EndsWith("/")) url += "/"; return url; } }

    private string strDstPath = "";
    // end with "/"
    private string StrDstPath { get { if (string.IsNullOrEmpty(strDstPath)) strDstPath = Application.persistentDataPath + "/"; return strDstPath; } }

    private string strConfigName = "ConfigList.txt";
    private string StrConfigURL { get { return Url + strConfigName; } }
    private string StrConfigPath { get { return StrDstPath + strConfigName; } }

    long lBytes = 0;
    int iDownTotalNum = 0;
    SortedList<string, int> lstDownList = new SortedList<string, int>();
    SortedList<string, string> lstLocal = new SortedList<string, string>();

    void OnEnable()
    {
        lstDownList = new SortedList<string, int>();
        lstLocal = new SortedList<string, string>();

        DownloadListFile(
            delegate(string name) {
                // TODO:
                lBytes -= lstDownList[name];
                lstDownList.Remove(name);
                int hasDown = iDownTotalNum - lstDownList.Count;
                fProcess = hasDown / iDownTotalNum;
                Debug.Log(hasDown + "/" + iDownTotalNum + " " + lBytes);
            }, 
            delegate (bool bFinish) {
                if (bFinish)
                {
                    // TODO: Update UI
                }
            });
    }

    void DownloadListFile(System.Action<string> finishOne, System.Action<bool> finish)
    {
        lstLocal.Clear();
        lstDownList.Clear();

        // [JSON]config files
        DownloadManager.Instance.download(StrConfigURL, delegate (string contents) {
            if (string.IsNullOrEmpty(contents))
            {
                Debug.LogError("Download List File Error! \nurl:" + StrConfigURL);
                return;
            }

            // 目前没有做无用表删除；更新到一半停了，重启需要再重新下载
            LitJson.JsonData jd = LitJson.JsonMapper.ToObject(contents);
            Download(jd, finishOne, finish);
        });        

        if (File.Exists(StrConfigPath))
        {
            LitJson.JsonData jdLocal = LitJson.JsonMapper.ToObject(File.ReadAllText(StrConfigPath));
            for (int i = 1; i <= jdLocal.Count; ++i)
            {
                string key = i.ToString();
                lstLocal.Add((string)jdLocal[key][0], (string)jdLocal[key][1]);
            }
        }
    }

    void Download(LitJson.JsonData jd, System.Action<string> finishOne, System.Action<bool> finish)
    {
        Debug.LogError(StrDstPath);

        for(int i = 1; i <= jd.Count; ++i)
        {
            string index = i.ToString();
            string name = (string)jd[index][0];
            string md5 = (string)jd[index][1];
            int length = (int)jd[index][2];
            if (lstLocal.ContainsKey(name) && lstLocal[name].Equals(md5))
                continue;

            lBytes += length;
            ++iDownTotalNum;
            lstDownList.Add(name, length);

            // 下载压缩文件
            DecompressManager.Instance.DownloadAndSave(Url + name, StrDstPath + name, delegate (bool bSuccess) {
                if (bSuccess)
                {
                    if (finishOne != null) finishOne(name);

                    if (!bFinish && lstDownList.Count == 0 && i > jd.Count)
                    {
                        Debug.LogError("finish0");
                        finish(true);
                        bFinish = true;
                    }
                }
                else // download failed, then put into background
                {
                    StartCoroutine(DownloadOne(name, finishOne, finish));
                }
            });
        }

        // 没有需要更新的文件或者，更新的文件同步下载完成
        if(!bFinish && lstDownList.Count == 0)
        {
            Debug.LogError("finish1");
            finish(true);
            bFinish = true;
        }
    }

    bool bFinish = false;

    IEnumerator DownloadOne(string name, System.Action<string> finishOne, System.Action<bool> finish)
    {
        yield return new WaitForSeconds(0.22f);
        DecompressManager.Instance.DownloadAndSave(Url + name, StrDstPath + name, delegate (bool bSuccess) {
            if (bSuccess)
            {
                if (finishOne != null) finishOne(name);

                if (!bFinish && lstDownList.Count == 0)
                {
                    Debug.LogError("finish0");
                    finish(true);
                    bFinish = true;
                }
            }
            else
            {
                StartCoroutine(DownloadOne(name, finishOne, finish));
            }
        });
    }

}
