using System.Runtime.CompilerServices;

namespace gbemu;

public class Memory
{
    private Gameboy _gameboy;
    private byte[] _rom = new byte[0x8000];
    private byte[] _bootRom = new byte[0x0100];
    private byte[] _wram = new byte[0x2000];
    private byte[] _vram = new byte[0x4000];
    private byte[] _zram = new byte[0x0080];
    internal bool _bootRomActive;
    
    public Memory(Gameboy gameboy, bool bootRomActive = true)
    {
        _gameboy = gameboy;
        _bootRom = File.ReadAllBytes("./dmg0.bin");
        _bootRomActive = bootRomActive;
    }

    public void loadRom(byte[] rom)
    {
        if (rom[0x0147] != 0x00)
        {
            throw new NotImplementedException("ROMs with switched banks are not supported");
        }

        _rom = rom;
    }

    public byte ReadByte(ushort offset)
    {
        return offset switch
        {
            < 0x0100 => _bootRomActive ? _bootRom[offset] : _rom[offset],
            < 0x4000 => _rom[offset],
            >= 0x8000 and < 0xA000 => _vram[offset - 0x8000],
            >= 0xC000 and < 0xE000 => _wram[offset - 0xC000],
            >= 0xE000 and < 0xFE00 => _wram[offset - 0xE000],
            >= 0xFF40 and < 0xFF80 => _gameboy._gpu.ReadByte((ushort)(offset - 0xFF40)),
            >= 0xFF80 => _zram[offset - 0xFF80],
            //_ => throw new NotImplementedException($"Memory area for address {offset:x} not yet implemented (attempted read)")
            _ => 0
        };
    }
    
    
    public byte[] ReadByteArray(ushort offset, ushort length)
    {
        byte[] ret = new byte[length];
        for (int i = 0; i < length; i++)
        {
            ret[i] = ReadByte((ushort)(offset + i));
        }

        return ret;
    }

    public void WriteByte(ushort offset, byte value)
    {
        switch (offset)
        {
            case < 0x8000:
                throw new Exception("Cannot write to ROM");
            case >= 0x8000 and < 0xA000:
                _vram[offset - 0x8000] = value;
                break;
            case >= 0xC000 and < 0xE000:
                _wram[offset - 0xC000] = value;
                break;
            case >= 0xE000 and < 0xFE00:
                _wram[offset - 0xE000] = value;
                break;
            case >= 0xFF0F and <= 0xFF0F:
                _gameboy._cpu.interrupts = value;
                break;
            case >= 0xFF40 and < 0xFF80:
                _gameboy._gpu.WriteByte((ushort)(offset - 0xFF40), value);
                break;
            case >= 0xFF80:
                _zram[offset - 0xFF80] = value;
                break;
            default:
                Console.WriteLine($"Memory area for address {offset:x} not yet implemented (attempted write)");
                Console.WriteLine("Trying to continue anyways...");
                break;
        }
    }
    
    public void WriteWord(ushort offset, ushort value)
    {
        
        WriteByte(offset, (byte)(value & 0xFF));
        WriteByte((ushort)(offset + 1), (byte)(value >> 8));
    }
}