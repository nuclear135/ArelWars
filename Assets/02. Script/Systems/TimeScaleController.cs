using UnityEngine;

// 게임 배속 제어 (1x / 2x / 4x)
public class TimeScaleController : MonoBehaviour
{
    [SerializeField] private float[] speeds = new float[] { 1f, 2f, 4f };

    private int idx = 1; // 기본 1x

    public void NextSpeed()
    {
        idx = (idx + 1) % speeds.Length;
        Time.timeScale = speeds[idx];
        Debug.Log("[Speed] x" + speeds[idx].ToString("0.0"));
    }

    public void SetSpeed(float s)
    {
        Time.timeScale = s;
    }
}
