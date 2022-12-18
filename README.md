# BK_TCPMessanger
This is a stupidly bad communication app based on sockets that only supports text.
Application based on WFP.

# Requirements
To run this application you need to install .net 5.0 runtime.

# Usage
This appliaction separate on Server side and Client side.

1. Before starting the client, you need to connect to the server side in order to connect to it in the future.
By default, you have a TCPMessanger.Server.dll.config file on the server where you can configure the ip address and port.
After you configure this file you can start server.

2. Next you should open client to connect to the server.
Client is a GUI application where you can specify server ip with port and your username.

3. Server uses username as a unique value to identify client. This is why you cannot use the same username.
When you specify the server IP address, port and username for the client, you will be able to connect to the server.

4. After connecting to the server you can start write between other connected clients.
