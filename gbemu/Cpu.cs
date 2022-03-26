namespace gbemu;

public class Cpu
{
    private Gameboy _gameboy;

    private byte A;
    private byte B;
    private byte C;
    private byte D;
    private byte E;
    private byte H;
    private byte L;
    private byte Flags;
    private ushort SP; // Stack Pointer
    private ushort PC; // Program Counter

    private ushort AF
    {
        get => (ushort)((ushort)(A << 8) | Flags);
        set
        {
            A = (byte)(value >> 8);
            Flags = (byte)(value & 0xFF);
        }
    }

    private ushort BC
    {
        get => (ushort)((ushort)(B << 8) | C);
        set
        {
            B = (byte)(value >> 8);
            C = (byte)(value & 0xFF);
        }
    }

    private ushort DE
    {
        get => (ushort)((ushort)(D << 8) | E);
        set
        {
            D = (byte)(value >> 8);
            E = (byte)(value & 0xFF);
        }
    }

    private ushort HL
    {
        get => (ushort)((ushort)(H << 8) | L);
        set
        {
            H = (byte)(value >> 8);
            L = (byte)(value & 0xFF);
        }
    }

    public Cpu(Gameboy gameboy)
    {
        _gameboy = gameboy;
        PC = 0x0100;
    }

public void run()
    {
        while (true)
        {
            byte[] lookahead = _gameboy._memory.GetByteArray(PC, 3);

            switch (lookahead[0])
            {
                case 0x00:
                    PC += 0x1;
                    break;
                case 0x05:
                    B--;
                    Flags &= 0b0001;
                    //Flags |= (byte)(B == 0x00 ? 0b1100 : 0b0100);
                    PC += 0x1;
                    break;
                case 0x06:
                    B = lookahead[1];
                    PC += 0x2;
                    break;
                case 0x0E:
                    C = lookahead[1];
                    PC += 0x2;
                    break;
                case 0x21:
                    H = lookahead[2];
                    L = lookahead[1];
                    PC += 0x3;
                    break;
                case 0x32:
                    _gameboy._memory.Set(HL, A);
                    PC += 0x01;
                    break;
                case 0xAF:
                    Flags = (byte)(A == 0x0000 ? 0b1000 : 0b0000);
                    PC += 0x1;
                    break;
                case 0xC3:
                    PC = (ushort)(lookahead[2] << 8 | lookahead[1]);
                    break;
                default:
                    throw new NotImplementedException(
                        $"OP-Code {lookahead[0]:x} {lookahead[1]:x} {lookahead[2]:x} is not implemented");
                    break;
            }
        }
            
    }
}