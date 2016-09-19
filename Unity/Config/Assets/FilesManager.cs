using UnityEngine;
using System.Collections;
using System.IO;

public class FilesManager:Singleton<FilesManager> {

    public string ReadAllText(string path)
    {
        if (!File.Exists(path)) Debug.LogError("Not Exists File: " + path);
        return File.ReadAllText(path);
    }
    public byte[] ReadAllBytes(string path)
    {
        if (!File.Exists(path)) Debug.LogError("Not Exists File: " + path);
        return File.ReadAllBytes(path);
    }
    public void WriteAllBytes(string path, byte[] contents)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!Directory.Exists(dir))
            Debug.LogError("Not Exists Directory: " + dir);
        File.WriteAllBytes(path, contents);
    }
    public void WriteAllText(string path, string contents)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!Directory.Exists(dir)) Debug.LogError("Not Exists Directory: " + dir);
        File.WriteAllText(path, contents);
    }

    public void AppendAllText(string path, string contents)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!Directory.Exists(dir)) Debug.LogError("Not Exists Directory: " + dir);
        File.AppendAllText(path, contents);
    }

    public void AppendAllBytes(string path, byte[] contents)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!Directory.Exists(dir)) Debug.LogError("Not Exists Directory: " + dir);

        FileStream fs = File.Open(path, FileMode.Append);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(contents);
        bw.Close();
        fs.Close();
    }

}
