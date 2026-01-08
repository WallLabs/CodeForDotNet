namespace CodeChief.Network;

/// <summary>
/// Contains helper methods and extensions for network operations.
/// </summary>
public static class WakeOnLan
{
    /// <summary>
    /// First port on which the Wake-On-LAN broadcast is normally sent.
    /// </summary>
    public const int WakeOnLanPort1 = 0;

    /// <summary>
    /// Second port on which the Wake-On-LAN broadcast is normally sent.
    /// </summary>
    public const int WakeOnLanPort2 = 7;

    /// <summary>
    /// Third port on which the Wake-On-LAN broadcast is normally sent.
    /// </summary>
    public const int WakeOnLanPort3 = 9;

    /// <summary>
    /// Size of a Wake-On-LAN "magic packet" in bytes.
    /// Contains a broadcast MAC (6 bytes, all 0xFF) followed by
    /// the target MAC address (6 bytes / 48 bits) repeatd 16 times.
    /// </summary>
    public const int WakeOnLanPacketSize = MacAddress.Size + (MacAddress.Size * WakeOnLanPacketTargetMacCount);

    /// <summary>
    /// Number of times the target MAC address is repeated in the packet, excluding the prefix.
    /// </summary>
    public const int WakeOnLanPacketTargetMacCount = 16;

    /// <summary>
    /// Broadcasts a Wake-On-LAN paket for the specified MAC address.
    /// </summary>
    public static void Wake(MacAddress mac)
    {
        // Create WOL packet
        var packet = CreateWakeOnLanPacket(mac);

        // Broadcast packet on all commonly used ports
        var ports = new[] { WakeOnLanPort1, WakeOnLanPort2, WakeOnLanPort3 };
        foreach (var port in ports)
            UdpExtensions.BroadcastPacket(packet, port);
    }

    /// <summary>
    /// Creates a Wake-On-LAN packet for the specified MAC address.
    /// </summary>
    public static byte[] CreateWakeOnLanPacket(MacAddress mac)
    {
        // Create packet buffer
        var packet = new byte[WakeOnLanPacketSize];

        // Add broadcast MAC address prefix
        var broadcastMacBytes = new MacAddress(MacAddress.Broadcast).ToByteArray();
        broadcastMacBytes.CopyTo(packet, 0);

        // Add the target MAC address many times
        var macBytes = mac.ToByteArray();
        for (var i = 0; i < WakeOnLanPacketTargetMacCount; i++)
            macBytes.CopyTo(packet, MacAddress.Size + (MacAddress.Size * i));

        // Return result
        return packet;
    }
}
