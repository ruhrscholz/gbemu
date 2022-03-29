namespace gbemu;

public class Gameboy
{
    internal Cpu _cpu;
    internal Memory _memory;
    internal Gpu _gpu;
    internal byte[] _framebuffer = new byte[160 * 144];
    
    public Gameboy()
    {
        _cpu = new Cpu(this);
        _memory = new Memory(this, bootRomActive: false);
        _gpu = new Gpu(this);
        Array.Fill(_framebuffer, (byte)0);
    }

    public void loadRom(byte[] rom)
    {
        _memory.loadRom(rom);
    }

    public void step()
    {
        if (_cpu.busyFor == 0)
        {
            _cpu.busyFor = _cpu.step()-1;
        }
        else
        {
            _cpu.busyFor--;
        }
        
        if (_gpu.busyFor == 0)
        {
            _gpu.busyFor = _gpu.step()-1;
        }
        else
        {
            _gpu.busyFor--;
        }
        
        
        //int gpuIterTaken = _gpu.step();
    }
}