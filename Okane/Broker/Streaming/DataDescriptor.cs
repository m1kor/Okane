using System;

namespace Okane.Broker.Streaming
{
    public class DataDescriptor : Attribute
    {
        public string Descriptor { get; private set; }

        public DataDescriptor(string descriptor)
        {
            Descriptor = descriptor;
        }
    }
}
