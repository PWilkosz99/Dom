/*
 Name:		Program.ino
 Created:	22.04.2017 12:58:20
 Author:	Szymon
*/

// Autoryzacja: 8;3;357996064361445

//Dolaczenie odpowiednich bibliotek
#include <Wire.h>
#include <PCF8574.h>
#include <MPU6050.h>
#include <EEPROM.h>
#include <SPI.h>
#include <MFRC522.h>

#pragma region PINOUT
#pragma region Ekspander

#define I_LEWY_MIGACZ_E_PIN 1
#define I_PRAWY_MIGACZ_E_PIN 2
#define I_SWIATLO_MIJANIA_E_PIN 3
#define I_SWIATLO_DROGOWE_E_PIN 4
#define I_BLINDA_E_PIN 5

#define I_SWIATLO_STOP_PRZOD_E_PIN 6
#define I_SWIATLO_STOP_TYL_E_PIN 7

#define O_SWIATLO_DROGOWE_E_PIN 0

#pragma endregion
#pragma region Mikrokontroler

#define O_ZAPLON_PIN 4
#define O_SWIATLO_MIJANIA_PIN 5

#define O_POSTOJOWKA_PIN A0
#define O_LEWY_MIGACZ_PIN A1
#define O_PRAWY_MIGACZ_PIN A2
#define O_SWIATLO_TYL_PIN 8
#define O_SWIATLO_STOP_PIN 7
#define O_DZIEN_NOC_PIN 6

#define RFID_SS_PIN 10
#define RFID_RST_PIN 9

#define UCC_PIN A3

#pragma endregion
#pragma endregion

#pragma region Interwaly
#define UCC_TIME 3000
#define TempRefreshTime 10000
#define AccelRefreshTime 500
#define KeepAliveRefreshTime 5000
#pragma endregion

#pragma region Klasy modulow
//Definicja Ekspandera
PCF8574 Exp;
//Definicja Akcelerometru
MPU6050 mpu;
//Definicja RFID
MFRC522 Rfid(RFID_SS_PIN, RFID_RST_PIN);
#pragma endregion

#pragma region Zmienne filtru kalmana
double Q_angle, Q_bias, R_measure;
double K_angle, K_bias, K_rate;
double P[2][2], K[2];
double S, y;
double dt, kt;
#pragma endregion

#pragma region Funkcje

// Definicja zmiennych nazw funkcji, w celu asynchronicznego programu
// Funkcje dzialaja "bistabilnie"

bool AUTH = false;

bool LMigaczOn = false;				// Wlaczenie lewego migacza
bool PMigaczOn = false;				// Wlaczenie prawego migacza
bool SwiatloDrogoweOn = false;		// Wlaczenie swiatel drogowych (dlugich)
bool SwiatloMijaniaOn = false;		// Wlaczenie swiatel mijania (krotkich)
bool ZaplonOn = false;				// Mozliwosc uruchomienia silnika
bool PostojowkaOn = false;			// Wlaczenie swiatel postojowych
bool SwiatloTylOn = false;			// Wlaczenie swiatla tylnego
bool SwiatloStopOn = false;			// Wlaczenie swiatla stop
bool DzienON = false;				// Bardziej jasne swiatla
bool BlindaOn = false;				// Blinda wlaczona
bool Uin = false;					// Napiecie wejsciowe
bool LHMigaczOn = false;			// Wlaczenie ciagle lewego migacza
bool PHMigaczOn = false;			// Wlaczenie ciagle prawego migacza
bool AwaryjneOn = false;

bool GetRFIDOn = false;				// Podanie numeru rfid
bool RFIDrdy = false;				//Numer gotow do odczytu
bool WriteRFIDtoEEPROM = false;		// Wpisz numr rfid do eeprom
bool GetTempOn = false;
bool GetKatOn = false;
bool KeepAliveOn = false;
bool KeepAliveDot = false;
bool MigaczAutoOn = false;

bool LMigaczOff = false;			// Wylaczenie lewego migacza
bool PMigaczOff = false;			// Wylaczenie prawego migacza
bool SwiatloDrogoweOff = false;		// Wylaczenie swiatel drogowych (dlugich)
bool SwiatloMijaniaOff = false;		// Wylaczenie swiatel mijania (krotkich)
bool ZaplonOff = false;				// Wylaczenie silnika
bool PostojowkaOff = false;			// Wylaczenie swiatel postojowych
bool SwiatloTylOff = false;			// Wylaczenie swiatla tylnego
bool SwiatloStopOff = false;		// Wylaczenie swiatla stop
bool DzienOff = false;				// Ciemniejsze swiatla
bool BlindaOff = false;				// Wylaczenie blindy
bool LHMigaczOff = false;			// Wylaczenie ciagle lewego migacza
bool PHMigaczOff = false;			// Wylaczenie ciagle prawego migacza
bool AwaryjneOff = false;
bool GetTempOff = false;
bool GetKatOff = false;
bool KeepAliveOff = false;
bool MigaczAutoOff = false;

// Poprzednie stany pinów
bool S_LewyMigacz;
bool S_PrawyMigacz;
bool S_SwiatloMijania;
bool S_SwiatloDrogowe;
bool S_Blinda;
bool S_SwiatloStopPrzod;
bool S_SwiatloStopTyl;

#pragma endregion

#pragma region Zmienne globalne

unsigned int EEPROMvar[20];

unsigned long int  MigaczCzas = 0;		// Ostatni czas zmiany stanu na migaczu
unsigned short int T_Migacz;			// Okres zmian stanu na migaczu
unsigned long int L_Czas;				// Czas kiedy osotanio wykonala sie cala petla
unsigned long int UCC_Czas;				// Ostatni czas funkcji
unsigned long int TempTime;				// Ostatni czas funkcji
unsigned long int PrzechylTime;			// Ostatni czas funkcji
unsigned long int ATime;
unsigned long int KeepAliveTime;		// Ostatni czas funkcji

