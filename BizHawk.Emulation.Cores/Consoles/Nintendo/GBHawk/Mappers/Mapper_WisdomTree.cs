﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;
using System;

namespace BizHawk.Emulation.Cores.Nintendo.GBHawk
{
	// Wisdom tree mapper (32K bank switching)
	public class MapperWT : MapperBase
	{
		public int ROM_bank;
		public int ROM_mask;

		public override void Initialize()
		{
			ROM_bank = 1;
			ROM_mask = Core._rom.Length / 0x8000 - 1;

			// some games have sizes that result in a degenerate ROM, account for it here
			if (ROM_mask > 4) { ROM_mask |= 3; }
			if (ROM_mask > 0x100) { ROM_mask |= 0xFF; }
		}

		public override byte ReadMemory(ushort addr)
		{
			if (addr < 0x8000)
			{
				return Core._rom[ROM_bank * 0x8000 + addr];
			}
			else
			{
				return 0xFF;
			}
		}

		public override byte PeekMemory(ushort addr)
		{
			return ReadMemory(addr);
		}

		public override void WriteMemory(ushort addr, byte value)
		{
			if (addr < 0x4000)
			{
				ROM_bank = ((addr << 1) & 0x1ff) >> 1;
				ROM_bank &= ROM_mask;
			}
		}

		public override void PokeMemory(ushort addr, byte value)
		{
			WriteMemory(addr, value);
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync("ROM_Bank", ref ROM_bank);
			ser.Sync("ROM_Mask", ref ROM_mask);
		}
	}
}
