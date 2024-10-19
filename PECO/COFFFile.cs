using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace PECO
{
    public struct COFFSectionData 
    { 
        public byte[] Data = { }; 
        public uint Address = 0;

        public COFFSectionData(byte[] data, uint address)
        {
            Data = data;
            Address = address;
        }
    }

    public class COFFFile
    {
        public COFFHeader Header;
        public List<COFFSection> Sections = new List<COFFSection>();
        public Dictionary<COFFSection, byte[]> SectionData = new Dictionary<COFFSection, byte[]>();
        public Dictionary<COFFSection, List<COFFRelocation>> Relocations = new Dictionary<COFFSection, List<COFFRelocation>>();
        public List<COFFSymTableEntry> SymbolTable = new List<COFFSymTableEntry>();

        public void Flush(string path)
        {
            using(var writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                #region Flush Header
                writer.Write((ushort)Header.Machine);
                writer.Write((ushort)Header.NumberOfSections);
                writer.Write((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                writer.Write(Header.PointerToSymbolTable);
                writer.Write(Header.NumberOfSymbols);
                writer.Write(Header.SizeOfOptionalHeader);
                writer.Write((ushort)Header.Characteristics);
                #endregion

                #region Section Headers & Data
                foreach(var section in Sections)
                {
                    writer.Write(Encoding.ASCII.GetBytes(section.Name.PadRight(8, '\0').Substring(0, 8)));
                    writer.Write(section.VirtualSize);
                    writer.Write(section.VirtualAddress);
                    writer.Write(section.SizeOfRawData);
                    writer.Write(section.PointerToRawData);
                    writer.Write(section.PointerToRelocations);
                    writer.Write(section.PointerToLineNumbers);
                    writer.Write(section.NumberOfRelocations);
                    writer.Write(section.NumberOfLineNumbers);
                    writer.Write((uint)section.Characteristics);
                }

                Header.NumberOfSections = (ushort)Sections.Count;

                var reworkedData = new Dictionary<COFFSection, COFFSectionData>();

                for (int i = 0; i < SectionData.Count; i++)
                {
                    var key = SectionData.Keys.ElementAt(i);
                    var sectionData = SectionData[key];

                    reworkedData.Add(key, new COFFSectionData(sectionData, (uint)writer.BaseStream.Position));

                    writer.Write(sectionData);
                    if(sectionData.Length < key.SizeOfRawData)
                    {
                        for (int x = 0; x < key.SizeOfRawData - sectionData.Length; x++)
                            writer.Write((byte)0);
                    }
                }

                for (int i = 0; i < Relocations.Count; i++)
                {
                    var key = Relocations.Keys.ElementAt(i);
                    var relocs = Relocations[key];
                    key.PointerToRelocations = (uint)writer.BaseStream.Length;

                    foreach (var reloc in relocs)
                    {
                        writer.Write(reloc.VirtualAddress);
                        writer.Write(reloc.SymbolTableIndex);
                        writer.Write((ushort)reloc.Type);
                    }

                    key.NumberOfRelocations = (ushort)relocs.Count;
                }
                #endregion

                Console.WriteLine($"Writing Symbol Table - Count: {SymbolTable.Count}");
                #region Symbol Table
                Header.PointerToSymbolTable = (uint)writer.BaseStream.Position;
                foreach(var symEnt in SymbolTable)
                {
                    Console.WriteLine($"Name: {symEnt.Name.Name}, Value: {symEnt.Value}, Section: {symEnt.SectionNumber}");
                    // Write the name(I hate PE/COFF so much, on a side-note.)
                    writer.Write(Encoding.ASCII.GetBytes(symEnt.Name.Name.PadRight(8, '\0').Substring(0, 8)));
                    writer.Write(symEnt.Name.Zeroes);
                    writer.Write(symEnt.Name.Offset);

                    writer.Write(symEnt.Value);

                    writer.Write(symEnt.SectionNumber);
                    writer.Write((ushort)symEnt.Type);

                    writer.Write((byte)symEnt.StorageClass);
                    writer.Write(symEnt.NumberOfAuxSymbols);
                }

                Header.NumberOfSymbols = (uint)SymbolTable.Count;
                #endregion

                #region Rewrite Section Headers and COFF Headers to match data.
                writer.Seek(0, SeekOrigin.Begin);

                writer.Write((ushort)Header.Machine);
                writer.Write((ushort)Header.NumberOfSections);
                writer.Write((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                writer.Write(Header.PointerToSymbolTable);
                writer.Write(Header.NumberOfSymbols);
                writer.Write(Header.SizeOfOptionalHeader);
                writer.Write((ushort)Header.Characteristics);

                foreach (var section in Sections)
                {
                    var dataSect = reworkedData.SingleOrDefault(a => a.Key.Name == section.Name);
                    var relocSect = Relocations.SingleOrDefault(a => a.Key.Name == section.Name);
                    writer.Write(Encoding.ASCII.GetBytes(section.Name.PadRight(8, '\0').Substring(0, 8)));
                    writer.Write(section.VirtualSize);
                    writer.Write(section.VirtualAddress);
                    writer.Write(section.SizeOfRawData);
                    writer.Write(dataSect.Value.Address);
                    writer.Write(relocSect.Key.PointerToRelocations);
                    writer.Write(section.PointerToLineNumbers);
                    writer.Write(section.NumberOfRelocations);
                    writer.Write(section.NumberOfLineNumbers);
                    writer.Write((uint)section.Characteristics);
                }
                #endregion
            }
        }
    }
}
