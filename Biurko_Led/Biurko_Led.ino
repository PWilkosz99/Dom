/*
 Name:		Biurko_Led.ino
 Created:	06.01.2017 20:53:45
 Author:	szymo
*/


#define G 9
#define R 10
#define B 11
#define Power 3


#define dot 124
#define plus 43
#define minus 45 
#define rowne 61
#define zero 15 // ?



void setup() 
{
	Serial.begin(115200);
  Serial.println("Siema");

  pinMode(2, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(7, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(12, OUTPUT);

  pinMode(9, OUTPUT);
  pinMode(10, OUTPUT);
  pinMode(11, OUTPUT);

  digitalWrite(2, LOW);  
  digitalWrite(4, LOW);
  digitalWrite(5, LOW);
  digitalWrite(6, LOW);
  digitalWrite(7, LOW);
  digitalWrite(8, LOW);
  digitalWrite(12, LOW);
  
  digitalWrite(9, LOW);
  digitalWrite(10, LOW);
  digitalWrite(11, LOW);  
}
void loop() 
{
	unsigned char bufor[50];
  
	if(Serial.available() > 0)
	{
		delay(2); // czas na kompletny odbior danych
		int a = Serial.available();
		int i=0;
		for(i; i <a;i++)
		{    
			bufor[i] = Serial.read();
			//Serial.print(char(bufor[i]));   
		}
		i=0;  
		for(i;i<a;i++)  
		{  
			if(bufor[i] == plus && bufor[i+1] == dot )   // +|255|255|255 
			{
				analogWrite(R, liczba(bufor[i+2], bufor[i+3], bufor[i+4]));
				analogWrite(G, liczba(bufor[i+6], bufor[i+7], bufor[i+8]));
				analogWrite(B, liczba(bufor[i+10], bufor[i+11], bufor[i+12]));
			}
			if(bufor[i] == minus && bufor[i+1] == dot ) //?? -|100|
			{
			  analogWrite(Power, 2.55*liczba(bufor[i+2], bufor[i+3], bufor[i+4]));

     // teests 
      
      //analogWrite(Power, 255);
			}
			if(bufor[i] == rowne && bufor[i+1] == dot ) //?? =|7|1|
			{
				digitalWrite(czesc(bufor[i+2]), liczba(63, 63, bufor[i+4]));
			}
		}
	}
}

int liczba(byte a, byte b, byte c)
{
  a = a-48;
  b = b-48;
  c= c-48;

  int var = 0;
  
  if(a == 15 && b == 15)  // ??c
  {
    var = c;
    return var;
  } 
  
  if(a == 15) // ?bc
  {
    var = 10*b;
    var = var + c;
    return var;
  }
  
  var = a*100+b*10+c;
  return var;
}

int czesc(byte part)
{
	part = part - 48;
	switch(part)
  {
    case 1:
    {
		return 2;
    }
    case 2:
    {
		return 4;
    }
    case 3:
    {
		return 6;
    }
    case 4:
    {
		return 5;
    }
    case 5:
    {
		return 7;
    }
    case 6:
    {
		return 12;
    }
    case 7:
    {
		return 8;
    }
  }
}
