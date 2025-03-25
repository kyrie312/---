using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // ���� UI ��
public class BuildingManager : MonoBehaviour
{
    public List<Building> placedBuildings = new List<Building>();  // ���õĽ������б�
    public GameObject[] buildingPrefabs;  // ����Ԥ������
    public SaveSystem saveSystem;  // ����ϵͳ
    private Dictionary<int, int> buildingCountChanges = new Dictionary<int, int>();   // ���ڼ�¼�����������仯���ֵ�

    // ���ڼ�¼�����������仯������
    public BuildingCount[] buildingCounts;

    public int playerMoney = 3000;  // ��ʼ��Ǯ
    public Text moneyText;


    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    // ���½�Ǯ�����ݽ����������仯
    public void UpdateMoney()
    {
        playerMoney = 3000;  // ���ý�ǮΪ1000

        // ���������������ֵ䣬�����Ǯ�仯
        foreach (var kvp in buildingCountChanges)
        {
            int prefabIndex = kvp.Key;
            int count = kvp.Value;

            if (prefabIndex == 0)  // ���轨����0��������0
            {
                playerMoney -= 200 * count;  // ÿ��������1����200��Ǯ
            }
            else if (prefabIndex == 1)  // ���轨����1��������1
            {
                playerMoney -= 200 * count;  // ÿ��������1����200��Ǯ
            }
            else if (prefabIndex == 2)  // ���轨����2��������2
            {
                playerMoney -= 150 * count;  // ÿ��������1����150��Ǯ
            }
            else if (prefabIndex == 3)  // ���轨����3��������3
            {
                playerMoney -= 50 * count;  // ÿ��������3����50��Ǯ
            }
            else if (prefabIndex == 4)  // ���轨����4��������4
            {
                playerMoney -= 250 * count;  // ÿ��������1����250��Ǯ
            }
            else if (prefabIndex == 5)  // ���轨����5��������5
            {
                playerMoney -= 40 * count;  // ÿ��������1����40��Ǯ
            }
            else if (prefabIndex == 6)  // ���轨����6��������6
            {
                playerMoney -= 100 * count;  // ÿ��������6����100��Ǯ
            }
            else if (prefabIndex == 7)  // ���轨����7��������7
            {
                playerMoney -= 300 * count;  // ÿ��������7����300��Ǯ
            }
        }

        // ����UI��ʾ�Ľ�Ǯ
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        moneyText.text = "��Ǯ: " + playerMoney.ToString();
    }



    public void AddBuilding(GameObject buildingObject, int buildingID)
    {
        // ��ȡ�����������
        string buildingName = buildingObject.name;

        // ����һ���µĽ��������
        Building newBuilding = new Building(
            buildingObject.transform.position,
            buildingObject.transform.rotation.eulerAngles,
            buildingName,  // ����������
            System.Guid.NewGuid().ToString(),  // ����ΨһID
            buildingName,  // ����������Ϊ����
            "residential",  // ʾ����𣬿��Ը���ʵ����Ҫ����
            Time.time  // ���蹹��ʱ��Ϊ��Ϸ����ʱ��
        );


        UpdateMoney();
        

        // ��ӵ��ѷ��ý������б���
        placedBuildings.Add(newBuilding);

        // ��������һ���ֵ�����¼ÿ�������������仯
        if (buildingCountChanges.ContainsKey(buildingID))
        {
            buildingCountChanges[buildingID] += 1;  // ����ý����Ѵ��ڣ���������
        }
        else
        {
            buildingCountChanges[buildingID] = 1;  // ����ý����ǵ�һ�γ��֣���ʼ������Ϊ1
        }

        // ��¼�����������仯�����飨�����������ݽṹ�������ں���ʹ��
        UpdateBuildingCount(buildingID);
        
    }

    private void UpdateBuildingCount(int buildingID)
    {
        // �������һ����������¼�����仯���������������
        // ���磬��������һ������洢������ID��������
        for (int i = 0; i < buildingCounts.Length; i++)
        {
            if (buildingCounts[i].buildingID == buildingID)
            {
                buildingCounts[i].count = buildingCountChanges[buildingID];
                break;
            }
        }
    }


    public void RemoveBuilding(GameObject buildingObject)
    {
        Building toRemove = placedBuildings.Find(b => b.position == buildingObject.transform.position);
        if (toRemove != null)
        {
            placedBuildings.Remove(toRemove);
            Destroy(buildingObject);
        }
    }

    public void SaveBuildings()
    {
        // ����SaveSystem��SaveBuildings���������뽨�����б�������仯�ֵ�
        saveSystem.SaveBuildings(placedBuildings, buildingCountChanges);
    }

    // ���ؽ���������
    public void LoadBuildings()
    {
        // ���ؽ��������ݺ������仯�ֵ�
        var (loadedBuildings, loadedBuildingCountChanges) = saveSystem.LoadBuildings();

        if (loadedBuildings != null && loadedBuildings.Count > 0)
        {
            // �����ǰ�ѷ��õĽ�����
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // ���·��õĽ������б�������仯�ֵ�
            placedBuildings = loadedBuildings;
            buildingCountChanges = loadedBuildingCountChanges;

            // ����ʵ����������
            foreach (Building building in placedBuildings)
            {
                GameObject prefab = buildingPrefabs[0];  // Ĭ��ʹ�õ�һ��Ԥ��
                Instantiate(prefab, building.position, Quaternion.Euler(building.rotation), transform);
            }
        }
        else
        {
            Debug.LogWarning("[BuildingManager] û�м��ص�����������");
        }
    }

    // ��ȡָ�����ͽ����������
    public int GetBuildingCount(int prefabIndex)
    {
        // ����ֵ����Ƿ�����ý�����ID��prefabIndex��
        if (buildingCountChanges.ContainsKey(prefabIndex))
        {
            // ������������ض�Ӧ�Ľ���������
            return buildingCountChanges[prefabIndex];
        }
        else
        {
            // ����ֵ���û�иý�����ID������0
            return 0;
        }
    }
    
}
