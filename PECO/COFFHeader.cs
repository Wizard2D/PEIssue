using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PECO
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COFFHeader
    {
        public COFFMachine Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public COFFHeaderCharacteristics Characteristics;
    }

    public enum COFFMachine : ushort
    {
        x64 = 0x8664,
        x86 = 0x14C
    }

    [Flags]
    public enum COFFHeaderCharacteristics : ushort
    {
        None = 0,
        IFRelocsStripped = 0x0001,
        IFExecutableImage = 0x0002,
        IFLineNumsStripped = 0x0004,
        IFLocalSymsStripped = 0x0008,
        IF16BitMachine = 0x0040,
        IFDebugStripped = 0x0200,
        IFDLL = 0x2000
    }
}
