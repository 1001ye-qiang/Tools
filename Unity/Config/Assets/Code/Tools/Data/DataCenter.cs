using UnityEngine;
using System.Collections;

public class DataCenter : Singleton<DataCenter> {
    public LitJson.JsonData netData = new LitJson.JsonData();
    public LitJson.JsonData useData = new LitJson.JsonData();
    
}