bool PoprzedniStanSwiatel[2];			// Informacja o stanie swiatel po opuszczeniu blindy 0 - mijania, 1 - drogowe

bool BylPrzechylony = false;			// Czy byl przechylony
byte TolerancjaPrzechylu = 20;			// Wartosc graniczna wyzwalajaca funkcje

char bufor[50];							// Bufur UART
unsigned short int buforLenght = 0;		// Pozycja w buforze

byte WritetoEEPROMindex = 4;			// Index danych EEPRomie
#pragma endregion

#pragma region ENUM y

//Kody komunikacyjne
enum Typ
{
	Swiatla = 2,
	Migacz = 3,	
	Akcelerometr = 4,
	EEProm = 6,
	Dodatk = 7,
	Auth = 8,
	Errors = 5,
};

enum Swiatlo
{
	LewyMigacz = 3,
	PrawyMigacz = 4,
	Blinda = 5,
	SwiatloMijania = 2,
	SwiatloDrogowe = 0,
	SwiatloTyl = 8,
	Stop = 7,
	Postojowka = 6,
	Awaryjne = 9,
};

enum Dodatkowe
{
	DzienNoc = 5,
	KeepAliveONOFF = 3,
	KeepAlive = 4,
	Sync = 2,
	Ucc = 6,
	EngineStart = 7,
	MigaczAutoONOFF = 8,
};

enum EEPROMDataType
{
	IMEI = 3,
	RFID = 4,
	T_MigaczCzas = 5,
	TolerancjaPrzechyluMPU = 6,
};

enum Akcel
{
	Temperatura = 2,
	Kat = 3,
};

enum ErrorCodes
{
	Card = 2,
	MPU = 3,
	Ekspander = 4,
};

#pragma endregion

void setup()
{
	//Rozpocz�cie transmisji UART z kt�r� b�dzie dzia�a�o BT
	Serial.begin(38400);
	//Rozpocz�cie pracy z ekspanderem
	Exp.begin(0x20);
	// Rozpoczecie pracy z akcelerometrem
	//----------------------PrzechylInit(0.01, 0.03, 0.3);
	//Ustawienie funkcji pin�w
#pragma region Zadeklarowanie I/O
#pragma region Ekspander
	Exp.digitalWrite(O_SWIATLO_DROGOWE_E_PIN, HIGH);
	Exp.pinMode(I_LEWY_MIGACZ_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_PRAWY_MIGACZ_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_MIJANIA_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_DROGOWE_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_BLINDA_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_STOP_PRZOD_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_STOP_TYL_E_PIN, INPUT_PULLUP);
	Exp.pinMode(O_SWIATLO_DROGOWE_E_PIN, OUTPUT);
#pragma endregion

#pragma region Mikrokontroler
	pinMode(O_ZAPLON_PIN, OUTPUT);
	pinMode(O_SWIATLO_MIJANIA_PIN, OUTPUT);
	pinMode(O_POSTOJOWKA_PIN, OUTPUT);
	pinMode(O_LEWY_MIGACZ_PIN, OUTPUT);
	pinMode(O_PRAWY_MIGACZ_PIN, OUTPUT);
	pinMode(O_SWIATLO_TYL_PIN, OUTPUT);
	pinMode(O_SWIATLO_STOP_PIN, OUTPUT);
	pinMode(O_DZIEN_NOC_PIN, OUTPUT);
#pragma endregion
#pragma endregion

#pragma region Pobranie wartosci zmiennych z EEPROM

	GetEEPROMValue(EEPROMDataType::T_MigaczCzas, NULL);
	if (EEPROMvar[0] == 255)
	{
		T_Migacz = 500;
	}
	else
	{
		T_Migacz = EEPROMvar[0];
	}	

	GetEEPROMValue(EEPROMDataType::TolerancjaPrzechyluMPU, NULL);
	if (EEPROMvar[0] == 255)
	{
		TolerancjaPrzechylu = 20;
	}
	else
	{
		TolerancjaPrzechylu = EEPROMvar[0];
	}
#pragma endregion

	SPI.begin();		// Init SPI bus
	Rfid.PCD_Init();	// Init MFRC522	

	//GetRFIDOn = true;
	//WritetoEEPROMindex = 0;
	//WriteRFIDtoEEPROM = true;



//	byte nr[15] = { 3,5,7,9,9,6,0,6,4,3,6,1,4,4,5 };
//	SetEEPROMValue(EEPROMDataType::IMEI, 0, nr);


}

