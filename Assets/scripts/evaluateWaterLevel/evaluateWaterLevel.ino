// FOR WIFI COMMUNICATION
#include <WiFi.h>
#include <WiFiClient.h>
#include <ArduinoJson.h>

const char* ssid = "Wifi";
const char* password = "Password";
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
long lastSendTime = 0; // sending time control
const long sendInterval = 1000;

void setup() {
  Serial.begin(115200);
  

  pinMode(sensorPinMoisture, INPUT);
  pinMode(pumpPin, OUTPUT);
  // pin to demonstrate activation of the pump
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

  // wait 5 s until Bootloader ready and TCP-Sockets configured
  delay(5000);
  Serial.print("Bootloader ready");
}

void loop() {
  // --- 1. check for new connection ---
  // if no client connected, lokk for new client
   WiFiClient client = server.available();
  if (!client) return;
  
  if (!activeClient || !activeClient.connected()) {
    activeClient.stop(); // close old connections safely

  WiFiClient client = server.available();
  // if (!client) return;
  if (activeClient) {
      // If new client connected wait for 1 s
      delay(10); 
      Serial.println("active client");
    }
    // if no new client exit loop() and restart
    if (!activeClient) return; 
  }

  // --- 2. recive data (COMMANDS) ---
  // read only if data avalibale
  if (activeClient.available()) {
    Serial.println("client avalibale");
    String receivedJson = activeClient.readStringUntil('\n');

    if (receivedJson.length() > 0) {
      StaticJsonDocument<256> doc;
      DeserializationError error = deserializeJson(doc, receivedJson);
      
      if (!error) {
        const char* command = doc["command"];
        
        if(command != nullptr && String(command) == "WATER_PLANT"){
          // Pump-Logic 
          digitalWrite(pumpPin, HIGH);
          digitalWrite(LED_BUILTIN, HIGH);
          delay(3000);
          digitalWrite(pumpPin, LOW);
          digitalWrite(LED_BUILTIN, LOW);
        }
      }
    }
  }
  // --- 3. send data (fixed time interval) ---
  // Send only if connected and time has passed
  long now = millis();
  if (activeClient.connected() && (now - lastSendTime >= sendInterval)) {
    lastSendTime = now;
    
    soilMoistureValue = analogRead(sensorPinMoisture);
    lightValue = analogRead(sensorPinSun);
    
    // create JSON-Document and send
    StaticJsonDocument<256> doc;
    doc["soilValue"] = soilMoistureValue;
    doc["lightValue"] = lightValue;

    String outputJson;
    serializeJson(doc, outputJson);
    
    // Send
    activeClient.println(outputJson); 
    Serial.println(outputJson);
  }
}