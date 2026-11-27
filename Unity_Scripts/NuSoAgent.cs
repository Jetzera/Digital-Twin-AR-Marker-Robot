using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class NuSoAgent : Agent
{
    [Header("Network Connection")]
    public RobotUDPSender udpSender;

    public override void Initialize()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. ส่งพิกัดหุ่นยนต์ตัวเอง (X, Z) ที่ถูกขยับโดยกล้อง AprilTag
        sensor.AddObservation(transform.localPosition.x / 5f);
        sensor.AddObservation(transform.localPosition.z / 5f);

        // 2. ส่งทิศทางการหันหน้า
        float rotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;
        sensor.AddObservation(rotation_normalized);

        // (เป้าหมายถูกจัดการอัตโนมัติโดย Ray Perception Sensor)
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // รับคำสั่งที่สมอง AI (ONNX) ตัดสินใจออกมา
        int action = actions.DiscreteActions[0];

        // ส่งคำสั่งนั้นเป็นตัวเลข (0, 1, 2, 3) ข้าม Wi-Fi ไปให้หุ่นยนต์จริง M5AtomS3
        if (udpSender != null)
        {
            udpSender.SendActionToRobot(action);
        }

        // หักคะแนนทุกเฟรมให้รีบทำเวลา ป้องกันหุ่นเดินอู้หรือหมุนเล่นอยู่กับที่
        AddReward(-0.001f);
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // เผื่อไว้กดคีย์บอร์ดเทสต์ระบบ UDP 
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0; 

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            discreteActions[0] = 3;
    }

    // 1. ชนเป้าหมาย (แก้ปัญหาเดินวนหาเป้า)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal")) 
        {
            SetReward(1.0f); // ได้คะแนนเมื่อถึงเป้าหมาย
            EndEpisode();    // สั่งจบรอบ
        }
    }

    // 2. ชนกำแพง / หลุดกล้อง (พุ่งชนครั้งแรก)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall")) //
        {
            AddReward(-0.01f); // หักคะแนนเมื่อเดินชนขอบ
            
            // แนะนำ: หากต้องการให้หลุดจอแล้วจบรอบทันที ให้ลบเครื่องหมาย // บรรทัดล่างออกครับ
            // EndEpisode(); 
        }
    }

    // 3. ไถกำแพง (แก้ปัญหาเดินไถลออกนอกแมพ)
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall")) //
        {
            AddReward(-0.001f); // โดนหักคะแนนเรื่อยๆ ถ้ายืนไถกำแพง
        }
    }
}