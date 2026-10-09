using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrthographicAspectAdjuster : MonoBehaviour
{
    private const float TargetWidth = 1920f;
    private const float TargetHeight = 1080f;
    private const float TargetAspect = TargetWidth / TargetHeight;
    [SerializeField] private Sprite m_background;

    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographic = true;
        // 1920x1080, PPU 100 기준 기본 Size = 5.4
        float defaultSize = TargetHeight / (2f * 100f);
        float currentAspect = (float)Screen.width / Screen.height;

        // 화면이 16:9보다 좁은 경우 가로 영역을 보장하기 위해 Size 확대
        if (currentAspect < TargetAspect)
        {
            cam.orthographicSize = defaultSize * (TargetAspect / currentAspect);
        }
        else
        {
            cam.orthographicSize = defaultSize;
        }
    }
}