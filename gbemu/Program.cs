// See https://aka.ms/new-console-template for more information

using gbemu;

Gameboy gameboy = new Gameboy();

byte[] rom = File.ReadAllBytes("/home/merlin/Projects/gbemu/gbemu/gbemu/tetris.gb");



gameboy.loadRom(rom);
gameboy.start();