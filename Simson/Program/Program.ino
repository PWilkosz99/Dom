/*
 Name:		Program.ino
 Created:	22.04.2017 12:58:20
 Author:	Szymon
*/

//Dolaczenie odpowiednich bibliotek
#include <Wire.h>
#include <PCF8574.h>
#include <MPU6050.h>
#include <EEPROM.h>

#pragma region PINOUT
#pragma region Ekspander

#define I_LEWY_MIGACZ_E_PIN 1
#define I_PRAWY_MIGACZ_E_PIN 2
#define I_SWIATLO_MIJANIA_E_PIN 3
#define I_SWIATLO_DROGOWE_E_PIN 4
#define I_BLINDA_E_PIN 5

#define I_SWIATLO_STOP_PRZOD_E_PIN 6
#define I_SWIATLO_STOP_TYL_E_PIN 7

#pragma endregion

#pragma region Mikrokontroler

#define O_ZAPLON_PIN 4
#define O_SWIATLO_MIJANIA_PIN 5
#define O_SWIATLO_DROGOWE_PIN 9

#define O_POSTOJOWKA_PIN A0
#define O_LEWY_MIGACZ_PIN A1
#define O_PRAWY_MIGACZ_PIN A2
#define O_SWIATLO_TYL_PIN 8
#define O_SWIATLO_STOP_PIN 7
#define O_DZIEN_NOC_PIN 6
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

#pragma endregion

#pragma region ENUM y

//Kody komunikacyjne
enum Typ
{
	Swiatla = 2,
	Migacz = 3,
	EEProm = 6,
	Dodatk = 7,
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

	Temperatura = 1,
	Kat = 9,
};

enum EEPROMDataType
{
	T_MigaczCzas = 5,
	TolerancjaPrzechyluMPU = 6,
	EKeepAlive = 4,
	Migaczautooff = 7,
};

enum ErrorCodes
{
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
	int tmp[] = { (int)Typ::Errors, (int)ErrorCodes::Ekspander };
	send(tmp, 2);
	// Rozpoczecie pracy z akcelerometrem
	PrzechylInit(0.01, 0.03, 0.3);
	//Ustawienie funkcji pin�w
#pragma region Zadeklarowanie I/O
#pragma region Ekspander
	Exp.pinMode(I_LEWY_MIGACZ_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_PRAWY_MIGACZ_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_MIJANIA_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_DROGOWE_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_BLINDA_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_STOP_PRZOD_E_PIN, INPUT_PULLUP);
	Exp.pinMode(I_SWIATLO_STOP_TYL_E_PIN, INPUT_PULLUP);
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
	pinMode(O_SWIATLO_DROGOWE_PIN, OUTPUT);
#pragma endregion
#pragma endregion

#pragma region Pobranie wartosci zmiennych z EEPROM

	int value = GetEEPROMValue(EEPROMDataType::T_MigaczCzas, NULL);
	if (value == 255)
	{
		T_Migacz = 500;
	}
	else
	{
		T_Migacz = value;
	}

	int value2 = GetEEPROMValue(EEPROMDataType::TolerancjaPrzechyluMPU, NULL);
	if (value2 == 255)
	{
		TolerancjaPrzechylu = 20;
	}
	else
	{
		TolerancjaPrzechylu = value2;
	}

	int value3 = GetEEPROMValue(EEPROMDataType::Migaczautooff, NULL);
	if (value3 == 255)
	{
		MigaczAutoOff = true;
	}
	else
	{
		if (value3 == 1)
		{
			MigaczAutoOn = true;
		}
		else
		{
			MigaczAutoOff = true;
		}
	}

	if (GetEEPROMValue(EEPROMDataType::EKeepAlive, NULL) == 1)
	{
		ZaplonOff = true;
		int tmp[] = { (int)Typ::EEProm, (int)EEPROMDataType::EKeepAlive , 1 };// wyslij info ze pasowalo by zdjac blokade
		send(tmp, 3);
	}
#pragma endregion

	DzienON = true; // domyslnie tryb dnia
	SwiatloTylOn = true; // domyslnie zalaczone tyle swiatlo
	PostojowkaOn = true; // domyslnie postojowka

}