void loop()
{
#pragma region Obsługa manetki
	if (AUTH) // jesli autoryzowany
	{
		// Swiatlo mijania
		if (S_SwiatloMijania != Exp.digitalRead(I_SWIATLO_MIJANIA_E_PIN))
		{
			S_SwiatloMijania = Exp.digitalRead(I_SWIATLO_MIJANIA_E_PIN);
			if (!S_SwiatloMijania)  // jesli zwarty do masy
			{
				SwiatloTylOn = true;
				SwiatloMijaniaOn = true;
			}
			else
			{
				SwiatloMijaniaOff = true;
			}
		}

		// Swiatlo drogowe
		if (S_SwiatloDrogowe != Exp.digitalRead(I_SWIATLO_DROGOWE_E_PIN))
		{
			S_SwiatloDrogowe = Exp.digitalRead(I_SWIATLO_DROGOWE_E_PIN);
			if (!S_SwiatloDrogowe)  // jesli zwarty do masy
			{
				SwiatloTylOn = true;
				SwiatloDrogoweOn = true;
			}
			else
			{
				SwiatloDrogoweOff = true;
			}
		}

		// Blinda
		if (S_Blinda != Exp.digitalRead(I_BLINDA_E_PIN))
		{
			S_Blinda = Exp.digitalRead(I_BLINDA_E_PIN);
			if (!S_Blinda)  // jesli zwarty do masy
			{
				BlindaOn = true;
			}
			else
			{
				BlindaOff = true;
			}
		}

		// Swiatlo Stop
		if (S_SwiatloStopPrzod != Exp.digitalRead(I_SWIATLO_STOP_PRZOD_E_PIN) || S_SwiatloStopTyl != Exp.digitalRead(I_SWIATLO_STOP_TYL_E_PIN))
		{
			S_SwiatloStopPrzod = Exp.digitalRead(I_SWIATLO_STOP_PRZOD_E_PIN);
			S_SwiatloStopTyl = Exp.digitalRead(I_SWIATLO_STOP_TYL_E_PIN);

			if (!(S_SwiatloStopPrzod && S_SwiatloStopTyl))  // jesli zwarty do masy
			{
				SwiatloStopOn = true;
			}
			else
			{
				SwiatloStopOff = true;
			}
		}
		
		// Migacz w lewo
		if (S_LewyMigacz != Exp.digitalRead(I_LEWY_MIGACZ_E_PIN))
		{
			S_LewyMigacz = Exp.digitalRead(I_LEWY_MIGACZ_E_PIN);
			if (!S_LewyMigacz)  // jesli zwarty do masy
			{
				AwaryjneOff = true;
				LMigaczOn = true;
			}
			else
			{
				LMigaczOff = true;
			}
		}
		
		// Migacz w prawo
		if (S_PrawyMigacz != Exp.digitalRead(I_PRAWY_MIGACZ_E_PIN))
		{
			S_PrawyMigacz = Exp.digitalRead(I_PRAWY_MIGACZ_E_PIN);
			if (!S_PrawyMigacz)  // jesli zwarty do masy
			{
				AwaryjneOff = true;
				PMigaczOn = true;
			}
			else
			{
				PMigaczOff = true;
			}
		}
	}
	else // jak nie sprawdzaj karte
	{
		GetRFIDOn = true; // wlacz sprawdzanie
		if (RFIDrdy) // jak sa dostepne dane
		{
			byte tmp[11];
			byte size = Rfid.uid.size;
			for (int i = 0; i < size; i++)
			{
				tmp[i] = Rfid.uid.uidByte[i];				
			}
			tmp[10] = size;
			// od 0 do 9 id a 10 rozmiar
			RFIDrdy = false;
			if (CheckAuth(EEPROMDataType::RFID, tmp))
			{
				AUTH = true;
				int tmp[3] = { (byte)Typ::Auth, (byte)EEPROMDataType::RFID,  1 };
				send(tmp, 3);
				GetRFIDOn = false;
				ZaplonOn = true;
				//zgadza sie
			}
			else
			{
				int tmp[3] = { (byte)Typ::Auth, (byte)EEPROMDataType::RFID,  0 };
				send(tmp, 3);
			}

		}
	}
#pragma endregion

#pragma region Funkcje

#pragma region Zaplon
	if (ZaplonOn)
	{
		if (AUTH)
		{
			digitalWrite(O_ZAPLON_PIN, HIGH);
			int tmp[] = { (byte)Typ::Dodatk, (byte)Dodatkowe::EngineStart, 1 };
			send(tmp, 3);
			ZaplonOn = false;
		}
	}
	if (ZaplonOff)
	{
		digitalWrite(O_ZAPLON_PIN, LOW);
		int tmp[] = { (byte)Typ::Dodatk, (byte)Dodatkowe::EngineStart, 0 };
		send(tmp, 3);
		ZaplonOff = false;
	}
#pragma endregion

#pragma region Migacze
	//Migacze funkcje
	if (LMigaczOn)
	{
		if (MigaczCzas + T_Migacz < millis())
		{
			digitalWrite(O_LEWY_MIGACZ_PIN, !digitalRead(O_LEWY_MIGACZ_PIN));
			MigaczCzas = millis();
			int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::LewyMigacz, digitalRead(O_LEWY_MIGACZ_PIN) };
			send(tmp, 3);
			int tmp2[] = { (int)Typ::Migacz, (int)Swiatlo::LewyMigacz, 1 };
			send(tmp2, 3);
			if (digitalRead(O_PRAWY_MIGACZ_PIN))
			{
				PMigaczOff = true;
			}
		}

	}
	if (PMigaczOn)
	{
		if (MigaczCzas + T_Migacz < millis())
		{
			digitalWrite(O_PRAWY_MIGACZ_PIN, !digitalRead(O_PRAWY_MIGACZ_PIN));
			MigaczCzas = millis();
			int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::PrawyMigacz, digitalRead(O_PRAWY_MIGACZ_PIN) };
			send(tmp, 3);
			int tmp2[] = { (int)Typ::Migacz, (int)Swiatlo::PrawyMigacz, 1 };
			send(tmp2, 3);
			if (digitalRead(O_LEWY_MIGACZ_PIN))
			{
				LMigaczOff = true;
			}

		}
	}
	if (LMigaczOff)
	{
		digitalWrite(O_LEWY_MIGACZ_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::LewyMigacz, 0 };
		send(tmp, 3);
		tmp[0] = (int)Typ::Migacz;
		send(tmp, 3);
		LMigaczOn = false;
		LMigaczOff = false;
	}
	if (PMigaczOff)
	{
		digitalWrite(O_PRAWY_MIGACZ_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::PrawyMigacz, 0 };
		send(tmp, 3);
		tmp[0] = (int)Typ::Migacz;
		send(tmp, 3);
		PMigaczOn = false;
		PMigaczOff = false;
	}
	// Migacze ciagle swiatlo
	if (LHMigaczOn)
	{
		digitalWrite(O_LEWY_MIGACZ_PIN, HIGH);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::LewyMigacz, 1 };
		send(tmp, 3);
		LHMigaczOn = false;
	}
	if (PHMigaczOn)
	{
		digitalWrite(O_PRAWY_MIGACZ_PIN, HIGH);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::PrawyMigacz, 1 };
		send(tmp, 3);
		PHMigaczOn = false;
	}
	if (LHMigaczOff)
	{
		digitalWrite(O_LEWY_MIGACZ_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::LewyMigacz, 0 };
		send(tmp, 3);
		LHMigaczOff = false;
	}
	if (PHMigaczOff)
	{
		digitalWrite(O_PRAWY_MIGACZ_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::PrawyMigacz, 0 };
		send(tmp, 3);
		PHMigaczOff = false;
	}
