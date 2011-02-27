using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using BizHawk.Emulation.CPUs.M6502;


namespace BizHawk.Emulation.Consoles.Nintendo
{
	partial class NES
	{
		static class Palettes
		{
			static float[] rtmul = { 1.239f, 0.794f, 1.019f, 0.905f, 1.023f, 0.741f, 0.75f };
			static float[] gtmul = { 0.915f, 1.086f, 0.98f, 1.026f, 0.908f, 0.987f, 0.75f };
			static float[] btmul = { 0.743f, 0.882f, 0.653f, 1.277f, 0.979f, 0.101f, 0.75f };

			public static void ApplyDeemphasis(ref int r, ref int g, ref int b, int deemph_bits)
			{
				if (deemph_bits == 0) return;
				int d = deemph_bits-1;
				r = (int)(r * rtmul[d]);
				g = (int)(g * gtmul[d]);
				b = (int)(b * btmul[d]);
				if (r > 0xFF) r = 0xFF;
				if (g > 0xFF) g = 0xFF;
				if (b > 0xFF) b = 0xFF;
			}
			public static int[,] FCEUX_Standard = new int[,]
			{
				{ 0x1D<<2, 0x1D<<2, 0x1D<<2 }, /* Value 0 */
				{ 0x09<<2, 0x06<<2, 0x23<<2 }, /* Value 1 */
				{ 0x00<<2, 0x00<<2, 0x2A<<2 }, /* Value 2 */
				{ 0x11<<2, 0x00<<2, 0x27<<2 }, /* Value 3 */
				{ 0x23<<2, 0x00<<2, 0x1D<<2 }, /* Value 4 */
				{ 0x2A<<2, 0x00<<2, 0x04<<2 }, /* Value 5 */
				{ 0x29<<2, 0x00<<2, 0x00<<2 }, /* Value 6 */
				{ 0x1F<<2, 0x02<<2, 0x00<<2 }, /* Value 7 */
				{ 0x10<<2, 0x0B<<2, 0x00<<2 }, /* Value 8 */
				{ 0x00<<2, 0x11<<2, 0x00<<2 }, /* Value 9 */
				{ 0x00<<2, 0x14<<2, 0x00<<2 }, /* Value 10 */
				{ 0x00<<2, 0x0F<<2, 0x05<<2 }, /* Value 11 */
				{ 0x06<<2, 0x0F<<2, 0x17<<2 }, /* Value 12 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 13 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 14 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 15 */
				{ 0x2F<<2, 0x2F<<2, 0x2F<<2 }, /* Value 16 */
				{ 0x00<<2, 0x1C<<2, 0x3B<<2 }, /* Value 17 */
				{ 0x08<<2, 0x0E<<2, 0x3B<<2 }, /* Value 18 */
				{ 0x20<<2, 0x00<<2, 0x3C<<2 }, /* Value 19 */
				{ 0x2F<<2, 0x00<<2, 0x2F<<2 }, /* Value 20 */
				{ 0x39<<2, 0x00<<2, 0x16<<2 }, /* Value 21 */
				{ 0x36<<2, 0x0A<<2, 0x00<<2 }, /* Value 22 */
				{ 0x32<<2, 0x13<<2, 0x03<<2 }, /* Value 23 */
				{ 0x22<<2, 0x1C<<2, 0x00<<2 }, /* Value 24 */
				{ 0x00<<2, 0x25<<2, 0x00<<2 }, /* Value 25 */
				{ 0x00<<2, 0x2A<<2, 0x00<<2 }, /* Value 26 */
				{ 0x00<<2, 0x24<<2, 0x0E<<2 }, /* Value 27 */
				{ 0x00<<2, 0x20<<2, 0x22<<2 }, /* Value 28 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 29 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 30 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 31 */
				{ 0x3F<<2, 0x3F<<2, 0x3F<<2 }, /* Value 32 */
				{ 0x0F<<2, 0x2F<<2, 0x3F<<2 }, /* Value 33 */
				{ 0x17<<2, 0x25<<2, 0x3F<<2 }, /* Value 34 */
				{ 0x10<<2, 0x22<<2, 0x3F<<2 }, /* Value 35 */
				{ 0x3D<<2, 0x1E<<2, 0x3F<<2 }, /* Value 36 */
				{ 0x3F<<2, 0x1D<<2, 0x2D<<2 }, /* Value 37 */
				{ 0x3F<<2, 0x1D<<2, 0x18<<2 }, /* Value 38 */
				{ 0x3F<<2, 0x26<<2, 0x0E<<2 }, /* Value 39 */
				{ 0x3C<<2, 0x2F<<2, 0x0F<<2 }, /* Value 40 */
				{ 0x20<<2, 0x34<<2, 0x04<<2 }, /* Value 41 */
				{ 0x13<<2, 0x37<<2, 0x12<<2 }, /* Value 42 */
				{ 0x16<<2, 0x3E<<2, 0x26<<2 }, /* Value 43 */
				{ 0x00<<2, 0x3A<<2, 0x36<<2 }, /* Value 44 */
				{ 0x1E<<2, 0x1E<<2, 0x1E<<2 }, /* Value 45 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 46 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 47 */
				{ 0x3F<<2, 0x3F<<2, 0x3F<<2 }, /* Value 48 */
				{ 0x2A<<2, 0x39<<2, 0x3F<<2 }, /* Value 49 */
				{ 0x31<<2, 0x35<<2, 0x3F<<2 }, /* Value 50 */
				{ 0x35<<2, 0x32<<2, 0x3F<<2 }, /* Value 51 */
				{ 0x3F<<2, 0x31<<2, 0x3F<<2 }, /* Value 52 */
				{ 0x3F<<2, 0x31<<2, 0x36<<2 }, /* Value 53 */
				{ 0x3F<<2, 0x2F<<2, 0x2C<<2 }, /* Value 54 */
				{ 0x3F<<2, 0x36<<2, 0x2A<<2 }, /* Value 55 */
				{ 0x3F<<2, 0x39<<2, 0x28<<2 }, /* Value 56 */
				{ 0x38<<2, 0x3F<<2, 0x28<<2 }, /* Value 57 */
				{ 0x2A<<2, 0x3C<<2, 0x2F<<2 }, /* Value 58 */
				{ 0x2C<<2, 0x3F<<2, 0x33<<2 }, /* Value 59 */
				{ 0x27<<2, 0x3F<<2, 0x3C<<2 }, /* Value 60 */
				{ 0x31<<2, 0x31<<2, 0x31<<2 }, /* Value 61 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 62 */
				{ 0x00<<2, 0x00<<2, 0x00<<2 }, /* Value 63 */
			};
		
		} //class palettes
	
	} //partial class NES

} //namespace