void loop()
{
#pragma region Obsługa manetki
	// Swiatlo mijania
	if (S_SwiatloMijania != Exp.digitalRead(I_SWIATLO_MIJANIA_E_PIN))
	{
		S_SwiatloMijania = Exp.digitalRead(I_SWIATLO_MIJANIA_E_PIN);
		if (!S_SwiatloMijania)  // jesli zwarty do masy
		{
			PostojowkaOff = true;
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
			PostojowkaOff = true;
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
#pragma endregion

#pragma region Funkcje

#pragma region Zaplon
	if (ZaplonOn)
	{
		digitalWrite(O_ZAPLON_PIN, LOW);
		int tmp[] = { (byte)Typ::Dodatk, (byte)Dodatkowe::EngineStart, 1 };
		send(tmp, 3);
		ZaplonOn = false;
	}
	if (ZaplonOff)
	{
		digitalWrite(O_ZAPLON_PIN, HIGH);
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
		PoprzedniStanSwiatel[1] = digitalRead(O_SWIATLO_DROGOWE_PIN);
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
		digitalWrite(O_SWIATLO_DROGOWE_PIN, HIGH);
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
		digitalWrite(O_SWIATLO_DROGOWE_PIN, LOW);
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
			SetEEPROMValue(EEPROMDataType::EKeepAlive, 1); // zapisz bladd zeby po restarcie nadal blokowal zaplon
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
			int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::Temperatura,  mpu.readTemperature() };
			send(tmp, 3);

		}
	}
	if (GetTempOff)
	{
		GetTempOn = false;
		GetTempOff = false;
		int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::Temperatura, 0 };
		send(tmp, 3);
	}
#pragma endregion

#pragma region Pobierz kat w pionowej plaszczyznie
	if (GetKatOn)
	{
		if (PrzechylTime + AccelRefreshTime < millis())
		{
			Serial.print((int)Typ::Dodatk);
			Serial.print(';');
			Serial.print((int)Dodatkowe::Kat);
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
			else if (BylPrzechylony)  // jesli byl przechylony a teraz jest wyprostowany
			{
				LMigaczOff = true;
				PMigaczOff = true;
				BylPrzechylony = false;
				int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::MigaczAutoONOFF , 1 };
				send(tmp, 3);
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
		int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::MigaczAutoONOFF , 0 };
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

	Przechyl();
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
	
	//Serial.print("9: ");
	//for (int i = 0; i < buforLenght; i++)
	//{
	//	Serial.print((char)bufor[i]);
	//}
	//Serial.println("");
		

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
			int tmp[] = { (int)Typ::Dodatk, (int)Dodatkowe::Sync , digitalRead(O_LEWY_MIGACZ_PIN), digitalRead(O_PRAWY_MIGACZ_PIN), digitalRead(O_SWIATLO_MIJANIA_PIN), digitalRead(O_SWIATLO_DROGOWE_PIN), digitalRead(O_SWIATLO_TYL_PIN), digitalRead(O_SWIATLO_STOP_PIN), digitalRead(O_POSTOJOWKA_PIN), digitalRead(O_DZIEN_NOC_PIN), !digitalRead(O_ZAPLON_PIN) };
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
			break;
		}

		case Temperatura:
		{
			if ((bool)(bufor[4] - 48))
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
			if ((bool)(bufor[4] - 48))
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

		break;
	}
	case EEProm:
	{
		EEPROMDataType EEPROMTyp = (EEPROMDataType)(bufor[2] - 48);

		switch (EEPROMTyp)
		{
		case T_MigaczCzas:
		{
			// Zawsze wartosc ma wynoscic 3 cyfry jak nie to 0
			// Wartosc ma przyjsc podzielona przez 10
			int value = 100 * (bufor[4] - 48) + 10 * (bufor[6] - 48) + (bufor[8] - 48);
			if ((value <= 255) && (value >= 10))
			{
				SetEEPROMValue(EEPROMDataType::T_MigaczCzas, value);
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
				SetEEPROMValue(EEPROMDataType::TolerancjaPrzechyluMPU, value);
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
		case EKeepAlive:
		{
			// Zawsze wartosc ma wynoscic 1 cyfre

			int value = bufor[4] - 48;
			if (value == 1)
			{
				SetEEPROMValue(EEPROMDataType::EKeepAlive, value);
				Serial.print((int)Typ::EEProm);
				Serial.print(';');
				Serial.print((int)EEPROMDataType::EKeepAlive);
				Serial.print(';');
				Serial.print(value);
				Serial.println('#');
			}
			else if (value == 0)
			{
				SetEEPROMValue(EEPROMDataType::EKeepAlive, value);
				Serial.print((int)Typ::EEProm);
				Serial.print(';');
				Serial.print((int)EEPROMDataType::EKeepAlive);
				Serial.print(';');
				Serial.print(value);
				Serial.println('#');
			}
			break;
		}

		case Migaczautooff:
		{
			// Zawsze wartosc ma wynoscic 1 cyfre

			int value = bufor[4] - 48;
			if (value == 1)
			{
				SetEEPROMValue(EEPROMDataType::Migaczautooff, value);
				Serial.print((int)Typ::EEProm);
				Serial.print(';');
				Serial.print((int)EEPROMDataType::Migaczautooff);
				Serial.print(';');
				Serial.print(value);
				Serial.println('#');
				MigaczAutoOn = true;
			}
			else if (value == 0)
			{
				SetEEPROMValue(EEPROMDataType::Migaczautooff, value);
				Serial.print((int)Typ::EEProm);
				Serial.print(';');
				Serial.print((int)EEPROMDataType::Migaczautooff);
				Serial.print(';');
				Serial.print(value);
				Serial.println('#');
				MigaczAutoOff = true;
			}
			break;
		}

		default:
			break;
		}


		break;
	}	
	default:
	{
		break;
	}
	}
}



