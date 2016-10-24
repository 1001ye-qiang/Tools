using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// editor qiang.zhou 20160321
/// </summary>
public class UIManager : SingletonMono<UIManager>
{
    private List<GameObject> nowShowUI = new List<GameObject>();
    private List<GameObject> selfManageUI = new List<GameObject>();

    Transform rootTransform = null;

    public delegate void OnFinishLoadUI(Transform trans);

    /// <summary>
    /// 所有关闭界面请求
    /// </summary>
    public void CloseRequestUI(GameObject gameObj, bool bDoGC = true)
    {
        // 判空
        if (gameObj == null) { uLog.LogError(uLog.uTools, "GUIManager, close error, obj is null!"); return; }

        // 移除
        if (nowShowUI.Contains(gameObj))
        {
            nowShowUI.Remove(gameObj);
        }
        else if (selfManageUI.Contains(gameObj))
        {
            selfManageUI.Remove(gameObj);
        }
        else
        {
            uLog.LogError(uLog.uTools, "GUIManager, close error, control is null or not found!");
        }

        // 销毁
        if (bDoGC)
        {
            UnityEngine.Object.DestroyImmediate(gameObj);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        else
        {
            UnityEngine.Object.Destroy(gameObj);
        }
        
        // 显示
        ShowDefaultUI();
    }

    /// <summary>
    /// 所有打开界面请求走这里
    /// </summary>
    /// <param name="uiName">界面资源路径</param>
    /// <param name="data">界面初始化需要的数据</param>
    /// <param name="bHideOther">非一级界面时，是否隐藏其他界面</param>
    public void RequestFirstUI(GUILevelEnum level, object data = null, bool bHideOther = true, bool bStack = true, System.Action<GameObject> finish = null)
    {
        LoadUI(UIDefine.Instance.DicUI[level], delegate (Transform trans)
        {
            if (trans != null)
            {
                trans.parent = transform;
                trans.gameObject.SetActive(true);

                int iLev = (int)level / 1000;
                trans.localPosition = new Vector3(0, 0, -(iLev >= 9 ? 5 : iLev) * 800);
                trans.localScale = Vector3.one; // new Vector3(0.88f, 0.88f, 0f);

                trans.gameObject.name = (int)level + "_" + trans.gameObject.name;

                if (data == null)
                    data = level;
                trans.SendMessage("InitData", data, SendMessageOptions.DontRequireReceiver);

                if (trans != null) // InitData 可能销毁自己
                    ChangeUI((int)level, bHideOther, bStack, trans.gameObject);

                if (finish != null)
                    finish(trans != null ? trans.gameObject : null);
            }
        });

    }

    static void LoadUI(string path, OnFinishLoadUI onLoad)
    {
        if (path.Contains("@"))
        {
            //GameObject cache = new GameObject(path);
            //NGUILoader.Instance.LoadUIPrefab(path, delegate (GameObject asset)
            //{
            //    Debug.LogError("finish Load ui " + path);
            //    if (asset != null)
            //    {
            //        Transform trans = (Instantiate(asset) as GameObject).transform;
            //        cache.transform.parent = trans;
            //        onLoad(trans);
            //    }
            //    else
            //    {
            //        onLoad(null);
            //    }
            //}, cache);
        }
        else
        {
            string uiName = "UI/Prefab/" + path;

            Object obj = Resources.Load(uiName);
            if (obj == null)
            {
                uLog.LogError(uLog.uTools, "GUIManager, load ui res not found!");
                onLoad(null);
                return;
            }

            Transform trans = (Instantiate(obj) as GameObject).transform;
            onLoad(trans);
        }
    }


    /// <summary>
    /// 界面层级处理
    /// </summary>
    /// <param name="level">层级ID</param>
    /// <param name="hideOhter">隐藏其它主要界面</param>
    /// <param name="stackType">栈式存入，顶掉本层，更高层</param>
    /// <param name="objAdd">obj</param>
    private void ChangeUI(int level, bool hideOhter, bool stackType, GameObject objAdd)
    {
        int caseValue = (int)(level / 1000);
        if (stackType) ClearUI(caseValue);
        if (hideOhter) HideAllUI();

        if(caseValue == UIDefine.iLevelUI9)
            selfManageUI.Add(objAdd);
        else
            nowShowUI.Add(objAdd);        
    }

    /// <summary>
    /// 清理
    /// </summary>
    private void CheckUI()
    {
        for (int i = nowShowUI.Count - 1; i >= 0; --i)
        {
            if (nowShowUI[i] == null)
            {
                nowShowUI.RemoveAt(i);
                uLog.LogError(uLog.uTools, "GUIManager, remove null item!");
            }
        }

        for (int i = selfManageUI.Count - 1; i >= 0; --i)
        {
            if (selfManageUI[i] == null)
            {
                selfManageUI.RemoveAt(i);
                uLog.Log(uLog.uTools, "GUIManager, remove null item!");
            }
        }
    }

    /// <summary>
    /// 删除所有界面，除主界面外
    /// </summary>
    public void ClearUI(int level = 0)
    {
        for (int i = nowShowUI.Count - 1; i >= 0; --i)
        {
            if (GetUILevel(nowShowUI[i]) / 1000 >= level)
            {
                Destroy(nowShowUI[i]);
                nowShowUI.RemoveAt(i);
            }
        }
        CheckUI();
    }

    private int GetUILevel(GameObject obj)
    {
        if (obj != null)
        {
            string[] names = obj.name.Split('_');
            return System.Convert.ToInt32(names[0]);
        }
        return -1;
    }

    /// <summary>
    /// 隐藏所有UI
    /// </summary>
    private List<GameObject> lstHideUI = new List<GameObject>();
    private List<GameObject> lstTmpHide = new List<GameObject>();
    public void HideAllUI()
    {
        lstTmpHide.Clear();
        for (int i = 0; i < nowShowUI.Count; ++i)
        {
            GameObject obj = nowShowUI[i];
            if (obj != null)
            {
                if (obj.activeSelf)
                    lstTmpHide.Add(obj);
                obj.SetActive(false);
            }
        }

        if (lstTmpHide.Count > 1)
        {
            lstHideUI.Clear();
            lstHideUI = lstTmpHide;
        }
    }

    /// <summary>
    /// 显示默认UI
    /// </summary>
    public void ShowDefaultUI()
    {
        // 清理
        CheckUI();

        // hide时隐藏的UI
        if (lstHideUI.Count > 1)
        {
            while (lstHideUI.Count > 0)
            {
                if (lstHideUI[0] != null)
                    lstHideUI[0].SetActive(true);
                lstHideUI.RemoveAt(0);
            }
        }
        else
        {
            if (nowShowUI.Count > 0)
            {
                nowShowUI[nowShowUI.Count - 1].SetActive(true);
            }
            else
            {
                uLog.Log(uLog.uTools, "GUIManager, Who & why destory main ui!"); // 0 level ui close self, will log this!
            }
        }
    }

    /// <summary>
    /// 显示请求的UI
    /// </summary>
    public void ShowRequestUI(GameObject gameObj)
    {
        if (gameObj == null)
        {
            ShowDefaultUI();
        }
        else
        {
            gameObj.SetActive(true);
        }
    }

    public GameObject FindUI(GUILevelEnum id)
    {
        GameObject target = null;
        for (int i = nowShowUI.Count - 1; i >= 0; --i)
        {
            if (nowShowUI[i] != null && nowShowUI[i].name.Contains(((int)id).ToString()))
            {
                target = nowShowUI[i];
                break;
            }
        }
        if (target == null)
        {
            for (int j = selfManageUI.Count - 1; j >= 0; --j)
            {
                if (selfManageUI[j] != null && selfManageUI[j].name.Contains(((int)id).ToString()))
                {
                    target = selfManageUI[j];
                    break;
                }
            }
        }
        return target;
    }
}


