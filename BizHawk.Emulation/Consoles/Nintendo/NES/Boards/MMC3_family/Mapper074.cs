﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Consoles.Nintendo
{
	class Mapper074 : MMC3Board_Base
	{
		//http://wiki.nesdev.com/w/index.php/INES_Mapper_074

		public override bool Configure(NES.EDetectionOrigin origin)
		{
			//analyze board type
			switch (Cart.board_type)
			{
				case "MAPPER074":
					break;
				default:
					return false;
			}

			BaseSetup();
			return true;
		}

		public override void WritePPU(int addr, byte value)
		{
			if (addr < 0x2000)
			{
				VRAM[addr & 0x7FF] = value;
			}
			else
			{
				base.WritePPU(addr, value);
			}
		}

		private int GetBankNum(int addr)
		{
			int bank_1k = Get_CHRBank_1K(addr);
			bank_1k &= chr_mask;
			return bank_1k;
		}

		public override byte ReadPPU(int addr)
		{
			if (addr < 0x2000)
			{
				int bank = GetBankNum(addr);
				if (bank == 0x08)
				{
					return VRAM[addr & 0x03FF];
				}
				else if (bank == 0x09)
				{
					return VRAM[(addr & 0x03FF) + 0x400];
				}
				else
				{
					addr = MapCHR(addr);
					return VROM[addr + extra_vrom];
				}

			}
			else return base.ReadPPU(addr);
		}
	}
}
