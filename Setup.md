## 1. Unity & Simulation Environment
* **Game Engine:** Unity 6 (`6000.x`)
* **Unity Packages:**
  * **Unity ML-Agents (`com.unity.ml-agents`):** Reinforcement Learning framework (`Agent`, `VectorSensor`, `RayPerceptionSensor`, `ActionBuffers`)
  * **AprilTag for Unity (`jp.keijiro.apriltag`):** Real-time fiducial marker detection (`AprilTag.TagDetector`)
  * **Unity Sentis / Barracuda:** Neural network inference engine for executing the exported `.onnx` model during real-world deployment

---

## 2. Reinforcement Learning Training (Python)
* **Language:** Python 3.10+
* **Core Libraries:**
  * `mlagents` (Python training package matching the Unity ML-Agents C# SDK)
  * `torch` (PyTorch backend for PPO policy optimization)
  * `onnx` (Model export format)
  * `tensorboard` (Training metrics and cumulative reward visualization)

---

## 3. Embedded Firmware
* **IDE:** Arduino IDE 2.x (with ESP32 Board Manager installed)
* **Target Board:** M5Stack AtomS3 (ESP32-S3)
* **Required Libraries:**
  * `M5AtomS3` (Display and hardware initialization)
  * `L298NX2` (Dual DC motor PWM and direction control)
  * `WiFi & WiFiUdp` (Built-in ESP32 Wi-Fi stack for receiving UDP packets)

---

## 4. Network & Hardware Configuration
* **Communication Protocol:** UDP over Local Wi-Fi
* **Camera Input:** Standard USB Overhead Webcam
* **Marker Specification:** AprilTag Tag36h11