#pragma endregion

#pragma region Blinda
	if (BlindaOn)
	{
		PoprzedniStanSwiatel[0] = digitalRead(O_SWIATLO_MIJANIA_PIN);
		PoprzedniStanSwiatel[1] = Exp.digitalRead(O_SWIATLO_DROGOWE_E_PIN);
		SwiatloMijaniaOn = true;
		SwiatloDrogoweOn = true;
		int tmp[] = { (int)Typ::Swiatla, I_BLINDA_E_PIN, 1 };
		send(tmp, 3);
		BlindaOn = false;
	}
	if (BlindaOff)
	{
		if (PoprzedniStanSwiatel[0]) // czy byly zalaczone swiatla mijania
		{
			SwiatloMijaniaOn = true;
		}
		else  // jak nie by�y 
		{
			SwiatloMijaniaOff = true;
		}

		if (PoprzedniStanSwiatel[1]) // czy byly zalaczone swiatla mijania
		{
			SwiatloDrogoweOn = true;
		}
		else  // jak nie by�y 
		{
			SwiatloDrogoweOff = true;
		}
		int tmp[] = { (int)Typ::Swiatla, I_BLINDA_E_PIN, 0 };
		send(tmp, 3);
		BlindaOff = false;
	}
#pragma endregion

#pragma region Swiatla
	// Przod
	if (SwiatloMijaniaOn)
	{
		digitalWrite(O_SWIATLO_MIJANIA_PIN, HIGH);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::SwiatloMijania, 1 };
		send(tmp, 3);
		SwiatloMijaniaOn = false;
	}
	if (SwiatloDrogoweOn)
	{
		Exp.digitalWrite(O_SWIATLO_DROGOWE_E_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla,  (int)Swiatlo::SwiatloDrogowe, 1 };
		send(tmp, 3);
		SwiatloDrogoweOn = false;
	}
	
	if (SwiatloMijaniaOff)
	{
		digitalWrite(O_SWIATLO_MIJANIA_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::SwiatloMijania, 0 };
		send(tmp, 3);
		SwiatloMijaniaOff = false;
	}
	if (SwiatloDrogoweOff)
	{
		Exp.digitalWrite(O_SWIATLO_DROGOWE_E_PIN, HIGH);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::SwiatloDrogowe, 0 };
		send(tmp, 3);
		SwiatloDrogoweOff = false;
	}
	
	//Tyl
	if (SwiatloTylOn)
	{
		digitalWrite(O_SWIATLO_TYL_PIN, HIGH);
		int tmp[] = { (int)Typ::Swiatla, O_SWIATLO_TYL_PIN, 1 };
		send(tmp, 3);
		SwiatloTylOn = false;
	}
	if (SwiatloTylOff)
	{
		digitalWrite(O_SWIATLO_TYL_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, O_SWIATLO_TYL_PIN, 0 };
		send(tmp, 3);
		SwiatloTylOff = false;
	}

	// Stop
	if (SwiatloStopOn)
	{
		digitalWrite(O_SWIATLO_STOP_PIN, HIGH);
		int tmp[] = { (int)Typ::Swiatla, O_SWIATLO_STOP_PIN, 1 };
		send(tmp, 3);
		SwiatloStopOn = false;
	}
	if (SwiatloStopOff)
	{
		digitalWrite(O_SWIATLO_STOP_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, O_SWIATLO_STOP_PIN, 0 };
		send(tmp, 3);
		SwiatloStopOff = false;
	}

	// Postojowka
	if (PostojowkaOn)
	{
		digitalWrite(O_POSTOJOWKA_PIN, HIGH);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::Postojowka, 1 };
		send(tmp, 3);
		PostojowkaOn = false;
	}
	if (PostojowkaOff)
	{
		digitalWrite(O_POSTOJOWKA_PIN, LOW);
		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::Postojowka, 0 };
		send(tmp, 3);
		PostojowkaOff = false;
	}
#pragma endregion

