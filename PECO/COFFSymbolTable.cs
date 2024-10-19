using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PECO
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct COFFSymTableEntry
    {
        public COFFSymbol Name;
        public uint Value;
        public ushort SectionNumber;
        public SymbolType Type;
        public StorageClass StorageClass;
        public byte NumberOfAuxSymbols;
    }

    public enum SymbolType : ushort
    {
        Function = 0x20,
        NotAFunction = 0x00,
    }

    public enum StorageClass : byte
    {
        Null = 0,
        Automatic = 0x20,
        Static = 0x40,
        External = 2
    }
}
