namespace gbemu;

public class Cpu
{
    private Gameboy _gameboy;
    internal int busyFor;

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
    internal byte interrupts;
    private bool _interruptEnabled;

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
        _interruptEnabled = true;
        busyFor = 0;
        PC = 0x0100;
    }

    public int step()
    {
        int t = op();
        if (interrupts != 0x00)
        {
            throw new NotImplementedException("Interrupt encountered");
        }
        return t;
    }
    public int op()
    {
        byte[] lookahead = _gameboy._memory.ReadByteArray(PC, 3);

        Console.WriteLine($"{lookahead[0]:x}");
        
        switch (lookahead[0])
        {
            case 0x00:
                PC += 0x1;
                return 4;
            case 0x05:
                PC += 0x1;
                Flags &= 1<<4; // Static flags
                Flags |= (byte)((B & 1<<4) != (B-1 & 1<<4) ? 1<<5 : 0); // H flag and dec
                B--;
                Flags |= (byte)(B == 0x00 ? 1<<7 : 0); // Z flag
                return 4;
            case 0x06:
                PC += 0x2;
                B = lookahead[1];
                return 8;
            case 0x0D:
                PC += 0x1;
                Flags &= 1<<4; // Static flags
                Flags |= (byte)((C & 1<<4) != (C-1 & 1<<4) ? 1<<5 : 0); // H flag and dec
                C--;
                Flags |= (byte)(C == 0x00 ? 1<<7 : 0); // Z flag
                return 4;
            case 0x0E:
                C = lookahead[1];
                PC += 0x2;
                return 8;
            case 0x0F:
                PC += 0x1;
                Flags = (byte)((A & 0b0000_0001) == 0b0000_0001 ? 0b0001_0000 : 0b0000_0000);
                A >>= 1;
                Flags |= (byte)(A == 0xFF ? 0b1000_0000 : 0b0000_0000);
                return 4;
            case 0x20:
                if ((Flags & 1 << 7) == 0)
                {
                    PC += (ushort)((sbyte)lookahead[1]+0x1);
                    return 12;
                } else
                {
                    PC += 0x2;
                    return 8;
                }
            case 0x21:
                PC += 0x3;
                H = lookahead[2];
                L = lookahead[1];
                return 12;
            case 0x32:
                PC += 0x01;
                _gameboy._memory.WriteByte(HL--, A);
                return 12;
            case 0x3E:
                PC += 0x2;
                A = lookahead[1];
                return 8;
            case 0xAF:
                PC += 0x1;
                Flags = (byte)(A == 0x0000 ? 0b1000 : 0b0000);
                return 4;
            case 0xC3:
                PC = (ushort)(lookahead[2] << 8 | lookahead[1]);
                return 16;
            case 0xE0:
                PC += 0x2;
                _gameboy._memory.WriteByte((ushort)(0xFF00 + lookahead[1]), A);
                return 12;
            case 0xF0:
                PC += 0x2;
                A = _gameboy._memory.ReadByte((ushort)(0xFF00 + lookahead[1]));
                return 12;
            case 0xF2:
                PC += 0x1;
                A = _gameboy._memory.ReadByte((ushort)(0xFF00 + C));
                return 8;
            case 0xF3:
                _interruptEnabled = false;
                PC += 0x1;
                return 4;
            case 0xFB:
                _interruptEnabled = true;
                PC += 0x1;
                return 4;
            case 0xFE:
                PC += 0x2;
                Flags = 0b0100_0000;
                Flags |= (byte)(A == lookahead[1] ? 1 << 7 : 0); // Z flag
                Flags |= (byte)((A & 1<<4) != ((A - lookahead[1]) & 1<<4) ? 1 << 5 : 0);  // H flag
                Flags |= (byte)(A <  lookahead[1] ? 1 << 4 : 0); // C Flag
                return 8;
            
            default:
                throw new NotImplementedException(
                    $"OP-Code {lookahead[0]:x} {lookahead[1]:x} {lookahead[2]:x} is not implemented");
        }
    }
            
}