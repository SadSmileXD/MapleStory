using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFitter : MonoBehaviour
{
    public SpriteRenderer sr;
    private void Start()
    {
        FitToScreen();
    }

    public void FitToScreen()
    {
         sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        // 1. 스프라이트의 가로/세로 월드 크기 (단위: Unit)
        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        // 2. 카메라의 가로/세로 월드 크기
        float worldScreenHeight = mainCam.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * mainCam.aspect;

        // 3. 카메라 영역을 꽉 채우기 위한 Scale 계산
        Vector3 newScale = transform.localScale;
        newScale.x = worldScreenWidth / spriteWidth;
        newScale.y = worldScreenHeight / spriteHeight;

        transform.localScale = newScale;
    }
}