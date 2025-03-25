using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 引入 UI 库
public class BuildingManager : MonoBehaviour
{
    public List<Building> placedBuildings = new List<Building>();  // 放置的建筑物列表
    public GameObject[] buildingPrefabs;  // 建筑预设数组
    public SaveSystem saveSystem;  // 保存系统
    private Dictionary<int, int> buildingCountChanges = new Dictionary<int, int>();   // 用于记录建筑物数量变化的字典

    // 用于记录建筑物数量变化的数组
    public BuildingCount[] buildingCounts;

    public int playerMoney = 3000;  // 初始金钱
    public Text moneyText;


    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    // 更新金钱，根据建筑物数量变化
    public void UpdateMoney()
    {
        playerMoney = 3000;  // 重置金钱为1000

        // 遍历建筑物数量字典，计算金钱变化
        foreach (var kvp in buildingCountChanges)
        {
            int prefabIndex = kvp.Key;
            int count = kvp.Value;

            if (prefabIndex == 0)  // 假设建筑物0的索引是0
            {
                playerMoney -= 200 * count;  // 每个建筑物1减少200金钱
            }
            else if (prefabIndex == 1)  // 假设建筑物1的索引是1
            {
                playerMoney -= 200 * count;  // 每个建筑物1减少200金钱
            }
            else if (prefabIndex == 2)  // 假设建筑物2的索引是2
            {
                playerMoney -= 150 * count;  // 每个建筑物1减少150金钱
            }
            else if (prefabIndex == 3)  // 假设建筑物3的索引是3
            {
                playerMoney -= 50 * count;  // 每个建筑物3减少50金钱
            }
            else if (prefabIndex == 4)  // 假设建筑物4的索引是4
            {
                playerMoney -= 250 * count;  // 每个建筑物1减少250金钱
            }
            else if (prefabIndex == 5)  // 假设建筑物5的索引是5
            {
                playerMoney -= 40 * count;  // 每个建筑物1减少40金钱
            }
            else if (prefabIndex == 6)  // 假设建筑物6的索引是6
            {
                playerMoney -= 100 * count;  // 每个建筑物6减少100金钱
            }
            else if (prefabIndex == 7)  // 假设建筑物7的索引是7
            {
                playerMoney -= 300 * count;  // 每个建筑物7减少300金钱
            }
        }

        // 更新UI显示的金钱
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        moneyText.text = "金钱: " + playerMoney.ToString();
    }



    public void AddBuilding(GameObject buildingObject, int buildingID)
    {
        // 获取建筑物的名称
        string buildingName = buildingObject.name;

        // 创建一个新的建筑物对象
        Building newBuilding = new Building(
            buildingObject.transform.position,
            buildingObject.transform.rotation.eulerAngles,
            buildingName,  // 建筑物名称
            System.Guid.NewGuid().ToString(),  // 生成唯一ID
            buildingName,  // 假设名称作为类型
            "residential",  // 示例类别，可以根据实际需要更改
            Time.time  // 假设构建时间为游戏运行时间
        );


        UpdateMoney();
        

        // 添加到已放置建筑物列表中
        placedBuildings.Add(newBuilding);

        // 假设你有一个字典来记录每个建筑的数量变化
        if (buildingCountChanges.ContainsKey(buildingID))
        {
            buildingCountChanges[buildingID] += 1;  // 如果该建筑已存在，数量增加
        }
        else
        {
            buildingCountChanges[buildingID] = 1;  // 如果该建筑是第一次出现，初始化数量为1
        }

        // 记录建筑物数量变化的数组（或者其他数据结构），用于后续使用
        UpdateBuildingCount(buildingID);
        
    }

    private void UpdateBuildingCount(int buildingID)
    {
        // 如果你有一个数组来记录数量变化，可以在这里更新
        // 例如，假设你有一个数组存储建筑物ID和数量：
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
        // 调用SaveSystem的SaveBuildings方法，传入建筑物列表和数量变化字典
        saveSystem.SaveBuildings(placedBuildings, buildingCountChanges);
    }

    // 加载建筑物数据
    public void LoadBuildings()
    {
        // 加载建筑物数据和数量变化字典
        var (loadedBuildings, loadedBuildingCountChanges) = saveSystem.LoadBuildings();

        if (loadedBuildings != null && loadedBuildings.Count > 0)
        {
            // 清除当前已放置的建筑物
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // 更新放置的建筑物列表和数量变化字典
            placedBuildings = loadedBuildings;
            buildingCountChanges = loadedBuildingCountChanges;

            // 重新实例化建筑物
            foreach (Building building in placedBuildings)
            {
                GameObject prefab = buildingPrefabs[0];  // 默认使用第一个预设
                Instantiate(prefab, building.position, Quaternion.Euler(building.rotation), transform);
            }
        }
        else
        {
            Debug.LogWarning("[BuildingManager] 没有加载到建筑物数据");
        }
    }

    // 获取指定类型建筑物的数量
    public int GetBuildingCount(int prefabIndex)
    {
        // 检查字典中是否包含该建筑物ID（prefabIndex）
        if (buildingCountChanges.ContainsKey(prefabIndex))
        {
            // 如果包含，返回对应的建筑物数量
            return buildingCountChanges[prefabIndex];
        }
        else
        {
            // 如果字典中没有该建筑物ID，返回0
            return 0;
        }
    }
    
}
