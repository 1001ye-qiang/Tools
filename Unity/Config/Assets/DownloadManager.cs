using UnityEngine;
using System.Collections;

public class DownloadManager : SingletonMono<DownloadManager> {

    // download and get string.
    public void download(string url, System.Action<string> finish)
    {
        //System.Action<string> onfinish = delegate (string _data) { data = _data; };
        StartCoroutine(download(url, null, finish));
    }

    // download and get byte[]
    public void download(string url, System.Action<byte[]> finish)
    {
        StartCoroutine(download(url, finish, null));
    }

    // download and save to path
    public void download(string url, string path, System.Action<bool> finish)
    {
        download(url, delegate (byte[] res) {
            if (res == null) finish(false);
            else
            {
                FilesManager.Instance.WriteAllBytes(path, res);
                finish(true);
            }
        });
    }

    // download files and save to path
    public void download(System.Collections.Generic.List<string> urls, string path, System.Action<bool> finish)
    {
        int iCount = 0;
        for(int i = 0; i < urls.Count; ++i)
        {
            download(urls[i], path, delegate (bool bOk) {
                ++iCount;
                if(iCount == urls.Count)
                {
                    finish(true);
                }
            });
        }
    }


    /// <summary>
    /// 必须至少写一个回调函数，不然数据不知道给谁
    /// </summary>
    /// <param name="url"></param>
    /// <param name="bytes_cb"></param>
    /// <param name="string_cb"></param>
    /// <returns></returns>
    private IEnumerator download(string url, System.Action<byte[]> bytes_cb, System.Action<string> string_cb)
    {
        if(string_cb == null && bytes_cb == null)
        {
            Debug.LogError("Not Allow All Callback Are Null.");
            yield break;
        }

        WWW www = new WWW(url);
        yield return www;

        if(string.IsNullOrEmpty(www.error) && www.isDone)
        {
            if(bytes_cb != null) bytes_cb(www.bytes);
            if(string_cb != null) string_cb(www.text);
        }
        else
        {
            Debug.LogError("Load File Error: " + url);
            if (bytes_cb != null) bytes_cb(null);
            if (string_cb != null) string_cb(null);
        }
    }
}
