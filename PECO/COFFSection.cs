using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PECO
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct COFFSection
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Name;
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLineNumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLineNumbers;
        public COFFSectionCharacteristics Characteristics;
    }

    [Flags]
    public enum COFFSectionCharacteristics : uint
    { 
        CNT_CODE = 0x00000020,
        CNT_INITDATA = 0x00000040,
        CNT_UNINITDATA = 0x00000080,
        LNK_COMDAT = 0x00001000,
        LNK_REMOVE = 0x00000800,
        MEM_EXECUTE = 0x20000000,
        MEM_READ = 0x40000000,
        MEM_WRITE = 0x80000000,
        MEM_SHARED = 0x10000000,
    }
}
