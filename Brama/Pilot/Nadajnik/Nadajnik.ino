#include <SPI.h>   
#include <RF24.h>  

#define pauza 100

RF24 radio (9, 10); 

byte addresses[6] = {"degtf"};

unsigned short int data;

bool mig[3];

unsigned long lasttime;

unsigned long lastanswerdtime = millis();



void setup()
{
  Serial.begin(115200);

  if(!radio.begin())
  {
    Serial.println(".begin zwraca blad");
  }
  else{
    Serial.println("OK");
  }
  
  radio.setChannel(108); 
  
  radio.setPALevel(RF24_PA_MAX);

  radio.openWritingPipe(addresses); // Use the first entry in array 'addresses' (Only 1 right now)

  radio.openReadingPipe(1, addresses); // Use the first entry in array 'addresses' (Only 1 right now)
 
  radio.startListening();

  pinMode(2, INPUT_PULLUP); // otworz 1
  pinMode(3, INPUT_PULLUP); // stop   2
  pinMode(4, INPUT_PULLUP); //zamknij 3

  pinMode(A3, OUTPUT); 
  pinMode(A4, OUTPUT); 
  pinMode(A5, OUTPUT);

  send(7); // zapytanie

}

void send(unsigned short int data)
{
    radio.stopListening();
    if(!radio.write( &data, sizeof(data) ))
    {
      Serial.println("blad");
    }
    radio.startListening();
}

void loop()
{ 
    if(digitalRead(2) == LOW)
    {
      send(1);
    }
    if(digitalRead(3) == LOW)
    {
      send(2);     
    }
    if(digitalRead(4) == LOW)
    {
      send(3);
    }

 if ( radio.available())
  {
      radio.read( &data, sizeof(data) );
      Serial.println(data);
      switch(data)
      {
        case 11: // otwieranie
        {
          mig[0]=true;
          mig[1]=false;
          mig[2]=false;
          break;
        }
        case 12: //zamykanie
        {
          mig[0]=false;
          mig[1]=false;
          mig[2]=true;
          break;
        }
        case 13: //otwarte
        {
          for(unsigned short i = 0; i<3;i++)
          {
            mig[i] = false;
          }
          digitalWrite(A4, LOW);
          digitalWrite(A5, LOW);
          digitalWrite(A3, HIGH);
          break;
        }
        case 14: // zamkniete
        {
          for(unsigned short i = 0; i<3;i++)
          {
            mig[i] = false;
          }
          digitalWrite(A3, LOW);
          digitalWrite(A4, LOW);
          digitalWrite(A5, HIGH);
          break;
        }
        case 15: // czujnik przerwania wiazki ir
        {
          mig[0]=false;
          mig[1]=true;
          mig[2]=false;
          break;
        }
        case 16: // ani zamknieta, ani otwarta
        {
          for(unsigned short i = 0; i<3;i++)
          {
            mig[i] = false;
          }
          digitalWrite(A3, LOW);
          digitalWrite(A5, LOW);
          digitalWrite(A4, HIGH);
          break;
        }
        default:
        {
          Serial.println("default");
        }
      }
        

   }

   for(unsigned short i = 0; i < 3;i++)
   {
    if(mig[i])
    {
      if(lasttime + 700 < millis())
      {
        lasttime = millis();
        if(i == 0)
        {
          digitalWrite(A4, LOW);
          digitalWrite(A5, LOW);
          digitalWrite(A3, !digitalRead(A3));
        }
        if(i == 1)
        {
          digitalWrite(A3, LOW);
          digitalWrite(A5, LOW);
          digitalWrite(A4, !digitalRead(A4));
        }
        if(i == 2)
        {
          digitalWrite(A3, LOW);
          digitalWrite(A4, LOW);
          digitalWrite(A5, !digitalRead(A5));
        }
      }   
    } 
   }
   if(lastanswerdtime + 2000 < millis())
   {
    send(7);
    lastanswerdtime = millis();
   }
}

