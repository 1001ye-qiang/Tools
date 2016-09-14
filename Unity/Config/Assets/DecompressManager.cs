using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

public class DecompressManager : Singleton<DecompressManager> {

    /// <summary>
    /// Download a zip file and save those files to the path after decompress it;
    /// </summary>
    /// <param name="url"></param>
    /// <param name="path"></param>
    public void DownloadAndSave(string url, string path, System.Action finish = null)
    {
        DownloadManager.Instance.download(url, delegate (byte[] contents)
        {
            DecompressAndSave(path, contents);
            if(finish != null) finish();
        });
    }

    public void DownloadAndSave(System.Collections.Generic.List<string> urls, string path, System.Action<float> Process)
    {
        float fProcess = 0f;
        for(int i = 0; i < urls.Count; ++i)
        {
            DownloadAndSave(urls[i], path, delegate() {
                Process(fProcess += 1 / urls.Count);
            });
        }
    }


    /// <summary>
    /// Decompresses the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    public void DecompressAndSave(string path, byte[] data)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream(data);
        ZipInputStream stream = new ZipInputStream(ms);

        int offset = 0;
        ZipEntry entry;
        while (null != (entry = stream.GetNextEntry()))
        {
            byte[] buf = new byte[entry.Size];
            stream.Read(buf, offset, (int)entry.Size);
            offset += (int)entry.Size;            
            
            string name = System.IO.Path.GetFileName(entry.Name);
            FilesManager.Instance.WriteAllBytes(path + "/" + name, buf);
        }
    }
}
