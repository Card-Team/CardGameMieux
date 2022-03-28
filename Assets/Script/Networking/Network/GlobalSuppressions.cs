using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Stil", "IDE1006:Benennungsstile", Justification = "My Events are written lower case (° ͜ʖ͡°)╭∩╮",
        Scope = "member", Target = "~E:Network.ClientConnectionContainer.connectionLost")]
[assembly:
    SuppressMessage("Stil", "IDE1006:Benennungsstile", Justification = "My Events are written lower case (° ͜ʖ͡°)╭∩╮",
        Scope = "member", Target = "~E:Network.ClientConnectionContainer.connectionEstablished")]
[assembly:
    SuppressMessage("Stil", "IDE1006:Benennungsstile", Justification = "My Events are written lower case (° ͜ʖ͡°)╭∩╮",
        Scope = "member", Target = "~E:Network.ServerConnectionContainer.connectionEstablished")]
[assembly:
    SuppressMessage("Stil", "IDE1006:Benennungsstile", Justification = "My Events are written lower case (° ͜ʖ͡°)╭∩╮",
        Scope = "member", Target = "~E:Network.ServerConnectionContainer.connectionLost")]
[assembly:
    SuppressMessage("Stil", "IDE1006:Benennungsstile", Justification = "My Events are written lower case (° ͜ʖ͡°)╭∩╮",
        Scope = "member",
        Target =
            "~M:Network.ServerConnectionContainer.udpConnectionReceived(Network.TcpConnection,Network.UdpConnection)")]
[assembly:
    SuppressMessage("Stil", "IDE1006:Benennungsstile", Justification = "My Events are written lower case (° ͜ʖ͡°)╭∩╮",
        Scope = "member",
        Target = "~M:Network.ServerConnectionContainer.connectionClosed(Network.Enums.CloseReason,Network.Connection)")]