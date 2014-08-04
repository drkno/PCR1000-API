/*
 * IComm
 * Interface for serial communication component of the PCR1000 Library
 * 
 * Copyright Matthew Knox © 2013-Present.
 * This program is distributed with no warentee or garentee
 * what so ever. Do what you want with it as long as attribution
 * to the origional authour and this comment is provided at the
 * top of this source file and any derivative works. Also any
 * modifications must be in real Australian, New Zealand or
 * British English where the language allows.
 */

// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

using System;

namespace PCR1000
{
    /// <summary>
    /// In AutoUpdate mode data was received.
    /// </summary>
    /// <param name="sender">The IComm object that received the communication.</param>
    /// <param name="recvTime">The time which the communication was received.</param>
    /// <param name="data">The data received.</param>
    public delegate void AutoUpdateDataRecv(IComm sender, DateTime recvTime, string data);

    /// <summary>
    /// Communications interface. All PCR1000 communications should happen through
    /// classes derived from this interface.
    /// </summary>
    public interface IComm : IDisposable
    {
        /// <summary>
        /// Data received in autoupdate mode.
        /// </summary>
        event AutoUpdateDataRecv AutoUpdateDataReceived;

        /// <summary>
        /// Gets and sets autoupdate mode.
        /// </summary>
        bool AutoUpdate { get; set; }

        /// <summary>
        /// Gets the object refering to the raw communications port.
        /// </summary>
        /// <returns>The communications port object.</returns>
        object GetRawPort();

#if DEBUG
        /// <summary>
        /// Enables or disables debug logging in the comminication library.
        /// </summary>
        /// <param name="debug">Enable or disable.</param>
        void SetDebugLogger(bool debug);
#endif

        /// <summary>
        /// Sends a messsage to the PCR1000.
        /// </summary>
        /// <param name="cmd">The command to send.</param>
        /// <returns>If sending succeeded.</returns>
        bool Send(string cmd);

        /// <summary>
        /// Sends a message to the PCR1000 and waits for a reply.
        /// </summary>
        /// <param name="cmd">The command to send.</param>
        /// <param name="overrideAutoupdate">When in autoupdate mode behaves like Send()
        /// this overrides that behaviour.</param>
        /// <returns>The reply or "" if nothing is received.</returns>
        string SendWait(string cmd, bool overrideAutoupdate = false);

        /// <summary>
        /// Gets the latest message from the PCR1000.
        /// </summary>
        /// <returns>The latest message.</returns>
        string GetLastReceived();

        /// <summary>
        /// Gets the previously received message.
        /// </summary>
        /// <returns>The previous message.</returns>
        string GetPrevReceived();

        /// <summary>
        /// Opens the PCR1000 serial port.
        /// </summary>
        /// <returns>Success</returns>
        bool PcrOpen();

        /// <summary>
        /// Closes the PCR1000 serial port.
        /// </summary>
        /// <returns>Success</returns>
        bool PcrClose();
    }
}
