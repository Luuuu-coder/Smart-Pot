#include <ArduinoJson.h>

int sensorPinMoisture = 32;
int sensorPinSun = 33;
#define LED_BUILTIN 2
int pumpPin = 25;

int soilMoistureValue;
int lightValue;

long lastSendTime = 0;
const long sendInterval = 1000; // ms

void setup() {
  Serial.begin(115200);
  delay(2000); // wichtig für stabile USB-Verbindung

  pinMode(sensorPinMoisture, INPUT);
  pinMode(sensorPinSun, INPUT);
  pinMode(pumpPin, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);

  digitalWrite(pumpPin, LOW);
  digitalWrite(LED_BUILTIN, LOW);

  Serial.println("ESP32 Serial ready");
}

void loop() {
  // --- 1. DATEN EMPFANGEN (Commands über Serial) ---
  if (Serial.available()) {
    String receivedJson = Serial.readStringUntil('\n');

    if (receivedJson.length() > 0) {
      StaticJsonDocument<256> doc;
      DeserializationError error = deserializeJson(doc, receivedJson);

      if (!error) {
        const char* command = doc["command"];

        if (command && String(command) == "WATER_PLANT") {
          Serial.println("Pump activated");

          digitalWrite(pumpPin, HIGH);
          digitalWrite(LED_BUILTIN, HIGH);
          delay(3000);  // bewusst blockierend
          digitalWrite(pumpPin, LOW);
          digitalWrite(LED_BUILTIN, LOW);
        }
      }
    }
  }

  // --- 2. DATEN SENDEN (zeitgesteuert) ---
  long now = millis();
  if (now - lastSendTime >= sendInterval) {
    lastSendTime = now;

    soilMoistureValue = analogRead(sensorPinMoisture);
    lightValue = analogRead(sensorPinSun);

    StaticJsonDocument<256> doc;
    doc["soilValue"] = soilMoistureValue;
    doc["lightValue"] = lightValue;

    String outputJson;
    serializeJson(doc, outputJson);

    Serial.println(outputJson); // wichtig: \n als Paketende
  }
}
