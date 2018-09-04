#include <SoftwareSerial.h>
#include <SPI.h>
#include <RF24.h> 

SoftwareSerial esp (6,5);//NA PCB

bool changed = false; // potwierdzenia na wifi
bool rchanged = false; // zapytanie na module radiowym

class obroty
{
private: bool ON = false;
private: unsigned short pin;
		// public: unsigned long int czas = millis();
public: obroty(char str)
{
	if(str == 'l')
	{
      pin = 7;// lewe
    }
    if (str == 'r')
    {
      pin = 8; // prawe
    }   
	pinMode(pin, OUTPUT);
    digitalWrite(pin, HIGH);
}
public: void on()
{
	// if((czas + sleep) < millis())
	// {
	digitalWrite(pin, LOW);
	ON = true;
    changed=true;
	//    czas = millis();
  //  }
}
public: void off()
{
	if(ON)
    {
		digitalWrite(pin, HIGH);
		ON = false;
		//czas = millis();		
		changed = true;
	}
}
public: bool ison()
{
	if(ON == true)
	{
		return true;
	}
	else
	{
		return false;
	}
}
};

obroty otworz = 0; // pusta klasa występująca globalnie
obroty zamknij = 0; // pusta klasa występująca globalnie

void ako() // a - aktywna k - krancowka 0 - otwarcia
{
	otworz.off(); // zatrzymaj otwieranie bramy
	changed = true;
	pisz("ako\n");
}
void akz()  // a - aktywna k - krancowka z - zamkniecia
{
	zamknij.off(); // zatrzymaj zamykanie bramy
	changed = true;
	pisz("akz\n");
}

bool debug;
bool isbegin = false;

void pisz(String text) // pisze tylko wtedy gdy jest w trybie debugowania
{
	if(!debug)
	{
		if(!isbegin)
		{
			Serial.begin(115200);
			isbegin = true;
			Serial.println("Tryb debugowania\n");
		}
		Serial.print(text);      
	}
}

RF24 radio(9,10);

byte addresses[6] = {"degtf"}; // adres rury modułu radiowego

unsigned short int sleep;

void send(unsigned short int data)
{
	radio.stopListening();
    if(!radio.write( &data, sizeof(data) ))
    {
     // pisz("blad");
    }
    radio.startListening();
}


