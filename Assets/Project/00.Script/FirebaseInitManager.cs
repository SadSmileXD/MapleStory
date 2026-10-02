using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseInitManager : MonoBehaviour
{
    // Firebase 앱 인스턴스
    private FirebaseApp app;

    private void Awake()
    {
        InitFirebase();
    }

    private void InitFirebase()
    {
        // 1. 필요한 의존성 파일(Google Play Services 등)이 올바르게 설치되어 있는지 비동기로 확인
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // 의존성 확인 성공 -> 기본 FirebaseApp 인스턴스 생성
                app = FirebaseApp.DefaultInstance;
                Debug.Log("[Firebase] 성공적으로 초기화되었습니다.");

                // 서비스 사용 준비 완료 후 필요한 로직 호출 (예: 인증, Firestore, Remote Config 등)
                OnFirebaseInitialized();
            }
            else
            {
                Debug.LogError($"[Firebase] 의존성 확인 실패: {dependencyStatus}");
            }
        });
    }

    private void OnFirebaseInitialized()
    {
        // 예: Auth, Firestore, Database 등 초기화 작업 수행
    }
}