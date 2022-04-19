using System;

namespace RouterApi.Observers
{
    public abstract class BaseEventHandler<T>
        where T : EventArgs
    {
        public event EventHandler<T> OnUpdate;

        public event EventHandler<T> OnCreate;

        public event EventHandler<T> OnDelete;

        public abstract void InitializeEvent();

        public void UpdateInvoke(object sender, T e)
        {
            OnUpdate?.Invoke(sender, e);
        }

        public void CreateInvoke(object sender, T e)
        {
            OnCreate?.Invoke(sender, e);
        }

        public void DeleteInvoke(object sender, T e)
        {
            OnDelete?.Invoke(sender, e);
        }
    }
}
