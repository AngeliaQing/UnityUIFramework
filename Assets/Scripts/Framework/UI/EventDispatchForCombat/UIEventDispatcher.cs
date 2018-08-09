using System.Collections.Generic;

public interface IUIEventDispatcher
{
    void AddListener(int event_type, UIEventListenerContext listener_context);
    void RemoveListener(int event_type, UIEventListenerContext listener_context);
    void RemoveAllListeners();
    void NotifyUI(int event_type, System.Object event_data);
}

public class UIEventDispatcher : Singleton<UIEventDispatcher>, IUIEventDispatcher
{
    SortedDictionary<int, List<UIEventListenerContext>> m_all_listeners = new SortedDictionary<int, List<UIEventListenerContext>>();
    private UIEventDispatcher()
    {
    }

    public void AddListener(int event_type, UIEventListenerContext listener_context)
    {
        List<UIEventListenerContext> listeners;
        if (!m_all_listeners.TryGetValue(event_type, out listeners))
        {
            listeners = new List<UIEventListenerContext>();
            m_all_listeners[event_type] = listeners;
        }

        if (listeners.Contains(listener_context))
            return;
        listeners.Add(listener_context);
    }
    public void RemoveListener(int event_type, UIEventListenerContext listener_context)
    {
        List<UIEventListenerContext> listeners;
        if (!m_all_listeners.TryGetValue(event_type, out listeners))
            return;
        if (listeners.Contains(listener_context))
            listeners.Remove(listener_context);
    }

    public void RemoveAllListeners()
    {
        m_all_listeners.Clear();
    }
    public void NotifyUI(int event_type, System.Object event_data)
    {
        List<UIEventListenerContext> listeners;
        if (!m_all_listeners.TryGetValue(event_type, out listeners))
            return;
        int listener_cnt = listeners.Count;
        if (listener_cnt == 0)
            return;
        int index = 0;
        while (index < listener_cnt)
        {
            UIEventListenerContext context = listeners[index];
            IUIEventListener listener = context.m_listener;
            if (listener == null)
                listeners.RemoveAt(index);
            else
                listener.ReceiveEvent(event_type, event_data);

            int new_count = listeners.Count;
            if (new_count < listener_cnt)
                listener_cnt = new_count;
            else
                ++index;
        }
    }

    public override void Destruct()
    {
        RemoveAllListeners();
    }
}