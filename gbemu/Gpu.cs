namespace gbemu;

public class Gpu
{
    private Gameboy _gameboy;
    private byte _mode;
    private byte _line;
    
    // https://gbdev.io/pandocs/LCDC.html?highlight=ff40#ff40---lcdc-lcd-control-rw
    private byte _lcdc;
    
    // https://gbdev.io/pandocs/STAT.html
    private byte _lcdStat;
    
    // https://gbdev.io/pandocs/Scrolling.html
    private byte _scx, _scy;


    public int busyFor;

    public Gpu(Gameboy gameboy)
    {
        _gameboy = gameboy;
        busyFor = 0;
        _line = 0;
    }

    public byte ReadByte(ushort offset)
    {
        return offset switch
        {
            0xFF44 => _line,
            _ => 0
        };
    }

    public void WriteByte(ushort offset, byte value)
    {
        switch (offset)
        {
            case 0x00:
                _lcdc = value;
                break;
            case 0x01:
                _lcdStat = value;
                break;
            case 0x02:
                _scy = value;
                break;
            case 0x03:
                _scx = value;
                break;
            default:
                Console.WriteLine($"Memory area for GPU address {offset:x} not yet implemented (attempted write). Trying to continue anyways...");
                break;
        }
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
                if (++_line >= 143)
                {
                    _line = 0;
                    _mode = 1;
                }
                else
                {
                    _mode = 2;
                }
                return 204;
            case 1:
                _mode = 2;
                return 70224-4560;
            default:
                throw new InvalidOperationException($"Invalid GPU mode ({_mode}). This should not be possible to reach.");
        }
    }
}