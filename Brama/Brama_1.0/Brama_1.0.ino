
#define engtime 1000 // czas pomiędzy zmianami kierunku obrotów


#include <VirtualWire.h>
#include <SoftwareSerial.h>

class obroty
{
  public: bool ON = false;
  private: unsigned short pin;
  
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
  }
  public: void on()
  {
    digitalWrite(pin, LOW);
    ON = true;
    delay(100);
  }
  public: void off()
  {
    digitalWrite(pin, HIGH);
    ON = false;
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
/*
unsigned int odlegloscsharp()
{
  unsigned int tmp = 4800/(analogRead(A1)-20);
  if(tmp > 81 || tmp < 10)
    {
      Serial.print("poza zakresem, ");
      tmp = 0;
    }
  return tmp;
 }*/


SoftwareSerial esp (2, 3);

obroty l = 0; // puta klasa
obroty p = 0;

//unsigned long int sharptime;

  
//dane do radia
uint8_t buf[VW_MAX_MESSAGE_LEN];
uint8_t buflen = VW_MAX_MESSAGE_LEN;

//9 krancowka zamkni 10 otw

void setup() 
{
  
Serial.begin(115200);
Serial.println("Setup");

//przyciski
pinMode(9, INPUT_PULLUP); // lewy
pinMode(10, INPUT_PULLUP); // prawy
pinMode(12, INPUT_PULLUP); // Stop

esp.begin(38400);
// podlÄ…czenie klass
l = obroty('l');
digitalWrite(7, HIGH);

p = obroty('r');
digitalWrite(8, HIGH);

// setup do radia
vw_set_ptt_inverted(true); // Required for DR3100
vw_setup(2000);  // Bits per sec
vw_rx_start();       // Start the receiver PLL running

pinMode(5, INPUT_PULLUP); // zamknieta krancowka
pinMode(6, INPUT_PULLUP);  // otwarta krancowka

pinMode(13, INPUT_PULLUP); // pomarańczowy przewód imitacja czujnika przerwania wiązki IR

}

void loop() 
{

//unsigned long int currtime = millis();

if(digitalRead(12)== LOW)
{
  l.off();
  p.off();
}



if(digitalRead(13) == LOW)
{
  l.off();
  p.off();
}

  
if(esp.available() > 0)
{
  String data = esp.readString();//Until('\n');
  
 if(data == "o")
 {
  if(l.ison() == true)
  {
    l.off();
    delay(engtime);
  }
  if(l.ison() == false)
  {
    p.on();
  }
 }
 
 if( data == "z")
 {
  if(p.ison() == true)
  {
    p.off();
    delay(engtime);
  }
  if(p.ison() == false)
  {
   l.on();
  }  
 }
 if(data == "s")
 {
  l.off();
  p.off();
 }
}

/*
if (l.ison() == true)
{
  l.off();
}
if (p.ison() == true)
{
  p.off();
}
*/
// krancowki

if(digitalRead(5) == LOW)
{
  l.off();
  p.off();
}
if(digitalRead(6) == LOW)
{
  l.off();
  p.off();
}

//przyciski fizyczne


if(digitalRead(9) == LOW)
{ 
  if(p.ison() == true)
  {
    p.off();
    delay(engtime);
  } 
  if(p.ison() == false)
  {
    l.on();
  }
}
if(digitalRead(10) == LOW) 
{
  if(l.ison() == true)
  {
    l.off();
    delay(engtime);
  }
  if(l.ison() == false)
  {
   p.on();
  }
}

// radio

String wiad;

if (vw_get_message(buf, &buflen)) 
  {
    int i;   
    for (i = 0; i < buflen; i++)
    { 
        wiad +=char(buf[i]);
    }
    if(wiad == "s1")
    {
     l.off();
     p.off(); 
    }     
    if(wiad == "l1")
    {
    if(p.ison() == true)
    {
      p.off();
      delay(engtime);
    }    
    if(p.ison() == false)
    {
      l.on();
    }
    }
    
    if(wiad == "r1")
    {
     if(l.ison() == true)
     {
      l.off(); 
      delay(engtime);
     }
    if(l.ison() == false)
    {
      p.on();
    }
    }


  }
/*
if(currtime - sharptime > 3000)
{
  sharptime = currtime;
  Serial.print("Odczyt z czujnika: ");
  Serial.println(odlegloscsharp());
 } */

}

