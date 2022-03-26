// See https://aka.ms/new-console-template for more information

using gbemu;

Gameboy gameboy = new Gameboy();

byte[] rom = File.ReadAllBytes("tetris.gb");



gameboy.loadRom(rom);
gameboy.start();