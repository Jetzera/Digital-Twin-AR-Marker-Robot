# Digital Twin Robot Control using AR Markers

A prototype Digital Twin control system for a two-wheeled differential-drive mobile robot, integrating **6-DoF AprilTag visual localization**, **Unity 3D simulation**, **Reinforcement Learning (Unity ML-Agents / PPO)**, and **low-latency Wi-Fi UDP communication** on an **ESP32 (M5AtomS3)** microcontroller.

> **Research Affiliation:** Developed during a 4-month undergraduate research exchange at the **Co-creative Robotics Laboratory**, Institute of Technologists (Saitama, Japan), under the supervision of **Assoc. Prof. Dr. Hosoai Shintaro** (April – July 2026).

---

## System Architecture

1. **Physical State Extraction:** An overhead webcam tracks `Tag36h11` AprilTag fiducial markers mounted on the physical robot and target location to compute 3D pose coordinates in real time.
2. **Digital Twin Synchronization:** Extracted coordinates and orientation angles are streamed into Unity 3D to mirror the physical robot's state frame-by-frame.
3. **AI Policy Inference:** A neural network trained via Proximal Policy Optimization (PPO) in Unity ML-Agents observes the robot's state and outputs discrete navigation commands (`0: Stop`, `1: Forward`, `2: Turn Left`, `3: Turn Right`).
4. **Wireless Actuation:** Control commands are transmitted over Wi-Fi via **UDP** to an **M5AtomS3 (ESP32)** microcontroller driving dual DC motors through an **L298NX2** H-bridge driver.

---

## Hardware Pinout & Specifications

* **Microcontroller:** M5AtomS3 (ESP32-S3)
* **Motor Driver:** L298NX2 Dual H-Bridge
* **Chassis:** Custom 3D-Printed Differential-Drive Frame (Designed in Autodesk Fusion 360)
* **Fiducial Marker:** AprilTag (`Tag36h11`, Tag Size: `0.05 m`, Robot ID: `1`, Target ID: `2`)

| L298NX2 Pin | M5AtomS3 GPIO | Function |
| :---: | :---: | :--- |
| `ENA` | `GPIO 38` | Left Motor PWM Speed |
| `IN1` / `IN2` | `GPIO 5` / `GPIO 6` | Left Motor Direction |
| `ENB` | `GPIO 39` | Right Motor PWM Speed |
| `IN3` / `IN4` | `GPIO 7` / `GPIO 8` | Right Motor Direction |

---

## Repository Structure

* `Unity_Scripts/`
  * `SoAgent.cs`: Reinforcement Learning training environment and reward shaping in Unity simulation.
  * `NuSoAgent.cs`: Deployment agent script synchronizing real-world AprilTag poses and sending UDP actions.
  * `DetectionTest.cs`: Real-time webcam capture and AprilTag 6-DoF pose estimation.
  * `RobotUDPSender.cs`: Low-latency UDP socket client transmitting discrete action IDs.
  * `GuiRobotAgent.cs`: On-screen UI overlay displaying real-time `Step` count and `Cumulative Reward`.
* `ESP32_Firmware/`
  * `NewUDP.ino`: Arduino/C++ firmware for M5AtomS3 receiving UDP packets with a 5-second safety timeout.
* `Config/`
  * `RobotDrive.yaml`: PPO hyperparameter configuration (`learning_rate: 0.0003`, `batch_size: 512`, `hidden_units: 128`, `num_layers: 2`, `max_steps: 1,000,000`).
* `Model/`
  * `RobotDrive.onnx`: Exported ONNX neural network policy trained in Unity ML-Agents.
* `Docs/`
  * `Presentation_JP.pdf`: Research presentation slides in Japanese (13 pages).

---

## Key Findings & Future Work (Sim-to-Real Gap)

* **Simulation Performance:** The PPO agent successfully converged within `1,000,000` training steps (achieving a cumulative reward plateau of `~0.85` on TensorBoard) and navigated reliably to randomized targets in Unity.
* **Real-World Deployment Insights:** Physical testing revealed **Sim-to-Real transfer discrepancies**—specifically target overshoot/oscillation and low-speed stalling—caused by surface friction variance, end-to-end camera/network latency, and DC motor deadzones.
* **Future Direction:** Implementing **Domain & Dynamics Randomization** (randomizing mass, friction, and latency during training) alongside **PWM Deadzone Compensation** to bridge the Sim-to-Real gap.
