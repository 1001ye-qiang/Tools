using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System;

public class UIControlParent : GUIControl
{
    JsonData jd;
    List<JsonData> lstChild = new List<JsonData>();
    public override void InitUI(JsonData param = null, int vTabIndex = 0, int hTabIndex = 0)
    {
        jd = param;
        foreach(JsonData item in jd["children"])
        {
            lstChild.Add(item);
        }

        foreach(JsonData item in jd["label"])
        {
            UITools.Instance.SetLabel(transform.Find((string)item[0]), (string)item[1]);
        }

        foreach (JsonData item in jd["texture"])
        {
            UITools.Instance.SetTexture(transform.Find((string)item[0]), (string)item[1]);
        }

        foreach (JsonData item in jd["Dlabel"])
        {
            UITools.Instance.SetLabel(transform.Find((string)item[0]), (string)DataCenter.Instance.netData[(string)item[1]]);
        }

        foreach (JsonData item in jd["Dtexture"])
        {
            UITools.Instance.SetLabel(transform.Find((string)item[0]), (string)DataCenter.Instance.netData[(string)item[1]]);
        }
    }

    public override void UpdateData(JsonData param = null)
    {
        throw new NotImplementedException();
    }
}