#pragma region Awaryjne
	if (AwaryjneOn)
	{
		if (MigaczCzas + T_Migacz < millis())
		{		

			digitalWrite(O_LEWY_MIGACZ_PIN, !digitalRead(O_LEWY_MIGACZ_PIN));
			digitalWrite(O_PRAWY_MIGACZ_PIN, !digitalRead(O_PRAWY_MIGACZ_PIN));
			MigaczCzas = millis();

			int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::LewyMigacz, digitalRead(O_LEWY_MIGACZ_PIN) };
			send(tmp, 3);			
			int tmp1[] = { (int)Typ::Swiatla, (int)Swiatlo::PrawyMigacz, digitalRead(O_PRAWY_MIGACZ_PIN) };
			send(tmp1, 3);
			int tmp2[] = { (int)Typ::Swiatla, (int)Swiatlo::Awaryjne, 1 };
			send(tmp2, 3);
		}

	}
	if (AwaryjneOff)
	{
		AwaryjneOn = false;
		digitalWrite(O_LEWY_MIGACZ_PIN, LOW);
		digitalWrite(O_PRAWY_MIGACZ_PIN, LOW);		

		int tmp[] = { (int)Typ::Swiatla, (int)Swiatlo::LewyMigacz, 0 };
		send(tmp, 3);
		int tmp1[] = { (int)Typ::Swiatla, (int)Swiatlo::PrawyMigacz, 0 };
		send(tmp1, 3);
		int tmp2[] = { (int)Typ::Swiatla, (int)Swiatlo::Awaryjne, 0 };
		send(tmp2, 3);
		AwaryjneOff = false;
	}
#pragma endregion

#pragma region Keep Alive
	if (KeepAliveOn)
	{
		if (KeepAliveTime + KeepAliveRefreshTime < millis())
		{// nie odpowiedzial o czasie	
			ZaplonOff = true;
			KeepAliveOn = false;
			KeepAliveOff = true;
		}
	}

	if (KeepAliveOff)
	{
		int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::KeepAliveONOFF , 0 };
		send(tmp, 3);
		KeepAliveOn = false;
		KeepAliveOff = false;
	}

	if (KeepAliveDot)
	{
		KeepAliveTime = millis();
		KeepAliveDot = false;
	}
#pragma endregion

#pragma region Temperatura
	if (GetTempOn)
	{
		if (TempTime + TempRefreshTime < millis())
		{
			TempTime = millis();
			int tmp[] = { (int)Typ::Akcelerometr, (int)Akcel::Temperatura,  mpu.readTemperature() };
			send(tmp, 3);

		}
	}
	if (GetTempOff)
	{
		GetTempOn = false;
		GetTempOff = false;
	}
#pragma endregion

#pragma region Pobierz kat w pionowej plaszczyznie
	if (GetKatOn)
	{
		if (PrzechylTime + AccelRefreshTime < millis())
		{
			Serial.print((int)Typ::Akcelerometr);
			Serial.print(';');
			Serial.print((int)Akcel::Kat);
			Serial.print(';');
			Serial.println((int)Przechyl());
			PrzechylTime = millis();
		}
	}

	if (GetKatOff)
	{
		GetKatOn = false;
		GetKatOff = false;
	}
#pragma endregion

#pragma region Automatyczne wylaczenie migaczy
	if (MigaczAutoOn)
	{
		if (LMigaczOn || PMigaczOn)  // jestli jest wlaczona funkcja i jest wlaczony ktorykolwiek migacz
		{
			int przechyl = Przechyl();

			if ((przechyl > TolerancjaPrzechylu) || (przechyl < -(TolerancjaPrzechylu)))  // jeśli albo przechylił się do -20 albo +20
			{
				BylPrzechylony = true;
			}

			if (BylPrzechylony && ((przechyl < TolerancjaPrzechylu) || (przechyl > -(TolerancjaPrzechylu))))  // jak wlaczyl migacz byl przechylony i jest wyprostowany to wyłącz migacz
			{
				LMigaczOff = true;
				PMigaczOff = true;
				BylPrzechylony = false;
			}
		}
		else
		{
			BylPrzechylony = false;
		}
	}
	if (MigaczAutoOff)
	{
		MigaczAutoOn = false;
		MigaczAutoOff = false;
		int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::MigaczAutoONOFF , 1 };
		send(tmp, 3);
	}
#pragma endregion

	// Intensywnosc swiatel
	if (DzienON)
	{
		digitalWrite(O_DZIEN_NOC_PIN, HIGH);
		int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::DzienNoc, 1 };
		send(tmp, 3);
		DzienON = false;
	}
	if (DzienOff)
	{
		digitalWrite(O_DZIEN_NOC_PIN, LOW);
		int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::DzienNoc, 0 };
		send(tmp, 3);
		DzienOff = false;
	}

	// Podanie wartosci U wej
	if (Uin) 
	{
		int tmp[] = { (int)Typ::Dodatk,(int)Dodatkowe::Ucc, analogRead(UCC_PIN) };
		send(tmp, 3);
		Uin = false;
	}

	// Zczytaj UID karty RFID
	if (GetRFIDOn)
	{
		if (Rfid.PICC_IsNewCardPresent())
		{
			if (Rfid.PICC_ReadCardSerial())
			{ //Since a PICC placed get Serial and continue	
				Rfid.PICC_HaltA();				
				RFIDrdy = true; // dane gotowe do odczytu
			}
		}		
	}

	// Wczytaj UID RFIDa do EEPROM
	if (WriteRFIDtoEEPROM)
	{
		// wczesniej zainicjuj odczyt
		if (RFIDrdy)
		{
			byte tmp[11];
			byte size = Rfid.uid.size;
			for (int i = 0; i < size; i++)
			{
				tmp[i] = Rfid.uid.uidByte[i];
			}
			tmp[10] = size;

			SetEEPROMValue(EEPROMDataType::RFID, WritetoEEPROMindex, tmp);

			WriteRFIDtoEEPROM = false;
			RFIDrdy = false;
		}		
	}
#pragma endregion

#pragma region UART Read
	if (Serial.available() > 0)
	{
		//delay(10); // czas na kompletny odbior danych
		byte a = Serial.available();
		for (int i = 0; i < a; i++)
		{
			char tmp = Serial.read();
			//Serial.println(tmp-48);
			if (tmp == '#') // koniec komendy
			{
				Code();
				buforLenght = 0;
				char bufor[50];
			}
			else
			{				
				bufor[buforLenght] = tmp;
				buforLenght++;
			}
		}
	}
