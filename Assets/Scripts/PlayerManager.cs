using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    [Header("Player Settings")]
    public Parameter parameter;

    public bool CheckACParameter(Parameter need)
    {
        if (parameter.UT < need.UT) return false;
        if (parameter.CD < need.CD) return false;
        if (parameter.LD < need.LD) return false;
        if (parameter.SK < need.SK) return false;
        if (parameter.ML < need.ML) return false;
        if (parameter.NG < need.NG) return false;
        if (parameter.TX < need.TX) return false;
        if (parameter.QL < need.QL) return false;
        if (parameter.TB < need.TB) return false;
        if (parameter.V < need.V) return false;
        if (parameter.L < need.L) return false;

        return true;
    }
    public void UpdateParameter(Parameter change)
    {
        parameter.UT += change.UT;
        parameter.CD += change.CD;
        parameter.LD += change.LD;
        parameter.SK += change.SK;
        parameter.ML += change.ML;
        parameter.NG += change.NG;
        parameter.TX += change.TX;
        parameter.QL += change.QL;
        parameter.TB += change.TB;
        parameter.V += change.V;
        parameter.L += change.L;
    }

}
[System.Serializable]
public class Parameter
{
    public int UT = 6; // uy tín: mức độ kính nể cá nhân.
    public int CD = 4; // chính danh: hợp pháp hóa quyền lực (dựa vào danh nghĩa, ủng hộ của hào trưởng, tôn thất).
    public int LD = 7; // lòng dân: thiện cảm của dân chúng (ảnh hưởng tuyển dân binh, thu thuế).
    public int SK = 6; // sĩ khí: tinh thần quân đội (ảnh hưởng giao chiến).
    public int ML = 5; // mưu lược: mở lựa chọn mưu kế, phản gián, lừa địch.
    public int NG = 4; // ngoại giao: thành bại khi đàm phán, hôn phối, liên minh.
    public int TX = 5; // thủy chiến: hiệu quả trận sông, đầm, bãi triều.
    public int QL = 600; // quân lực: tổng sức mạnh quân (0–10.000).
    public int TB = 3; // tình báo: mức độ nắm bắt thông tin (ảnh hưởng phản gián, mưu kế).
    public int V = 30; // vàng
    public int L = 120;
}
