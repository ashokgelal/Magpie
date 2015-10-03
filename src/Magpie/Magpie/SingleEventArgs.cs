using System;

namespace Magpie
{
    public class SingleEventArgs<T> : EventArgs
    {
        public T Payload { get; private set; }

        public SingleEventArgs(T payload)
        {
            Payload = payload;
        }
    }
}