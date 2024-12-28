using UnityEngine;

[ExecuteAlways]
public class CardFaceController : MonoBehaviour
{
    [SerializeField] private GameObject _frontFace;
    [SerializeField] private GameObject _backFace;

    private Camera _mainCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_mainCam || !_frontFace || !_backFace) return;

        var cam = _mainCam;
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying && Camera.current) 
            cam = Camera.current;
#endif
       
        var isFacingFront = Vector3.Dot(transform.forward, cam.transform.forward) > 0;
        _frontFace.SetActive(isFacingFront);
        _backFace.SetActive(!isFacingFront);
    }
}
