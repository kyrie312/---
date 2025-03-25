using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Dispatchanager : MonoBehaviour
{
    [Header("UI References")]
    public Text questionText;           // ��ʾ��Ŀ
    public Toggle[] optionToggles;      // �ĸ� Toggle������ѡ��
    public Text[] optionTexts;          // �ĸ�ѡ������

    public Button submitButton;         // ���ύ����ť
    public Text feedbackText;           // ��ʾ��ʾ��ǰ�÷֣���ѡ��

    [Header("Quiz Data")]
    // ���ڴ洢��Ŀ���ݵ��б������飩
    public List<QuestionData> questions = new List<QuestionData>();

    private int currentQuestionIndex = 0;  // ��ǰ��Ŀ����
    private int score = 0;                // ��ǰ����

    public Image panelBackground;  // ��������Image���
    public Sprite[] backgroundImages; // �洢5�ű���ͼƬ
    private int currentIndex = 0; // ��¼��ǰͼƬ����

    public GameObject Feedbackpanel;
    public Button back;

    void Start()
    {
        // ���û���ֶ��� Inspector �����⣬����������д����Ŀ����ӱ𴦼��ء�
        if (questions.Count == 0)
        {
            LoadDefaultQuestions();
        }
        SetMode(0);
        if (backgroundImages.Length > 0 && panelBackground != null)
        {
            panelBackground.sprite = backgroundImages[currentIndex];
        }
        Feedbackpanel.SetActive(false);

        // �����һ����Ŀ
        DisplayQuestion(currentQuestionIndex);

        // ���ύ��ť�󶨼����¼�
        submitButton.onClick.AddListener(OnSubmit);
        back.onClick.AddListener(closefeedback);
    }

    /// <summary>
    /// ��������ύ����ťʱ���жϵ�ǰ��Ŀ����ȷ�ԣ�����������������һ�⡣
    /// </summary>
    private void OnSubmit()
    {
        if (currentQuestionIndex >= questions.Count)
            return;

        QuestionData currentQ = questions[currentQuestionIndex];

        // ������Ŀ���ͣ���ȡ�����ѡѡ��
        // ��ѡ��ֻ�������һ��ѡ��
        // ��ѡ��������ѡ
        bool isCorrect = false;

        if (currentQ.questionType == QuestionType.SingleChoice)
        {
            // �ҵ����ѡ���ѡ�� index
            int selectedIndex = -1;
            for (int i = 0; i < optionToggles.Length; i++)
            {
                if (optionToggles[i].isOn)
                {
                    selectedIndex = i;
                    break;
                }
            }

            // ��ѡ��ֻ��Ҫ�ж� selectedIndex �Ƿ����ȷ����ͬ
            if (selectedIndex == currentQ.correctSingleAnswerIndex)
            {
                isCorrect = true;
            }
        }
        else if (currentQ.questionType == QuestionType.MultipleChoice)
        {
            // ��ѡ����Ҫ�ж��ĸ�ѡ���Ƿ�����ȷѡ�����ƥ��
            bool[] userSelected = new bool[optionToggles.Length];
            for (int i = 0; i < optionToggles.Length; i++)
            {
                userSelected[i] = optionToggles[i].isOn;
            }

            isCorrect = CheckMultipleChoiceAnswer(userSelected, currentQ.correctMultiAnswers);
        }

        Feedbackpanel.SetActive(true);
        // �������ˣ�score +1
        if (isCorrect)
        {
            score++;
            feedbackText.text = "��ϲ��ɹ����������\n���ӱ�����Ϣ��������ҵƻ�" ;
        }
        else
        {
            feedbackText.text = "����ĵ��ȳ��˵�С���⣬\n���������ǿ��Եģ�" ;
        }

        // ������һ��
        currentQuestionIndex++;
        ChangeBackground();
        if (currentQuestionIndex == 2)
        {
            SetMode(1);
        }
        if (currentQuestionIndex == 4)
        {
            SetMode(2);
        }
        if (currentQuestionIndex < 8)
        {
            DisplayQuestion(currentQuestionIndex);
        }
        else
        {
            // ȫ�����꣬���Խ�����Ϸ��������չʾ
            feedbackText.text += "���Ѿ������������" ;
            // �������󣬿������ύ��ťʧЧ������ת���������
            submitButton.interactable = false;
            SceneManager.LoadScene("Scene_Quiz");
        }
    }

    /// <summary>
    /// �� UI ��չʾ��ǰ��Ŀ
    /// </summary>
    /// <param name="index"></param>
    private void DisplayQuestion(int index)
    {
        if (index < 0 || index >= questions.Count) return;

        QuestionData q = questions[index];
        questionText.text = q.questionTitle;

        // ����ѡ������
        for (int i = 0; i < optionTexts.Length; i++)
        {
            optionTexts[i].text = q.options[i];
            // ����ÿ�� Toggle �� isOn ״̬
            optionToggles[i].isOn = false;
        }
    }

    /// <summary>
    /// �ж϶�ѡ���Ƿ���ȫƥ����ȷ��
    /// </summary>
    private bool CheckMultipleChoiceAnswer(bool[] userSelected, bool[] correctAnswers)
    {
        if (userSelected.Length != correctAnswers.Length) return false;

        for (int i = 0; i < correctAnswers.Length; i++)
        {
            if (userSelected[i] != correctAnswers[i])
            {
                return false; // ֻҪ��һ��ѡ����ϣ��ʹ���
            }
        }
        return true;
    }

    /// <summary>
    /// ���δ�� Inspector ��д��Ŀ�������ڴ�д�� 5 ����Ŀ��ǰ 3 ��ѡ���� 2 ��ѡ����
    /// </summary>
    private void LoadDefaultQuestions()
    {
        QuestionData q1 = new QuestionData
        {
            questionTitle = "̫�����𣬳����������ѡ�������½������������õ��������̺�С�͹���Ҳ��ʼһ�����ҵ����Ȼ���ɿ�ʼ�����������廹��ƽ�ȡ�����Ҫ��\n        ��⵱ǰ�õ縺�����ƣ�\n        �ʶ���߷�����鹦�������\n        �������ȹ��ػ�Ƶ�����������ַ�������ڰ�ȫ������ת��\r\n����ĵ��ؿ����ó��а�����õ�˳���ް������ͣ���",
            options = new string[]
    {
        "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
    },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 1 // �� 2 ��ѡ��Ϊ��ȷ��
        };

        QuestionData q2 = new QuestionData
        {
            questionTitle = "���Ƴ��ϣ��������Ƿ׷׻ؼң��õ������ڶ�ʱ���ڿ����������Ҽһ������ơ����������֣��̳����޺��Ҳ���𡣴˿̵�ҹ���õ�߷��������첻һ������Ҫ�����Ӧ�ԡ�\n        �����õ縺�ɵĿ���������\n        �ʵ����߷���������������������㣻\n        ��������ȵ��ȴ�����ϵͳ�𵴡�",
            options = new string[]
            {
        "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
            },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 1 // �� 2 ��ѡ��Ϊ��ȷ��
        };

        QuestionData q3 = new QuestionData
        {
            questionTitle = "�峿������������ɽ�ȣ�΢����棬���������ѿ�ʼ����ת����ˮ��ˮλ���ڽ�Ϊ�����ķ�Χ�������ʶȳ�������ʱ�ĳ��и�����������������̫�ߡ�����Ҫ��\n    �۲쵱ǰ���٣������ŷ������������\n    ��ˮ��������е�ˮƽ���У�����ϻ��������������õ�����\n    Ԥ��һ��������������Ӧ����ʱ���ɲ���������仯��",
            options = new string[]
            {
       "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
            },
            questionType = QuestionType.MultipleChoice,
            // ��ȷ�𰸣��� 2 ��Ϊtrue������Ϊfalse
            correctMultiAnswers = new bool[] { false, true, false, false, false }
        };

        QuestionData q4 = new QuestionData
        {
            questionTitle = "ҹĻ���٣����������õ������������Ȧ�޺�ƿ��������н���ҹ���Ծʱ�Ρ������ϰ������м�����������һ�������ˮ�����������ƽ�ָ��ɼ�塣��Ҫ��\n    ��עҹ�为�ɸ߷壬�ʶ���߻���ˮ�����Ӧ���õ�������\n    �������½�������Ӧ���ȱ�֤�����ȶ��������ֹȱ�ڣ�\n    ����ˮ�������ȷ�ˮ����������޷������峿��ͻ����������",
            options = new string[]
            {
        "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
            },
            questionType = QuestionType.MultipleChoice,
            // ��ȷ�𰸣��� 1 �͵� 3 ��Ϊ true
            correctMultiAnswers = new bool[] { true, false, true, false, false }
        };

        QuestionData q5 = new QuestionData
        {
            questionTitle = "�ڽ���İ����ҹ���㽫�״γ��Ե��Ȱ����˵����ĵ������˵�����ѽ��벢���׶Σ������и߸����ȶ����������Ҫ�ڸ��ɵ�ʱ����˵�������࣬ҲҪԤ����һ������ˮ����ڿռ䡣\n    ���õ��Դ���վ����������ȣ������츺�������ٶȹ��죬����վ�ɿ���������ܣ�ҹ����𽥽���ʱ����ɽ�������ܳ��봢���豸�������˷ѿ�������Դ��˵���ء�\n    ���ݵ�Ƶվ���ṩ�޹�������Ƶ��֧�֡������ַ�����������ˮ����ڵ��µ���Ƶ�ʺ��ߺ��ͣ������˼�ʱ���õ�Ƶ���ܡ�",
            options = new string[]
            {
        "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
            },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 1 // �� 2 ��ѡ��Ϊ��ȷ��
        };

        QuestionData q6 = new QuestionData
        {
            questionTitle = "����Ԥ��Ԥʾ��һ���������ȵ�Ӳ�̣������׻���������������������������ҹ��\n    �׻����գ������·�ͱ��վ����������ǿ���׻���������·��բ�����ͣ�硣׼���ü��޶���ͱ�����·�ĵ��ȷ�����\n    �������ױ�������������˲��������轵��\n    �˵������Ҫ�ȶ������������Ҫʱ�̼����Ҫ�����·�ĸ��������",
            options = new string[]
            {
       "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
            },
            questionType = QuestionType.MultipleChoice,
            // ��ȷ�𰸣��� 1 �͵� 3 ��Ϊ true
            correctMultiAnswers = new bool[] { true, false, true, false, false }
        };

        QuestionData q7 = new QuestionData
        {
            questionTitle = "����չ����ֽӵ������������ӽ�������������ҹ�������轵�����в�ů���ɼ���������\n    ������ҹ��ĸ��ɽ�Զ����ƽʱ�������Ǿ���͹��������ĵ��������磻\n    �˵�����ά��ǿ���Ļ��ɣ�������Ҫͨ������ˮ���Լ����ܿ��ٵ��ڷ�ֵ��",
            options = new string[]
            {
       "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
            },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 2 // �� 3 ��ѡ��Ϊ��ȷ��
        };

        QuestionData q8 = new QuestionData
        {
            questionTitle = "һ�����ֹ�����Ż�Ƶ���½�����ͬʱ���õ��ݵ�Ƶվ�ȶ�ϵͳ��\n    ����ʹ�ô��ܣ������ڼ���ܳ��ֳ����߸��ɣ����ȷŵ��Ҫȷ�����㹻ʱ��͹��ʽ����ٳ�硣\n    ����轵���£���ĵ��Ⱦ��߽�Ӱ���ǧ�����˵Ĳ�ů����������Ҫ��ֹ��Χ�޵磬��Ҫ��֤�˵����İ�ȫ���������鼴�ڴ˿̣���",
            options = new string[]
            {
        "����1",
        "����2",
        "����3",
        "����4",
        "ʵʱ���",
        "����1",
        "����2",
        "����1",
        "����2",
        "�������",
        " ",
        "��Ԫ1",
        "��Ԫ2",
        "��Ԫ1",
        "��Ԫ2"
            },
            questionType = QuestionType.MultipleChoice,
            // ��ȷ�𰸣��� 2 ��Ϊ true������Ϊ false
            correctMultiAnswers = new bool[] { false, true, false, false, false }
        };

        questions.Add(q1);
        questions.Add(q2);
        questions.Add(q3);
        questions.Add(q4);
        questions.Add(q5);
        questions.Add(q6);
        questions.Add(q7);
        questions.Add(q8);
        
    }


    /// <summary>
    /// ��Ŀ���ͣ���ѡ or ��ѡ
    /// </summary>
    /// 
    public void ChangeBackground()
    {
        if (backgroundImages.Length == 0 || panelBackground == null)
            return;

        currentIndex = (currentIndex + 1) % backgroundImages.Length; // ��˳��ѭ��
        panelBackground.sprite = backgroundImages[currentIndex]; // ��������
    }



    // ����Toggle��ĳ��ģʽ�µ�����
    [System.Serializable]
    public class ToggleConfig
    {
        // �Ƿ���ʾ��Toggle
        public bool isActive;
        // �����ʾ����������ê��λ�ã�һ������UI���֣�
        public Vector2 position;
    }

    // һ��ģʽ�¶�����Toggle������
    [System.Serializable]
    public class ModeConfig
    {
        // ģʽ���ƣ���ѡ�����ڱ�ʶ����ԣ�
        public string modeName;
        // ÿ��Toggle�����ã�Ҫ��������optionTogglesһ�»�������optionToggles
        public ToggleConfig[] toggleConfigs;
    }

    // Ԥ��3��ģʽ��������Inspector��չ����������
    public ModeConfig[] modeConfigs = new ModeConfig[3];

    /// <summary>
    /// ����ģʽ�������Toggle����ʾ״̬��λ�á�
    /// modeIndex: 0, 1, 2 ����ģʽ�е�һ��
    /// </summary>
    /// <param name="modeIndex">ģʽ���</param>
    public void SetMode(int modeIndex)
    {
        if (modeIndex < 0 || modeIndex >= modeConfigs.Length)
        {
            Debug.LogError("��Ч��ģʽ��ţ�" + modeIndex);
            return;
        }

        ModeConfig selectedMode = modeConfigs[modeIndex];

        // ����ÿ��Toggle���������õ���״̬��λ��
        for (int i = 0; i < optionToggles.Length; i++)
        {
            // �����ǰģʽ�������ж�Ӧ��Toggle���ã�����������
            if (i < selectedMode.toggleConfigs.Length)
            {
                ToggleConfig config = selectedMode.toggleConfigs[i];
                // ����Toggle��ʾ������
                optionToggles[i].gameObject.SetActive(config.isActive);

                // �����Toggle��ʾ���������λ��
                if (config.isActive)
                {
                    RectTransform rt = optionToggles[i].GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rt.anchoredPosition = config.position;
                    }
                }
            }
            else
            {
                // ���������û�ж�Ӧ�����Ĭ�����ظ�Toggle
                optionToggles[i].gameObject.SetActive(false);
            }
        }
    }

    public void closefeedback()
    {
        Feedbackpanel.SetActive(false);
    }
}

