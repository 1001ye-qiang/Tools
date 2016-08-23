
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CheckSkyCityConfig : EditorWindow
{
    string _configPath = "./Assets/conf/txt";

    [MenuItem("Tools/Check TXT Editor")]
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

        if (GUILayout.Button("Check Field Format By Regex"))
            CheckFiles(_configPath);
    }

    void CheckFiles(string path)
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
                    DataTable table = ReadFileToTable(file.FullName);

                    if (!CheckRegexSuccess(table)) // 需要检查，并且检查不合格
                        UnityEngine.Debug.LogError("Error: " + file.FullName + " check not pass!!!!");
                }
                catch (Exception e) {
                    UnityEngine.Debug.LogError("Error: " + file.Name + " " + e.Message.ToString());
                }
            }
            else
            {
                CheckFiles(files[i].FullName);
            }
        }
    }

    DataTable ReadFileToTable(string path)
    {
        string contents = File.ReadAllText(path);
        if (contents.Contains("\r"))
            UnityEngine.Debug.Log("Warning: " + Path.GetFileName(path) + " contians [\\r]");
        contents = contents.Replace("\r\n", "\n");

        DataTable data = new DataTable();
        data.TableName = Path.GetFileName(path);

        string[] lines = contents.Split('\n');

        if (lines.Length <= 0) throw new Exception("lines.Length <= 0");
        string[] line0contents = lines[0].Split('\t');
        for (int j = 0; j < line0contents.Length; ++j)
        {
            data.Columns.Add(new DataColumn(line0contents[j]));
        }

        for (int i = 0; i < lines.Length; ++i)
        {
            if (string.IsNullOrEmpty(lines[i]))
            {
                UnityEngine.Debug.Log("Warning: " + Path.GetFileName(path) + " has a empty line.");
                continue;
            }

            DataRow dataRow = data.NewRow();
            string[] column = lines[i].Split('\t');
            for (int j = 0; j < column.Length; ++j)
            {
                dataRow[j] = column[j];
            }
            data.Rows.Add(dataRow);
        }

        return data;
    }



    bool CheckRegexSuccess(DataTable table)
    {
        bool res = true;

        for (int i = 0; i < table.Columns.Count; ++i) // 列
        {
            string strRex = (string)table.Rows[0][i];
            if (!strRex.Contains("$")) throw new Exception(strRex + " Not Found $!");
            Regex regex = new Regex(strRex.Split('$')[1]);
            for (int j = 2; j < table.Rows.Count; ++j) // 行
            {
                if (!regex.IsMatch(table.Rows[j][i].ToString()))
                {
                    res = false;
                    UnityEngine.Debug.LogError("Error: Table " + table.TableName + "Line Num: " + (j + 1) + "Column Num: " + (i + 1) + " " + table.Rows[j][i]);
                }
            }
        }
        return res;
    }
}
