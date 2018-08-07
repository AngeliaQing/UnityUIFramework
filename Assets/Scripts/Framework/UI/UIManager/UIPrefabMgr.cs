using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public class UIPrefabMgr : IUIResourceMgr
    {
        SortedDictionary<string, KeyValuePair<GameObject, IUIBase>> m_loaded_prefab = new SortedDictionary<string, KeyValuePair<GameObject, IUIBase>>();
        public void LoadUI(string ui_name, Action<IUIBase> on_load, string ui_dir_path = "")
        {
            KeyValuePair<GameObject, IUIBase> pair;
            if (m_loaded_prefab.TryGetValue(ui_name, out pair))
            {
                if (on_load != null)
                    on_load(pair.Value);
                return;
            }
            GameObject prefab = Resources.Load("UI/" + ui_dir_path + ui_name, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                Debug.LogError("UIPrefabMgr LoadUI fail prefab, UI/" + ui_dir_path + ui_name);
                if (on_load != null)
                    on_load(null);
                return;
            }
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            if (go == null)
            {
                Debug.LogError("UIPrefabMgr LoadUI fail prefab, UI/" + ui_dir_path + ui_name);
                if (on_load != null)
                    on_load(null);
                return;
            }
            go.SetActive(false);  // 等UIBase.Show()时再显示
            UIBase ui_base = go.GetComponent<UIBase>();
            if (ui_base == null)
            {
                Debug.LogError("UIPrefabMgr LoadUI fail prefab, UI/" + ui_dir_path + ui_name);
                if (on_load != null)
                    on_load(null);
                return;
            }
            Debug.Log("UIPrefabMgr LoadUI, UI/" + ui_name);
            go.name = ui_name;  // AssetBundle的名字 == 界面GameObject的名字 == 脚本的名字
            Transform tf = go.transform;
            tf.parent = ui_base.GetUIParent();
            tf.localScale = Vector3.one;
            tf.localPosition = Vector3.zero;
            m_loaded_prefab[ui_name] = new KeyValuePair<GameObject, IUIBase>(go, ui_base);

            // 成功
            if (on_load != null)
                on_load(ui_base);
        }

        public IUIBase GetLoadedUI(string ui_name)
        {
            KeyValuePair<GameObject, IUIBase> pair;
            if (m_loaded_prefab.TryGetValue(ui_name, out pair))
                return pair.Value;
            return null;
        }

        public GameObject GetObjectInAssetBundle(string ui_name, string obj_name)  // ui_name没用
        {
            KeyValuePair<GameObject, IUIBase> pair;
            if (m_loaded_prefab.TryGetValue(obj_name, out pair))
                return pair.Key;
            else
                return Resources.Load("UI/" + obj_name, typeof(GameObject)) as GameObject;
        }

        public void DestroyUI(string ui_name)
        {
            if (!m_loaded_prefab.ContainsKey(ui_name))
                return;
            m_loaded_prefab.Remove(ui_name);
            Debug.Log("UIPrefabMgr DestroyUI, UI/Layout/" + ui_name);
        }

        public void ClearLoadedResourse()
        {
            m_loaded_prefab.Clear();
        }
    }
}