using System;
using System.Collections.Generic;

public class EventBus
{
    private static EventBus instance;

    public static EventBus Instance()
    {
        if (instance == null)
        {
            instance = new EventBus();
        }

        return instance;
    }

    private Dictionary<Type, Delegate> events;
    
    private EventBus()
    {
        events = new Dictionary<Type, Delegate>();
    }
    
    public void Subscribe<T>(Action<T> handler){
        if (events.TryGetValue(typeof(T), out var del))
            events[typeof(T)] = (Action<T>)del + handler;
        else
            events[typeof(T)] = handler;
    }

    public void UnSubscribe<T>(Action<T> handler)
    {
        if (events.TryGetValue(typeof(T), out var del))
        {
            var newDel = (Action<T>)del - handler;
            if (newDel == null) events.Remove(typeof(T));
            else events[typeof(T)] = newDel;
        }
    }
    
    public void Publish<T>(T eventData)
    {
        if (events.TryGetValue(typeof(T), out var del))
        {
            ((Action<T>)del)?.Invoke(eventData);
        }
    }
    
}
