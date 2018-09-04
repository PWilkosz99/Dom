#include <SoftwareSerial.h>
#include <SPI.h>
#include <RF24.h> 

#define OtworzPin 7
#define ZamknijPin 8
#define ESPRX 6
#define ESPTX 5
#define Radio_CE 9
#define Radio_CS 10
#define Debug_Pin A5
#define Krancowka_Otwarcia 2
#define Krancowka_Zamkniecia 3
#define Przerwanie_Wiazki 4
#define Otworz_Pin A2
#define Stop_Pin A3
#define Zamknij_Pin A4
#define Sleep_Pin A0

SoftwareSerial esp (ESPRX,ESPTX);//NA PCB  RX, TX

bool changed = false; // potwierdzenia na wifi
bool rchanged = false; // zapytanie na module radiowym

unsigned short int sleep;

bool debug;
bool isbegin = false;

void pisz(String text) // pisze tylko wtedy gdy jest w trybie debugowania
{
	if (!debug)
	{
		if (!isbegin)
		{
			Serial.begin(115200);
			isbegin = true;
			Serial.println("Tryb debugowania\n");
		}
		Serial.print(text);
	}
}

enum Akcja
{
	On = 1,
	Off = 2,
};

bool Otworz_On;
bool Zamknij_On;

unsigned long OtworzTime;
unsigned long ZamknijTime;

bool Otworz();
void Otworz(Akcja akcja);

bool Zamknij();
void Zamknij(Akcja akcja);

bool Otworz()
{
	return Otworz_On;
}

bool Zamknij()
{
	return Zamknij_On;
}

void Otworz(Akcja akcja)
{
	switch (akcja)
	{
		case On:
		{	
			if (Zamknij_On)
			{	
				Zamknij(Akcja (Off));
				delay(sleep);
			}
			digitalWrite(OtworzPin, LOW);
			Otworz_On = true;
			changed = true;
			break;
		}	
		case Off:
		{
			if (Otworz_On)
			{
				digitalWrite(OtworzPin, HIGH);
				Otworz_On = false;
				changed = true;
				OtworzTime = millis();
			}
			break;
		}
	}
}

void Zamknij(Akcja akcja)
{
	switch (akcja)
	{
		case On:
		{		
			if(Otworz_On)
			{
				Otworz(Akcja(Off));
				delay(sleep);
			}
			digitalWrite(ZamknijPin, LOW);
			Zamknij_On = true;
			changed = true;
			break;
		}
		case Off:
		{
			if (Zamknij_On)
			{
				digitalWrite(ZamknijPin, HIGH);
				Zamknij_On = false;
				changed = true;
				ZamknijTime = millis();
			}
			break;
		}
	}
}

void ako() // a - aktywna k - krancowka 0 - otwarcia
{
	Otworz(Akcja(Off)); // zatrzymaj otwieranie bramy
	pisz("ako\n");
}

void akz()  // a - aktywna k - krancowka z - zamkniecia
{
	Zamknij(Akcja(Off)); // zatrzymaj zamykanie bramy
	pisz("akz\n");
}

RF24 radio(Radio_CE,Radio_CS);

byte addresses[6] = {"degtf"}; // adres rury modułu radiowego

void RSend(unsigned short int data)
{
	radio.stopListening();
	radio.write(&data, sizeof(data));
    radio.startListening();
}

void setup() 
{
	pisz("Konfiguracja IO\n");
	
	digitalWrite(OtworzPin, HIGH);
	digitalWrite(ZamknijPin, HIGH);

	pinMode(OtworzPin, OUTPUT);
	pinMode(ZamknijPin, OUTPUT);
			
	pinMode(Debug_Pin, INPUT_PULLUP); // Zwarty do masy powoduje przejscie do trybu debugowania
	debug = digitalRead(Debug_Pin);
	
	pinMode(Krancowka_Otwarcia, INPUT_PULLUP); // a, Krancowka otwarcie, umozliwia przez to sprawdzenie stanu
	pinMode(Krancowka_Zamkniecia, INPUT_PULLUP); // b, krancowka zamkniecia, umozliwia przez to sprawdzenie stanu
	
	pinMode(Przerwanie_Wiazki, INPUT_PULLUP); // Czujnik przerwania wiązki IR
	
	pinMode(Otworz_Pin, INPUT_PULLUP); // Fizyczny przycisk, otworz
	pinMode(Stop_Pin, INPUT_PULLUP); // Fizyczny przycisk, stop
	pinMode(Zamknij_Pin, INPUT_PULLUP); // Fizyczny przycisk, zamknij
	
	pisz("Konfiguracja przerwan\n");
	
	attachInterrupt(0, ako, FALLING);  // a, Krancowka otwarcie, po zmianie ze stanu wysokiego na niski
	attachInterrupt(1, akz, FALLING);  // b, krancowka zamkniecia, po zmianie ze stanu wysokiego na niski
	
	sleep = analogRead(Sleep_Pin) / 10 * 40;
	
	pisz("Opoznienie po wlaczeniu wynosi: ");
	pisz((String)sleep);
	
	pisz("\nKonfiguracja modlu radiowego\n");
	
	if(radio.begin())
	{
		pisz("Uruchomino radio .begin() wykonane pomyslnie\n");
	}
	else
	{
		pisz("Niepoprawna konfiguracja modulu radiowego\n");
	}
  
	radio.setChannel(108);	
	radio.setPALevel(RF24_PA_MAX);	
	radio.openWritingPipe(addresses); // do wysylania potwierdzen	
	radio.openReadingPipe(1, addresses); // Use the first entry in array 'addresses' (Only 1 right now)	
	radio.startListening();	
	pisz("Skonfigurowano radio\n");
	
	esp.begin(57600);
	
	changed = true;

	pisz("\nWychodze z SETUP\n\n");
}

