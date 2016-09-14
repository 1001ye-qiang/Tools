using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System;
using System.Data;
using LitJson;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;


// 警告：所有配置表最好不要在使用时自己保存拷贝，否则必须重启app才能达到更新的效果
public class ExportConfigEditor : EditorWindow
{
    enum ExportType
    {
        json,
        text,
    }

    readonly string _dstTypeName = "ConfigTemp";
    string _dstTypePath = "ConfigTemp";
    readonly string _dstCompressName = "ConfigCompress";
    string _dstCompressPath = "ConfigCompress";

    string _configFullPath;

    string _configPath;
    ExportType _expType;

    bool _checkRegex = true;
    int _regexLineNum = 4;

    int _compressLevel = 5;
    string _ConfigLstFile = "ConfigList.txt";


    private readonly int[] IntArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private readonly string[] CompressLevel = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    private readonly string[] RegexLineNum = new string[] { "1", "2", "3", "4", "5" };


    [MenuItem("Tools/Export Config Editor")]
    static void OpenViewEditor()
    {
        EditorWindow window = EditorWindow.GetWindow(new StackTrace().GetFrame(0).GetMethod().ReflectedType);
        window.Show();
    }

    void Awake()
    {
        string path = PlayerPrefs.GetString("_configPath");
        if (!string.IsNullOrEmpty(path))
            _configPath = path;
    }

    void OnGUI()
    {
        _configPath = (string)EditorGUILayout.TextField("Config Path: ", _configPath);
        if (GUILayout.Button("SavePath"))
            PlayerPrefs.SetString("_configPath", _configPath);

        _expType = (ExportType)EditorGUILayout.EnumPopup("Export Type: ", _expType);

        _checkRegex = GUILayout.Toggle(_checkRegex, "Check Format By Regex?");
        _regexLineNum = EditorGUILayout.IntPopup("Regex Line Num: ", _regexLineNum, RegexLineNum, IntArray);


        if (GUILayout.Button("Switch Type And Check Field Format"))
            ParseExcel();

        _compressLevel = EditorGUILayout.IntPopup("Compress Level: ", _compressLevel, CompressLevel, IntArray);
        _ConfigLstFile = (string)EditorGUILayout.TextField("Output List File Name: ", _ConfigLstFile);
        
        if (GUILayout.Button("Compress Files"))
            Compress();
    }

    #region parse excel
    void ParseExcel(bool bCheck = true)
    {
        PrepareTypePath();

        IteratorDirAndParseExcel(_configPath);
    }

    #region prepare path
    void PrepareTypePath()
    {
        string parent = GetParentPath();

        _dstTypePath = parent + "/" + _dstTypeName;
        EmptyDir(_dstTypePath);        
    }

    void PrepareCompressPath()
    {
        string parent = GetParentPath();

        _dstTypePath = parent + "/" + _dstTypeName;
        _dstCompressPath = parent + "/" + _dstCompressName;
        EmptyDir(_dstCompressPath);
    }

    string GetParentPath()
    {
        if (!Directory.Exists(_configPath)) throw new Exception("Not Found DIR: " + _configPath);

        DirectoryInfo di = new DirectoryInfo(_configPath);
        _configFullPath = di.FullName.Replace('\\', '/');

        string parent = di.Parent.FullName.Replace('\\', '/');
        return parent;
    }

    void EmptyDir(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        if (di.Exists)
        {
            FileSystemInfo[] fsi = di.GetFileSystemInfos();
            for (int i = 0; i < fsi.Length; ++i)
            {
                FileInfo fi = fsi[i] as FileInfo;
                if (fi != null)
                {
                    fi.Delete();
                }
                else
                {
                    (fsi[i] as DirectoryInfo).Delete(true);
                }
            }
        }
    }
    #endregion

