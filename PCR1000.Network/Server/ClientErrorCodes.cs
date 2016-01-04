// ReSharper disable InconsistentNaming
namespace PCR1000.Network.Server
{
    internal enum ClientErrorCode
    {
        // Client Hello
        ERR_HELLO_NOTFOUND,
        ERR_HELLO_INVALID,
        WAR_HELLO_UNKNOWN,
        ERR_PROTOVER_TOOOLD,
        SUC_HELLO_PASSED,

        // Auth
        ERR_AUTH_NOTFOUND,
        ERR_AUTH_INVALID,
        ERR_AUTH_INCORRECT,
        SUC_AUTH_PASSED,

        // Other
        SUC_ECHO_RESPONSE,
        SUC_HASCONTROL_RESPONSE,
        SUC_TAKECONTROL_RESPONSE,
        WAR_COMMAND_UNKNOWN,
        INF_CLIENT_DISCONNECT,

        // Query
        ERR_QUERY_FAILED,
        ERR_HASCONTROL_RESPONSE
    }
}
