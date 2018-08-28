using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public interface IUIResourceMgr
    {
        // 加载UI资源ui_name，加载成功后回调on_load
        void LoadUI(UIName ui_name, Action<IUIBase, object> on_load, string ui_dir_path = "", object param = null);
        // 从已经加载的资源中查找ui_name
        IUIBase GetLoadedUI(UIName ui_name);
        // 在ui_name这个资源中查找obj_name这个资源（一个AssetBundle可以有多个资源；一个prefab只能有一份资源）
        GameObject GetObjectInAssetBundle(UIName ui_name, string obj_name);
        // 释放资源ui_name
        void DestroyUI(UIName ui_name);
        // 释放所有资源
        void ClearLoadedResourse();
    }
}