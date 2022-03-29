namespace gbemu;

public class Gpu
{
    private Gameboy _gameboy;
    private byte _mode;
    public int busyFor;

    public Gpu(Gameboy gameboy)
    {
        _gameboy = gameboy;
        busyFor = 0;
    }

    public byte ReadByte(ushort offset)
    {
        throw new NotImplementedException();
    }

    public void WriteByte(ushort offset, byte value)
    {
        throw new NotImplementedException();
    }

    public int step()
    {
        switch (_mode)
        {
            case 2:
                _mode = 3;
                return 80;
            case 3:
                _mode = 0;
                return 172;
            case 0:
                _mode = 1;
                return 204+456;
            case 1:
                _mode = 2;
                return 456*10+70224;
            default:
                throw new InvalidOperationException($"Invalid GPU mode ({_mode}). This should not be possible to reach.");
        }
    }
}