#pragma endregion

	//-------------Przechyl();
	//L_Czas = millis();
}

// Funkcja wysylajaca kody komujikacyjne na UART
// data[] tablica znaków, lenght rozmiar tablicy danych
void send(int data[], unsigned short int lenght)
{
	for (size_t i = 0; i < lenght - 1; i++)
	{
		Serial.print(data[i]);
		Serial.print(';');
	}
	Serial.print(data[lenght - 1]);
	Serial.println('#');
}

// Obsluga informacji z UART
void Code()
{
	/*
	Serial.print("in: ");
	for (int i = 0; i < buforLenght; i++)
	{
		Serial.print((char)bufor[i]);
	}
	Serial.println("");
		*/
	if (AUTH)
	{
		Typ typ = (Typ)(bufor[0] - 48);
		switch (typ)
		{
		case Swiatla:
		{
			Swiatlo swiatlo = (Swiatlo)(bufor[2] - 48);
			bool on = (bool)(bufor[4] - 48); // czy on off
			switch (swiatlo)
			{
			case LewyMigacz:
			{
				if (on)
				{
					LHMigaczOn = true;
				}
				else
				{
					LHMigaczOff = true;
				}
				break;
			}
			case PrawyMigacz:
			{
				if (on)
				{
					PHMigaczOn = true;
				}
				else
				{
					PHMigaczOff = true;
				}
				break;
			}
			case Blinda:
			{
				if (on)
				{
					BlindaOn = true;
				}
				else
				{
					BlindaOff = true;
				}
				break;
			}
			case SwiatloMijania:
			{
				if (on)
				{
					SwiatloMijaniaOn = true;
				}
				else
				{
					SwiatloMijaniaOff = true;
				}
				break;
			}
			case SwiatloDrogowe:
			{
				if (on)
				{
					SwiatloDrogoweOn = true;
				}
				else
				{
					SwiatloDrogoweOff = true;
				}
				break;
			}
			case SwiatloTyl:
			{
				if (on)
				{
					SwiatloTylOn = true;
				}
				else
				{
					SwiatloTylOff = true;
				}
				break;
			}
			case Stop:
			{
				if (on)
				{
					SwiatloStopOn = true;
				}
				else
				{
					SwiatloStopOff = true;
				}
				break;
			}
			case Postojowka:
			{
				if (on)
				{
					PostojowkaOn = true;
				}
				else
				{
					PostojowkaOff = true;
				}
				break;
			}
			case Awaryjne:
			{
				if (on)
				{
					AwaryjneOn = true;
				}
				else
				{
					AwaryjneOff = true;
				}
				break;
			}
			default:
			{
				break;
			}
			}
			break;
		}
		case Migacz:
		{
			Swiatlo swiatlo = (Swiatlo)(bufor[2] - 48);
			bool on = (bool)(bufor[4] - 48); // czy on off

			switch (swiatlo)
			{
			case LewyMigacz:
			{
				if (on)
				{
					LMigaczOn = true;
				}
				else
				{
					LMigaczOff = true;
				}
				break;
			}
			case PrawyMigacz:
			{
				if (on)
				{
					PMigaczOn = true;
				}
				else
				{
					PMigaczOff = true;
				}
				break;
			}
			default:
			{
				break;
			}
			}

			break;
		}
		case Dodatk:
		{
			Dodatkowe Add = (Dodatkowe)(bufor[2] - 48);			
			switch (Add)
			{
			case DzienNoc:
			{
				bool on = (bool)(bufor[4] - 48); // czy on off
				if (on)
				{
					DzienON = true;
				}
				else
				{
					DzienOff = true;
				}
				break;
			}
			case Sync:
			{
				// Typ, lewy migacz, prawy migacz, mijania, drogowe, tyl, stop, postojowka, zaplon
				int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::Sync , digitalRead(O_LEWY_MIGACZ_PIN), digitalRead(O_PRAWY_MIGACZ_PIN), digitalRead(O_SWIATLO_MIJANIA_PIN), !Exp.digitalRead(O_SWIATLO_DROGOWE_E_PIN), digitalRead(O_SWIATLO_TYL_PIN), digitalRead(O_SWIATLO_STOP_PIN), digitalRead(O_POSTOJOWKA_PIN), digitalRead(O_DZIEN_NOC_PIN), digitalRead(O_ZAPLON_PIN) };
				send(tmp, 11);
				break;
			}
			case Ucc:
			{
				break;
			}
			case EngineStart:
			{
				if ((bool)(bufor[4] - 48))
				{
					ZaplonOn = true;
				}
				else
				{
					ZaplonOff = true;
				}
				break;
			}
			case KeepAliveONOFF:
			{
				if ((bool)(bufor[4] - 48))
				{
					KeepAliveOn = true;
					int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::KeepAliveONOFF , 1 };
					send(tmp, 3);
					KeepAliveTime = millis();
				}
				else
				{
					KeepAliveOff = true;
				}
				break;

			}
			case KeepAlive:
			{
				KeepAliveDot = true;
				break;
			}
			case MigaczAutoONOFF:
			{
				if ((bool)(bufor[4] - 48))
				{
					MigaczAutoOn = true;
					int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::MigaczAutoONOFF , 1 };
					send(tmp, 3);
				}
				else
				{
					MigaczAutoOff = true;
				}
			}

			default:
				break;
			}

			break;
		}
		case EEProm:
		{
			EEPROMDataType EEPROMTyp = (EEPROMDataType)(bufor[2] - 48);

			switch (EEPROMTyp)
			{
			case IMEI:
			{
				
				// numer IMEI zaczyna sie od 6 indeksu
				// a nr slotu to 4

				byte nr[15];
				for (byte i = 0; i < 15; i++)
				{
					nr[i] = (bufor[i+6] -48);
				}
				SetEEPROMValue(EEPROMDataType::IMEI, (bufor[4]-48), nr);
				break;
			}
			case RFID:
			{
				WritetoEEPROMindex = (bufor[4] - 48);
				GetRFIDOn = true;
				WriteRFIDtoEEPROM = true;
				int tmp[4] = { (byte)Typ::EEProm, (byte)EEPROMDataType::RFID, WritetoEEPROMindex, 1 };
				send(tmp, 4);
				break;
			}
			case T_MigaczCzas:
			{
				// Zawsze wartosc ma wynoscic 3 cyfry jak nie to 0
				int value = 100 * (bufor[4] - 48) + 10 * (bufor[6] - 48) + (bufor[8] - 48);
				if ((value <= 255) && (value >= 10))
				{
					byte tmp[1] = { value };
					SetEEPROMValue(EEPROMDataType::T_MigaczCzas, NULL, tmp);
					T_Migacz = 10 * value;

					Serial.print((int)Typ::EEProm);
					Serial.print(';');
					Serial.print((int)EEPROMDataType::T_MigaczCzas);
					Serial.print(';');
					Serial.print(T_Migacz);
					Serial.println('#');
				}
				break;
			}
			case TolerancjaPrzechyluMPU:
			{
				// Zawsze wartosc ma wynoscic 3 cyfry jak nie to 0
				int value = 100 * (bufor[4] - 48) + 10 * (bufor[6] - 48) + (bufor[8] - 48);
				if ((value <= 255) && (value >= 1))
				{
					byte tmp[1] = { value };
					SetEEPROMValue(EEPROMDataType::TolerancjaPrzechyluMPU, NULL, tmp);
					TolerancjaPrzechylu = value;

					Serial.print((int)Typ::EEProm);
					Serial.print(';');
					Serial.print((int)EEPROMDataType::TolerancjaPrzechyluMPU);
					Serial.print(';');
					Serial.print(TolerancjaPrzechylu);
					Serial.println('#');
				}
				break;
			}
			default:
				break;
			}


			break;
		}
		case Auth:
		{
			int tmp2[] = { (byte)Typ::Auth, (byte)EEPROMDataType::IMEI, 1 };
			send(tmp2, 3);
			break;
		}

		case Akcelerometr:
		{
			Akcel AkcelType  = (Akcel)(bufor[2] - 48);
			bool on = (bool)(bufor[4] - 48); // czy on off
			{
				switch (AkcelType)
				{
				case Temperatura:
				{
					if (on)
					{
						GetTempOn = true;
					}
					else
					{
						GetTempOff = true;
					}
					break;
				}
				case Kat:
				{

					if (on)
					{
						GetKatOn = true;
					}
					else
					{
						GetKatOff = true;
					}

					break;
				}
				default:
					break;
				}
			}
			break;
		}
		default:
		{
			break;
		}
		}
	}
	else
	{
		if ((bufor[0] - 48) == (byte)Typ::Auth)
		{

			if ((bufor[2] - 48) == (byte)EEPROMDataType::IMEI)
			{
				byte tmp[15];
				// zaczynam od 4
				for (int i = 0; i < 15; i++)
				{
					tmp[i] = bufor[i + 4];
				}
				if (CheckAuth(EEPROMDataType::IMEI, tmp))
				{// PASSED
					AUTH = true;
					int tmp2[] = { (byte)Typ::Auth, (byte)EEPROMDataType::IMEI, 1 };
					send(tmp2, 3);
				}
				else
				{
					AUTH = false;
					int tmp2[] = { (byte)Typ::Auth, (byte)EEPROMDataType::IMEI, 0 };
					send(tmp2, 3);
				}
			}

		}
	}
}

