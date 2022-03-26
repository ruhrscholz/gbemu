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

    public byte Get(ushort offset)
    {
        return _memory[offset];
    }
    
    public byte[] Get(ushort offset, int bytes)
    {
        byte[] ret = new byte[bytes];
        for(int i = 0; i < bytes; i++)
        {
            
        }
        return ret;
    }
}