    #region iterator source dir and output tmp source files
    void IteratorDirAndParseExcel(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        if (!di.Exists) return;
        
        FileSystemInfo[] files = di.GetFileSystemInfos();
        for (int i = 0; i < files.Length; ++i)
        {
            FileInfo file = files[i] as FileInfo;
            if (file != null)
            {
                if (file.Extension == ".meta")
                    continue;
                try
                {
                    System.Data.DataTable table = ExcelManager.Instance.ReadExcel(file.FullName, null);

                    if (_checkRegex && !CheckRegexSuccess(table)) // 需要检查，并且检查不合格
                        continue;

                    string tmp_path = file.FullName.Substring(_configFullPath.Length).Replace('\\', '/');
                    switch (_expType)
                    {
                        case ExportType.text:
                            tmp_path = Path.ChangeExtension(tmp_path, "txt");
                            WriteToFile(ToText(table), _dstTypePath + tmp_path);
                            break;
                        case ExportType.json:
                            tmp_path = Path.ChangeExtension(tmp_path, "json");
                            WriteToFile(ToJson(table), _dstTypePath + tmp_path);
                            break;
                    }
                }
                catch (Exception e){
                    UnityEngine.Debug.LogError("Parse Excel Error: " + e.Message);
                }
            }
            else
            {
                IteratorDirAndParseExcel(files[i].FullName);
            }
        }
    }
    
    string ToText(DataTable table)
    {
        string contents = null;

        for (int i = 0; i < table.Rows.Count; ++i)
        {
            for (int j = 0; j < table.Columns.Count; ++j)
            {
                if (j == 0)
                    contents += table.Rows[i][j];
                else
                    contents += "\t" + table.Rows[i][j];
            }
            contents += "\n";
        }

        return contents;
    }

    string ToJson(DataTable table)
    {
        JsonData data = new JsonData();

        for (int i = 0; i < table.Rows.Count; ++i)
        {
            JsonData row = new JsonData();
            switch (i)
            {
                case 0: // name ch
                    break;
                case 1: // name en
                    for (int j = 0; j < table.Columns.Count; ++j)
                    {
                        row[table.Rows[i][j].ToString()] = j;
                    }
                    data["Title"] = row;
                    break;
                case 2: // descript
                    break;
                case 3: // regex
                    break;
                default:
                    for (int j = 0; j < table.Columns.Count; ++j)
                    {
                        row.Add(table.Rows[i][j].ToString());
                    }
                    data[table.Rows[i][0].ToString()] = row;
                    break;
            }
        }
        //UnityEngine.Debug.Log(GetData(data, "3", "attr"));
        
        return data.ToJson();
    }
    #endregion

    #endregion

    #region compress tmp source files
    void Compress()
    {
        PrepareCompressPath();

        IteratorDirAndCompress(_dstTypePath);
    }

    void IteratorDirAndCompress(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        if (!di.Exists) return;

        FileSystemInfo[] files = di.GetFileSystemInfos();
        for (int i = 0; i < files.Length; ++i)
        {
            FileInfo file = files[i] as FileInfo;
            if (file != null)
            {
                if (file.Extension == ".meta")
                    continue;
                try
                {
                    string tmp_path = file.FullName.Substring(_dstTypePath.Length).Replace('\\', '/');
                    tmp_path = Path.ChangeExtension(tmp_path, "zip");

                    List<string> lstFiles = new List<string>();
                    lstFiles.Add(file.FullName);
                    WriteZipFile(lstFiles, _dstCompressPath + tmp_path, _compressLevel);
                }
                catch (Exception e) {
                    UnityEngine.Debug.LogError("Compress Error: " + e.Message);
                }
            }
            else
            {
                IteratorDirAndCompress(files[i].FullName);
            }
        }
    }
    #endregion

