using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoogleTokenFetcher : MonoBehaviour
{
    [Header("Firebase Web Client Credentials")]
    [Tooltip("GCP 콘솔의 웹 애플리케이션 클라이언트 ID")]
    [SerializeField] private string webClientId = "여기에_웹_클라이언트_ID_입력";

    [Tooltip("GCP 콘솔의 웹 애플리케이션 클라이언트 보안 비밀번호")]
    [SerializeField] private string webClientSecret = "여기에_웹_클라이언트_보안_비밀번호_입력";

    [Header("UI Elements")]
    [Tooltip("로그인 버튼")]
    public Button m_btn;

 

    private const int LocalPort = 7123;
    private FirebaseAuth auth;
   
    private void Start()
    {
        var keydata = googleSheetManager.Instance.GetClassData<SecurityKey>();
        webClientId = keydata.FindById("0").key;
        webClientSecret = keydata.FindById("1").key;
        auth = FirebaseAuth.DefaultInstance;

        // 시작 시 로딩 UI 비활성화
      

        if (m_btn != null)
        {
            m_btn.onClick.AddListener(GetGoogleToken);
        }
    }

    [ContextMenu("Get Google Token & Firebase Auth Test")]
    public async void GetGoogleToken()
    {
       
      

        Debug.Log("1. 구글 로그인 웹 브라우저를 엽니다...");

        TokenResponse tokenData = await FetchTokenFromGoogleAsync();

        if (tokenData != null && !string.IsNullOrEmpty(tokenData.id_token))
        {
            Debug.Log("<color=green><b>[구글 토큰 발급 성공!]</b></color>");
        

            await SignInWithFirebaseAsync(tokenData.id_token, tokenData.access_token);
        }
        else
        {
            Debug.LogError("구글 토큰 발급 실패");
        
        }
    }

    private async Task<TokenResponse> FetchTokenFromGoogleAsync()
    {
        string redirectUri = $"http://127.0.0.1:{LocalPort}/";
        TcpListener tcpListener = null;

        try
        {
            tcpListener = new TcpListener(IPAddress.Loopback, LocalPort);
            tcpListener.Start();
        }
        catch (Exception ex)
        {
            Debug.LogError($"TcpListener 시작 실패: {ex.Message}");
            return null;
        }

        string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        string scope = Uri.EscapeDataString("openid email profile");

        StringBuilder urlBuilder = new StringBuilder();
        urlBuilder.Append($"{authorizationEndpoint}?");
        urlBuilder.Append($"response_type=code");
        urlBuilder.Append($"&client_id={webClientId}");
        urlBuilder.Append($"&redirect_uri={Uri.EscapeDataString(redirectUri)}");
        urlBuilder.Append($"&scope={scope}");

        Application.OpenURL(urlBuilder.ToString());

        string code = null;

        try
        {
            while (string.IsNullOrEmpty(code))
            {
                using (TcpClient client = await tcpListener.AcceptTcpClientAsync())
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] requestBuffer = new byte[2048];
                    int bytesRead = await stream.ReadAsync(requestBuffer, 0, requestBuffer.Length);
                    string requestText = Encoding.UTF8.GetString(requestBuffer, 0, bytesRead);

                    if (!string.IsNullOrEmpty(requestText) && requestText.Contains("code="))
                    {
                        int codeIndex = requestText.IndexOf("code=");
                        if (codeIndex != -1)
                        {
                            string paramSubstring = requestText.Substring(codeIndex + 5);
                            int spaceOrAmp = paramSubstring.IndexOfAny(new char[] { ' ', '&', '\r', '\n' });
                            code = (spaceOrAmp != -1) ? paramSubstring.Substring(0, spaceOrAmp) : paramSubstring;
                            code = Uri.UnescapeDataString(code);
                        }
                    }

                    string responseHtml = "<html><head><meta charset='utf-8'></head>" +
                                           "<body style='text-align:center; padding-top:50px; font-family:sans-serif;'>" +
                                           "<h1>Google Login Completed!</h1>" +
                                           "<p>You can close this window and return to Unity.</p>" +
                                           "</body></html>";

                    byte[] htmlBytes = Encoding.UTF8.GetBytes(responseHtml);
                    string header = "HTTP/1.1 200 OK\r\n" +
                                    "Content-Type: text/html; charset=utf-8\r\n" +
                                    $"Content-Length: {htmlBytes.Length}\r\n" +
                                    "Connection: close\r\n\r\n";

                    byte[] headerBytes = Encoding.UTF8.GetBytes(header);

                    await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                    await stream.WriteAsync(htmlBytes, 0, htmlBytes.Length);
                    await stream.FlushAsync();

                    client.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"소켓 수신 오류: {ex.Message}");
            return null;
        }
        finally
        {
            tcpListener.Stop();
        }

        if (string.IsNullOrEmpty(code))
        {
            return null;
        }

        return await ExchangeCodeForTokenAsync(code, redirectUri);
    }

    private async Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri)
    {
        string tokenEndpoint = "https://oauth2.googleapis.com/token";

        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("client_id", webClientId);
        form.AddField("client_secret", webClientSecret);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("grant_type", "authorization_code");

        using (UnityWebRequest request = UnityWebRequest.Post(tokenEndpoint, form))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                return JsonUtility.FromJson<TokenResponse>(json);
            }
            else
            {
                Debug.LogError($"[Token Error] {request.error}\n{request.downloadHandler.text}");
                return null;
            }
        }
    }

    private async Task SignInWithFirebaseAsync(string idToken, string accessToken)
    {
        try
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            FirebaseUser user = await auth.SignInWithCredentialAsync(credential);

            if (user != null)
            {
                Debug.Log("<color=cyan><b>[Firebase 인증 성공!]</b></color>");

                // 로그인 완료 후 씬 전환 (로딩 UI는 씬이 전환되면서 자동 해제)
                SceneManager.LoadScene(2);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Firebase 로그인 실패] {ex.Message}");
          
        }
    }

   

    [Serializable]
    public class TokenResponse
    {
        public string id_token;
        public string access_token;
        public int expires_in;
        public string token_type;
    }
}