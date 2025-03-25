using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DeepSeekDialogueManager : MonoBehaviour
{
    // API配置
    [Header("API Settings")]
    [SerializeField] private string apiKey = "在此处填入你申请的API密钥";// DeepSeek API密钥
    [SerializeField] private string modelName = "deepseek-chat";// 使用的模型名称
    [SerializeField] private string apiUrl = "https://api.deepseek.com/v1/chat/completions";// API请求地址

    // 对话参数
    [Header("Dialogue Settings")]
    [Range(0, 2)] public float temperature = 0.7f;// 控制生成文本的随机性（0-2，值越高越随机）
    [Range(1, 1000)] public int maxTokens = 150;// 生成的最大令牌数（控制回复长度）

    // 角色设定
    [System.Serializable]
    public class NPCCharacter
    {
        public string name;
        [TextArea(3, 10)]
        public string personalityPrompt = "你是虚拟人物Unity-Chan，是个性格活泼，聪明可爱的女生。擅长Unity和C#编程知识。";// 角色设定提示词
    }

    [SerializeField] public NPCCharacter npcCharacter;

    // 回调委托，用于异步处理API响应
    public delegate void DialogueCallback(string response, bool isSuccess);
    /// <summary>
    /// 发送对话请求
    /// </summary>
    /// <param name="userMessage">玩家的输入内容</param>
    /// <param name="callback">回调函数，用于处理API响应</param>
    public void SendDialogueRequest(string userMessage, DialogueCallback callback)
    {
        StartCoroutine(ProcessDialogueRequest(userMessage, callback));
    }
    /// <summary>
    /// 处理对话请求的协程
    /// </summary>
    /// <param name="userInput">玩家的输入内容</param>
    /// <param name="callback">回调函数，用于处理API响应</param>
    private IEnumerator ProcessDialogueRequest(string userInput, DialogueCallback callback)
    {
        // 构建消息列表，包含系统提示和用户输入
        List<Message> messages = new List<Message>
        {
            new Message { role = "system", content = npcCharacter.personalityPrompt },// 系统角色设定
            new Message { role = "user", content = userInput }// 用户输入
        };

        // 构建请求体
        ChatRequest requestBody = new ChatRequest
        {
            model = modelName,// 模型名称
            messages = messages,// 消息列表
            temperature = temperature,// 温度参数
            max_tokens = maxTokens// 最大令牌数
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
        Debug.Log("Sending JSON: " + jsonBody); // 调试用，打印发送的JSON数据

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
            callback?.Invoke(name + "（陷入沉默）", false);
        }
    }
    /// <summary>
    /// 创建UnityWebRequest对象
    /// </summary>
    /// <param name="jsonBody">请求体的JSON字符串</param>
    /// <returns>配置好的UnityWebRequest对象</returns>
    private UnityWebRequest CreateWebRequest(string jsonBody)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        var request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);// 设置上传处理器
        request.downloadHandler = new DownloadHandlerBuffer();// 设置下载处理器
        request.SetRequestHeader("Content-Type", "application/json");// 设置请求头
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");// 设置认证头
        request.SetRequestHeader("Accept", "application/json");// 设置接受类型
        return request;
    }
    /// <summary>
    /// 检查请求是否出错
    /// </summary>
    /// <param name="request">UnityWebRequest对象</param>
    /// <returns>如果请求出错返回true，否则返回false</returns>
    private bool IsRequestError(UnityWebRequest request)
    {
        return request.result == UnityWebRequest.Result.ConnectionError ||
               request.result == UnityWebRequest.Result.ProtocolError ||
               request.result == UnityWebRequest.Result.DataProcessingError;
    }
    /// <summary>
    /// 解析API响应
    /// </summary>
    /// <param name="jsonResponse">API响应的JSON字符串</param>
    /// <returns>解析后的DeepSeekResponse对象</returns>
    private DeepSeekResponse ParseResponse(string jsonResponse)
    {
        try
        {
            return JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON解析失败: {e.Message}\n响应内容：{jsonResponse}");
            return null;
        }
    }

    // 可序列化数据结构
    [System.Serializable]
    private class ChatRequest
    {
        public string model;// 模型名称
        public List<Message> messages;// 消息列表
        public float temperature;// 温度参数
        public int max_tokens;// 最大令牌数
    }

    [System.Serializable]
    public class Message
    {
        public string role;// 角色（system/user/assistant）
        public string content;// 消息内容
    }

    [System.Serializable]
    private class DeepSeekResponse
    {
        public Choice[] choices;// 生成的选择列表
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;// 生成的消息
    }
}