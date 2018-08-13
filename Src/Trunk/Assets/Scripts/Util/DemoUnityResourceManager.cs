using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public struct Pool
    {
        public int max_cnt;
        public List<GameObject> unity_objs;
    }

public class DemoUnityResourceManager<T> : Singleton<DemoUnityResourceManager<T>> where T : UnityEngine.Object
{
    Dictionary<string, int> max_cnt_table = new Dictionary<string, int>();
    //加载到内存的资源存储，key：资源名称
    Dictionary<string, T> m_loaded_res = new Dictionary<string, T>();
    //实例化的资源存储， key: 资源名称
    Dictionary<string, Pool> m_pools = new Dictionary<string, Pool>();

    #region constant
    static int DEFAULT_MAX_CNT = 50;
    #endregion

    private DemoUnityResourceManager()
    {
    }
    public override void Destruct()
    {
        ReleaseAllResource();
    }

    //预加载到内存的assert
    void CacheResource(string asset_name)
    {
        GetObject(asset_name);
    }

    //获取实例化的资源,肯定是GameObject
    public GameObject AllocResource(string asset_name)
    {
        if (m_pools.ContainsKey(asset_name))
        {
            Pool pool = m_pools[asset_name];
            int size = pool.unity_objs.Count;
            if (size > 0)
            {
                GameObject unity_obj = pool.unity_objs[size - 1];
                pool.unity_objs.RemoveAt(size - 1);
                UIHelper.SetActive(unity_obj, true);
                return unity_obj;
            }
        }

        GameObject prefab = GetObject(asset_name) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("The Object you want to Instantiate is Null, asset_name=" + asset_name);
            return null;
        }
        //必须实例化，不能直接返回
        GameObject new_unity_obj = GameObject.Instantiate(prefab) as GameObject;
        UIHelper.SetActive(new_unity_obj, true);
        return new_unity_obj;

    }

    //释放资源,肯定是GameObject
    public void ReleaseResource(string asset_name, GameObject unity_obj)
    {
        if (unity_obj == null)
            return;

        Pool pool;
        if (!m_pools.ContainsKey(asset_name))
        {
            if (max_cnt_table.ContainsKey(asset_name))
            {
                int max_cnt = max_cnt_table[asset_name];
                if (max_cnt == 0)
                {
                    GameObject.Destroy(unity_obj);
                    return;
                }
                else
                {
                    pool = new Pool();
                    pool.max_cnt = max_cnt;
                }
            }
            else
            {
                pool = new Pool();
                pool.max_cnt = DEFAULT_MAX_CNT;
            }
            pool.unity_objs = new List<GameObject>();
            m_pools.Add(asset_name, pool);
        }
        else
            pool = m_pools[asset_name];

        int size = pool.unity_objs.Count;
        unity_obj.transform.parent = null;
        if (size < pool.max_cnt)
        {
            UIHelper.SetActive(unity_obj, false);
            pool.unity_objs.Add(unity_obj);
        }
        else
            GameObject.Destroy(unity_obj);
    }

    public T GetObject(string asset_name)
    {
        T res = null;
        if (asset_name == null)
            return res;
        if (!m_loaded_res.TryGetValue(asset_name, out res))
        {
            res = Resources.Load(asset_name, typeof(T)) as T;
            m_loaded_res[asset_name] = res;
        }
        return res;
    }

    void ReleaseAllResource()
    {
        foreach (Pool pool in m_pools.Values)
        {
            for (int i = 0; i < pool.unity_objs.Count; ++i)
            {
                GameObject.Destroy(pool.unity_objs[i]);
            }
        }
        m_pools.Clear();
    }
}
