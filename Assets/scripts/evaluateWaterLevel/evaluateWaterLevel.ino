#include <WiFi.h>
#include <WiFiClient.h>
#include <ArduinoJson.h>

// const char* ssid = "SmartPotAP";
// const char* password = "SmartPotAP";
WiFiServer server(3333);
// IPAddress local_ip(192, 168, 4, 1);
// IPAddress gateway_ip(192, 168, 4, 1);
// IPAddress subnet_mask(255, 255, 255, 0);

int sensorPinMoisture = 32;
int sensorPinSun = 33;
int LED_BUILTIN = 2;
int pumpPin = 5;


int soilMoistureValue;
int lightValue;

//bool automationflag;
String unityInput;

WiFiClient activeClient;
long lastSendTime = 0; // Für zeitgesteuertes Senden
const long sendInterval = 1000;

void setup() {
  Serial.begin(115200);
  

  pinMode(sensorPinMoisture, INPUT);
  pinMode(pumpPin, OUTPUT);
  // pin to semontrate activation of the pump
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);
  // 1. Starte Access Point (AP)
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
  }
  Serial.print("IP: ");
  Serial.println(WiFi.localIP());

  server.begin();

  // WICHTIG: 5 Sekunden warten, bis der Bootloader fertig ist und die TCP-Sockets sauber sind
  delay(5000);
  Serial.print("Bootloader ready");
}

void loop() {
  // --- 1. AUF NEUE VERBINDUNG PRÜFEN ---
  // Wenn kein Client aktiv ist, prüfen, ob ein neuer verfügbar ist
   WiFiClient client = server.available();
  if (!client) return;
  
  if (!activeClient || !activeClient.connected()) {
    activeClient.stop(); // Alte, getrennte Verbindung sicher beenden

  WiFiClient client = server.available();
  // if (!client) return;
  if (activeClient) {
      // Wenn ein neuer Client verbunden ist, kurz warten
      delay(10); 
      Serial.println("active client");
    }
    // Wenn kein neuer Client da ist, die loop() verlassen und neu starten
    if (!activeClient) return; 
  }

  // --- 2. DATEN EMPFANGEN (COMMANDS) ---
  // Nur lesen, wenn Daten verfügbar sind
  if (activeClient.available()) {
    Serial.println("client avalibale");
    String receivedJson = activeClient.readStringUntil('\n');

    if (receivedJson.length() > 0) {
      StaticJsonDocument<256> doc;
      DeserializationError error = deserializeJson(doc, receivedJson);
      
      if (!error) {
        const char* command = doc["command"];
        
        if(command != nullptr && String(command) == "WATER_PLANT"){
          // Pumpen-Logik (bleibt blockierend, da es ein Befehl ist)
          digitalWrite(pumpPin, HIGH);
          digitalWrite(LED_BUILTIN, HIGH);
          delay(3000);
          digitalWrite(pumpPin, LOW);
          digitalWrite(LED_BUILTIN, LOW);
        }
      }
    }
  }
  // --- 3. DATEN SENDEN (ZEITGESTEUERT) ---
  // Senden nur, wenn eine aktive Verbindung besteht UND die Zeit vergangen ist
  long now = millis();
  if (activeClient.connected() && (now - lastSendTime >= sendInterval)) {
    lastSendTime = now;
    
    soilMoistureValue = analogRead(sensorPinMoisture);
    lightValue = analogRead(sensorPinSun);
    
    // JSON-Dokument erstellen und senden (wie zuvor)
    StaticJsonDocument<256> doc;
    doc["soilValue"] = soilMoistureValue;
    doc["lightValue"] = lightValue;

    String outputJson;
    serializeJson(doc, outputJson);
    
    // Senden
    activeClient.println(outputJson); 
    Serial.println(outputJson);
    // WICHTIG: KEIN delay HIER! Die loop() läuft weiter.
  }
}