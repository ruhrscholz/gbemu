using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

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
        PC = 0x0000;
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
        if (PC >= 0x0100 && _gameboy._memory._bootRomActive)
        {
            _gameboy._memory._bootRomActive = false;
        }
        
        byte[] lookahead = _gameboy._memory.ReadByteArray(PC, 3);

        Console.WriteLine($"{lookahead[0]:x}");
        
        switch (lookahead[0])
        {
            case 0x00:
                PC += 0x1;
                return 4;
            case 0x05:
                return OP_DEC(ref B);
            case 0x06:
                return OP_LD(out B, lookahead[1]);
            case 0x0C:
                return OP_INC(ref C);
            case 0x0D:
                return OP_DEC(ref C);
            case 0x0E:
                return OP_LD(out C, lookahead[1]);
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
            case 0x11:
                return OP_LD(out D,  out E, (ushort)(lookahead[2] << 8 | lookahead[1]));
            case 0x21:
                return OP_LD(out H,  out L, (ushort)(lookahead[2] << 8 | lookahead[1]));
            case 0x31:
                return OP_LD(out SP, (ushort)(lookahead[2] << 8 | lookahead[1]));
            case 0x32:
                return OP_LD(HL--, ref A);
            case 0x3E:
                return OP_LD(out A, lookahead[1]);
            case 0x7C:
                return OP_LD(out A, ref H);
            case 0x77:
                return OP_LD(HL, ref A);
            case 0xAF:
                return OP_XOR(ref A);
            case 0xC3:
                PC = (ushort)(lookahead[2] << 8 | lookahead[1]);
                return 16;
            case 0xCB:
                switch (lookahead[1])
                {
                    // BIT
                    case >= 0x40 and <= 0x7F:
                        byte bit = lookahead[1] switch
                        {
                            >= 0x40 and <= 0x47 => 0,
                            >= 0x48 and <= 0x4F => 1,
                            >= 0x50 and <= 0x57 => 2,
                            >= 0x58 and <= 0x5F => 3,
                            >= 0x60 and <= 0x67 => 4,
                            >= 0x68 and <= 0x6F => 5,
                            >= 0x70 and <= 0x77 => 6,
                            >= 0x78 and <= 0x7F => 7,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        switch (lookahead[1] & 0x0F)
                        {
                            case 0x00 or 0x08:
                                return OP_BIT(bit, ref B);
                            case 0x01 or 0x09:
                                return OP_BIT(bit, ref C);
                            case 0x02 or 0x0A:
                                return OP_BIT(bit, ref D);
                            case 0x03 or 0x0B:
                                return OP_BIT(bit, ref E);
                            case 0x04 or 0x0C:
                                return OP_BIT(bit, ref H);
                            case 0x05 or 0x0D:
                                return OP_BIT(bit, ref L);
                            case 0x06 or 0x0E:
                                throw new NotImplementedException("2 byte BIT opcode not yet implemented");
                                return 12;
                            case 0x07 or 0x0F:
                                return OP_BIT(bit, ref A);
                        }
                        break;

                    default:
                        throw new NotImplementedException(
                            $"OP-Code {lookahead[0]:x} {lookahead[1]:x} {lookahead[2]:x} is not implemented");
                }

                throw new NotImplementedException(
                    $"OP-Code {lookahead[0]:x} {lookahead[1]:x} {lookahead[2]:x} is not implemented");
            
            case 0xE0:
                PC += 0x2;
                _gameboy._memory.WriteByte((ushort)(0xFF00 + lookahead[1]), A);
                return 12;
            case 0xE2:
                PC += 0x1;
                _gameboy._memory.WriteByte((ushort)(0xFF00 + C), A);
                return 8;
            case 0xE5:
                return OP_PUSH(ref H, ref L);
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
                return OP_CP(lookahead[1]);
            default:
                throw new NotImplementedException(
                    $"OP-Code {lookahead[0]:x} {lookahead[1]:x} {lookahead[2]:x} is not implemented");
        }
    }

    private int OP_BIT(byte bit, ref byte register)
    {
        PC += 0x1;
        Flags &= 0b0001_0000;
        if ((register & (byte)(1 << bit)) == 0x00)
        {
            Flags |= 0b0010_0000;
        }
        else
        {
            Flags |= 0b1010_0000;
        }

        return 8;
    }

    private int OP_CP(ref byte register)
    {
        PC += 0x01;
        Flags = 0b0100_0000;
        Flags |= (byte)(A == register ? 1 << 7 : 0); // Z flag
        Flags |= (byte)((A & 1<<4) != ((A - register) & 1<<4) ? 1 << 5 : 0);  // H flag
        Flags |= (byte)(A <  register ? 1 << 4 : 0); // C Flag
        return 4;
    }
    
    private int OP_CP(byte value)
    {
        PC += 0x02;
        Flags = 0b0100_0000;
        Flags |= (byte)(A == value ? 1 << 7 : 0); // Z flag
        Flags |= (byte)((A & 1<<4) != ((A - value) & 1<<4) ? 1 << 5 : 0);  // H flag
        Flags |= (byte)(A <  value ? 1 << 4 : 0); // C Flag
        return 8;
    }
    private int OP_INC(ref byte register)
    {
        PC += 0x1;
        Flags &= 1<<4; // Static flags
        Flags |= (byte)((register & 1<<4) != (register+1 & 1<<4) ? 1<<5 : 0); // H flag
        register++;
        Flags |= (byte)(register == 0x00 ? 1<<7 : 0); // Z flag
        return 8;
    }

    private int OP_LD(out byte rh, out byte rl, ushort value)
    {
        PC += 0x03;
        rh = (byte)(value >> 8);
        rl = (byte)(value & 0xFF);
        return 12;
    }
    private int OP_LD(out byte register, byte value)
    {
        PC += 0x02;
        register = value;
        return 8;
    }
    private int OP_LD(out ushort register, ushort value)
    {
        PC += 0x03;
        register = value;
        return 12;
    }
    private int OP_LD(out byte target, ref byte value)
    {
        PC += 0x1;
        target = value;
        return 4;
    }

    private int OP_LD(ushort offset, ref byte value)
    {
        PC += 0x1;
        _gameboy._memory.WriteByte(offset, value);
        return 8;
    }

    private int OP_DEC(ref byte register)
    {
        PC += 0x1;
        Flags &= 1<<4; // Static flags
        Flags |= (byte)((register & 1<<4) != (register-1 & 1<<4) ? 1<<5 : 0); // H flag 
        register--;
        Flags |= (byte)(register == 0x00 ? 1<<7 : 0); // Z flag
        return 8;
    }

    private int OP_PUSH(ref byte rh, ref byte rl)
    {
        PC += 0x1;
        _gameboy._memory.WriteWord(--SP, (ushort)(rh<<8|rl));
        return 16;
    }

    private int OP_XOR(ref byte register)
    {
        PC += 0x1;
        A = (byte)(A | register);
        Flags = (byte)(A == 0x00 ? 0b1000_0000 : 0b0000_0000);
        return 4;
    }
    
            
}