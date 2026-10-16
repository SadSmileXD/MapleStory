using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChanageScene : MonoBehaviour
{
    Button m_button;
    public string m_sceneName;
    void Start()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(() => {
            SceneManager.LoadScene(m_sceneName); 
        });
    }

    
}
