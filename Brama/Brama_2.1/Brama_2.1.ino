#include <SoftwareSerial.h>
#include <SPI.h>
#include <RF24.h> 

//SoftwareSerial esp (5,6);
SoftwareSerial esp (6,5);//NA PCB

class obroty
{
  private: bool ON = false;
  private: unsigned short pin;
  public: unsigned short int sleep = analogRead(A0)/10*40;
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
    delay(sleep);
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
  pisz("ako\n");
}
void akz()  // a - aktywna k - krancowka z - zamkniecia
{
  zamknij.off(); // zatrzymaj zamykanie bramy
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

// Konfiguracja ESP -----------------------

unsigned short int espconfigured = 1;

unsigned long int tcipmux = millis();
unsigned long int tcipserver = millis();;



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

otworz = obroty('l');  // podlaczani klasy napedu otwarcia bramy
zamknij = obroty('r'); // podlaczani klasy napedu otwarcia bramy

pisz("Opoznienie po wlaczeniu wynosi: ");
pisz((String)otworz.sleep);


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

radio.openReadingPipe(1, addresses); // Use the first entry in array 'addresses' (Only 1 right now)

radio.startListening();
pisz("Skonfigurowano radio\n");

esp.begin(57600);

//delay(100); // nie wiem czemu ale jak go nie ma to cip mux sie nie wykonuje
tcipserver = 7000+ millis(); // zeby konfiguracja esp ka nastapila po 1 sek, a pierwsza rzecza jaka zaczyna rb to cipmux

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


  
    //PRZYCISKI FIZYCZNE -------------
  
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

   //RADIO------------


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
    }

    // WIFI------------

    if(espconfigured == 1) // czy skonfigurowano
    {
      
      if((tcipserver + 2000) < millis())
      {
      pisz("Konfiguracja esp\n");
      pisz("at+cipmux=1\n");
      esp.println("AT+CIPMUX=1");
      espconfigured = 2;
      tcipmux = millis();
      }
    }
    if(espconfigured == 2)
    {
      if((tcipmux + 3000) < millis())
      {
        esp.println("AT+CIPSERVER=1,80");
        pisz("at+cipserver\n");
        tcipserver = millis();
        pisz("Skonfigurowano esp\n");
        espconfigured = 3; // OK
      }
    }
// koniec konfigu esp ka
unsigned char bufor[100];


if(esp.available() > 0)
{
  delay(300); // czas na kompletny odbior danych
  int a = esp.available();
  int i=0;
  for(i; i <a;i++)
  {
     bufor[i] = esp.read();
     pisz(String(char(bufor[i])));
  }
  
  i=0;
  for(i;i<a;i++)
  {


   
  if(bufor[i] == 87 && bufor[i+1] == 73 && bufor[i+2] == 70 && bufor[i+3] == 73 && bufor[i+5] == 71 && bufor[i+6] == 79 && bufor[i+7] == 84 && bufor[i+9] == 73&& bufor[i+10] == 80 )  //WIFI GOT IP // polaczyl sie z siecia albo po uruchomieniu albo ponownym polaczeniu
  {
    pisz("Ponowne poloczenie\n");
    tcipserver= 7000 + millis(); // ustawienie oczekiwania 7 s
    espconfigured = 1;
  }
  
 
  

   if(bufor[i] != 255 && bufor[i] != 0 )
  {
    if(bufor[i] ==43) //+
    {
      pisz("\n\n+");
       if(bufor[i+1] ==73) //I
          {
            pisz("i");
           if(bufor[i+2] ==80)  //P
            {
              pisz("p");
             if(bufor[i+3] ==68) // D
              {
                pisz("d");
                if(bufor[i+7] == 49) // 1 +ipd,0,taliczba ilosc znakow jak jeden to z apki dane  
                  {
                     if(bufor[i+9] == 111 )//o
                      {
                         zamknij.off(); //jak zamyka niech przestanie
                        if(digitalRead(2) == HIGH && digitalRead(4) == HIGH) // jak nie aktywna krancowka otwarcia i jak nie przerwana wiazka IR
                         {
                            otworz.on(); // otworz
                            pisz(":o\nOtwieranie bramy\n->modul wifi\n");
                          }
                        else
                          {
                          pisz(":o\nbrama jest już otwarta\n->modul wifi\n");
                           }
                       }
                     if(bufor[i+9] == 115 )//s
                      {
                        zamknij.off();
                        otworz.off();
                        pisz(":s\nRzadanie zatrzymania\n->modul wifi\n");
                         
                      }
                     if(bufor[i+9] == 122 )//z
                      {
                        otworz.off(); // jak otwiera niech przestanie
                      if(digitalRead(3) == HIGH && digitalRead(4) == HIGH) // jesli czujnik zamkniecia nie aktywny i jesli nie aktywny czujnik przerwania wiazki IR
                       {
                         zamknij.on(); // zamknij
                         pisz(":z\nZamykanie bramy\n->modul wifi\n");
                       }
                      else
                       {
                       pisz(":z\nBrama jest już zamknięta\n->modul wifi\n");
                        }

                      
                      }
                  }
              }
            }
          }
    }
  }
  }
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
        for(b; b < a; b++)
        {
          esp.print(char(tmp[b]));
        }
        if(b == a)
        {
          esp.println();
        }
      }
    }
  }
}



}
