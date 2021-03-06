#include <ESP8266WiFi.h>

//how many clients should be able to telnet to this ESP8266
#define MAX_SRV_CLIENTS 5


WiFiServer server(80);
WiFiClient serverClients[MAX_SRV_CLIENTS];

void setup() 
{
	Serial.begin(57600);
	WiFi.begin("Home_WiFi", "rb26dett");
}

void loop() 
{
	while (WiFi.status() != WL_CONNECTED)
	{
		delay(1);
		if (WiFi.status() == WL_CONNECTED)
		{
			server.begin();
			server.setNoDelay(true);
			Serial.println("ready");
			//start UART and the server
		}
	}



	uint8_t i;
	//check if there are any new clients
	if (server.hasClient()) 
	{
		Serial.println("u");
		for (i = 0; i < MAX_SRV_CLIENTS; i++) 
		{
			//find free/disconnected spot
			if (!serverClients[i] || !serverClients[i].connected())
			{
				if (serverClients[i]) serverClients[i].stop();
				serverClients[i] = server.available();				
				continue;
			}
		}
		//no free/disconnected spot so reject
		WiFiClient serverClient = server.available();
		serverClient.stop();
	}
	//check clients for data
	for (i = 0; i < MAX_SRV_CLIENTS; i++) 
	{
		if (serverClients[i] && serverClients[i].connected()) 
		{
			if (serverClients[i].available())
			{
				//get data from the telnet client and push it to the UART
				while (serverClients[i].available()) Serial.write(serverClients[i].read());
			}
		}
	}
	//check UART for data
	if (Serial.available()) 
	{
		size_t len = Serial.available();
		uint8_t sbuf[len];
		Serial.readBytes(sbuf, len);
		//push UART data to all connected telnet clients
		for (i = 0; i < MAX_SRV_CLIENTS; i++) 
		{
			if (serverClients[i] && serverClients[i].connected())
			{
				serverClients[i].write(sbuf, len);
				delay(1);
			}
		}
	}
}