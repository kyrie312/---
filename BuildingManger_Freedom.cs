using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // ���� UI ��



public class BuildingManager_Freedom : MonoBehaviour
{
    public List<Building> placedBuildings = new List<Building>();  // ���õĽ������б�
    public GameObject[] buildingPrefabs;  // ����Ԥ������
    public SaveSystem saveSystem;  // ����ϵͳ
    private Dictionary<int, int> buildingCountChanges = new Dictionary<int, int>();   // ���ڼ�¼�����������仯���ֵ�

    // ���ڼ�¼�����������仯������
    public BuildingCount[] buildingCounts;

    public int playerMoney = 1000;  // ��ʼ��Ǯ
    public Text moneyText;


    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    // ���½�Ǯ�����ݽ����������仯
    public void UpdateMoney()
    {
        playerMoney = 1000;  // ���ý�ǮΪ1000

        // ���������������ֵ䣬�����Ǯ�仯
        foreach (var kvp in buildingCountChanges)
        {
            int prefabIndex = kvp.Key;
            int count = kvp.Value;

            if (prefabIndex == 1)  // ���轨����1��������1
            {
                playerMoney -= 200 * count;  // ÿ��������1����200��Ǯ
            }
            else if (prefabIndex == 0)  // ���轨����2��������2
            {
                playerMoney -= 100 * count;  // ÿ��������2����100��Ǯ
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


    private bool buildingPrefabMatches(Building building, int prefabIndex)
    {
        // ���� 1��ͨ������������ͣ�����ÿ�������ﶼ��һ�������ֶΣ�
        //if (building.type == buildingPrefabs[prefabIndex].name)  // ����prefab�������뽨��������һ��
        // {
        //    return true;
        //}

        // ���� 2��ͨ���������ID������ÿ����������Ψһ��ID��
        // if (building.id == buildingPrefabs[prefabIndex].name)  // ����ID��Ԥ�����������
        // {
        //    return true;
        // }

        //���� 3��ͨ������������ƣ�����ÿ�������������ƣ�
        // if (building.name == buildingPrefabs[prefabIndex].name)
        //{
        //   return true;
        // }
         if (building.name == "Building_1")
        {
          return true;
         }

        // ���� 4��ͨ���������λ�ã�����ÿ����������λ�����ԣ�������Ҫ�����ӵıȽϣ�
        // if (building.position == buildingPrefabs[prefabIndex].transform.position)
        // {
        //     return true;
        // }

        // ���� 5��ͨ�������������ĳ���ض����ԣ����罨�����Ƿ�����ĳ���ض����ܣ�
        // if (building.category == "residential")  // ���磬���ǿ��Ը��ݽ������������ƥ��
        // {
        //     return true;
        // }

        // ���� 6��ͨ��������Ĺ���ʱ�����ĳЩ����������ÿ����������ʱ����������������ֶΣ�
        // if (building.buildTime == Time.time)  // ���磬���ݹ���ʱ�����ƥ��
        // {
        //     return true;
        // }

        return false;
    }
}