void loop() 
{
	if(digitalRead(Krancowka_Otwarcia) == LOW)
    {
		Otworz(Akcja(Off));
    }
	if(digitalRead(Krancowka_Zamkniecia) == LOW)
    {
		Zamknij(Akcja(Off));
    }
    if(digitalRead(Przerwanie_Wiazki) == LOW)
    {
		Otworz(Akcja(Off));
		Zamknij(Akcja(Off));
    }

//-------------------------PRZYCISKI FIZYCZNE ----------------------------
  
    if(digitalRead(Stop_Pin) == LOW) // wcisnieto przycisk stop
    {
		Zamknij(Akcja(Off));
		Otworz(Akcja(Off));
		pisz("Zatrzymano proces\n->przyciski fizyczne\n");
    }
    if(digitalRead(Otworz_Pin) == LOW) // wcisnieto przycisk otwarcia bramy
    {
		Otworz(Akcja(On));
		pisz("Otwieranie\n->przyciski fizyczne\n");
	}
	if(digitalRead(Zamknij_Pin) == LOW) // wcisnieto przycisk zamkniecia bramy
    {
		Zamknij(Akcja(On));
		pisz("Zamykanie\n->przyciski fizyczne\n");
	}
	
//-------------------------------RADIO------------------------------------

    unsigned short int data;
    if ( radio.available())
    {   
      radio.read( &data, sizeof(data) );     
      if(data == 1)
      {
          Otworz(Akcja(On)); // otworz
          pisz("Otwieranie bramy\n->modul radiowy\n");
      }      
      if(data == 2)
      {
		  Zamknij(Akcja(Off));
		  Otworz(Akcja(Off));
		  pisz("Rzadanie zatrzymania\n->modul radiowy\n");
	  }
      if(data == 3)
	  {
		  Zamknij(Akcja(On)); // zamknij
		  pisz("Zamykanie bramy\n->modul radiow\ny");
      }
      if(data == 7) // odpowiedz
      {
		  rchanged=true;
      }
    }

//------------------------------WIFI----------------------------------

	unsigned char bufor[10];
	
	if(esp.available() > 0)
	{
		delay(10); // czas na kompletny odbior danych
		int a = esp.available();
		int i=0;
		for(i; i <a;i++)
		{
			bufor[i] = esp.read();			
		}

		i=0;

		for(i;i<a;i++)
		{	
			if (bufor[i] == 117)//u
			{
				changed = true;
			}
			if(bufor[i] == 145 )//æ
			{
				Otworz(Akcja(On)); // otworz
				pisz("Otwieranie bramy\n->modul wifi\n");				
			}
			if(bufor[i] == 155 )//¢
			{
				Zamknij(Akcja(Off));
				Otworz(Akcja(Off));
				pisz("Rzadanie zatrzymania\n->modul wifi\n");
			}	
			if(bufor[i] == 167 )//º
			{
				Zamknij(Akcja(On)); // zamknij
				pisz("Zamykanie bramy\n->modul wifi\n");
			}
		}
	}

	if(changed)// potwierdzenia n wifi
	{
		rchanged = true;
		if(Otworz())
		{
			esp.print("opning");
		}
		else if(Zamknij())
		{		  
			esp.print("cloing");
		}
		else if(!digitalRead(2))
		{
			esp.print("opened");    
		}
		else if(!digitalRead(3))
		{
			esp.print("closed");
		}
		else if(!digitalRead(4))
		{
			esp.print("brokee");
		}
		else
		{
			esp.print("none..");
		}
		changed = false;
	}	

	if(rchanged)
	{
	  if(Otworz())
        {
          RSend(11);
        }
        else if(Zamknij())
        {
          RSend(12);
        }
        else if(!digitalRead(2))
        {
          RSend(13);
        }
        else if(!digitalRead(3))
        {
          RSend(14);
        }
        else if(!digitalRead(4))
        {
          RSend(15);
        }
        else
        {
          RSend(16);
        }
        rchanged = false;        
  }

	if(!debug) // komunikacja z ESP
	{
	  if(Serial.available() > 0)
	  {
		  delay(300);
		  int a = Serial.available();
		  byte tmp[100];
		  pisz("\nMax 100znakow\nSkladnia dla esp: \"esp, kod\"\n");
		  int i=0;
		  for(i; i<a; i++)
		  {
			  tmp[i] = Serial.read();
		  }

		  i = 0;
		  
		  for(i;i<a;i++)
		  {
			  if(tmp[i] == 101 && tmp[i+1] == 115 && tmp[i+2] == 112) // esp
			  {
				  int b = i+5; // kod 
				  for (b; b < a; b++)
				  {
					  esp.print(char(tmp[b]));
				  }
			  }
		  }
	  }
  }
}