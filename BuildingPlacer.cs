using UnityEngine;
using UnityEngine.UI; // ����UI���

public class BuildingPlacer : MonoBehaviour
{
    public GameObject selectedBuildingPrefab;  // ��ǰѡ��Ľ�����Ԥ����
    public int buildingindex;
    private GameObject previewBuilding;        // Ԥ��������
    private bool isPlacingBuilding = false;    // �Ƿ��ڷ��ý���״̬
    public LayerMask groundLayer;
    public BuildingManager buildingManager;
    public Text warningText;                  // ������ʾ������ı����
    public GameObject warningPanel;           // �������
    public Button closeWarningButton;         // �رվ������İ�ť

    public int maxBuildings = 10;             // �����������
    private int currentBuildingCount = 0;     // ��ǰ����������

    public float requiredMoney = 100f;        // ���ý���������Ľ�Ǯ
    private float currentMoney = 200f;        // ��ǰ��ҽ�Ǯ

    public Camera cam;  // �����
    public GameObject selectedBuilding;  // ��ǰѡ�еĽ���
    public LayerMask buildingLayer;  // ���������ڵĲ�
    public GameObject optionsPanel;  // ����ѡ�����





    void Start()
    {
        // ���ؾ�������ʼ
        warningPanel.SetActive(false);
        optionsPanel.SetActive(false);
        closeWarningButton.onClick.AddListener(CloseWarningPanel);
    }

    void Update()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //��ת��ɾ��������
        // ͨ���������ѡ����
        if (Input.GetMouseButtonDown(0))  // ������
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);  // �����λ�÷�������
            

            // ��������Ƿ�����˽�����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingLayer))
            {
                selectedBuilding = hit.collider.gameObject;  // ѡ�н�����
                ShowBuildingOptions();  // ��ʾ��������
            }
        }




        // ���û��ѡ��������ߴ��ڷǷ���״̬��ֱ���˳�
        if (selectedBuildingPrefab == null || !isPlacingBuilding)
            return;

        // ������������λ�÷�������
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // ���Ԥ�������ﲻ���ڣ��򴴽�һ��
            if (previewBuilding == null)
            {
                previewBuilding = Instantiate(selectedBuildingPrefab);
                SetPreviewMaterial(previewBuilding, true); // ����͸����
            }

            previewBuilding.transform.position = hit.point;

            // ���������������ж��Ƿ���Է���
            if (Input.GetMouseButtonDown(0))
            {
                if (CanPlaceBuilding(hit.point))
                {
                    Vector3 roundedPosition = new Vector3(
                          Mathf.Floor(hit.point.x/10f)*10f,
                          hit.point.y,
                          Mathf.Floor(hit.point.z/10f)*10f
                     );

                    // ���ý�����
                    GameObject newBuilding = Instantiate(selectedBuildingPrefab, roundedPosition, Quaternion.identity);
                    buildingManager.AddBuilding(newBuilding,buildingindex);
                    currentBuildingCount++; // ���ӽ���������
                    currentMoney = currentMoney - requiredMoney; // �۳���Ǯ

                    Destroy(previewBuilding);  // ����Ԥ��������
                    previewBuilding = null;    // ����Ԥ��������

                    // ���ú��˳�����ģʽ
                    isPlacingBuilding = false;
                }
                else
                {
                    // ���λ�ò�������ã��������洰��
                    ShowWarning("�ʽ�����������������������Ŷ");
                }
            }
        }
    }



    void ShowBuildingOptions()
    {
        // ��ʾ�������
        optionsPanel.SetActive(true);
    }

    public void RotateBuilding()
    {
        if (selectedBuilding != null)
        {
            // ��ת����
            selectedBuilding.transform.Rotate(Vector3.up, 180f);  // ÿ����ת90��
        }
    }

    public void DeleteBuilding()
    {
        if (selectedBuilding != null)
        {
            Destroy(selectedBuilding);  // ɾ��ѡ�еĽ���
            selectedBuilding = null;  // ���ѡ�н���
            optionsPanel.SetActive(false);  // ���ز������
        }
    }

    // ѡ����ʱ���ô˷���
    public void SetSelectedBuilding(GameObject buildingPrefab,int index)
    {
        selectedBuildingPrefab = buildingPrefab;
        buildingindex = index;
        isPlacingBuilding = true;  // ��������ģʽ

        // �������Ԥ��������������
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
            previewBuilding = null;
        }
    }

    // ����Ƿ�������ý���������
    bool CanPlaceBuilding(Vector3 position)
    {
        // ��齨���������Ƿ񳬹��������
        //if (currentBuildingCount >= maxBuildings)
        //{
        //    ShowWarning("You cannot place more buildings.");
        //    return false;
        //}

        // ����Ǯ�Ƿ��㹻
        if (buildingManager.playerMoney < 140)
        {
            ShowWarning("��Ǯ���㣡");
            return false;
        }

        // ������λ�õ�z�Ƿ����0
        //if (position.z <= 0)
        //{
        //    ShowWarning("Buildings can only be placed where z > 0.");
        //    return false;
        //}

        // ʹ�����߼��Y > 0λ���Ƿ�������
        //RaycastHit hit;
        //if (Physics.Raycast(position + Vector3.up * 1f, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        //{
        //    if (hit.point.y > 0) // ���Y > 0��������
        //    {
        //        ShowWarning("There is an obstacle above this position.");
        //        return false; // ���Y > 0�������壬���ܷ��ý���
        //    }
        //}

            // ʹ�����߼��Y <= 0λ���Ƿ�������
            //if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            //{
            //    // ���Y <= 0�������壬�������
            //    if (hit.collider != null)
            //    {
            //        return true; // ���Y <= 0�������壬�������
            //    }
            //}

            // ���û�к��ʵĵط�����
            //ShowWarning("This position is not valid for placing a building.");
       else {
            return true; // ������������㣬����false
        }
    }

    // ��ʾ������Ϣ
    void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true); // ��ʾ�������

        // ���ý�����ʱ�Զ��ͷ�
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);  // ����Ԥ��������
            previewBuilding = null;    // ����Ԥ��������
        }

        // ֹͣ����״̬
        isPlacingBuilding = false;

        // 3������ؾ������
        Invoke(nameof(HideWarning), 3f);
    }

    void HideWarning()
    {
        warningPanel.SetActive(false);
    }


    // �رվ������
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
