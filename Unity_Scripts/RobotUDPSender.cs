using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class RobotUDPSender : MonoBehaviour
{
    [Header("Network Settings")]
    public string robotIP = "192.168.1.100"; // เปลี่ยนเป็น IP ของหุ่นยนต์จริง (Python/ESP)
    public int port = 5005;                  // เปลี่ยนให้ตรงกับฝั่งรับ

    private UdpClient udpClient;
    private IPEndPoint endPoint;

    void Start()
    {
        udpClient = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse(robotIP), port);
        Debug.Log($"UDP Sender Ready to connect: {robotIP}:{port}");
    }

    // ฟังก์ชันนี้จะถูกเรียกโดย ML-Agent 
    public void SendActionToRobot(int actionID)
    {
        // รูปแบบข้อมูล: ส่งเป็นตัวเลขเพียวๆ "0", "1", "2", "3" ไปให้ Python แปลความหมายต่อ
        string message = actionID.ToString(); 
        byte[] data = Encoding.UTF8.GetBytes(message);
        
        try
        {
            udpClient.Send(data, data.Length, endPoint);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("UDP Send Error: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (udpClient != null) udpClient.Close();
    }
}