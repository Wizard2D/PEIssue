using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PECO
{
    public struct COFFRelocation
    {
        public uint VirtualAddress;
        public uint SymbolTableIndex;
        public COFFRelocationType Type;
    }

    public enum COFFRelocationType : ushort
    { 
        AMD64_ABSOLUTE = 0,
        AMD64_ADDR64 = 0x0001,
        AMD64_ADDR32 = 0x0002,
    }
}
