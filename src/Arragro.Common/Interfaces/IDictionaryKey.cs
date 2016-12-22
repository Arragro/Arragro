using System;

namespace Arragro.Common.Interfaces
{
    public interface IDictionaryKey
    {
        string DictionaryKey { get; set; }
        DateTime LastWriteDate { get; set; }
    }
}
