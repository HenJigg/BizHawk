﻿using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Waterbox;
using System;
using System.Collections.Generic;

namespace BizHawk.Emulation.Cores.Consoles.NEC.PCFX
{
	[Core(CoreNames.TST,
		author: "Mednafen Team",
		isPorted: true,
		isReleased: true,
		portedVersion: "1.26.1",
		portedUrl: "https://mednafen.github.io/releases/",
		singleInstance: false)]
	public class Tst : NymaCore
	{
		[CoreConstructor("PCFX")]
		public Tst(CoreLoadParameters<NymaSettings, NymaSyncSettings> lp)
			: base(lp.Comm, "PCFX", "PC-FX Controller", lp.Settings, lp.SyncSettings)
		{
			if (lp.Roms.Count > 0)
				throw new InvalidOperationException("To load a PC-FX game, please load the CUE file and not the BIN file.");
			var firmwares = new Dictionary<string, FirmwareID>
			{
				{ "FIRMWARE:pcfx.rom", new("PCFX", "BIOS") },
			};

			DoInit<LibNymaCore>(lp, "pcfx.wbx", firmwares);
		}

		protected override IDictionary<string, SettingOverride> SettingOverrides { get; } = new Dictionary<string, SettingOverride>
		{
			{ "pcfx.input.port1.multitap", new() { Hide = true } },
			{ "pcfx.input.port2.multitap", new() { Hide = true } },
			{ "pcfx.bios", new() { Hide = true } },
			{ "pcfx.fxscsi", new() { Hide = true } },
			{ "nyma.rtcinitialtime", new() { Hide = true } },
			{ "nyma.rtcrealtime", new() { Hide = true } },

			{ "pcfx.slstart", new() { NonSync = true, NoRestart = true } },
			{ "pcfx.slend", new() { NonSync = true, NoRestart = true } },

			{ "pcfx.mouse_sensitivity", new() { Hide = true } },
			{ "pcfx.nospritelimit", new() { NonSync = true } },
			{ "pcfx.high_dotclock_width", new() { NonSync = true } },
			{ "pcfx.rainbow.chromaip", new() { NonSync = true } },

			{ "pcfx.adpcm.suppress_channel_reset_clicks", new() { NonSync = true } },
			{ "pcfx.adpcm.emulate_buggy_codec", new() { NonSync = true } },

			{ "pcfx.resamp_quality", new() { NonSync = true } },
			{ "pcfx.resamp_rate_error", new() { Hide = true } },
		};

		protected override HashSet<string> ComputeHiddenPorts()
		{
			// NB: Since we're hiding these settings up above, this will always trim us down to 2 ports
			var devCount = 8;
			if (SettingsQuery("pcfx.input.port1.multitap") != "1")
				devCount -= 3;
			if (SettingsQuery("pcfx.input.port2.multitap") != "1")
				devCount -= 3;
			var ret = new HashSet<string>();
			for (var i = 1; i <= 8; i++)
			{
				if (i > devCount)
					ret.Add($"port{i}");
			}
			return ret;
		}
	}
}
