//
// Water sensor
//
// Detects and counts rotations of the water meter and reports them to a web api
//

#include "webapi/webapi.h"


bool alreadyAboveLevel = false;
int level = 500;
int count = 0;
int blinkCount = 10;
int blinkOn = false;
int PIN_INFO = D4;

void setup() {
      int timer = millis();
      
      // setup output
      pinMode(PIN_INFO, OUTPUT);

      // signal setup
      digitalWrite(PIN_INFO, HIGH);

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
      digitalWrite(PIN_INFO, LOW);
}

void loop() {
    int voltage = analogRead(A0);

    bool aboveLevel = voltage > level;
    if (aboveLevel && !alreadyAboveLevel) {
      // we just crossed the threshold
      count++;
      Serial.println(count);
      sendWaterCount(count); 
    }

    alreadyAboveLevel = aboveLevel;

    blink();
    delay(100);
}

void blink() {
  if (blinkCount == 0) {
      blinkCount = 10;

      if (blinkOn) {
        digitalWrite(PIN_INFO, LOW);
      } else {
        digitalWrite(PIN_INFO, HIGH);
      }
      blinkOn = !blinkOn;
    } else {
      blinkCount--;
    }
}
