using UnityEngine;

public class GuiRobotAgent : MonoBehaviour
{
    public NuSoAgent robotAgent; // ลากตัวหุ่นยนต์ (Robot) มาใส่ช่องนี้

    private GUIStyle labelStyle;
    private GUIStyle positiveRewardStyle;
    private GUIStyle negativeRewardStyle;

    void Start()
    {
        // ตั้งค่าหน้าตาของฟอนต์
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 24;
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontStyle = FontStyle.Bold;

        positiveRewardStyle = new GUIStyle(labelStyle);
        positiveRewardStyle.normal.textColor = Color.green;

        negativeRewardStyle = new GUIStyle(labelStyle);
        negativeRewardStyle.normal.textColor = Color.red;
    }

    void OnGUI()
    {
        if (robotAgent == null) return;

        // ดึงข้อมูลจำนวนสเตปและคะแนนปัจจุบันจากตัวหุ่นยนต์
        string stats = $"Step: {robotAgent.StepCount}";
        string reward = $"Reward: {robotAgent.GetCumulativeReward():F3}";

        GUIStyle currentRewardStyle = robotAgent.GetCumulativeReward() >= 0 ? positiveRewardStyle : negativeRewardStyle;

        // วาดตัวหนังสือลงบนหน้าจอ
        GUI.Label(new Rect(20, 20, 300, 30), stats, labelStyle);
        GUI.Label(new Rect(20, 50, 300, 30), reward, currentRewardStyle);
    }
}