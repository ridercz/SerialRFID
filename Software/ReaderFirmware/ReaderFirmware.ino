/*
	Altairis Serial RFID Reader Firmware for Arduino
	Copyright (c) Michal A. Valasek - Altairis, 2017
	https://github.com/ridercz/SerialRFID/

	This firmware is based on public domain examples of the MFRC522 library
	https://github.com/miguelbalboa/rfid
*/

#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN 10
#define RST_PIN 9
#define BUZZER_PIN 3
#define RS232_SPEED 115200

MFRC522 rfid(SS_PIN, RST_PIN);
MFRC522::MIFARE_Key key;

void setup() {
	// Initialize serial and wait for it
	Serial.begin(RS232_SPEED);
	while (!Serial);

	// Init SPI
	SPI.begin();

	// Init MFRC-522
	rfid.PCD_Init();

	// Create default key
	for (byte i = 0; i < 6; i++) {
		key.keyByte[i] = 0xFF;
	}

#ifdef BUZZER_PIN
	// Play startup tones
	tone(BUZZER_PIN, 1000, 100);
	delay(100);
	tone(BUZZER_PIN, 1500, 100);
	delay(100);
	tone(BUZZER_PIN, 2000, 100);
	delay(100);
#endif // BUZZER_PIN

	// Print header
	Serial.print(F("Version: 1.0.0 Altairis Serial RFID Reader Firmware for Arduino\r\n"));
	Serial.print(F("Key: "));
	printHex(key.keyByte, MFRC522::MF_KEY_SIZE);
	Serial.print("\r\n");
}

void loop() {
	// Look for new cards
	if (!rfid.PICC_IsNewCardPresent())
		return;

	// Verify if the NUID has been readed
	if (!rfid.PICC_ReadCardSerial())
		return;

	// Check is the PICC of Classic MIFARE type
	MFRC522::PICC_Type piccType = rfid.PICC_GetType(rfid.uid.sak);
	if (piccType != MFRC522::PICC_TYPE_MIFARE_MINI &&
		piccType != MFRC522::PICC_TYPE_MIFARE_1K &&
		piccType != MFRC522::PICC_TYPE_MIFARE_4K) {

		Serial.print(F("Error: Unsupported card type: "));
		Serial.print(rfid.PICC_GetTypeName(piccType));
		Serial.println();

#ifdef BUZZER_PIN
		tone(BUZZER_PIN, 500, 500);
		delay(500);
#endif // BUZZER_PIN

		return;
	}

	// Print UID
	Serial.print("Card: ");
	printHex(rfid.uid.uidByte, rfid.uid.size);
	Serial.print(" ");
	Serial.print(rfid.PICC_GetTypeName(piccType));
	Serial.print(F("\r\n"));

#ifdef BUZZER_PIN
	tone(BUZZER_PIN, 1200, 200);
	delay(200);
#endif // BUZZER_PIN

	// Halt PICC
	rfid.PICC_HaltA();

	// Stop encryption on PCD
	rfid.PCD_StopCrypto1();
}

void printHex(byte *buffer, byte bufferSize) {
	for (byte i = 0; i < bufferSize; i++) {
		Serial.print(buffer[i] < 0x10 ? "0" : "");
		Serial.print(buffer[i], HEX);
	}
}
