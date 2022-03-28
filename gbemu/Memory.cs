using System.Runtime.CompilerServices;

namespace gbemu;

public class Memory
{
    private Gameboy _gameboy;
    private byte[] _rom = new byte[0x8000];
    private byte[] _bootRom = new byte[0x0100];
    private byte[] _wram = new byte[0x2000];
    private byte[] _zram = new byte[0x0080];
    private bool _bootRomActive;
    
    public Memory(Gameboy gameboy, bool bootRomActive = true)
    {
        _gameboy = gameboy;
        File.ReadAllBytes("./dmg0.bin").CopyTo(_bootRom, 0);
        _bootRomActive = bootRomActive;
    }

    public void loadRom(byte[] rom)
    {
        if (rom.Length != 0x8000)
        {
            throw new NotImplementedException("ROMs with switched banks are not supported");
        }

        _rom = rom;
    }

    public byte GetByte(ushort offset)
    {
        return offset switch
        {
            < 0x0100 => _bootRomActive ? _bootRom[offset] : _rom[offset],
            < 0x3000 => _rom[offset],
            >= 0xC000 and < 0xE000 => _wram[offset - 0xC000],
            < 0xE000 and < 0xFE00 => _wram[offset - 0xE000],
            >= 0xFF80 => _zram[offset - 0xFE00],
            _ => throw new NotImplementedException($"Memory area for address {offset:x} not yet implemented")
        };
    }
    
    
    public byte[] GetByteArray(ushort offset, ushort length)
    {
        byte[] ret = new byte[length];
        for (int i = 0; i < length; i++)
        {
            ret[i] = GetByte((ushort)(offset + i));
        }

        return ret;
    }

    public void Set(ushort offset, byte value)
    {
        switch (offset)
        {
            case < 0x3000:
                throw new Exception("Cannot write to ROM");
            case >= 0xC000 and < 0xE000:
                _wram[offset - 0xC000] = value;
                break;
            case >= 0xE000 and < 0xFE00:
                _wram[offset - 0xE000] = value;
                break;
            case >= 0xFF80:
                _zram[offset - 0xFE00] = value;
                break;
            default:
                throw new NotImplementedException($"Memory area for address {offset:x} not yet implemented");
        }
    }
    
    public void Set(ushort offset, ushort value)
    {
        
        Set(offset, (byte)(value & 0xFF));
        Set((ushort)(offset + 1), (byte)(value >> 8));
    }
}