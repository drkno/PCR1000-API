/*
 * PcrControl
 * Control component of the PCR1000 Library
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
// ReSharper disable UnusedMethodReturnValue.Global

#region

using System;
using System.Diagnostics;
using System.Globalization;
using PCR1000.Annotations;

#endregion

namespace PCR1000
{
    /// <summary>
    ///     Control class for the PCR1000
    /// </summary>
    public class PcrControl
    {
        /// <summary>
        ///     Default COM port to listen on.
        /// </summary>
        private const string PcrDefaultPort = "COM1";

        /// <summary>
        ///     Default rate.
        /// </summary>
        private const int PcrDefaultSpeed = 9600;

        /// <summary>
        ///     The currently active Primitive Communication Object.
        /// </summary>
        private IComm _pcrComm;

        /// <summary>
        ///     Was there an error reading from the PComm object?
        /// </summary>
        [UsedImplicitly] private bool _pcrErrRead;

        /// <summary>
        ///     Currently active radio data.
        /// </summary>
        private PRadInf _pcrRadio;

        /// <summary>
        ///     The state of the PComm object (on or off).
        /// </summary>
        private bool _pcrStatus;

        /// <summary>
        ///     Constructor. Initialises the class.
        /// </summary>
        /// <param name="port">COM Port to use.</param>
        /// <param name="speed">COM Speed to use.</param>
        public PcrControl(string port = PcrDefaultPort, int speed = PcrDefaultSpeed)
        {
            Debug.WriteLine("PcrControl Setup");
            _pcrRadio = new PRadInf();
            _pcrComm = new PcrComm(port, speed);

            _pcrRadio.PcrPort = port;
            _pcrRadio.PcrSpeed = speed;
            _pcrRadio.PcrVolume = 0;
            _pcrRadio.PcrSquelch = 0;
            _pcrRadio.PcrFreq = 146000000;
            _pcrRadio.PcrMode = PcrDef.PCRMODNFM;
            _pcrRadio.PcrFilter = PcrDef.PCRFLTR15;
            _pcrRadio.PcrToneSq = "";
            _pcrRadio.PcrToneSqFloat = 0.0f;
            _pcrRadio.PcrAutoGain = false;
            _pcrRadio.PcrNoiseBlank = false;
            _pcrRadio.PcrRfAttenuator = false;
            _pcrRadio.PcrAutoUpdate = false;
            _pcrStatus = false;
        }

        /// <summary>
        ///     Constructor. Initialises the class.
        /// </summary>
        /// <param name="nethost">Internet server to use.</param>
        /// <param name="netport">Internet port to use.</param>
        public PcrControl(int netport, string nethost)
        {
            Debug.WriteLine("PcrControl NetSetup");
            _pcrRadio = new PRadInf();
            _pcrComm = new PcrNetworkClient(nethost, netport);
            _pcrRadio.PcrVolume = 0;
            _pcrRadio.PcrSquelch = 0;
            _pcrRadio.PcrFreq = 146000000;
            _pcrRadio.PcrMode = PcrDef.PCRMODNFM;
            _pcrRadio.PcrFilter = PcrDef.PCRFLTR15;
            _pcrRadio.PcrToneSq = "";
            _pcrRadio.PcrToneSqFloat = 0.0f;
            _pcrRadio.PcrAutoGain = false;
            _pcrRadio.PcrNoiseBlank = false;
            _pcrRadio.PcrRfAttenuator = false;
            _pcrRadio.PcrAutoUpdate = false;
            _pcrStatus = false;
        }

        /// <summary>
        ///     Internally called method to check radio response.
        ///     Read from the radio for the #PCRAOK and #PCRABAD reply.
        /// </summary>
        /// <param name="response">The response to check.</param>
        /// <param name="overrideAutoupdate">Trys to verify response during autoupdate mode.</param>
        /// <returns>
        ///     true - for PCRAOK, false - for PCRABAD, false -
        ///     and sets ErrRead to true if garbage was read.
        ///     If autoupdate mode is enabled will return true
        ///     without overrideAutoupdate enabled.
        /// </returns>
        private bool PcrCheckResponse(string response, bool overrideAutoupdate = false)
        {
            Debug.WriteLine("PcrControl PcrCheckResponse");
            if (!overrideAutoupdate && _pcrComm.AutoUpdate) return true;

            if (response == PcrDef.PCRAOK || response == PcrDef.PCRBOK)
            {
                _pcrErrRead = false;
                return true;
            }
            if (response == PcrDef.PCRABAD)
            {
                _pcrErrRead = false;
                return false;
            }
            _pcrErrRead = true;
            return false;
        }

        /// <summary>
        ///     Get current session's autogain value.
        ///     Checks #PcrRadio struct for member #PcrAutoGain
        ///     for the current auto-gain setting.
        /// </summary>
        /// <returns>
        /// The boolean of the current setting. True/false :: On/off.
        /// </returns>
        public bool PcrGetAutoGain()
        {
            Debug.WriteLine("PcrControl PcrGetAutoGain");
            return _pcrRadio.PcrAutoGain;
        }

        /// <summary>
        ///     Get current session's autogain value.
        /// </summary>
        /// <returns></returns>
        public string PcrGetAutoGainStr()
        {
            Debug.WriteLine("PcrControl GetAutoGainStr");
            return PcrGetAutoGain() ? "1" : "0";
        }

        /// <summary>
        ///     Get the current session's filter setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetFilter()
        {
            Debug.WriteLine("PcrControl PcrGetFilter");
            return _pcrRadio.PcrFilter;
        }

        /// <summary>
        ///     Get the current session's filter setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetFilterStr()
        {
            Debug.WriteLine("PcrControl PcrGetFilterStr");
            if (PcrDef.PCRFLTR230 == _pcrRadio.PcrFilter)
            {
                return "230";
            }

            if (PcrDef.PCRFLTR50 == _pcrRadio.PcrFilter)
            {
                return "50";
            }

            if (PcrDef.PCRFLTR15 == _pcrRadio.PcrFilter)
            {
                return "15";
            }

            if (PcrDef.PCRFLTR6 == _pcrRadio.PcrFilter)
            {
                return "6";
            }

            if (PcrDef.PCRFLTR3 == _pcrRadio.PcrFilter)
            {
                return "3";
            }

            return _pcrRadio.PcrFilter;
        }

        /// <summary>
        ///     Gets current session's frequency setting.
        /// </summary>
        /// <returns></returns>
        public ulong PcrGetFreq()
        {
            Debug.WriteLine("PcrControl PcrGetFreq");
            return _pcrRadio.PcrFreq;
        }

        /// <summary>
        ///     Gets current session's frequency setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetFreqStr()
        {
            Debug.WriteLine("PcrControl PcrGetFreqStr");
            return _pcrRadio.PcrFreq.ToString("0000000000");
        }

        /// <summary>
        ///     Gets current session's mode setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetMode()
        {
            Debug.WriteLine("PcrControl PcrGetMode");
            return _pcrRadio.PcrMode;
        }

        /// <summary>
        ///     Gets current session's mode setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetModeStr()
        {
            Debug.WriteLine("PcrControl PcrGetModeStr");
            if (PcrDef.PCRMODWFM == _pcrRadio.PcrMode)
            {
                return "WFM";
            }

            if (PcrDef.PCRMODNFM == _pcrRadio.PcrMode)
            {
                return "NFM";
            }

            if (PcrDef.PCRMODCW == _pcrRadio.PcrMode)
            {
                return "CW";
            }

            if (PcrDef.PCRMODAM == _pcrRadio.PcrMode)
            {
                return "AM";
            }

            if (PcrDef.PCRMODUSB == _pcrRadio.PcrMode)
            {
                return "USB";
            }

            if (PcrDef.PCRMODLSB == _pcrRadio.PcrMode)
            {
                return "LSB";
            }

            return "UNKNOWN";
        }

        /// <summary>
        ///     Get current session's noiseblank value.
        /// </summary>
        /// <returns></returns>
        public bool PcrGetNb()
        {
            Debug.WriteLine("PcrControl PcrGetNb");
            return _pcrRadio.PcrNoiseBlank;
        }

        /// <summary>
        ///     Get current session's noiseblank value.
        /// </summary>
        /// <returns></returns>
        public string PcrGetNbStr()
        {
            Debug.WriteLine("PcrControl PcrGetNbStr");
            return PcrGetNb() ? "1" : "0";
        }

        /// <summary>
        ///     Gets current port / serial device setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetPort()
        {
            Debug.WriteLine("PcrControl PcrGetPort");
            return _pcrRadio.PcrPort;
        }

        /// <summary>
        ///     Retrieves the current radio struct.
        /// </summary>
        /// <returns></returns>
        public PRadInf PcrGetRadioInfo()
        {
            Debug.WriteLine("PcrControl PcrGetRadioInfo");
            return _pcrRadio;
        }

        /// <summary>
        ///     Sets the radio structure and values then updates the radio to reflect them.
        ///     PcrSpeed and PcrPort are currently ignored due to implementation bugs.
        /// </summary>
        /// <param name="radioInf">New radio structure.</param>
        public void PcrSetRadioInfo(PRadInf radioInf)
        {
            Debug.WriteLine("PcrControl PcrSetRadioInfo");
            PcrSetAutoupdate(radioInf.PcrAutoUpdate);
            PcrSetAutoGain(radioInf.PcrAutoGain);
            PcrSetNb(radioInf.PcrNoiseBlank);
            // radioInf.PcrPort; - TODO: Fix Buggy Implementation
            PcrSetRfAttenuator(radioInf.PcrRfAttenuator);
            // PcrSetSpeed(radioInf.PcrSpeed); - TODO: Fix Buggy implementation
            // radioInf.PcrInitSpeed; - same as above
            _pcrRadio.PcrMode = radioInf.PcrMode;
            _pcrRadio.PcrFilter = radioInf.PcrFilter;
            PcrSetFreq(radioInf.PcrFreq);
            PcrSetSquelch(radioInf.PcrSquelch);
            PcrSetToneSq(radioInf.PcrToneSqFloat);
            PcrSetVolume(radioInf.PcrVolume);
        }

        /// <summary>
        ///     Get current session's RF Attenuation value.
        /// </summary>
        /// <returns></returns>
        public bool PcrGetRfAttenuator()
        {
            Debug.WriteLine("PcrControl PcrGetRfAttenuator");
            return _pcrRadio.PcrRfAttenuator;
        }

        /// <summary>
        ///     Get current session's RF Attenuation value.
        /// </summary>
        /// <returns></returns>
        public string PcrGetRfAttenuatorStr()
        {
            Debug.WriteLine("PcrControl PcrGetRfAttenuatorStr");
            return PcrGetRfAttenuator() ? "1" : "0";
        }

        /// <summary>
        ///     Gets current speed.
        /// </summary>
        /// <returns></returns>
        public int PcrGetSpeedT()
        {
            Debug.WriteLine("PcrControl PcrGetSpeedT");
            return _pcrRadio.PcrSpeed;
        }

        /// <summary>
        ///     Gets current speed.
        /// </summary>
        /// <returns></returns>
        public string PcrGetSpeed()
        {
            Debug.WriteLine("PcrControl PcrGetSpeed");
            switch (_pcrRadio.PcrSpeed)
            {
                case 300:
                case 600:
                case 1200:
                case 1800:
                case 2400:
                case 4800:
                case 9600:
                case 19200:
                case 38400:
                case 57600:
                    return _pcrRadio.PcrSpeed.ToString(CultureInfo.InvariantCulture);
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        ///     Gets current session's squelch setting.
        /// </summary>
        /// <returns></returns>
        public int PcrGetSquelch()
        {
            Debug.WriteLine("PcrControl PcrGetSquelch");
            return _pcrRadio.PcrSquelch;
        }

        /// <summary>
        ///     Gets current session's squelch setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetSquelchStr()
        {
            Debug.WriteLine("PcrControl PcrGetSquelchStr");
            return _pcrRadio.PcrSquelch.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Gets the current session's tone squelch (undecoded version).
        /// </summary>
        /// <returns></returns>
        public string PcrGetToneSq()
        {
            Debug.WriteLine("PcrControl PcrGetToneSq");
            return _pcrRadio.PcrToneSq;
        }

        /// <summary>
        ///     Gets the current session's tone squelch (decoded version).
        /// </summary>
        /// <returns></returns>
        public string PcrGetToneSqStr()
        {
            Debug.WriteLine("PcrControl PcrGetToneSqStr");
            return _pcrRadio.PcrToneSqFloat.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Gets current session's volume setting.
        /// </summary>
        /// <returns></returns>
        public int PcrGetVolume()
        {
            Debug.WriteLine("PcrControl PcrGetVolume");
            return _pcrRadio.PcrVolume;
        }

        /// <summary>
        ///     Gets current session's volume setting.
        /// </summary>
        /// <returns></returns>
        public string PcrGetVolumeStr()
        {
            Debug.WriteLine("PcrControl PcrGetVolumeStr");
            return PcrGetVolume().ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Initialise the radio.
        /// </summary>
        /// <param name="autoUpdate">Initialise the radio in autoUpdate mode</param>
        /// <returns>On success : true otherwise false.</returns>
        public bool PcrInit(bool autoUpdate = false)
        {
            Debug.WriteLine("PcrControl PcrInit");
            if (!autoUpdate)
            {
                Debug.WriteLine("Radio is coming up. Please wait...\n");

                if (!_pcrStatus) return false;
                if (!PcrCheckResponse(_pcrComm.SendWait(PcrDef.PCRINITA))) return false;
                _pcrRadio.PcrAutoUpdate = false;
                _pcrComm.AutoUpdate = false;
                return true;
            }

            if (!_pcrStatus) return false;
            if (!PcrCheckResponse(_pcrComm.SendWait(PcrDef.PCRINITA))) return false;
            _pcrRadio.PcrAutoUpdate = true;
            _pcrComm.AutoUpdate = true;
            return true;
        }

        /// <summary>
        ///     Inquire radio status.
        /// </summary>
        /// <returns></returns>
        public bool PcrIsOn()
        {
            Debug.WriteLine("PcrControl PcrIsOn");
            return _pcrStatus;
        }

        /// <summary>
        ///     Powers the radio down.
        /// </summary>
        /// <returns></returns>
        public bool PcrPowerDown()
        {
            Debug.WriteLine("PcrControl PcrPowerDown");
            // if (PcrCheckResponse()) {
            PcrCheckResponse(_pcrComm.SendWait(PcrDef.PCRPWROFF));
            _pcrStatus = false;
            return true;
        }

        /// <summary>
        ///     Powers the radio on.
        /// </summary>
        /// <returns></returns>
        public bool PcrPowerUp()
        {
            Debug.WriteLine("PcrControl PcrPowerUp");
            if (!PcrCheckResponse(_pcrComm.SendWait(PcrDef.PCRPWRON))) return false;
            //_pcrComm.SendWait("G301");
            //	PcrCheckResponse();
            _pcrStatus = true;
            return true;
        }

        /// <summary>
        ///     Querys radio acutator status.
        /// </summary>
        /// <returns></returns>
        public bool PcrQueryOn()
        {
            Debug.WriteLine("PcrControl PcrQueryOn");
            const string mesg = "H1";
            var temp = _pcrComm.SendWait(mesg);
            if (temp == "") return false;
            return temp == "H101";
        }

        /// <summary>
        ///     Querys radio's squelch status.
        /// </summary>
        /// <returns></returns>
        public bool PcrQuerySquelch()
        {
            Debug.WriteLine("PcrControl PcrQuerySquelch");
            var tempvar1 = PcrDef.PCRASQL + PcrDef.PCRASQLCL;
            var temp = _pcrComm.SendWait(PcrDef.PCRQSQL);
            if (temp == "") return false;
            return temp == tempvar1;
        }

        /// <summary>
        ///     Toggle autogain functionality.
        /// </summary>
        /// <param name="value"></param>
        public bool PcrSetAutoGain(bool value)
        {
            Debug.WriteLine("PcrControl PcrSetAutoGain");
            if (!PcrCheckResponse(_pcrComm.SendWait(value ? PcrDef.PCRAGCON : PcrDef.PCRAGCOFF))) return false;
            _pcrRadio.PcrAutoGain = value;
            return true;
        }

        /// <summary>
        ///     Sets current session's filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool PcrSetFilter(string filter)
        {
            Debug.WriteLine("Setting PcrRadio.PcrFilter");
            switch (filter)
            {
                case "3":
                    _pcrRadio.PcrFilter = PcrDef.PCRFLTR3; break;
                case "6":
                    _pcrRadio.PcrFilter = PcrDef.PCRFLTR6; break;
                case "15":
                    _pcrRadio.PcrFilter = PcrDef.PCRFLTR15; break;
                case "50":
                    _pcrRadio.PcrFilter = PcrDef.PCRFLTR50; break;
                case "230":
                    _pcrRadio.PcrFilter = PcrDef.PCRFLTR230; break;
                default:
                    return false;
            }
            
            var temp = PcrDef.PCRFRQ + _pcrRadio.PcrFreq.ToString("0000000000") + _pcrRadio.PcrMode + _pcrRadio.PcrFilter + "00";
            return PcrCheckResponse(_pcrComm.SendWait(temp));
        }

        /// <summary>
        ///     Sets if autoupdate is enabled.
        /// </summary>
        /// <param name="autoupdate"></param>
        public void PcrSetAutoupdate(bool autoupdate)
        {
            if (PcrCheckResponse(_pcrComm.SendWait(autoupdate ? PcrDef.PCRSIGON : PcrDef.PCRSIGOFF)))
            {
                _pcrComm.AutoUpdate = _pcrRadio.PcrAutoUpdate = autoupdate;
            }
        }

        /// <summary>
        ///     Sets current session's filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool PcrSetFilter(int filter)
        {
            Debug.WriteLine("PcrControl PcrSetFilter");
            return PcrSetFilter(filter.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Set the current frequency.
        /// </summary>
        /// <param name="freq"></param>
        /// <returns></returns>
        public bool PcrSetFreq(ulong freq)
        {
            Debug.WriteLine("PcrControl PcrSetFreq");
            if ((PcrDef.LOWERFRQ <= freq) && (freq <= PcrDef.UPPERFRQ))
            {
                var freqConv = freq.ToString("0000000000");
                var temp = PcrDef.PCRFRQ + freqConv + _pcrRadio.PcrMode + _pcrRadio.PcrFilter + "00";
                var resp = _pcrComm.SendWait(temp);
                if (PcrCheckResponse(resp))
                {
                    _pcrRadio.PcrFreq = freq;
                    Debug.WriteLine("PcrControl PcrSetFreq - Success");
                    return true;
                }
            }
            Debug.WriteLine("PcrControl PcrSetFreq - Failed");
            return false;
        }

        /// <summary>
        ///     Set the current session's mode.
        ///     Valid arguments for \a mode:
        ///     - USB	upper side band
        ///     - LSB	lower side band
        ///     - AM	amplitude modulated
        ///     - NFM	narrow band FM
        ///     - WFM	wide band FM
        ///     - CW	continuous wave
        ///     The concept is the same as above ( #PcrSetFreq ) except it accepts
        ///     standard text for "USB"/"LSB" etc... Use of the pcrdef codes
        ///     are not necessary, they will be decoded based on \a mode.
        /// </summary>
        /// <param name="mode">Plain text string of mode (eg: "USB")</param>
        /// <returns>True or false based on success or failure.</returns>
        public bool PcrSetMode(string mode)
        {
            Debug.WriteLine("Setting PcrRadio.PcrMode");
            mode = mode.ToLower();

            switch (mode)
            {
                case "am":
                    _pcrRadio.PcrMode = PcrDef.PCRMODAM; break;
                case "cw":
                    _pcrRadio.PcrMode = PcrDef.PCRMODCW; break;
                case "lsb":
                    _pcrRadio.PcrMode = PcrDef.PCRMODLSB; break;
                case "usb":
                    _pcrRadio.PcrMode = PcrDef.PCRMODUSB; break;
                case "nfm":
                    _pcrRadio.PcrMode = PcrDef.PCRMODNFM; break;
                case "wfm":
                    _pcrRadio.PcrMode = PcrDef.PCRMODWFM; break;
                default:
                    return false;
            }

            var temp = PcrDef.PCRFRQ + _pcrRadio.PcrFreq.ToString("0000000000") + _pcrRadio.PcrMode + _pcrRadio.PcrFilter + "00";
            return PcrCheckResponse(_pcrComm.SendWait(temp));
        }

        /// <summary>
        ///     Toggle Noiseblanking functionality.
        ///     Valid values for \a value are:
        ///     - true to activate noiseblanking
        ///     - false to deactivate noiseblanking
        ///     Sets the noise blanking to \a value
        ///     (on/off) true/false... checks the radio response
        ///     if ok, then sets the value
        /// </summary>
        /// <param name="value">Value true or false for noiseblanking on or off</param>
        /// <returns>
        /// True, on success otherwise returns false.
        /// </returns>
        public bool PcrSetNb(bool value)
        {
            Debug.WriteLine("PcrControl PcrSetNb");
            if (!PcrCheckResponse(_pcrComm.SendWait(value ? PcrDef.PCRNBON : PcrDef.PCRNBOFF))) return false;
            _pcrRadio.PcrNoiseBlank = value;
            return true;
        }

        /// <summary>
        ///     Set the port for the current session.
        ///     Sets port by closing the filedes and reopening it
        ///     on the new port.
        /// </summary>
        /// <param name="port">The port</param>
        /// <returns>
        /// True or false if the serial device can be opened on the new port.
        /// </returns>
        public bool PcrSetPort(string port)
        {
            Debug.WriteLine("PcrControl PcrSetPort");
            _pcrComm.PcrClose();
            try
            {
                _pcrComm = new PcrComm(port);
                _pcrComm.PcrOpen();
                _pcrComm.AutoUpdate = _pcrRadio.PcrAutoUpdate;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Toggle RF Attenuation functionality.
        ///     Valid values for \a value are:
        ///     - true to activate RF Attenuation
        ///     - false to deactivate RF Attenuation
        ///     Sets the RF Attenuation to \a value
        ///     (on/off) true/false... checks the radio response
        ///     if ok, then sets the value
        /// </summary>
        /// <param name="value">Value true or false for RF Attenuation on or off</param>
        /// <returns>
        /// True, on success otherwise returns false.
        /// </returns>
        public bool PcrSetRfAttenuator(bool value)
        {
            Debug.WriteLine("PcrControl PcrSetRfAttenuator");
            if (!PcrCheckResponse(_pcrComm.SendWait(value ? PcrDef.PCRRFAON : PcrDef.PCRRFAOFF))) return false;
            _pcrRadio.PcrRfAttenuator = value;
            return true;
        }

        /// <summary>
        ///     Sets the speed for current session.
        ///     First we check to see if the baudrate passed 
        ///     in \a speed is right, if not then we just quietly 
        ///     return false. Then we decode \a speed and set 
        ///     #PcrInitSpeed to #pcrcmd_t version.
        ///     Then we tell the radio to switch speeds and
        ///     set baudrate on the port by destroying PComm
        ///     and reinitiating it with the new baud setting
        ///     Warning: follow these procedures to use this function.
        ///     -create the object (at last known baudrate).
        ///     -call init
        ///     -call power up
        ///     -call this function
        ///     -delete the object
        ///     -create the object with the new speed setting
        /// </summary>
        /// <param name="speed">Speed baudrate.</param>
        /// <returns>True or false based on success.</returns>
        public bool PcrSetSpeed(int speed)
        {
            Debug.WriteLine("PcrControl PcrSetSpeed");
            if ((300 > speed) || (speed > 38400)) return false;
            switch (speed)
            {
                case 38400:
                    // you probably want to set the speed
                    // to fastest available, so let's put
                    // this here first
                    _pcrRadio.PcrInitSpeed = PcrDef.PCRBD38400;
                    break;
                case 19200:
                    _pcrRadio.PcrInitSpeed = PcrDef.PCRBD19200;
                    break;
                case 300:
                    _pcrRadio.PcrInitSpeed = PcrDef.PCRBD300;
                    break;
                case 1200:
                    _pcrRadio.PcrInitSpeed = PcrDef.PCRBD1200;
                    break;
                case 2400:
                    _pcrRadio.PcrInitSpeed = PcrDef.PCRBD2400;
                    break;
                default:
                    // if all else fails, we'll always
                    // have paris! ~=^)
                    _pcrRadio.PcrInitSpeed = PcrDef.PCRBD9600;
                    break;
            }
            _pcrComm.Send(_pcrRadio.PcrInitSpeed);
            _pcrComm.PcrClose();
            _pcrComm = new PcrComm(_pcrRadio.PcrPort, speed);
            // investigate possible responses, i dont think one is given.
            // PcrCheckResponse();
            _pcrRadio.PcrSpeed = speed;
            return true;
        }

        /// <summary>
        ///     Set the current session's squelch.
        ///     sprintf converts (and combines) the cmd #PCRSQL with
        ///     the argument \a squelch , such that the argument has a 
        ///     minimum field width of two chars. If the field 
        ///     is less than 2 chars (ie: arg=5) then it pads the field 
        ///     with one zero.
        /// </summary>
        /// <param name="squelch">an integer between 0 and 100</param>
        /// <returns>
        /// true or false based on #PcrCheckResponse to indicate
        /// success or failure
        /// </returns>
        public bool PcrSetSquelch(int squelch)
        {
            Debug.WriteLine("PcrControl PcrSetSquelch");
            if ((0 > squelch) || (squelch > 100)) return false;
            squelch = (int)((256.0 / 100.0) * squelch);
            var temp = PcrDef.PCRSQL + squelch.ToString("X2");
            if (!PcrCheckResponse(_pcrComm.SendWait(temp))) return false;
            _pcrRadio.PcrSquelch = squelch;
            return true;
        }

        /// <summary>
        ///     Sets current session CTCSS.
        ///     set's the tone squelch for the radio. The default is
        ///     value 00 for off. The values are \b NOT the hz, but the
        ///     #pcrdef.h vals, 01=67.0 02=69.3 etc... 
        /// </summary>
        /// <param name="value">character string of 01-35 hex</param>
        /// <returns>
        /// true or false based on #PcrCheckResponse success or failure.
        /// </returns>
        public bool PcrSetToneSq(string value)
        {
            Debug.WriteLine("PcrControl PcrSetTonSq");
            var temp = PcrDef.PCRTSQL + value;
            if (!PcrCheckResponse(_pcrComm.SendWait(temp))) return false;
            _pcrRadio.PcrToneSq = value;
            return true;
        }

        /// <summary>
        ///     Sets session CTCSS based on a float value.
        ///     Since the previous method requires the programmer to
        ///     remember the PCR-1000's internal number that corresponds
        ///     to the tone squelch frequency, this overloaded method
        ///     allows the programmer to pass a float, where the float
        ///     is the frequency (Hz) in question.
        /// </summary>
        /// <param name="passValue">tone squelch in Hz</param>
        /// <returns>
        /// true or false based on #PcrCheckResponse 
        /// success or failure. On failure, it turns off CTCSS
        /// and returns false.
        /// </returns>
        public bool PcrSetToneSq(float passValue)
        {
            Debug.WriteLine("PcrControl PcrSetToneSq");

            var tone = (int) (passValue*10.0 + .1);
            _pcrRadio.PcrToneSqFloat = passValue;

            switch (tone)
            {
                case 0:
                    return PcrSetToneSq("00");
                case 670:
                    return PcrSetToneSq("01");
                case 693:
                    return PcrSetToneSq("02");
                case 710:
                    return PcrSetToneSq("03");
                case 719:
                    return PcrSetToneSq("04");
                case 744:
                    return PcrSetToneSq("05");
                case 770:
                    return PcrSetToneSq("06");
                case 797:
                    return PcrSetToneSq("07");
                case 825:
                    return PcrSetToneSq("08");
                case 854:
                    return PcrSetToneSq("09");
                case 885:
                    return PcrSetToneSq("0A");
                case 915:
                    return PcrSetToneSq("0B");
                case 948:
                    return PcrSetToneSq("0C");
                case 974:
                    return PcrSetToneSq("0D");
                case 1000:
                    return PcrSetToneSq("0E");
                case 1035:
                    return PcrSetToneSq("0F");
                case 1072:
                    return PcrSetToneSq("10");
                case 1109:
                    return PcrSetToneSq("11");
                case 1148:
                    return PcrSetToneSq("12");
                case 1188:
                    return PcrSetToneSq("13");
                case 1230:
                    return PcrSetToneSq("14");
                case 1273:
                    return PcrSetToneSq("15");
                case 1318:
                    return PcrSetToneSq("16");
                case 1365:
                    return PcrSetToneSq("17");
                case 1413:
                    return PcrSetToneSq("18");
                case 1462:
                    return PcrSetToneSq("19");
                case 1514:
                    return PcrSetToneSq("1A");
                case 1567:
                    return PcrSetToneSq("1B");
                case 1598:
                    return PcrSetToneSq("1C");
                case 1622:
                    return PcrSetToneSq("1D");
                case 1655:
                    return PcrSetToneSq("1E");
                case 1679:
                    return PcrSetToneSq("1F");
                case 1713:
                    return PcrSetToneSq("20");
                case 1738:
                    return PcrSetToneSq("21");
                case 1773:
                    return PcrSetToneSq("22");
                case 1799:
                    return PcrSetToneSq("23");
                case 1835:
                    return PcrSetToneSq("24");
                case 1862:
                    return PcrSetToneSq("25");
                case 1899:
                    return PcrSetToneSq("26");
                case 1928:
                    return PcrSetToneSq("27");
                case 1966:
                    return PcrSetToneSq("28");
                case 1995:
                    return PcrSetToneSq("29");
                case 2035:
                    return PcrSetToneSq("2A");
                case 2065:
                    return PcrSetToneSq("2B");
                case 2107:
                    return PcrSetToneSq("2C");
                case 2181:
                    return PcrSetToneSq("2D");
                case 2257:
                    return PcrSetToneSq("2E");
                case 2291:
                    return PcrSetToneSq("2F");
                case 2336:
                    return PcrSetToneSq("30");
                case 2418:
                    return PcrSetToneSq("31");
                case 2503:
                    return PcrSetToneSq("32");
                case 2541:
                    return PcrSetToneSq("33");
                default:
                    PcrSetToneSq("00");
                    break;
            }
            return false;
        }

        /// <summary>
        ///     Set the current session's volume.
        ///     sprintf converts (and combines) the cmd #PCRVOL with
        ///     the argument, such that the argument has a minimum field width
        ///     of two chars. If the field is less than 2 chars (ie: arg=5) then it
        ///     pads the field with one zero.
        /// </summary>
        /// <param name="volume">Volume an integer between 0 and 100</param>
        /// <returns>
        /// true or false based on #PcrCheckResponse to indicate success or failure
        /// </returns>
        public bool PcrSetVolume(int volume)
        {
            Debug.WriteLine("PcrControl PcrSetVolume");
            if ((0 > volume) || (volume > 100)) return false;
            volume = (int)((256.0 / 100.0) * volume);
            var temp = PcrDef.PCRVOL + volume.ToString("X2");
            if (!PcrCheckResponse(_pcrComm.SendWait(temp))) return false;
            _pcrRadio.PcrVolume = volume;
            return true;
        }

        /// <summary>
        ///     Querys the signal strength.
        /// </summary>
        /// <returns>integer value of 0-255 on signal strength.</returns>
        public int PcrSigStrength()
        {
            Debug.WriteLine("PcrControl PcrSigStrength");

            int sigstr;
            var temp = _pcrComm.SendWait(PcrDef.PCRQRST);
            if (temp == "") return 0;
            var digit = temp[2];
            if ((digit >= 'A') && (digit <= 'F'))
            {
                sigstr = (digit - 'A' + 1)*16;
            }
            else
            {
                sigstr = int.Parse(digit.ToString(CultureInfo.InvariantCulture))*16;
            }

            digit = temp[3];
            if ((digit >= 'A') && (digit <= 'F'))
            {
                sigstr += digit - 'A' + 1;
            }
            else
            {
                sigstr += int.Parse(digit.ToString(CultureInfo.InvariantCulture));
            }
            return sigstr;
        }

        /// <summary>
        ///     Querys the signal strength.
        /// </summary>
        /// <returns>
        /// null on failure, otherwise a character string
        /// with the current signal strenth. This includes the I1
        /// header, plus the last two characters which is the
        /// \b hex value from \a 00-99
        /// </returns>
        public string PcrSigStrengthStr()
        {
            Debug.WriteLine("PcrControl PcrSigStrengthStr");
            var sigstr = _pcrComm.SendWait(PcrDef.PCRQRST);
            return sigstr == "" ? null : sigstr;
        }

        /// <summary>
        ///     Stores the important radio information for the current
        ///     state of the radio.
        /// </summary>
        public struct PRadInf
        {
            /// <summary>
            ///     Currenly set autogain
            /// </summary>
            public bool PcrAutoGain;

            /// <summary>
            ///     Currently set update mode?
            /// </summary>
            public bool PcrAutoUpdate;

            /// <summary>
            ///     Currently set radio Filter [128]
            /// </summary>
            public string PcrFilter; //[128];

            /// <summary>
            ///     Currently set frequency
            /// </summary>
            public ulong PcrFreq;

            /// <summary>
            ///     Currently set speed (char * version, unstable) [8]
            /// </summary>
            public string PcrInitSpeed; //[8];

            /// <summary>
            ///     Currently set radio Mode [128]
            /// </summary>
            public string PcrMode; //[128];

            /// <summary>
            ///     Currently set noiseblanking
            /// </summary>
            public bool PcrNoiseBlank;

            /// <summary>
            ///     Currently active port/device [64]
            /// </summary>
            public string PcrPort; // = new char[64];

            /// <summary>
            ///     Currently set RF Attenuation
            /// </summary>
            public bool PcrRfAttenuator;

            /// <summary>
            ///     Currently set speed (uint var)
            /// </summary>
            public int PcrSpeed;

            /// <summary>
            ///     Currently set squlech
            /// </summary>
            public int PcrSquelch;

            /// <summary>
            ///     Currently set CTCSS (unstable)
            /// </summary>
            public string PcrToneSq;

            /// <summary>
            ///     Currently set CTCSS (float)
            /// </summary>
            public float PcrToneSqFloat;

            /// <summary>
            ///     Currently set volume
            /// </summary>
            public int PcrVolume;
        }

#if DEBUG
        /// <summary>
        /// Writes Raw COM Port IO to the Console
        /// </summary>
        /// <param name="log">Enable/Disable</param>
        public void SetComDebugLogging(bool log)
        {
            Debug.WriteLine("PcrControl SetComDebugLogging");
            _pcrComm.SetDebugLogger(log);
        }

        /// <summary>
        /// Sends a raw string to the port
        /// </summary>
        /// <param name="raw">The raw string to send</param>
        public void DebugRawSend(string raw)
        {
            _pcrComm.Send(raw);
        }

        /// <summary>
        /// Sends a raw string to the socket and waits for a response
        /// </summary>
        /// <param name="raw">The raw string to send</param>
        /// <returns>The port response</returns>
        public string DebugRawSendWait(string raw)
        {
            return _pcrComm.SendWait(raw);
        }
#endif
    }
}