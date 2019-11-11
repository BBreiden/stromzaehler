#ifndef WEBAPI_H
#define WEBAPI_H

#include "Arduino.h"
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include "../secrets/secrets.h"

HTTPClient http;
String baseUrl = "http://reni:5000/api/counter";

// returns http result code
int __doSend(int count, char type[]) {
  if (WiFi.isConnected()) {
    Serial.println("connected.");
  } else {
    Serial.println("not connected.");
    WiFi.reconnect();
    while (!WiFi.isConnected()) {
      Serial.print(".");
      delay(100);
    }
    Serial.println("reconnected.");
  }
  
  http.begin(baseUrl);
  http.addHeader("Content-Type", "application/json");
  char message[64];
  sprintf(message, "{\"Count\":%d,\"SourceString\":\"%s\"}", count, type);
  Serial.println(message);
  int code = http.POST(message);
  http.end();
  return code;
}

// returns http result code
int sendPowerCount(int count) {
  int code = __doSend(count, "Power");
  return code;
}

// returns http result code
int sendWaterCount(int count) {
  int code = __doSend(count, "Water");
  return code;
}

void connectToWiFi() {
    WiFi.disconnect();
    WiFi.begin(WLAN_NAME, WLAN_SECRET);
    WiFi.setSleepMode(WIFI_NONE_SLEEP, 30000);
    WiFi.mode(WIFI_STA);
    WiFi.setAutoReconnect(true);
  
    Serial.print("Connecting");
    while (WiFi.status() != WL_CONNECTED)
    {
      delay(500);
      Serial.print(".");
    }
    Serial.println();
}

#endif
