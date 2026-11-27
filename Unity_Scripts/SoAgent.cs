using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;

public class RobotAgent : Agent
{
    [Header("Settings")]
    public Transform target; 
    public float moveSpeed = 10f; 
    
    // ปรับ Turn Speed ให้น้อยลงแล้ว (เพราะพอเป็น Angular Velocity ค่า 5 ก็หมุนเร็วแล้วครับ)
    public float turnSpeed = 5f; 

    [Header("Environment")]
    public Transform ground; 

    private Rigidbody rb;
    private Vector3 startingPosition;
    private Bounds groundBounds;

    [Header("Visual Feedback")]
    public Renderer groundRenderer; 
    private Color defaultGroundColor;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.localPosition;

        if (ground != null) groundBounds = ground.GetComponent<Collider>().bounds;
        if (groundRenderer != null) defaultGroundColor = groundRenderer.material.color;
    }

    public override void OnEpisodeBegin()
    {
        // 1. รีเซ็ตตำแหน่ง การหมุน และความเร็ว
        transform.localPosition = startingPosition;
        transform.localRotation = Quaternion.identity; // รีเซ็ตให้หันหน้ากลับทิศเดิม
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2. สุ่มตำแหน่งเป้าหมาย
        float spawnRangeX = (groundBounds.extents.x - 1f); 
        float spawnRangeZ = (groundBounds.extents.z - 1f);
        
        target.localPosition = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            0.5f, 
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. พิกัดตัวเอง (แกน X และ Z) บีบสเกลหาร 5
        sensor.AddObservation(transform.localPosition.x / 5f);
        sensor.AddObservation(transform.localPosition.z / 5f);

        // 2. ทิศทางการหันหน้า (แกน Y) แปลงองศาให้อยู่ในช่วง -1 ถึง 1
        float rotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;
        sensor.AddObservation(rotation_normalized);

        // *** ไม่ต้องส่งพิกัด Target แล้ว ปล่อยให้ Ray Perception Sensor สแกนหาเอง! ***
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        Vector3 moveDirection = Vector3.zero;
        float rotationInput = 0f;

        // 0 = ยืนเฉยๆ
        if (action == 1) // 1 = เดินหน้า
        {
            moveDirection = transform.forward * moveSpeed;
        }
        else if (action == 2) // 2 = เลี้ยวซ้าย
        {
            rotationInput = -1f;
        }
        else if (action == 3) // 3 = เลี้ยวขวา
        {
            rotationInput = 1f;
        }

        // --- ใช้ระบบฟิสิกส์ล้วนๆ ขยับสมูท ไม่ทะลุกำแพง ---
        // 1. สั่งให้เคลื่อนที่ (หน้า/หลัง)
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        // 2. สั่งให้หมุน (ซ้าย/ขวา)
        rb.angularVelocity = Vector3.up * (rotationInput * turnSpeed);

        // Time Penalty (หักคะแนนทุกเฟรมให้รีบทำเวลา)
        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0; // ค่าเริ่มต้นคือยืนเฉยๆ

        // คุมด้วยปุ่ม W, A, D หรือลูกศร
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[0] = 3;
        }
    }

    // --- ระบบเช็กการชน ---

    // 1. ชนเป้าหมาย
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal")) 
        {
            SetReward(1.0f);
            if (groundRenderer != null) StartCoroutine(FlashGround(Color.green, 0.5f));
            EndEpisode();
        }
    }

    // 2. ชนกำแพง (พุ่งชนครั้งแรก)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.01f); 
        }
    }

    // 3. ไถกำแพง
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.001f); 
        }
    }

    // ฟังก์ชันกระพริบสีพื้น
    private IEnumerator FlashGround(Color flashColor, float duration)
    {
        if (groundRenderer == null) yield break;
        
        groundRenderer.material.color = flashColor;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            groundRenderer.material.color = Color.Lerp(flashColor, defaultGroundColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        groundRenderer.material.color = defaultGroundColor;
    }
}