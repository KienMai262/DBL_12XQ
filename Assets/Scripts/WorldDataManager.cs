using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "WorldData", menuName = "Story/World Data Manager")]
public class WorldDataManager : ScriptableObject
{
    public List<Warlord> warlords;

    public void ResetAllWarlords()
    {
        foreach (var warlord in warlords)
        {
            warlord.status = WarlordStatus.Undefeated;
        }
    }

    public Warlord GetWarlordByName(string name)
    {
        return warlords.FirstOrDefault(w => w.warlordName == name);
    }

    // Hàm được tối ưu hóa
    public List<Warlord> GetAttackableWarlords()
    {
        // Sử dụng LINQ để code ngắn gọn và dễ đọc hơn
        return warlords.Where(warlord => 
            warlord.status == WarlordStatus.Undefeated && 
            (warlord.prerequisites == null || warlord.prerequisites.All(p => p.status == WarlordStatus.Defeated))
        ).ToList();
    }
}