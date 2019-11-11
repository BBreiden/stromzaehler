// 
// Power sensor
//
// Counts LED blinks produced by the power meter and reports them to a web service
//
// compile for board NodeMCU 1.0 (otherwise pins might not be defined)

#include "webapi/webapi.h"

bool aboveLevel = false;
int level = 300;
int count = 0;

int PIN_INFO = D5;
int PIN_ON = D1;
int PIN_ERROR = D0; 

void setup() {
    int timer = millis();
  
    // setup outputs
    pinMode(PIN_INFO, OUTPUT);
    pinMode(PIN_ON, OUTPUT);
    pinMode(PIN_ERROR, OUTPUT);

    // turn on all outputs to signal initialization
    digitalWrite(PIN_ON, HIGH);
    digitalWrite(PIN_INFO, HIGH);
    digitalWrite(PIN_ERROR, HIGH);
    
    Serial.begin(115200);
    Serial.println(millis()-timer);

    connectToWiFi();
    http.setReuse(true);
    
    Serial.println(millis()-timer);
    
    // turn off error and info to signal end of initialization
    digitalWrite(PIN_ERROR, LOW);
    digitalWrite(PIN_INFO, LOW);
    Serial.println(millis()-timer);
}

void loop() {
  int v = analogRead(A0);
  
  // if voltage is below level, update state
  if (v <= level) {
    aboveLevel = false;
  } else {
    // voltage is above level
    // if state is aboveLevel, we're still in a peak, so return
    if (aboveLevel) return;
    
    // This is a new peak, so send the info to the web server
    aboveLevel = true;
    count++;
    Serial.println("new peak");
  
    sendCount(count);
  }
}

void sendCount(int count) {
  digitalWrite(PIN_INFO, HIGH);
  int startMs = millis();

  int res = sendPowerCount(count);
  if (res != 200) {
    Serial.printf("HTTP POST failed. Return code=%d\n", res);
    if (res > 0) {
      // some kind of HTTP error
      blink(1, PIN_ERROR);
    } else {
      // WiFi failed
      blink(2, PIN_ERROR);
    }
  }
  
  digitalWrite(PIN_INFO, LOW);
  
  Serial.printf("Send time in ms: %d\n", millis()-startMs );
}

/*
 * Blinks the selected pin count times.
 */
void blink(int count, int pin) {
    int state = digitalRead(pin);
    
    while (count > 0) {
      count--;
      digitalWrite(pin, !state);
      delay(100);
      digitalWrite(pin, state);
      delay(100);
    }
}
