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
    public void DownloadAndSave(string url, string path, System.Action<bool> finish = null)
    {
        DownloadManager.Instance.download(url, delegate (byte[] contents)
        {
            if (contents == null)
            {
                Debug.LogError("Down null contents, " + url);
                if (finish != null) finish(false);
            }
            else {
                DecompressAndSave(path, contents);
                if (finish != null) finish(true);
            }
        });
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
        if (null != (entry = stream.GetNextEntry()))
        {
            byte[] buf = new byte[entry.Size];
            stream.Read(buf, offset, (int)entry.Size);
            offset += (int)entry.Size;
            
            FilesManager.Instance.WriteAllBytes(Path.ChangeExtension(path, Path.GetExtension(entry.Name)), buf);
        }
    }
}
