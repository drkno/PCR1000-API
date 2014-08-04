/*
 * PcrDef
 * Command definitions component of the PCR1000 Library
 * 
 * Copyright Matthew Knox © 2013-Present.
 * This program is distributed with no warentee or garentee
 * what so ever. Do what you want with it as long as attribution
 * to the origional authour and this comment is provided at the
 * top of this source file and any derivative works. Also any
 * modifications must be in real Australian, New Zealand or
 * British English where the language allows.
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToConstant.Global

namespace PCR1000
{
    /// <summary>
    /// Class containing PCR1000 commands.
    /// </summary>
    public static class PcrDef
    {
        /*
	        This is the PCR-1000 Command Set define file. Basically this file
	        consists of all of the pertinent command prefixes that are sent to
	        the radio.
         */

        // ReSharper disable InconsistentNaming

        /// <summary>
		/// Suffix for Radio Query
		/// </summary>
		public static readonly string PCRQST = @"\?";
        /// <summary>
		/// Suffix for execute command 
		/// </summary>
		public static readonly string PCRECMD = "\r\n";

        /// <summary>
		/// Init, manual probe 
        /// \b Warning: after issueing an init DO NOT
        ///      read(). If you do, the read() will block and wont return.
        ///      the radio doesn't return data after an initialization. You
        ///      must close the socket, and reopen it. You wont have to reopen
        ///      the socket with wierd opts, unless you reset the socket to 
        ///      the state as it was before .
	    /// \sa PCRINITA
		/// </summary>
		public static readonly string PCRINITM = "H101\r\nG300\r\n"; 
        /// <summary>
		/// Init, Auto probe
        /// \b Warning: after issueing an init DO NOT
        ///      read(). If you do, the read() will block and wont return.
        ///      the radio doesn't return data after an initialization. You
        ///      must close the socket, and reopen it. You wont have to reopen
        ///      the socket with wierd opts, unless you reset the socket to 
        ///      the state as it was before .
	    /// \sa PCRINITM
		/// </summary>
		public static readonly string PCRINITA = "H101\r\nG301\r\n";
        /// <summary>
        /// Signal Update (G3)
        /// </summary>
        public static readonly string PCRSIG = "G3";
        /// <summary>
		/// Program should poll status from radio (G300)
		/// </summary>
		public static readonly string PCRSIGOFF =	"G300";
        /// <summary>
		/// Radio sends status automagically when a change (G301)
		/// </summary>
		public static readonly string PCRSIGON	="G301";
        /// <summary>
		/// Binary mode off (G302)
		/// </summary>
		public static readonly string PCRSIGBOFF = "G302";
        /// <summary>
		/// Binary mode on  (G303)
		/// </summary>
		public static readonly string PCRSIGBON = "G303";

        /// <summary>
		/// Power (H1)
		/// </summary>
		public static readonly string PCRPWR = "H1";
        /// <summary>
		/// Power radio down (H100)
		/// </summary>
		public static readonly string PCRPWROFF = "H100";
        /// <summary>
		/// Power radio up   (H101)
		/// </summary>
		public static readonly string PCRPWRON = "H101";
        /// <summary>
        /// Radio power query
        /// </summary>
        public static readonly string PCRPWRQRY = "H10?";

        /// <summary>
		/// Volume prefix (J40) 
		/// </summary>
		public static readonly string PCRVOL = "J40";
        /// <summary>
		/// Volume at 75 % (J4075)
		/// </summary>
		public static readonly string PCRVOLON = "J4075";
        /// <summary>
		/// Volume at MUTE (J4000)
		/// </summary>
		public static readonly string PCRVOLOFF = "J4000";

        /// <summary>
		/// Squelch Prefix (J41) 
		/// </summary>
		public static readonly string PCRSQL = "J41";
        /// <summary>
		/// Fully Open (J4100) 
		/// </summary>
		public static readonly string PCRSQLO = "J4100";
        /// <summary>
		/// Closed squelch at 45% (J4145) 
		/// </summary>
		public static readonly string PCRSQLC = "J4145";

        /// <summary>
		/// IF Shift Prefix (J43) 
		/// </summary>
		public static readonly string PCRIF = "J43";
        /// <summary>
		/// IF Centered (J4380)
		/// </summary>
		public static readonly string PCRIFC = "J4380";

        /// <summary>
		/// Automatic Gain Control Prefix (J45) 
		/// </summary>
		public static readonly string PCRAGC = "J45";
        /// <summary>
		/// AGC Off (J4500) 
		/// </summary>
		public static readonly string PCRAGCOFF = "J4500";
        /// <summary>
		/// AGC On  (J4501) 
		/// </summary>
		public static readonly string PCRAGCON = "J4501";

        /// <summary>
		/// Noise Blanking Prefix (J46) 
		/// </summary>
		public static readonly string PCRNB = "J46";
        /// <summary>
		/// Noise Blanking Off (J4600) 
		/// </summary>
		public static readonly string PCRNBOFF = "J4600";
        /// <summary>
		/// Noise Blanking On  (J4601) 
		/// </summary>
		public static readonly string PCRNBON = "J4601";

        /// <summary>
		/// RF Attenuator Prefix 
		/// </summary>
		public static readonly string PCRRFA = "J47";
        /// <summary>
		/// RF Attenuator Off (J4700) 
		/// </summary>
		public static readonly string PCRRFAOFF = "J4700";
        /// <summary>
		/// RF Attenuator On (J4701) 
		/// </summary>
		public static readonly string PCRRFAON = "J4701";

        /// <summary>
		/// VSC Prefix (J50) 
		/// </summary>
		public static readonly string PCRVSC = "J50";
        /// <summary>
		/// VSC Off (J5000) 
		/// </summary>
		public static readonly string PCRVSCOFF = "J5000";
        /// <summary>
		/// VSC On  (J5001) 
		/// </summary>
		public static readonly string PCRVSCON = "J5001";
        /// <summary>
		/// CTCSS - Tone Squelch Prefix (J51) 
		/// </summary>
		public static readonly string PCRTSQL = "J51";
        /// <summary>
		/// CTCSS - Tone Squelch Off (J5100) 
		/// </summary>
		public static readonly string PCRTSQLOFF = "J5100";
        /// <summary>
        /// Unknown - 1
        /// </summary>
		public static readonly string PCRUNK01 = "J4A";
		/// <summary>
        /// Unknown - 2
		/// </summary>
		public static readonly string PCRUNK02 = "J4A80";
        /// <summary>
		/// Tracking filter Prefix (LD082) 
		/// </summary>
		public static readonly string PCRTFLTR = "LD82";
        /// <summary>
		/// Automagic Tracking Filter (LD8200) 
		/// </summary>
		public static readonly string PCRTFLTR00 = "LD8200";
        /// <summary>
		/// Manual Tracking Filter (LD8201) 
		/// </summary>
		public static readonly string PCRTFLTR01 = "LD8201";

        /// <summary>
		/// Freq. Header (K0) 
		/// </summary>
		public static readonly string PCRFRQ = "K0";
        /// <summary>
		/// freq. len. 10 bytes (padded) GMMMKKKHHH (10) 
		/// </summary>
		public static ulong MAXFRQLEN = 10;
        /// <summary>
		/// lower bounds for frequency 50 kHz (50000) 
		/// </summary>
		public static readonly ulong LOWERFRQ = 50000;	
        /// <summary>
		/// upper bound for frequency 1.3 GHz (1300000000) 
		/// </summary>
		public static readonly ulong UPPERFRQ = 1300000000;
        /// <summary>
		/// Lower sideband (00) 
		/// </summary>
		public static readonly string PCRMODLSB = "00";
        /// <summary>
		/// Upper sideband (01) 
		/// </summary>
		public static readonly string PCRMODUSB = "01";
        /// <summary>
		/// Amplitude Modulated (02) 
		/// </summary>
		public static readonly string PCRMODAM = "02";
        /// <summary>
		/// Continuous Mode (03) 
		/// </summary>
		public static readonly string PCRMODCW = "03";
        /// <summary>
		/// unknown mode -- (04) 
		/// </summary>
		public static readonly string PCRMODUNK = "04";
        /// <summary>
		/// Narrowband FM (05) 
		/// </summary>
		public static readonly string PCRMODNFM = "05";
        /// <summary>
		/// Wideband FM (06) 
		/// </summary>
		public static readonly string PCRMODWFM = "06";
        /// <summary>
		/// 3 kHz Filter (00)	
		/// </summary>
		public static readonly string PCRFLTR3 = "00";
        /// <summary>
		/// 6 kHz Filter (01) 	
		/// </summary>
		public static readonly string PCRFLTR6 = "01";
        /// <summary>
		/// 15 kHz Filter (02) 	
		/// </summary>
		public static readonly string PCRFLTR15 = "02";
        /// <summary>
		/// 50 kHz Filter (03) 	
		/// </summary>
		public static readonly string PCRFLTR50 = "03";
        /// <summary>
		/// 230 kHz Filter (04)
		/// </summary>
		public static readonly string PCRFLTR230 = "04";
        /// <summary>
		/// Query Squelch Setting (I0)
		/// </summary>
		public static readonly string PCRQSQL = "I0";
        /// <summary>
		/// Query Signal Strength (I1)
		/// </summary>
		public static readonly string PCRQRST = "I1?";
        /// <summary>
		/// Query Frequency Offset (I2)
		/// </summary>
		public static readonly string PCRQOFST = "I2";
        /// <summary>
		/// Query presense of DTMF Tone (I3)
		/// </summary>
		public static readonly string PCRQDTMF = "I3";
        /// <summary>
		/// Query Firmware revision (I4)
		/// </summary>
		public static readonly string PCRQWAREZ = "G4";
        /// <summary>
		/// Query Presense of DSP (I5)
		/// </summary>
		public static readonly string PCRQDSP = "GD";
        /// <summary>
		/// Query country / region (I6)
		/// </summary>
		public static readonly string PCRQCTY = "GE";
        /// <summary>
		/// Reply: Ok (G000)
		/// </summary>
		public static readonly string PCRAOK = "G000";
        /// <summary>
        /// Reply: Ok corrupt (G00?)
        /// </summary>
        public static readonly string PCRBOK = "G00?";
        /// <summary>
		/// Reply: There was an error (G001)
		/// </summary>
		public static readonly string PCRABAD = "G001";
        /// <summary>
		/// DSP Header (PCRQDSP)
		/// </summary>
		public static readonly string PCRADSP = PCRQDSP;
        /// <summary>
		/// Not present (GD00)
		/// </summary>
		public static readonly string PCRADSPNO = "GD00";
        /// <summary>
		/// Present (GD01)
		/// </summary>
		public static readonly string PCRADSPOK = "GD01";
        /// <summary>
		/// Squelch Header (PCRQSQL)
		/// </summary>
		public static readonly string PCRASQL = PCRQSQL;
        /// <summary>
		/// Sqlch Closed (04)
		/// </summary>
		public static readonly string PCRASQLCL = "04";
        /// <summary>
		/// Sqlch Open (07)
		/// </summary>
		public static readonly string PCRASQLOPN = "07";
        /// <summary>
		/// Signal Strength (PCRQRST)
        /// \b note: You have this header
		///          plus 00-FF from weak to strong
		/// </summary>
		public static readonly string PCRARST = PCRQRST;
        /// <summary>
		/// Frequency offset Header (PCRQOFST)
        ///	\b note: plus 00-7F from extreme (-) to near ctr OR
		///          plus 81-FF from near ctr to extreme (+)
		/// </summary>
		public static readonly string PCRAOFST = PCRQOFST;
        /// <summary>
		/// Frequency (offset) centered (I280)
		/// </summary>
		public static readonly string PCRAOFSTCTR = "I280";

        /// <summary>
		/// DTMF Header (PCRQDTMF)
		/// </summary>
		public static readonly string PCRADTMF = PCRQDTMF;
        /// <summary>
		/// DTMF Not Heard (I300)
		/// </summary>
		public static readonly string PCRADTMFNO = "I300";
        /// <summary>
		/// DTMF 0 (I310)
		/// </summary>
		public static readonly string PCRADTMF0 = "I310";
        /// <summary>
		/// DTMF 1 (I311)
		/// </summary>
		public static readonly string PCRADTMF1 = "I311";
        /// <summary>
		/// DTMF 2 (I312)
		/// </summary>
		public static readonly string PCRADTMF2 = "I312";
        /// <summary>
		/// DTMF 3 (I313)
		/// </summary>
		public static readonly string PCRADTMF3 = "I313";
        /// <summary>
		/// DTMF 4 (I314) 	
		/// </summary>
		public static readonly string PCRADTMF4 = "I314";
        /// <summary>
		/// DTMF 5 (I315)
		/// </summary>
		public static readonly string PCRADTMF5 = "I315";
        /// <summary>
		/// DTMF 6 (I315)
		/// </summary>
		public static readonly string PCRADTMF6 = "I316";
        /// <summary>
		/// DTMF 7 (I316)
		/// </summary>
		public static readonly string PCRADTMF7 = "I317";
        /// <summary>
		/// DTMF 8 (I318)
		/// </summary>
		public static readonly string PCRADTMF8 = "I318";
        /// <summary>
		/// DTMF 9 (I319)
		/// </summary>
		public static readonly string PCRADTMF9 = "I319";
        /// <summary>
		/// DTMF A (I31A)
		/// </summary>
		public static readonly string PCRADTMFA = "I31A";
        /// <summary>
		/// DTMF B (I31B)
		/// </summary>
		public static readonly string PCRADTMFB = "I31B";
        /// <summary>
		/// DTMF C (I31C)
		/// </summary>
		public static readonly string PCRADTMFC = "I31C";
        /// <summary>
		/// DTMF D (I31D)
		/// </summary>
		public static readonly string PCRADTMFD = "I31D";
        /// <summary>
		/// DTMF * (I31E)
		/// </summary>
		public static readonly string PCRADTMFS = "I31E";
        /// <summary>
		/// DTMF # (I31F)
		/// </summary>
		public static readonly string PCRADTMFP = "I31F";

        /* Radio miscellaneous functions */
        /// <summary>
		/// Baud Rate Header (G1)
		/// </summary>
		public static readonly string PCRBD = "G1";
        /// <summary>
		/// 300 baud (G100)
		/// </summary>
		public static readonly string PCRBD300 = "G100";
        /// <summary>
		/// 1200 baud (G101)
		/// </summary>
		public static readonly string PCRBD1200 = "G101";
        /// <summary>
		/// 2400 baud (G102)
		/// </summary>
		public static readonly string PCRBD2400 = "G102";
        /// <summary>
		/// 9600 baud (G103)
		/// </summary>
		public static readonly string PCRBD9600 = "G103";
        /// <summary>
		/// 19200 baud (G104)
		/// </summary>
		public static readonly string PCRBD19200 = "G104";
        /// <summary>
		/// 38400 baud (G105)
		/// </summary>
		public static readonly string PCRBD38400 = "G105";

        /* BandScope functions */
        /// <summary>
		/// bandscope prefix SENT (ME00001)
		/// </summary>
		public static readonly string PCRSBSC = "ME00001";	
        /// <summary>
		/// bandscope prefix RECV (NE1)
		/// </summary>
		public static readonly string PCRRBSC = "NE1";		
        /// <summary>
		/// packet 0 (NE100)
		/// </summary>
		public static readonly string PCRRBSC0 = "NE100";		
        /// <summary>
		/// packet 1 (NE110)
		/// </summary>
		public static readonly string PCRRBSC1 = "NE110";	
        /// <summary>
		/// packet 2 (NE120)
		/// </summary>
		public static readonly string PCRRBSC2 = "NE120";	
        /// <summary>
		/// packet 3 (NE130)
		/// </summary>
		public static readonly string PCRRBSC3 = "NE130";
        /// <summary>
		/// packet 4 (NE140)
		/// </summary>
		public static readonly string PCRRBSC4 = "NE140";
        /// <summary>
		/// packet 5 (NE150)
		/// </summary>
		public static readonly string PCRRBSC5 = "NE150";
        /// <summary>
		/// packet 6 (NE160)
		/// </summary>
		public static readonly string PCRRBSC6 = "NE160";
        /// <summary>
		/// packet 7 (NE170)
		/// </summary>
		public static readonly string PCRRBSC7 = "NE170";
        /// <summary>
		/// packet 8 (NE180)
		/// </summary>
		public static readonly string PCRRBSC8 = "NE180";
        /// <summary>
		/// packet 9 (NE190)
		/// </summary>
		public static readonly string PCRRBSC9 = "NE190";
        /// <summary>
		/// packet 10 (NE1A0)
		/// </summary>
		public static readonly string PCRRBSCA = "NE1A0";
        /// <summary>
		/// packet 11 (NE1B0)
		/// </summary>
		public static readonly string PCRRBSCB = "NE1B0";
        /// <summary>
		/// packet 12 (NE1C0)
		/// </summary>
		public static readonly string PCRRBSCC = "NE1C0";
        /// <summary>
		/// packet 13 (NE1D0)
		/// </summary>
		public static readonly string PCRRBSCD = "NE1D0";
        /// <summary>
		/// packet 14 (NE1E0)
		/// </summary>
		public static readonly string PCRRBSCE = "NE1E0";
        /// <summary>
		/// packet 15 (NE1F0)
		/// </summary>
		public static readonly string PCRRBSCF = "NE1F0";

        // ReSharper restore InconsistentNaming
    }
}
