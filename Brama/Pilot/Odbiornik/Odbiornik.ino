#include <SPI.h>   
#include <RF24.h>  

#define pauza 100

RF24 radio (9, 10); 

byte addresses[6] = {"degtf"};

unsigned short int data;

void send(unsigned short int data)
{
    radio.stopListening();
    if(!radio.write( &data, sizeof(data) ))
    {
      //Serial.println("blad");
    }
    radio.startListening();
}

void setup()
{
  Serial.begin(115200);

  if(!radio.begin())
  {
    Serial.println(".begin zwraca blad");
  }
  
  radio.setChannel(108); 
  
  radio.setPALevel(RF24_PA_MAX);

  radio.openWritingPipe(addresses); // Use the first entry in array 'addresses' (Only 1 right now)

  radio.openReadingPipe(1, addresses); // Use the first entry in array 'addresses' (Only 1 right now)
 
  radio.startListening();

  

}

void loop()
{ 

 if ( radio.available())
  {
      radio.read( &data, sizeof(data) );
      Serial.println(data);
      delay(100);
      send(10);

   }

   
}

