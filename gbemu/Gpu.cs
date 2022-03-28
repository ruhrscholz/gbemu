namespace gbemu;

public class Gpu
{
    private Gameboy _gameboy;

    public Gpu(Gameboy gameboy)
    {
        _gameboy = gameboy;
    }

    public byte ReadByte(ushort offset)
    {
        throw new NotImplementedException();
    }

    public void WriteByte(ushort offset, byte value)
    {
        throw new NotImplementedException();
    }
}