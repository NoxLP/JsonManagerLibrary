using System;
using System.Collections.Generic;
using System.Text;

namespace JsonManagerLibrary
{
    public interface IObjectWithSerializationProxy<TProxy>
    {
        TProxy GetSerializationProxy();
    }
}
