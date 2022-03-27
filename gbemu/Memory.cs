using System.Runtime.CompilerServices;

namespace gbemu;

public class Memory
{
    private Gameboy _gameboy;
    private byte[] _memory = new byte[0xFFFF];
    private byte[] _bootRom = new byte[0x0100];
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
        
        rom.CopyTo(_memory, 0x0000);
    }

    public byte GetByte(ushort offset)
    {
        return _memory[offset];
    }
    
    public ushort GetUInt16(ushort offset)
    {
        byte lo = _memory[offset];
        byte hi = _memory[offset+0x0001];
        
        return (ushort)(hi << 8 | lo);
    }
    
    public byte[] GetByteArray(ushort offset, ushort length)
    {
        return _memory[offset..(offset + length)];
    }

    public void Set(ushort offset, byte value)
    {
        _memory[offset] = value;
    }
    
    public void Set(ushort offset, ushort value)
    {
        _memory[offset] = (byte)(value & 0xFF);
        _memory[offset + 1] = (byte)(value >> 8);
    }
}