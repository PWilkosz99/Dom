#include <SPI.h>   
#include <RF24.h>  

#define pauza 50

RF24 radio (9, 10); 

byte addresses[6] = {"degtf"};


void setup()
{
	radio.begin();
	
	radio.setChannel(108);
	radio.setPALevel(RF24_PA_MAX);
	radio.openWritingPipe(addresses);
}

void send(unsigned short int data)
{
	radio.write(&data, sizeof(data));
}

void loop()
{ 
	send(3);
	delay(pauza);
}
