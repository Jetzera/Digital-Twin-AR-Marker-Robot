#include <M5AtomS3.h> // 
#include <L298NX2.h> // 
#include <WiFi.h> // 
#include <WiFiUdp.h> // 

// ขาสำหรับเชื่อมต่อมอเตอร์
const int ENA = 38; // 
const int ENB = 39; // 
const int IN1 = 5;  // 
const int IN2 = 6;  // 
const int IN3 = 7;  // 
const int IN4 = 8;  //
const unsigned long TIMEOUT = 5000; //

L298NX2 mybot(ENA, IN1, IN2, ENB, IN3, IN4); //

// ตั้งค่า Network
const char* ssid = "YOUR_WIFI_SSID"; //
const int udpPort = 5005; // *** ตั้งให้ตรงกับตัวแปร port ใน Unity ***

WiFiUDP udp; //
unsigned long lastRecvTime = 0; //

void setup() {
  auto cfg = M5.config();
  AtomS3.begin(cfg); //

  AtomS3.Display.fillScreen(TFT_BLACK); //
  AtomS3.Display.setTextColor(TFT_WHITE); //
  AtomS3.Display.setTextSize(1); //
  AtomS3.Display.setCursor(0, 10); //
  AtomS3.Display.println("Robot Connecting..."); //

  Serial.begin(115200); //

  WiFi.mode(WIFI_STA); //
  WiFi.begin(ssid); //

  while (WiFi.status() != WL_CONNECTED) { //
    delay(500); //
    Serial.print("."); //
  }

  // อัปเดตหน้าจอเมื่อต่อ WiFi สำเร็จ
  AtomS3.Display.fillScreen(TFT_BLUE); //
  AtomS3.Display.setTextColor(TFT_WHITE); //
  AtomS3.Display.setCursor(5, 10); //
  AtomS3.Display.println("ROBOT READY"); //
  AtomS3.Display.setCursor(5, 30); //
  AtomS3.Display.println("Connect to this IP:"); //
  AtomS3.Display.setCursor(5, 50); //
  AtomS3.Display.setTextSize(2); //
  AtomS3.Display.println(WiFi.localIP()); //

  Serial.println("\nConnected!"); //
  Serial.print("IP Address : "); //
  Serial.println(WiFi.localIP()); //

  udp.begin(udpPort); //
  mybot.setSpeed(50); //
  mybot.stop(); //
}

void loop() {
  AtomS3.update(); //

  int packetSize = udp.parsePacket(); //
  
  if (packetSize) {
    char incomingPacket[255]; 
    int len = udp.read(incomingPacket, 255);
    
    if (len > 0) {
      incomingPacket[len] = 0; // ปิด string
      lastRecvTime = millis(); // อัปเดตเวลาล่าสุดที่ได้รับข้อมูล
      
      // อ่านค่าตัวอักษรตัวแรกที่ได้รับ
      char action = incomingPacket[0];
      
      // ประมวลผลคำสั่งจาก ML-Agents (Unity)
      if (action == '0') {
        mybot.stop(); 
      } 
      else if (action == '1') {
        mybot.forward(); //
      } 
      else if (action == '2') {
        // เลี้ยวซ้าย 
        mybot.forwardB(); //
        mybot.backwardA(); //
      } 
      else if (action == '3') {
        // เลี้ยวขวา
        mybot.forwardA(); //
        mybot.backwardB(); //
      }
    }
  }

  // ระบบหยุดอัตโนมัติหากสัญญาณขาดหายเกินกำหนด
  if (millis() - lastRecvTime > TIMEOUT) { //
    mybot.stop(); //
  }
}
