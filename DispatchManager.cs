using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Dispatchanager : MonoBehaviour
{
    [Header("UI References")]
    public Text questionText;           // 显示题目
    public Toggle[] optionToggles;      // 四个 Toggle，用于选项
    public Text[] optionTexts;          // 四个选项文字

    public Button submitButton;         // “提交”按钮
    public Text feedbackText;           // 显示提示或当前得分（可选）

    [Header("Quiz Data")]
    // 用于存储题目数据的列表（或数组）
    public List<QuestionData> questions = new List<QuestionData>();

    private int currentQuestionIndex = 0;  // 当前题目索引
    private int score = 0;                // 当前分数

    public Image panelBackground;  // 关联面板的Image组件
    public Sprite[] backgroundImages; // 存储5张背景图片
    private int currentIndex = 0; // 记录当前图片索引

    public GameObject Feedbackpanel;
    public Button back;

    void Start()
    {
        // 如果没有手动在 Inspector 里填题，可以在这里写死题目，或从别处加载。
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

        // 载入第一个题目
        DisplayQuestion(currentQuestionIndex);

        // 给提交按钮绑定监听事件
        submitButton.onClick.AddListener(OnSubmit);
        back.onClick.AddListener(closefeedback);
    }

    /// <summary>
    /// 当点击“提交”按钮时，判断当前题目的正确性，给出反馈并进入下一题。
    /// </summary>
    private void OnSubmit()
    {
        if (currentQuestionIndex >= questions.Count)
            return;

        QuestionData currentQ = questions[currentQuestionIndex];

        // 根据题目类型，获取玩家所选选项
        // 单选：只允许最多一个选中
        // 多选：允许多项都选
        bool isCorrect = false;

        if (currentQ.questionType == QuestionType.SingleChoice)
        {
            // 找到玩家选择的选项 index
            int selectedIndex = -1;
            for (int i = 0; i < optionToggles.Length; i++)
            {
                if (optionToggles[i].isOn)
                {
                    selectedIndex = i;
                    break;
                }
            }

            // 单选题只需要判断 selectedIndex 是否和正确答案相同
            if (selectedIndex == currentQ.correctSingleAnswerIndex)
            {
                isCorrect = true;
            }
        }
        else if (currentQ.questionType == QuestionType.MultipleChoice)
        {
            // 多选题需要判断四个选项是否与正确选项布尔表匹配
            bool[] userSelected = new bool[optionToggles.Length];
            for (int i = 0; i < optionToggles.Length; i++)
            {
                userSelected[i] = optionToggles[i].isOn;
            }

            isCorrect = CheckMultipleChoiceAnswer(userSelected, currentQ.correctMultiAnswers);
        }

        Feedbackpanel.SetActive(true);
        // 如果答对了，score +1
        if (isCorrect)
        {
            score++;
            feedbackText.text = "恭喜你成功完成了任务！\n电子奔流不息，点亮万家灯火！" ;
        }
        else
        {
            feedbackText.text = "今天的调度出了点小问题，\n但相信你是可以的！" ;
        }

        // 进入下一题
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
            // 全部答完，可以进行游戏结束或结果展示
            feedbackText.text += "你已经完成所有任务！" ;
            // 根据需求，可以让提交按钮失效或者跳转到结算界面
            submitButton.interactable = false;
            SceneManager.LoadScene("Scene_Quiz");
        }
    }

    /// <summary>
    /// 在 UI 上展示当前题目
    /// </summary>
    /// <param name="index"></param>
    private void DisplayQuestion(int index)
    {
        if (index < 0 || index >= questions.Count) return;

        QuestionData q = questions[index];
        questionText.text = q.questionTitle;

        // 更新选项文字
        for (int i = 0; i < optionTexts.Length; i++)
        {
            optionTexts[i].text = q.options[i];
            // 重置每个 Toggle 的 isOn 状态
            optionToggles[i].isOn = false;
        }
    }

    /// <summary>
    /// 判断多选题是否完全匹配正确答案
    /// </summary>
    private bool CheckMultipleChoiceAnswer(bool[] userSelected, bool[] correctAnswers)
    {
        if (userSelected.Length != correctAnswers.Length) return false;

        for (int i = 0; i < correctAnswers.Length; i++)
        {
            if (userSelected[i] != correctAnswers[i])
            {
                return false; // 只要有一个选项不符合，就错误
            }
        }
        return true;
    }

    /// <summary>
    /// 如果未在 Inspector 填写题目，可以在此写死 5 个题目（前 3 单选，后 2 多选）。
    /// </summary>
    private void LoadDefaultQuestions()
    {
        QuestionData q1 = new QuestionData
        {
            questionTitle = "太阳升起，城市慢慢苏醒。居民们陆续开启各类家用电器，商铺和小型工厂也开始一天的作业。虽然负荷开始上升，但总体还算平稳。你需要：\n        监测当前用电负荷趋势；\n        适度提高发电机组功率输出；\n        避免大幅度过载或频繁调整，保持发电机组在安全区间运转。\r\n合理的调控可以让城市白天的用电顺畅无碍。加油！”",
            options = new string[]
    {
        "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
    },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 1 // 第 2 个选项为正确答案
        };

        QuestionData q2 = new QuestionData
        {
            questionTitle = "华灯初上，城镇人们纷纷回家，用电需求在短时间内快速提升。家家户户开灯、做饭、娱乐，商场的霓虹灯也亮起。此刻的夜间用电高峰可能与白天不一样，需要你灵活应对。\n        留意用电负荷的快速上升；\n        适当调高发电机组出力，避免电力不足；\n        谨防大幅度调度带来的系统震荡。",
            options = new string[]
            {
        "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
            },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 1 // 第 2 个选项为正确答案
        };

        QuestionData q3 = new QuestionData
        {
            questionTitle = "清晨的阳光铺洒在山谷，微风拂面，风力机组已开始缓缓转动。水库水位处于较为正常的范围，可以适度出力。此时的城市负荷逐步上升，还不算太高。你需要：\n    观察当前风速，合理安排风机并网出力；\n    让水电机组在中等水平运行，以配合火电机组满足白天的用电需求；\n    预留一定调节余量，以应对临时负荷波动或风力变化。",
            options = new string[]
            {
       "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
            },
            questionType = QuestionType.MultipleChoice,
            // 正确答案：第 2 项为true，其余为false
            correctMultiAnswers = new bool[] { false, true, false, false, false }
        };

        QuestionData q4 = new QuestionData
        {
            questionTitle = "夜幕降临，居民生活用电快速攀升，商圈霓虹灯开启，城市进入夜间活跃时段。风力较白天略有减弱，但仍有一定输出。水电出力可用于平抑负荷尖峰。你要：\n    关注夜间负荷高峰，适度提高火电或水电出力应对用电猛增；\n    若风力下降显著，应优先保证火电的稳定输出，防止缺口；\n    避免水电机组过度放水，以免后续无法满足清晨或突发负荷需求。",
            options = new string[]
            {
        "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
            },
            questionType = QuestionType.MultipleChoice,
            // 正确答案：第 1 和第 3 项为 true
            correctMultiAnswers = new bool[] { true, false, true, false, false }
        };

        QuestionData q5 = new QuestionData
        {
            questionTitle = "在今天的白天和夜晚，你将首次尝试调度包含核电机组的电网。核电机组已进入并网阶段，保持中高负荷稳定输出。你需要在负荷低时避免核电冗余过多，也要预留出一定火电或水电调节空间。\n    利用弹性储能站帮助削峰填谷：当白天负荷上升速度过快，储能站可快速输出电能；夜深负荷逐渐降低时，则可将多余电能充入储能设备，避免浪费可再生能源或核电过载。\n    电容调频站可提供无功补偿和频率支持。若发现风电出力波动或水电调节导致电网频率忽高忽低，别忘了及时启用调频功能。",
            options = new string[]
            {
        "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
            },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 1 // 第 2 个选项为正确答案
        };

        QuestionData q6 = new QuestionData
        {
            questionTitle = "气象预报预示着一场电网调度的硬仗：持续雷击天气即将到来，从延续整个深夜！\n    雷击风险：输电线路和变电站可能遭遇高强度雷击，导致线路跳闸或短暂停电。准备好检修队伍和备用线路的调度方案；\n    大风伴随雷暴，风电出力可能瞬间飙升或骤降；\n    核电机组仍要稳定输出，但你需要时刻监控主要输电线路的负载情况。",
            options = new string[]
            {
       "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
            },
            questionType = QuestionType.MultipleChoice,
            // 正确答案：第 1 和第 3 项为 true
            correctMultiAnswers = new bool[] { true, false, true, false, false }
        };

        QuestionData q7 = new QuestionData
        {
            questionTitle = "雷雨刚过，又接到寒潮警报：从今天白天持续到深夜，气温骤降，城市采暖负荷急剧上升。\n    白天与夜晚的负荷将远高于平时，尤其是居民和公共建筑的电热需求井喷；\n    核电依旧维持强劲的基荷，但你需要通过火电或水电以及储能快速调节峰值；",
            options = new string[]
            {
       "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
            },
            questionType = QuestionType.SingleChoice,
            correctSingleAnswerIndex = 2 // 第 3 个选项为正确答案
        };

        QuestionData q8 = new QuestionData
        {
            questionTitle = "一旦出现供电紧张或频率下降，可同时启用电容调频站稳定系统；\n    谨慎使用储能：寒潮期间可能出现持续高负荷，过度放电后，要确保有足够时间和功率进行再充电。\n    面对骤降气温，你的调度决策将影响成千上万人的采暖与照明。既要防止大范围限电，又要保证核电机组的安全余量，考验即在此刻！”",
            options = new string[]
            {
        "机组1",
        "机组2",
        "机组3",
        "机组4",
        "实时监控",
        "机组1",
        "机组2",
        "机组1",
        "机组2",
        "发电机组",
        " ",
        "单元1",
        "单元2",
        "单元1",
        "单元2"
            },
            questionType = QuestionType.MultipleChoice,
            // 正确答案：第 2 项为 true，其余为 false
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
    /// 题目类型：单选 or 多选
    /// </summary>
    /// 
    public void ChangeBackground()
    {
        if (backgroundImages.Length == 0 || panelBackground == null)
            return;

        currentIndex = (currentIndex + 1) % backgroundImages.Length; // 按顺序循环
        panelBackground.sprite = backgroundImages[currentIndex]; // 更换背景
    }



    // 单个Toggle在某个模式下的配置
    [System.Serializable]
    public class ToggleConfig
    {
        // 是否显示该Toggle
        public bool isActive;
        // 如果显示，则设置其锚点位置（一般用于UI布局）
        public Vector2 position;
    }

    // 一种模式下对所有Toggle的配置
    [System.Serializable]
    public class ModeConfig
    {
        // 模式名称（可选，用于标识或调试）
        public string modeName;
        // 每个Toggle的配置，要求数量与optionToggles一致或者少于optionToggles
        public ToggleConfig[] toggleConfigs;
    }

    // 预留3种模式，可以在Inspector中展开进行配置
    public ModeConfig[] modeConfigs = new ModeConfig[3];

    /// <summary>
    /// 根据模式编号设置Toggle的显示状态与位置。
    /// modeIndex: 0, 1, 2 三种模式中的一种
    /// </summary>
    /// <param name="modeIndex">模式编号</param>
    public void SetMode(int modeIndex)
    {
        if (modeIndex < 0 || modeIndex >= modeConfigs.Length)
        {
            Debug.LogError("无效的模式编号：" + modeIndex);
            return;
        }

        ModeConfig selectedMode = modeConfigs[modeIndex];

        // 遍历每个Toggle，按照配置调整状态和位置
        for (int i = 0; i < optionToggles.Length; i++)
        {
            // 如果当前模式配置中有对应的Toggle配置，则按配置设置
            if (i < selectedMode.toggleConfigs.Length)
            {
                ToggleConfig config = selectedMode.toggleConfigs[i];
                // 设置Toggle显示或隐藏
                optionToggles[i].gameObject.SetActive(config.isActive);

                // 如果该Toggle显示，则更新其位置
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
                // 如果配置中没有对应的项，则默认隐藏该Toggle
                optionToggles[i].gameObject.SetActive(false);
            }
        }
    }

    public void closefeedback()
    {
        Feedbackpanel.SetActive(false);
    }
}