    #region Zip Files and Create ListFile
    /// <summary>
    /// Writes the zip file.
    /// </summary>
    /// <param name="filesToZip">The files to zip.</param>
    /// <param name="path">The destination path.</param>
    /// <param name="compression">The compression level.</param>
    private void WriteZipFile(List<string> filesToZip, string path, int compression)
    {
        if (compression < 0 || compression > 9)
            throw new ArgumentException("Invalid compression rate.");

        MkDir(Path.GetDirectoryName(path));
        if (!Directory.Exists(new FileInfo(path).Directory.ToString()))
            throw new ArgumentException("The Path does not exist.");

        foreach (string c in filesToZip)
            if (!File.Exists(c))
                throw new ArgumentException(string.Format("The File{0}does not exist!", c));


        Crc32 crc32 = new Crc32();
        ZipOutputStream stream = new ZipOutputStream(File.Create(path));
        stream.SetLevel(compression);

        for (int i = 0; i < filesToZip.Count; i++)
        {
            ZipEntry entry = new ZipEntry(Path.GetFileName(filesToZip[i]));
            entry.DateTime = DateTime.Now;

            using (FileStream fs = File.OpenRead(filesToZip[i]))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                entry.Size = fs.Length;
                fs.Close();
                crc32.Reset();
                crc32.Update(buffer);
                entry.Crc = crc32.Value;
                stream.PutNextEntry(entry);
                stream.Write(buffer, 0, buffer.Length);

                ListFileAppend(path, buffer);
            }
        }
        stream.Finish();
        stream.Close();
    }

    int indexFile = 1;
    void ListFileAppend(string name, byte [] contents)
    {
        string path = name.Substring(_dstCompressPath.Length + 1);
        string md5 = Encode(contents);
        string line;

        FileStream fs = null;
        string lstFilePath = _dstCompressPath + "/" + _ConfigLstFile;
        
        if (!File.Exists(lstFilePath))
        {
            fs = File.Create(lstFilePath);
            indexFile = 1;

            line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", indexFile, path, md5, contents.Length, DateTime.Now.ToString());
        }
        else
        {
            fs = File.Open(lstFilePath, FileMode.Append);
            ++indexFile;

            line = string.Format("\n{0}\t{1}\t{2}\t{3}\t{4}", indexFile, path, md5, contents.Length, DateTime.Now.ToString());
        }

        StreamWriter sw = new StreamWriter(fs);
        sw.Write(line);
        sw.Close();
        fs.Close();
    }

    public string Encode(byte[] bs)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        bs = x.ComputeHash(bs);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            sb.Append(b.ToString("x2").ToLower());
        }
        return sb.ToString();
    }
    #endregion
    
    #region Check Regex Format
    bool CheckRegexSuccess(DataTable table)
    {
        bool res = true;
        if (!(table.Rows.Count >= _regexLineNum)) throw new Exception("!(table.Rows.Count >= _regexLineNum)");// 大于4行

        for(int i = 0; i < table.Columns.Count; ++i) // 列
        {
            Regex regex = new Regex((string)table.Rows[_regexLineNum-1][i]);
            for(int j = _regexLineNum; j < table.Rows.Count; ++j) // 行
            {
                if( !regex.IsMatch(table.Rows[j][i].ToString()) )
                {
                    res = false;
                    UnityEngine.Debug.LogError("Error: Table " + table.TableName + "Line Num: " + (j + 1) + "Column Num: " + (i + 1) + " " + table.Rows[j][i]);
                }
            }
        }
        return res;
    }
    #endregion
    
    #region base file opt
    void WriteToFile(byte[] contents, string path)
    {
        MkDir(Path.GetDirectoryName(path));
        File.WriteAllBytes(path, contents);
    }

    void WriteToFile(string contents, string path)
    {
        MkDir(Path.GetDirectoryName(path));
        File.WriteAllText(path, contents);
    }


    void MkDir(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new Exception("string.IsNullOrEmpty(path)");

        if (!Directory.Exists(path))
        {
            string parent = Directory.GetParent(path).FullName;
            if (string.IsNullOrEmpty(parent)) throw new Exception("string.IsNullOrEmpty(parent)");

            if (!Directory.Exists(parent))
                MkDir(parent);
            Directory.CreateDirectory(path);
        }        
    }
    #endregion
    
    /*
    string GetData(JsonData data, string id, string attr_name)
    {
        try {
            return (string)data[id][(int)data["Title"][attr_name]];
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("Not Found ID: " + id);
            return null;
        }
    }*/
}
