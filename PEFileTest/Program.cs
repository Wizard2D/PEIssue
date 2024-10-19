using PECO;

namespace PEFileTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var coffHeader = new COFFHeader
            {
                Machine = COFFMachine.x86,
                NumberOfSections = 1,
                PointerToSymbolTable = 0,
                NumberOfSymbols = 0,
                SizeOfOptionalHeader = 0,
                Characteristics = 0
            };

            var textSection = new COFFSection
            {
                Name = ".text",
                VirtualSize = 0x1000,
                VirtualAddress = 0x2000,
                SizeOfRawData = 0x40,
                PointerToRawData = 0,
                PointerToRelocations = 0,
                PointerToLineNumbers = 0,
                NumberOfRelocations = 0,
                NumberOfLineNumbers = 0,
                Characteristics = COFFSectionCharacteristics.CNT_CODE | COFFSectionCharacteristics.MEM_EXECUTE | COFFSectionCharacteristics.MEM_READ
            };

            var sectionData = new byte[] { 0xB8, 0x01, 0x00, 0x00, 0x00, // mov eax, 1
                                0xC3 }; // ret

            var symbolTable = new List<COFFSymTableEntry>
            {
                new COFFSymTableEntry
                {
                    Name = new COFFSymbol { Name = "_start" },
                    Value = 0x3D-1,
                    SectionNumber = 1,
                    Type = SymbolType.Function,
                    StorageClass = StorageClass.External,
                    NumberOfAuxSymbols = 0
                }
            };

            var coffFile = new COFFFile
            {
                Header = coffHeader,
                Sections = new List<COFFSection> { textSection },
                SectionData = new Dictionary<COFFSection, byte[]> { { textSection, sectionData } },
                SymbolTable = symbolTable
            };

            coffFile.Flush("out.obj");
        }
    }
}