// Odczytaj dane z EEPROM
byte GetEEPROMValue(EEPROMDataType DataType, byte index)
{
	const byte FirstAddress = 1;
	const byte IMEILenght = 15;
	const byte RFIDLenght = 11;
	const byte MigaczTimeLenght = 1;

	switch (DataType)
	{
	case IMEI:
	{
		byte tmp[IMEILenght];
		if (index > 3)
		{
			return 0;
		}
		byte firstaddr = FirstAddress + 1 + (index * IMEILenght);

		for (size_t i = 0; i < IMEILenght; i++)
		{
			EEPROMvar[i] = EEPROM.read(firstaddr);
			firstaddr++;
		}		
		return IMEILenght;
		break;
	}
	case RFID:
	{
		byte tmp[RFIDLenght];
		if (index > 3)
		{
			return 0;
		}

		byte firstaddr = FirstAddress + 3*IMEILenght + 1 + (index * RFIDLenght);

		EEPROMvar[10] = EEPROM.read(firstaddr + RFIDLenght - 1); // adres na koncu tablicy

		for (size_t i = 0; i < EEPROMvar[10]; i++)
		{
			EEPROMvar[i] = EEPROM.read(firstaddr);
			firstaddr++;
		}
		return RFIDLenght;
		break;
	}
	case T_MigaczCzas:
	{		
		EEPROMvar[0] = { 10*(EEPROM.read(FirstAddress)) };	
		return MigaczTimeLenght;
		break;
	}
	case TolerancjaPrzechyluMPU:
	{
		byte firstaddr = FirstAddress + 3 * IMEILenght + 3 * RFIDLenght + 1;
		EEPROMvar[0] = {(EEPROM.read(FirstAddress)) };
		return 1;
		break;
	}


	default:
		break;
	}
}

