//
// Water sensor
//
// Detects and counts rotations of the water meter and reports them to a web api
//

#include "webapi/webapi.h"


bool alreadyAboveLevel = false;
int level = 500;
int count = 0;
int PIN_INFO = D4;
int ledOn = LOW;    // don't ask me why, but they are inverted on this board
int ledOff = HIGH;

void setup() {
      int timer = millis();
      
      // setup output
      pinMode(PIN_INFO, OUTPUT);
      
      // signal setup
      digitalWrite(PIN_INFO, ledOn);
      
      Serial.begin(115200);
      Serial.println(millis() - timer);
      Serial.println("starting.");
      
      connectToWiFi();
      Serial.println(millis() - timer);
      Serial.println("connected to wifi");
      
      int v0 = analogRead(A0);
      alreadyAboveLevel = v0 > level;
      
      // signal setup done
      Serial.println(millis() - timer);
      Serial.println("setup done."); 
      digitalWrite(PIN_INFO, ledOff);
}

void loop() {
    int voltage = analogRead(A0);

    bool aboveLevel = voltage > level;
    if (aboveLevel && !alreadyAboveLevel) {
      // we just crossed the threshold
      digitalWrite(PIN_INFO, ledOn);
      count++;
      Serial.println(count);
      int res = sendWaterCount(count); 
      if (res != 200) {
        if (res > 0) {
          blink(2);
        } else {
          blink(5);
        }
      }
    }
    digitalWrite(PIN_INFO, ledOff);

    alreadyAboveLevel = aboveLevel;
    delay(100);
}

void blink(int count) {

  while (count > 0) {
    digitalWrite(PIN_INFO, ledOff);
    delay(100);
    digitalWrite(PIN_INFO, ledOn);
    delay(300);
    count--;
  }

  digitalWrite(PIN_INFO, ledOff);
}
