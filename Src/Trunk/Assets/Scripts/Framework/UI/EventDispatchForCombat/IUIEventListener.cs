
public interface IUIEventListener
{
    void ReceiveEvent(int event_type, System.Object event_data = null);
}

public class UIEventListenerContext : IRecyclable
{
    public IUIEventListener m_listener = null;
    public static UIEventListenerContext Create(IUIEventListener listener)
    {
        UIEventListenerContext context = RecyclableObject.Create<UIEventListenerContext>();
        context.m_listener = listener;
        return context;
    }
    public static void Recycle(UIEventListenerContext context)
    {
        RecyclableObject.Recycle(context);
    }
    public void Reset()
    {
        m_listener = null;
    }
}