// Inicjacja Akcelerometru
// angle - kat
// measure - miara
void PrzechylInit(double angle, double bias, double measure)
{
	mpu.begin(MPU6050_SCALE_2000DPS, MPU6050_RANGE_2G);

	int tmp[] = { (byte)Typ::Errors, (byte)ErrorCodes::MPU };
	send(tmp, 2);
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

// Odczytaj dane z EEPROM
int GetEEPROMValue(EEPROMDataType DataType, byte index)
{
	const byte T_MigaczCzas_Addr = 1;
	const byte TolerancjaPrzechyluMPU_Addr = 2;
	const byte KeepAlive_Addr = 3;
	const byte MigaczAutoOff_Addr = 4;

	switch (DataType)
	{

	case T_MigaczCzas:
	{
		return 10 * (EEPROM.read(T_MigaczCzas_Addr));
		break;
	}
	case TolerancjaPrzechyluMPU:
	{
		return EEPROM.read(TolerancjaPrzechyluMPU_Addr);
		break;
	}
	case EKeepAlive:
	{
		return EEPROM.read(KeepAlive_Addr);
	}
	case Migaczautooff:
	{
		return EEPROM.read(MigaczAutoOff_Addr);
	}
	default:
	{
		break;
	}
	}
}

// Zapisz dane do EEPROM
void SetEEPROMValue(EEPROMDataType DataType, byte data)
{
	const byte T_MigaczCzas_Addr = 1;
	const byte TolerancjaPrzechyluMPU_Addr = 2;
	const byte KeepAlive_Addr = 3;
	const byte MigaczAutoOff_Addr = 4;

	switch (DataType)
	{
	case T_MigaczCzas:
	{
		EEPROM.update(T_MigaczCzas_Addr, data);
		int tmp[] = { (byte)Typ::EEProm, (byte)EEPROMDataType::T_MigaczCzas };
		send(tmp, 2);
		return;
		break;
	}
	case TolerancjaPrzechyluMPU:
	{
		EEPROM.update(TolerancjaPrzechyluMPU_Addr, data);
		int tmp[] = { (byte)Typ::EEProm, (byte)EEPROMDataType::TolerancjaPrzechyluMPU };
		send(tmp, 2);
		return;
		break;
	}
	case KeepAlive:
	{
		EEPROM.update(KeepAlive_Addr, data);
		int tmp[] = { (byte)Typ::EEProm, (byte)EEPROMDataType::EKeepAlive };
		send(tmp, 2);
		return;
		break;
	}
	case Migaczautooff:
	{
		EEPROM.update(MigaczAutoOff_Addr, data);
		int tmp[] = { (byte)Typ::EEProm, (byte)EEPROMDataType::Migaczautooff };
		send(tmp, 2);
		return;
		break;
	}
	default:
		break;
	}
}
