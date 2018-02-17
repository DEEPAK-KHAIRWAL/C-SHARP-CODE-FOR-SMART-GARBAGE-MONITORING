# C--CODE-FOR-SMART-GARBAGE-MONITORING
ESP8266 is connected to the Arduino. 
Since, ESP8266 runs on 3.3V and will get damaged from Arduino’s 5V(if supplied directly); we make a voltage divider using 3 1k resistors.  Connect the RX pin through resistors to pin 11 and TX pin to pin 10 of Arduino 
The connections of HC-SR04 with Arduino:
   1. Connect VCC and GND with 5V and GND of Arduino respectively.
   2. Connect TRIG and echo at pin 8 and 9 respectively.
