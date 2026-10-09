using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Basefirestore", menuName = "firebase/Basefirestore")]
public class Basefirestore: ScriptableObject
{
    [SerializeField] private string[] Paths;
    private firestoreContext context;
    [SerializeField]private string typeofName;//저장할 데이터 타입을 문자열로 저장
    DocumentReference docRef;
    private Dictionary<string, PropertyInfo[]> propertyInfoCache = new();

    private CollectionReference m_current_World_Collection;
    public bool TypesEqual<T>()
    {
        return typeof(T).Name == typeofName;
    }

    public virtual void init(firestoreContext context)
    {
        this.context = context;
       
    }

    public virtual void VisitAgain(firestoreContext context)
    {
        this.context = context;

        var worldname = (context.world != null) ? context.world : "NULL";
        var uid = (context.auth.CurrentUser != null) ? context.auth.CurrentUser.UserId : "NULL";

        m_current_World_Collection = context.firestore.Collection("유저들").Document(uid).Collection(worldname);
        
    }

    protected virtual void SetPath()
    {
        if(Paths.Length % 2 != 0)
        {
            Debug.LogError("Invalid path segments. Please provide an even number of path segments.");
            return ;
        }

        docRef = null;
        var uid = (context.auth.CurrentUser != null) ? context.auth.CurrentUser.UserId : "NULL";

        var doc =context.firestore.Collection("Users").Document(uid);
        for (int i = 0; i < Paths.Length; i += 2)
        {

            if (docRef == null)
            {
                docRef = doc.Collection(Paths[i]).Document(Paths[i + 1]);
            }
            else
            {
                docRef = docRef.Collection(Paths[i]).Document(Paths[i + 1]);
            }
        }

        return;
    }
    public virtual Task Set<T>(T value)
    {
        bool flag = false ;
        docRef.SetAsync(value).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"[Firestore] Data set successfully for {typeof(T).Name}.");
                flag = true;
            }
            else
            {
                flag=false;
                Debug.LogError($"[Firestore] Failed to set data for {typeof(T).Name}: {task.Exception}");
            }
        });
        return (flag ==true) ? Task.CompletedTask : Task.FromException(new System.Exception("Failed to set data."));
    }

    public virtual Task<T> Get<T>()
    {

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    T data = snapshot.ConvertTo<T>();
                    Debug.Log($"[Firestore] Data retrieved successfully for {typeof(T).Name}.");
                    return data;
                }
                else
                {
                    Debug.LogWarning($"[Firestore] Document does not exist for {typeof(T).Name}.");
                    return default(T);
                }
            }
            else
            {
                Debug.LogError($"[Firestore] Failed to retrieve data for {typeof(T).Name}: {task.Exception}");
                return default(T);
            }
        });
        return null;
    }
    public virtual void Updatedata<T>(T value) where T : struct
    {
       
        Dictionary<string, object> updates = ConvertStructToDictionary(value);
        docRef.UpdateAsync(updates);
    }
    private Dictionary<string, object> ConvertStructToDictionary<T>(T value) where T : struct
    {
       
        var dictionary = new Dictionary<string, object>();
        PropertyInfo[] properties = null;
        if (propertyInfoCache.ContainsKey(typeof(T).Name) == false) 
        {
            properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            propertyInfoCache.Add(typeof(T).Name, properties);
        }
        else
        {
            properties= propertyInfoCache[typeof(T).Name];
        }

    

        foreach (PropertyInfo prop in properties)
        {
            // [FirestoreProperty] 속성이 붙어있는지 확인
            var attr = prop.GetCustomAttribute<FirestorePropertyAttribute>();
            if (attr == null) continue;

            object propValue = prop.GetValue(value);

            // Nullable struct 특성이거나 null이 아닌 값만 딕셔너리에 추가
            if (propValue != null)
            {
                // FirestoreProperty("custom_name") 처럼 별도 필드명이 지정된 경우 처리
                string fieldName = !string.IsNullOrEmpty(attr.Name) ? attr.Name : prop.Name;
                dictionary[fieldName] = propValue;
            }
        }

        return dictionary;
    }
    public void Delete()
    {
        docRef.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"[Firestore] Document deleted successfully.");
            }
            else
            {
                Debug.LogError($"[Firestore] Failed to delete document: {task.Exception}");
            }
        });
    }
}

public struct firestoreContext
{
    public FirebaseFirestore firestore;
    public FirebaseAuth auth;

    public string world;
    public string Charaterid;

    public firestoreContext(FirebaseFirestore firestore, FirebaseAuth auth,string world,string characterId)
    {
        this.firestore = firestore;
        this.auth = auth;
        this.world = world;
        this.Charaterid = characterId;
    }
}