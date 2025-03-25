using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DeepSeekDialogueManager : MonoBehaviour
{
    // API����
    [Header("API Settings")]
    [SerializeField] private string apiKey = "�ڴ˴������������API��Կ";// DeepSeek API��Կ
    [SerializeField] private string modelName = "deepseek-chat";// ʹ�õ�ģ������
    [SerializeField] private string apiUrl = "https://api.deepseek.com/v1/chat/completions";// API�����ַ

    // �Ի�����
    [Header("Dialogue Settings")]
    [Range(0, 2)] public float temperature = 0.7f;// ���������ı�������ԣ�0-2��ֵԽ��Խ�����
    [Range(1, 1000)] public int maxTokens = 150;// ���ɵ���������������ƻظ����ȣ�

    // ��ɫ�趨
    [System.Serializable]
    public class NPCCharacter
    {
        public string name;
        [TextArea(3, 10)]
        public string personalityPrompt = "������������Unity-Chan���Ǹ��Ը���ã������ɰ���Ů�����ó�Unity��C#���֪ʶ��";// ��ɫ�趨��ʾ��
    }

    [SerializeField] public NPCCharacter npcCharacter;

    // �ص�ί�У������첽����API��Ӧ
    public delegate void DialogueCallback(string response, bool isSuccess);
    /// <summary>
    /// ���ͶԻ�����
    /// </summary>
    /// <param name="userMessage">��ҵ���������</param>
    /// <param name="callback">�ص����������ڴ���API��Ӧ</param>
    public void SendDialogueRequest(string userMessage, DialogueCallback callback)
    {
        StartCoroutine(ProcessDialogueRequest(userMessage, callback));
    }
    /// <summary>
    /// ����Ի������Э��
    /// </summary>
    /// <param name="userInput">��ҵ���������</param>
    /// <param name="callback">�ص����������ڴ���API��Ӧ</param>
    private IEnumerator ProcessDialogueRequest(string userInput, DialogueCallback callback)
    {
        // ������Ϣ�б�����ϵͳ��ʾ���û�����
        List<Message> messages = new List<Message>
        {
            new Message { role = "system", content = npcCharacter.personalityPrompt },// ϵͳ��ɫ�趨
            new Message { role = "user", content = userInput }// �û�����
        };

        // ����������
        ChatRequest requestBody = new ChatRequest
        {
            model = modelName,// ģ������
            messages = messages,// ��Ϣ�б�
            temperature = temperature,// �¶Ȳ���
            max_tokens = maxTokens// ���������
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
        Debug.Log("Sending JSON: " + jsonBody); // �����ã���ӡ���͵�JSON����

        UnityWebRequest request = CreateWebRequest(jsonBody);
        yield return request.SendWebRequest();

        if (IsRequestError(request))
        {
            Debug.LogError($"API Error: {request.responseCode}\n{request.downloadHandler.text}");
            callback?.Invoke(null, false);
            yield break;
        }

        DeepSeekResponse response = ParseResponse(request.downloadHandler.text);
        if (response != null && response.choices.Length > 0)
        {
            string npcReply = response.choices[0].message.content;
            callback?.Invoke(npcReply, true);
        }
        else
        {
            callback?.Invoke(name + "�������Ĭ��", false);
        }
    }
    /// <summary>
    /// ����UnityWebRequest����
    /// </summary>
    /// <param name="jsonBody">�������JSON�ַ���</param>
    /// <returns>���úõ�UnityWebRequest����</returns>
    private UnityWebRequest CreateWebRequest(string jsonBody)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        var request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);// �����ϴ�������
        request.downloadHandler = new DownloadHandlerBuffer();// �������ش�����
        request.SetRequestHeader("Content-Type", "application/json");// ��������ͷ
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");// ������֤ͷ
        request.SetRequestHeader("Accept", "application/json");// ���ý�������
        return request;
    }
    /// <summary>
    /// ��������Ƿ����
    /// </summary>
    /// <param name="request">UnityWebRequest����</param>
    /// <returns>������������true�����򷵻�false</returns>
    private bool IsRequestError(UnityWebRequest request)
    {
        return request.result == UnityWebRequest.Result.ConnectionError ||
               request.result == UnityWebRequest.Result.ProtocolError ||
               request.result == UnityWebRequest.Result.DataProcessingError;
    }
    /// <summary>
    /// ����API��Ӧ
    /// </summary>
    /// <param name="jsonResponse">API��Ӧ��JSON�ַ���</param>
    /// <returns>�������DeepSeekResponse����</returns>
    private DeepSeekResponse ParseResponse(string jsonResponse)
    {
        try
        {
            return JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON����ʧ��: {e.Message}\n��Ӧ���ݣ�{jsonResponse}");
            return null;
        }
    }

    // �����л����ݽṹ
    [System.Serializable]
    private class ChatRequest
    {
        public string model;// ģ������
        public List<Message> messages;// ��Ϣ�б�
        public float temperature;// �¶Ȳ���
        public int max_tokens;// ���������
    }

    [System.Serializable]
    public class Message
    {
        public string role;// ��ɫ��system/user/assistant��
        public string content;// ��Ϣ����
    }

    [System.Serializable]
    private class DeepSeekResponse
    {
        public Choice[] choices;// ���ɵ�ѡ���б�
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;// ���ɵ���Ϣ
    }
}