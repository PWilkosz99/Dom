#include <ESP8266WiFi.h>
#include <OneWire.h>
#include <DallasTemperature.h>

#define Interval 1000  //refreshing time

//how many clients should be able to telnet to this ESP8266
#define MAX_SRV_CLIENTS 5


WiFiServer server(80);
WiFiClient serverClients[MAX_SRV_CLIENTS];

OneWire oneWire(2); //Podłączenie do GPIO2
DallasTemperature sensors(&oneWire); //Przekazania informacji do biblioteki

unsigned long lczas; // czas osatniego wysłania info

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

 unsigned long czas = millis();

 if(czas - lczas > Interval)
{
  lczas = czas;
  sensors.requestTemperatures();
  byte value = sensors.getTempFByIndex(0);
  Serial.println(sensors.getTempFByIndex(0));
  Write(value);
}
   
}

void Write(uint8_t temp)
{
  uint8_t sbuf[1];
  sbuf[0]= temp;
  for (int i = 0; i < MAX_SRV_CLIENTS; i++) 
    {
      if (serverClients[i] && serverClients[i].connected())
      {
        serverClients[i].write(sbuf, 1);
        delay(1);
      }
    }
  
}

