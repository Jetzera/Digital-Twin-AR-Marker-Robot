using UnityEngine;
using System;
using UI = UnityEngine.UI;

public class DetectionTest : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] int _cameraIndex = 0;
    [SerializeField] UI.RawImage _webcamPreview = null;

    [Header("AprilTag Setup")]
    [SerializeField] int _decimation = 2; // ปรับลงมาเพื่อให้จับภาพง่ายขึ้น
    [SerializeField] float _tagSize = 0.05f;

    [Header("Tag IDs Configuration")]
    [SerializeField] int robotTagID = 1;   // กำหนด ID ของ Tag ที่ติดบนหุ่นยนต์
    [SerializeField] int targetTagID = 2;  // กำหนด ID ของ Tag ที่เป็นเป้าหมาย

    [Header("3D Objects (Virtual Twin)")]
    [SerializeField] GameObject _robotObject = null; 
    [SerializeField] GameObject _targetObject = null; 

    WebCamTexture _webcam;
    AprilTag.TagDetector _detector;

    void Start()
    {
        var devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            if (_cameraIndex >= devices.Length || _cameraIndex < 0) _cameraIndex = 0;
            _webcam = new WebCamTexture(devices[_cameraIndex].name);
            _webcam.Play();
            
            if (_webcamPreview != null) _webcamPreview.texture = _webcam;
        }
    }

    void OnDestroy()
    {
        if (_webcam != null && _webcam.isPlaying) _webcam.Stop();
        _detector?.Dispose();
    }

    void LateUpdate()
    {
        if (_webcam == null || !_webcam.isPlaying || _webcam.width < 100) return;
        if (!_webcam.didUpdateThisFrame) return;

        if (_detector == null)
            _detector = new AprilTag.TagDetector(_webcam.width, _webcam.height, _decimation);

        Color32[] pixels = _webcam.GetPixels32();
        var imageSpan = new ReadOnlySpan<Color32>(pixels);

        var fov = Camera.main.fieldOfView * Mathf.Deg2Rad;
        _detector.ProcessImage(imageSpan, fov, _tagSize);

        // อัปเดตตำแหน่งของหุ่นยนต์และเป้าหมายตามที่กล้องมองเห็น
        foreach (var tag in _detector.DetectedTags)
        {
            if (tag.ID == robotTagID && _robotObject != null)
            {
                _robotObject.SetActive(true);
                _robotObject.transform.position = tag.Position;
                //_robotObject.transform.rotation = tag.Rotation;
            }
            else if (tag.ID == targetTagID && _targetObject != null)
            {
                _targetObject.SetActive(true);
                _targetObject.transform.position = tag.Position;
                //_targetObject.transform.rotation = tag.Rotation;
            }
        }
    }
}