// Zapisz dane do EEPROM
void SetEEPROMValue(EEPROMDataType DataType, byte index, byte data[])
{
	const byte FirstAddress = 1;
	const byte IMEILenght = 15;
	const byte RFIDLenght = 11;
	const byte MigaczTimeLenght = 1;

	switch (DataType)
	{
	case IMEI:
	{
		if (index < 3)
		{
			byte firstaddr = FirstAddress + 1 + (index * IMEILenght);
			
			for (byte i = 0; i < IMEILenght; i++)
			{
				//Serial.println(data[i]);
				EEPROM.update(firstaddr, data[i]);
				firstaddr++;
			}			
			int tmp[4] = {(byte) Typ::EEProm, (byte)EEPROMDataType::IMEI, index, 1 };
			send(tmp, 4);
		}
		return;
		break;
	}
	case RFID:
	{
		if (index < 3)
		{
			byte firstaddr = FirstAddress + 3 * IMEILenght + 1 + (index * RFIDLenght);

			byte size = data[10];  // na ostanim miejscu wymagam rozmiar
			EEPROM.update(firstaddr + RFIDLenght - 1, size);			

			for (size_t i = 0; i < size; i++)
			{
				EEPROM.update(firstaddr, data[i]);
				firstaddr++;
			}	
			int tmp[] = { (byte)Typ::EEProm, (byte)EEPROMDataType::RFID ,  index, 2 };
			send(tmp, 4);
		}
		return;
		break;
	}
	case T_MigaczCzas:
	{
		EEPROM.update(FirstAddress, data[0] );
		int tmp[] = { (byte)Typ::EEProm, (byte)EEPROMDataType::T_MigaczCzas};
		send(tmp, 2);
		return;
		break;
	}
	case TolerancjaPrzechyluMPU:
	{
		byte firstaddr = FirstAddress + 3 * IMEILenght + 3*RFIDLenght + 1;
		EEPROM.update(firstaddr, data[0]);
		int tmp[] = { (byte)Typ::EEProm, (byte)EEPROMDataType::TolerancjaPrzechyluMPU };
		send(tmp, 2);
		return;
		break;
	}
	default:
		break;
	}
}

// Autoryzacja
bool CheckAuth(EEPROMDataType Type, byte data[])
{
	switch (Type)
	{
	case IMEI:
	{
		for (size_t i = 0; i < 3; i++)
		{
			GetEEPROMValue(EEPROMDataType::IMEI, i);
			size_t j = 0;
			for (j; j < 14; j++)
			{
				if (!(EEPROMvar[j] == (data[j] - 48))) // jesli sie nie zgadzaja				
				{
					//lancuchy nie zgodne"
					break;
				}
			}
			if (j == 14)
			{
				// Jak nieprzerwanie przez blad tu doszlo to sie zgadza
				return true;
			}

		}
		return false;
		break;
	}
	case RFID:
	{
		for (short a = 0; a < 3; a++)
		{
			GetEEPROMValue(EEPROMDataType::RFID, a);
			if (EEPROMvar[10] == data[10])// jak rozmiary sie  zgadzaja to jest prawie ok
			{
				int b = 0;
				for (b; b < EEPROMvar[10]; b++)
				{
					if (!(EEPROMvar[b] == data[b])) // jesli sie nie zgadzaja
					{
						break;
					}
				}
				if (b == (EEPROMvar[10])) // jak ostatni bit sie zgodzil to okej
				{
					return true;
				}
			}
		}
		return false;
		break;
	}
	default:
		break;
	}
}

// Inicjacja Akcelerometru
// angle - kat
// measure - miara
void PrzechylInit(double angle, double bias, double measure)
{
	if(!mpu.begin(MPU6050_SCALE_2000DPS, MPU6050_RANGE_2G));
	{
		int tmp[] = { (byte)Typ::Errors, (byte)ErrorCodes::Ekspander};
		send(tmp, 2);
		return;
	}
	// Calibrate gyroscope. The calibration must be at rest.
	// If you don't want calibrate, comment this line.
	mpu.calibrateGyro();

	Q_angle = angle;
	Q_bias = bias;
	R_measure = measure;

	K_angle = 0;
	K_bias = 0;

	P[0][0] = 0;
	P[0][1] = 0;
	P[1][0] = 0;
	P[1][1] = 0;

	kt = (double)micros();
}

// Obsluga kata przechylu
double Przechyl()
{
	float accRoll = 0;
	float kalRoll = 0;
	Vector acc = mpu.readNormalizeAccel();
	Vector gyr = mpu.readNormalizeGyro();

	// Calculate Pitch & Roll from accelerometer (deg)
	accRoll = (atan2(acc.YAxis, acc.ZAxis) * 180.0) / M_PI;

	// Kalman filter
	dt = (double)(micros() - kt) / 1000000;

	K_rate = gyr.XAxis - K_bias;
	K_angle += dt * K_rate;

	P[0][0] += dt * (P[1][1] + P[0][1]) + Q_angle * dt;
	P[0][1] -= dt * P[1][1];
	P[1][0] -= dt * P[1][1];
	P[1][1] += Q_bias * dt;

	S = P[0][0] + R_measure;

	K[0] = P[0][0] / S;
	K[1] = P[1][0] / S;

	y = accRoll - K_angle;

	K_angle += K[0] * y;
	K_bias += K[1] * y;

	P[0][0] -= K[0] * P[0][0];
	P[0][1] -= K[0] * P[0][1];
	P[1][0] -= K[1] * P[0][0];
	P[1][1] -= K[1] * P[0][1];

	kt = (double)micros();

	return K_angle;
}
