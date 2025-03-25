using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 引入 UI 库



public class BuildingManager_Freedom : MonoBehaviour
{
    public List<Building> placedBuildings = new List<Building>();  // 放置的建筑物列表
    public GameObject[] buildingPrefabs;  // 建筑预设数组
    public SaveSystem saveSystem;  // 保存系统
    private Dictionary<int, int> buildingCountChanges = new Dictionary<int, int>();   // 用于记录建筑物数量变化的字典

    // 用于记录建筑物数量变化的数组
    public BuildingCount[] buildingCounts;

    public int playerMoney = 1000;  // 初始金钱
    public Text moneyText;


    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    // 更新金钱，根据建筑物数量变化
    public void UpdateMoney()
    {
        playerMoney = 1000;  // 重置金钱为1000

        // 遍历建筑物数量字典，计算金钱变化
        foreach (var kvp in buildingCountChanges)
        {
            int prefabIndex = kvp.Key;
            int count = kvp.Value;

            if (prefabIndex == 1)  // 假设建筑物1的索引是1
            {
                playerMoney -= 200 * count;  // 每个建筑物1减少200金钱
            }
            else if (prefabIndex == 0)  // 假设建筑物2的索引是2
            {
                playerMoney -= 100 * count;  // 每个建筑物2减少100金钱
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


    private bool buildingPrefabMatches(Building building, int prefabIndex)
    {
        // 方法 1：通过建筑物的类型（假设每个建筑物都有一个类型字段）
        //if (building.type == buildingPrefabs[prefabIndex].name)  // 假设prefab的名字与建筑物类型一致
        // {
        //    return true;
        //}

        // 方法 2：通过建筑物的ID（假设每个建筑物有唯一的ID）
        // if (building.id == buildingPrefabs[prefabIndex].name)  // 假设ID与预设名字相关联
        // {
        //    return true;
        // }

        //方法 3：通过建筑物的名称（假设每个建筑物有名称）
        // if (building.name == buildingPrefabs[prefabIndex].name)
        //{
        //   return true;
        // }
         if (building.name == "Building_1")
        {
          return true;
         }

        // 方法 4：通过建筑物的位置（假设每个建筑物有位置属性，可能需要更复杂的比较）
        // if (building.position == buildingPrefabs[prefabIndex].transform.position)
        // {
        //     return true;
        // }

        // 方法 5：通过建筑物的类别和某个特定属性（例如建筑物是否属于某个特定功能）
        // if (building.category == "residential")  // 例如，我们可以根据建筑物的类别进行匹配
        // {
        //     return true;
        // }

        // 方法 6：通过建筑物的构建时间或者某些条件（假设每个建筑物有时间戳或者其他条件字段）
        // if (building.buildTime == Time.time)  // 例如，根据构建时间进行匹配
        // {
        //     return true;
        // }

        return false;
    }
}