void setup() 
{
	
	pinMode(A5, INPUT_PULLUP); // Zwarty do masy powoduje przejscie do trybu debugowania
	debug = digitalRead(A5);
	
	pisz("Konfiguracja IO\n");
	
	pinMode(2, INPUT_PULLUP); // a, Krancowka otwarcie, umozliwia przez to sprawdzenie stanu
	pinMode(3, INPUT_PULLUP); // b, krancowka zamkniecia, umozliwia przez to sprawdzenie stanu
	
	pinMode(4, INPUT_PULLUP); // Czujnik przerwania wiązki IR
	
	pinMode(A2, INPUT_PULLUP); // Fizyczny przycisk, otworz
	pinMode(A3, INPUT_PULLUP); // Fizyczny przycisk, stop
	pinMode(A4, INPUT_PULLUP); // Fizyczny przycisk, zamknij
	
	pisz("Konfiguracja przerwan\n");
	
	attachInterrupt(0, ako, FALLING);  // a, Krancowka otwarcie, po zmianie ze stanu wysokiego na niski
	attachInterrupt(1, akz, FALLING);  // b, krancowka zamkniecia, po zmianie ze stanu wysokiego na niski
		
	pisz("Tworzenie obiektow klasy napedu\n");
	
	sleep = analogRead(A0) / 10 * 40;

	otworz = obroty('l');  // podlaczani klasy napedu otwarcia bramy
	zamknij = obroty('r'); // podlaczani klasy napedu otwarcia bramy
	
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
	if(digitalRead(2) == LOW)
    {
		otworz.off();
    }
	if(digitalRead(3) == LOW)
    {
		zamknij.off();
    }
    if(digitalRead(4) == LOW)
    {
		otworz.off();
		zamknij.off();
    }


  
    //PRZYCISKI FIZYCZNE ----------------------------
  
    if(digitalRead(A3) == LOW) // wcisnieto przycisk stop
    {
		zamknij.off();
		otworz.off();
		pisz("Zatrzymano proces\n->przyciski fizyczne\n");
    }
    if(digitalRead(A2) == LOW) // wcisnieto przycisk otwarcia bramy
    {
		zamknij.off(); //jak zamyka niech przestanie

      if(digitalRead(2) == HIGH && digitalRead(4) == HIGH) // jak nie aktywna krancowka otwarcia i jak nie przerwana wiazka IR,
      { 
		  otworz.on(); // otworz
          pisz("Otwieranie bramy\n->przyciski fizyczne\n");        
	  }
      else
	  {
        pisz("brama jest już otwarta\n->przyciski fizyczne\n");
      }
	}
	if(digitalRead(A4) == LOW) // wcisnieto przycisk zamkniecia bramy
    {
		otworz.off(); // jak otwiera niech przestanie

      if(digitalRead(3) == HIGH && digitalRead(4) == HIGH) // jesli czujnik zamkniecia nie aktywny i jesli nie aktywny czujnik przerwania wiazki IR
      {  
		  zamknij.on(); // zamknij
		  pisz("Zamykanie bramy\n->przyciski fizyczne\n");
	  }
      else
      {
		  pisz("Brama jest już zamknięta\n->przyciski fizyczne\n");
      }
    }

   //RADIO-------------------------------------------------------


    unsigned short int data;
    if ( radio.available())
    {
   // while (radio.available())
   // {
      //if(data != 0)
     // {
      radio.read( &data, sizeof(data) );
      //pisz("Data received = ");
     // pisz((String)data);
      if(data == 1)
      {
         zamknij.off(); //jak zamyka niech przestanie
        if(digitalRead(2) == HIGH && digitalRead(4) == HIGH) // jak nie aktywna krancowka otwarcia i jak nie przerwana wiazka IR
         {
          otworz.on(); // otworz
          pisz("Otwieranie bramy\n->modul radiowy\n");
         }
        else
         {
         // otworz.off();
          pisz("brama jest już otwarta\n->modul radiowy\n");
         }
      }
      
      if(data == 2)
      {
        zamknij.off();
        otworz.off();
        pisz("Rzadanie zatrzymania\n->modul radiowy\n");
      }
      if(data == 3)
      {
        otworz.off(); // jak otwiera niech przestanie
        if(digitalRead(3) == HIGH && digitalRead(4) == HIGH) // jesli czujnik zamkniecia nie aktywny i jesli nie aktywny czujnik przerwania wiazki IR
         {
            zamknij.on(); // zamknij
            pisz("Zamykanie bramy\n->modul radiow\ny");
          }
          else
          {
           // zamknij.off();
           pisz("Brama jest już zamknięta\n->modul radiowy\n");
          }
      }
      if(data == 7) // odpowiedz
      {
        rchanged=true;
      }
    }
    // WIFI----------------------------------------------------
	
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
				zamknij.off(); //jak zamyka niech przestanie
				if(digitalRead(2) == HIGH && digitalRead(4) == HIGH) // jak nie aktywna krancowka otwarcia i jak nie przerwana wiazka IR
				{
					otworz.on(); // otworz
					pisz("Otwieranie bramy\n->modul wifi\n");
				}
				else
				{
					pisz("nBrama jest już otwarta\n->modul wifi\n");
				}
			}
			if(bufor[i] == 155 )//¢
			{
				zamknij.off();
				otworz.off();
				changed = true;
				pisz("Rzadanie zatrzymania\n->modul wifi\n");
			}	
			if(bufor[i] == 167 )//º
			{
				otworz.off(); // jak otwiera niech przestanie
				if(digitalRead(3) == HIGH && digitalRead(4) == HIGH) // jesli czujnik zamkniecia nie aktywny i jesli nie aktywny czujnik przerwania wiazki IR
				{
					zamknij.on(); // zamknij
					pisz("Zamykanie bramy\n->modul wifi\n");
				}
				else
				{
					pisz("Brama jest już zamknięta\n->modul wifi\n");
				}
			}
		}
	}

	if(changed == true)// potwierdzenia n
	{
		rchanged = true;
		pisz("resp");
		if(otworz.ison())
		{
			esp.print("opning");
		}
		else if(zamknij.ison())
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
	  if(otworz.ison())
        {
          send(11);

        }
        else if(zamknij.ison())
        {
          send(12);

        }
        else if(!digitalRead(2))
        {
          send(13);
        }
        else if(!digitalRead(3))
        {
          send(14);
        }
        else if(!digitalRead(4))
        {
          send(15);
        }
        else
        {
          send(16);
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
			  // pisz(String(char(tmp[i])));
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
				  if(b == a)
				  {
					 // esp.print();
				  }
			  }
		  }
	  }
  }
}
