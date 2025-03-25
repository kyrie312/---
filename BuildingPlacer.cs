using UnityEngine;
using UnityEngine.UI; // 引入UI组件

public class BuildingPlacer : MonoBehaviour
{
    public GameObject selectedBuildingPrefab;  // 当前选择的建筑物预制体
    public int buildingindex;
    private GameObject previewBuilding;        // 预览建筑物
    private bool isPlacingBuilding = false;    // 是否处于放置建筑状态
    public LayerMask groundLayer;
    public BuildingManager buildingManager;
    public Text warningText;                  // 用于显示警告的文本组件
    public GameObject warningPanel;           // 警告面板
    public Button closeWarningButton;         // 关闭警告面板的按钮

    public int maxBuildings = 10;             // 最大建筑物数量
    private int currentBuildingCount = 0;     // 当前建筑物数量

    public float requiredMoney = 100f;        // 放置建筑物所需的金钱
    private float currentMoney = 200f;        // 当前玩家金钱

    public Camera cam;  // 摄像机
    public GameObject selectedBuilding;  // 当前选中的建筑
    public LayerMask buildingLayer;  // 建筑物所在的层
    public GameObject optionsPanel;  // 操作选项面板





    void Start()
    {
        // 隐藏警告面板初始
        warningPanel.SetActive(false);
        optionsPanel.SetActive(false);
        closeWarningButton.onClick.AddListener(CloseWarningPanel);
    }

    void Update()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //旋转和删除建筑物
        // 通过鼠标点击来选择建筑
        if (Input.GetMouseButtonDown(0))  // 左键点击
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);  // 从鼠标位置发射射线
            

            // 检测射线是否击中了建筑物
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingLayer))
            {
                selectedBuilding = hit.collider.gameObject;  // 选中建筑物
                ShowBuildingOptions();  // 显示操作窗口
            }
        }




        // 如果没有选择建筑物，或者处于非放置状态，直接退出
        if (selectedBuildingPrefab == null || !isPlacingBuilding)
            return;

        // 从摄像机向鼠标位置发射射线
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // 如果预览建筑物不存在，则创建一个
            if (previewBuilding == null)
            {
                previewBuilding = Instantiate(selectedBuildingPrefab);
                SetPreviewMaterial(previewBuilding, true); // 设置透明度
            }

            previewBuilding.transform.position = hit.point;

            // 如果鼠标左键点击，判断是否可以放置
            if (Input.GetMouseButtonDown(0))
            {
                if (CanPlaceBuilding(hit.point))
                {
                    Vector3 roundedPosition = new Vector3(
                          Mathf.Floor(hit.point.x/10f)*10f,
                          hit.point.y,
                          Mathf.Floor(hit.point.z/10f)*10f
                     );

                    // 放置建筑物
                    GameObject newBuilding = Instantiate(selectedBuildingPrefab, roundedPosition, Quaternion.identity);
                    buildingManager.AddBuilding(newBuilding,buildingindex);
                    currentBuildingCount++; // 增加建筑物数量
                    currentMoney = currentMoney - requiredMoney; // 扣除金钱

                    Destroy(previewBuilding);  // 销毁预览建筑物
                    previewBuilding = null;    // 重置预览建筑物

                    // 放置后退出放置模式
                    isPlacingBuilding = false;
                }
                else
                {
                    // 如果位置不允许放置，弹出警告窗口
                    ShowWarning("资金不足啦！可以试试其他方案哦");
                }
            }
        }
    }



    void ShowBuildingOptions()
    {
        // 显示操作面板
        optionsPanel.SetActive(true);
    }

    public void RotateBuilding()
    {
        if (selectedBuilding != null)
        {
            // 旋转建筑
            selectedBuilding.transform.Rotate(Vector3.up, 180f);  // 每次旋转90度
        }
    }

    public void DeleteBuilding()
    {
        if (selectedBuilding != null)
        {
            Destroy(selectedBuilding);  // 删除选中的建筑
            selectedBuilding = null;  // 清空选中建筑
            optionsPanel.SetActive(false);  // 隐藏操作面板
        }
    }

    // 选择建筑时调用此方法
    public void SetSelectedBuilding(GameObject buildingPrefab,int index)
    {
        selectedBuildingPrefab = buildingPrefab;
        buildingindex = index;
        isPlacingBuilding = true;  // 开启放置模式

        // 如果已有预览建筑，销毁它
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
            previewBuilding = null;
        }
    }

    // 检查是否满足放置建筑的条件
    bool CanPlaceBuilding(Vector3 position)
    {
        // 检查建筑物数量是否超过最大限制
        //if (currentBuildingCount >= maxBuildings)
        //{
        //    ShowWarning("You cannot place more buildings.");
        //    return false;
        //}

        // 检查金钱是否足够
        if (buildingManager.playerMoney < 140)
        {
            ShowWarning("金钱不足！");
            return false;
        }

        // 检查放置位置的z是否大于0
        //if (position.z <= 0)
        //{
        //    ShowWarning("Buildings can only be placed where z > 0.");
        //    return false;
        //}

        // 使用射线检测Y > 0位置是否有物体
        //RaycastHit hit;
        //if (Physics.Raycast(position + Vector3.up * 1f, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        //{
        //    if (hit.point.y > 0) // 如果Y > 0处有物体
        //    {
        //        ShowWarning("There is an obstacle above this position.");
        //        return false; // 如果Y > 0处有物体，不能放置建筑
        //    }
        //}

            // 使用射线检测Y <= 0位置是否有物体
            //if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            //{
            //    // 如果Y <= 0处有物体，允许放置
            //    if (hit.collider != null)
            //    {
            //        return true; // 如果Y <= 0处有物体，允许放置
            //    }
            //}

            // 如果没有合适的地方放置
            //ShowWarning("This position is not valid for placing a building.");
       else {
            return true; // 如果条件不满足，返回false
        }
    }

    // 显示警告信息
    void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true); // 显示警告面板

        // 放置建筑物时自动释放
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);  // 销毁预览建筑物
            previewBuilding = null;    // 重置预览建筑物
        }

        // 停止放置状态
        isPlacingBuilding = false;

        // 3秒后隐藏警告面板
        Invoke(nameof(HideWarning), 3f);
    }

    void HideWarning()
    {
        warningPanel.SetActive(false);
    }


    // 关闭警告面板
    void CloseWarningPanel()
    {
        warningPanel.SetActive(false);
    }

    private void SetPreviewMaterial(GameObject obj, bool transparent)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            Material mat = new Material(r.material);
            Color c = mat.color;
            c.a = transparent ? 0.5f : 1f;
            mat.color = c;
            r.material = mat;
        }
    }
}
