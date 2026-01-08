using System;
using System.Net;
using System.Net.Sockets;

namespace CodeChief.Network;

/// <summary>
/// Extensions for work with the UDP network protocol.
/// </summary>
public class UdpExtensions
{
    /// <summary>
    /// Sends a UDP (broadcast) packet to the specified mac address and port.
    /// </summary>
    public static void BroadcastPacket(byte[] packet, int port)
    {
        // Validate
        ArgumentNullException.ThrowIfNull(packet);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(port);

        // Initialize a UDP client for broadcast on the specified port
        using var client = new UdpClient();
        client.Connect(IPAddress.Broadcast, port);

        // Send packet
        client.Send(packet, packet.Length);
    }
}