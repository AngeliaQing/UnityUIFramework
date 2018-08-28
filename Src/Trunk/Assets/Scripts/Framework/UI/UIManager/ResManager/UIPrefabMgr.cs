using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YUIFramework
{
    public class UIPrefabMgr : IUIResourceMgr
    {
        SortedDictionary<int, KeyValuePair<GameObject, IUIBase>> m_loaded_prefab = new SortedDictionary<int, KeyValuePair<GameObject, IUIBase>>();
        public void LoadUI(UIName ui_name, Action<IUIBase,object> on_load, string ui_dir_path = "", object data = null)
        {
            int i_ui_name = (int)ui_name;
            KeyValuePair<GameObject, IUIBase> pair;
            if (m_loaded_prefab.TryGetValue(i_ui_name, out pair))
            {
                if (on_load != null)
                    on_load(pair.Value, data);
                return;
            }

            string str_ui_name = ui_name.ToString();
            string ui_prefab_path = UIRegister.UI_DIR + ui_dir_path + str_ui_name;
            GameObject prefab = Resources.Load(ui_prefab_path, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                Debug.LogError("UIPrefabMgr LoadUI fail prefab, "+ ui_prefab_path);
                if (on_load != null)
                    on_load(null, data);
                return;
            }
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            if (go == null)
            {
                Debug.LogError("UIPrefabMgr LoadUI fail prefab, "+ ui_prefab_path);
                if (on_load != null)
                    on_load(null, data);
                return;
            }
            go.SetActive(false);  // 等UIBase.Show()时再显示
            UIBase ui_base = go.GetComponent<UIBase>();
            if (ui_base == null)
            {
                Debug.LogError("UIPrefabMgr LoadUI fail prefab, "+ ui_prefab_path);
                if (on_load != null)
                    on_load(null, data);
                return;
            }
            Debug.Log("UIPrefabMgr LoadUI, "+ ui_prefab_path);
            go.name = str_ui_name;  // AssetBundle的名字 == 界面GameObject的名字 == 脚本的名字
            Transform tf = go.transform;
            tf.SetParent(ui_base.GetUIParent());
            //tf.parent = ;
            tf.localScale = Vector3.one;
            tf.localPosition = Vector3.zero;
            m_loaded_prefab[i_ui_name] = new KeyValuePair<GameObject, IUIBase>(go, ui_base);
            ui_base.Name = ui_name;

            // 成功
            if (on_load != null)
                on_load(ui_base, data);
        }

        public IUIBase GetLoadedUI(UIName ui_name)
        {
            KeyValuePair<GameObject, IUIBase> pair;
            if (m_loaded_prefab.TryGetValue((int)ui_name, out pair))
                return pair.Value;
            return null;
        }

        public GameObject GetObjectInAssetBundle(UIName ui_name, string obj_name)  // ui_name没用
        {
            KeyValuePair<GameObject, IUIBase> pair;
            if (m_loaded_prefab.TryGetValue((int)ui_name, out pair))
                return pair.Key;
            else
                return Resources.Load(UIRegister.UI_DIR + obj_name, typeof(GameObject)) as GameObject;
        }

        public void DestroyUI(UIName ui_name)
        {
            int i_ui_name = (int)ui_name;
            if (!m_loaded_prefab.ContainsKey(i_ui_name))
                return;
            m_loaded_prefab.Remove(i_ui_name);
            Debug.Log("UIPrefabMgr DestroyUI, "+ UIRegister.UI_DIR + ui_name.ToString());
        }

        public void ClearLoadedResourse()
        {
            m_loaded_prefab.Clear();
        }
    }
}