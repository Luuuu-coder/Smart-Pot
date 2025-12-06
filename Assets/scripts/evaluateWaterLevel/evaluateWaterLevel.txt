

int sensorPinMoisture = 1;
int sensorPinSun = 2;
int interactionPinWater = 3;
int interactionPinAutomation;

int soilMoistureValue;

int pumpPin = 5;

int sensorValue;
//bool automationflag;

char unityInput; 

const int AirValue = 520;   //you need to replace this value with Value_1
const int WaterValue = 260;  //you need to replace this value with Value_2
int intervals = (AirValue - WaterValue)/3;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(sensorPinMoisture, INPUT);
  pinMode(interactionPinWater, INPUT);
  pinMode(interactionPinAutomation, INPUT);
  pinMode(pumpPin, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);

}

void loop() {

  
  soilMoistureValue = analogRead(sensorPinMoisture);  //put Sensor insert into soil
  if(soilMoistureValue > WaterValue && soilMoistureValue < (WaterValue + intervals))
  {
    // Serial.println("Wet");
    // Serial.write(0);
    Serial.println(0);
    Serial.flush();
    delay(200);
  }
  else if(soilMoistureValue > (WaterValue + intervals) && soilMoistureValue < (AirValue - intervals))
  {
    // Serial.println("Medium");
    // Serial.write(1);
    Serial.println(1);
    Serial.flush();
    delay(200);
  }
  else if(soilMoistureValue < AirValue && soilMoistureValue > (AirValue - intervals))
  {
    // Serial.println("Dry");
    // Serial.write(2);
    Serial.println(2);
    Serial.flush();
    delay(200);

    // Serial.println("Turning pump ON");
    // digitalWrite(pumpPin, HIGH);
    delay(3000); // Wait three seconds
  
    //Serial.println("Turning pump OFF");
    //digitalWrite(pumpPin, LOW);
    //delay(3000); // Wait three seconds
  }
  
  if(Serial.available()>0){
    unityInput = Serial.read();
    if(unityInput=='A'){
        Serial.println(91);
        digitalWrite(pumpPin, HIGH);
        digitalWrite(LED_BUILTIN, HIGH);

        delay(3000); // Wait three seconds
      
        Serial.println("Turning pump OFF");
        digitalWrite(pumpPin, LOW);
        digitalWrite(LED_BUILTIN, LOW);
        delay(1000); // Wait three seconds
    }
  }
}