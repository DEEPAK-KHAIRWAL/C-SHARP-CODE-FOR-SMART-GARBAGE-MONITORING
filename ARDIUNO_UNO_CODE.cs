#include <SoftwareSerial.h>
#define DEBUG true
#define trigPin 13
#define echoPin 12
SoftwareSerial esp8266(2,3); //  RX  is pin 2,  TX Arduino line is pin 3.

long duration, distance;

void setup()
{

  Serial.begin(9600);
  esp8266.begin(9600);
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);


  sendData("AT\r\n",1000,DEBUG);
  sendData("AT+RST\r\n",2000,DEBUG); // rst


  int temp =0;
  while(!esp8266.find("OK") && temp<2){
  sendData("AT+CWMODE=3\r\n",1000,DEBUG);
  temp=temp+1;
  } //  access point

  sendData("AT+CIFSR\r\n",1000,DEBUG); // get ip address
  sendData("AT+CIPMUX=1\r\n",1000,DEBUG); // configure for multiple connections
  sendData("AT+CIPSERVER=1,80\r\n",1000,DEBUG); // turn on server on port 80


}

void loop()
{


  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);

  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  duration = pulseIn(echoPin, HIGH);
  distance = (duration/2) / 29.1;


  delay(1000);

  if(esp8266.available()) // check if the esp is sending a message
  {

   /* while(esp8266.available())
    {
      // The esp has data so display its output to the serial window
      char c = esp8266.read(); // read the next character.
      Serial.write(c);
    }*/

    if(esp8266.find("+IPD,"))
    {
     delay(1000);

     int connectionId = esp8266.read()-48;

     String webpage = "<html><body><h1>ONLINE GARBAGE MANAGEMENT INTERFACE</h1><h2> Bin 1 Status </h2><div style='height:400px; width:200px; position:relative; background-color:rgb(230,230,230);'><div style='height:";

     if(distance>=0 && distance<=20){
      webpage+= 350 - (400*(distance/20));
     }
     else if(distance>20){
      webpage+= 50;
     }
     else{
      webpage+= 350;
     }

     webpage+= "px ; width:200px ;position:absolute; background-color:green; bottom:0'></div></div>";

      if (distance<15)
      {
        webpage+= " <i>Trash can is Full  : <b>90%</b></i> ";
      }
      else{
        webpage+= " <i>Trash can is empty : <b>10%</b></i>";
      }
     webpage+= "<h3>BIN ID : 10287</h3> <h3>Location : SJT Ground Floor</h3> </body></html>";

     //--------------------------------------------------------- ---------------------------------------------------------  ---------------------------------------------------------

     String cipSend = "AT+CIPSEND=";
     cipSend += connectionId;
     cipSend += ",";
     cipSend +=webpage.length();
     cipSend +="\r\n";
     sendData(cipSend,1000,DEBUG);
     sendData(webpage,1000,DEBUG);
     String closeCommand = "AT+CIPCLOSE=";
     closeCommand+=connectionId; // append connection id
     closeCommand+="\r\n";

     sendData(closeCommand,3000,DEBUG);
    }
  }

}


String sendData(String command, const int timeout, boolean debug)
{
    String response = "";

    esp8266.print(command);

    long int time = millis();

    while( (time+timeout) > millis())
    {
      while(esp8266.available())
      {
        char c = esp8266.read();
        response+=c;
      }
    }

    if(debug)
    {
      Serial.print(response);
    }

    return response;
}
