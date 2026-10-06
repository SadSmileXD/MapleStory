using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

 
public class firestoreManager : Singleton<firestoreManager>
{
    private FirebaseApp app;
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;
    [SerializeField]private List<Basefirestore> basefirestores;
    private Dictionary<string , Basefirestore> firestoreDictionary=new();
    private Queue<Action> actionQueue = new Queue<Action>();
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    override protected void Awake()
    {
        base.Awake();
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
                auth = FirebaseAuth.DefaultInstance;
                firestore = FirebaseFirestore.DefaultInstance;
                Debug.Log("[Firebase] 성공적으로 초기화되었습니다.");

                init_basefirestore();


            }
            else
            {
                Debug.LogError($"[Firebase] 의존성 확인 실패: {dependencyStatus}");
            }
        });
    }
    
    private void init_basefirestore()
    {
        foreach (var basefirestore in basefirestores)
        {
            basefirestore.init(new firestoreContext(firestore, auth));
        }
    }

    private async void Push(Action action)
    {
        // 1. 큐에 작업 추가 (스레드 안전을 위해 lock 처리)
        lock (actionQueue)
        {
            actionQueue.Enqueue(action);
        }

        // 2. 이미 다른 작업이 진행 중이면 대기, 순서대로 진입
        await semaphore.WaitAsync();

        try
        {
            while (true)
            {
                Action currentAction = null;

                lock (actionQueue)
                {
                    if (actionQueue.Count == 0) break;
                    currentAction = actionQueue.Dequeue();
                }

                if (currentAction != null)
                {
                    try
                    {
                        //  주의: 메인 스레드(Unity API) 작업이 포함되어 있다면 Task.Run 대신 직접 실행 권장
                        await Task.Run(currentAction);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[Firebase] Action 실행 중 오류 발생: {e.Message}");
                    }
                }
            }
        }
        finally
        {
            // 3. 실행 완료 후 다음 작업이 진입할 수 있도록 해제
            semaphore.Release();
        }
    }



    public void SetData<T>(T value)
    {
        var valueType = typeof(T).Name;
        if(!firestoreDictionary.ContainsKey(valueType))
        {
            generator<T>(valueType);
        }

        try
        {
            Push(()=> firestoreDictionary[valueType].Set<T>(value));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Firebase] SetData 실패: {e.Message}");
        }

    }
    public void UpdateData<T>(T value) where T :struct
    {
        var valueType = typeof(T).Name;
        if (!firestoreDictionary.ContainsKey(valueType))
        {
            generator<T>(valueType);
        }
        try
        {
            Push(() => firestoreDictionary[valueType].Updatedata<T>(value));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Firebase] UpdateData 실패: {e.Message}");
        }
    }
    public void Delete<T>()
    {
        var valueType = typeof(T).Name;
        try
        {
            Push(() => firestoreDictionary[valueType].Delete());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Firebase] Delete 실패: {e.Message}");
        }
    }
    private void generator<T>(string valueType)
    {
        foreach (var m_basefirestore in basefirestores)
        {
            if (m_basefirestore.TypesEqual<T>())
            {
                firestoreDictionary.Add(valueType, m_basefirestore);
                
                return;
            }
        }
    }
    [ContextMenu("Test")]
    private void Test()
    {
     
        UpdateData<Testdata>(new Testdata("Sex1111111y",null));
    }
}
[FirestoreData]
[Serializable]
public struct Testdata
{
    [FirestoreProperty]
    public string name { get; set; }
    public int? age { get; set; }

    public Testdata(string name, int? age)
    {
        this.name = name;
        this.age = age;
    }
}