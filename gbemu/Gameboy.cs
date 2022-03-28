namespace gbemu;

public class Gameboy
{
    internal Cpu _cpu;
    internal Memory _memory;
    internal Gpu _gpu;
    public Gameboy()
    {
        _cpu = new Cpu(this);
        _memory = new Memory(this, bootRomActive: false);
        _gpu = new Gpu(this);
    }

    public void loadRom(byte[] rom)
    {
        this._memory.loadRom(rom);
    }

    public void start()
    {
        this._cpu.run();
    }
}