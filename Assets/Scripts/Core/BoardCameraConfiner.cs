using Unity.Cinemachine;
using UnityEngine;

public class BoardCameraConfiner : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D m_ConfinerBounds;

    [SerializeField]
    private CinemachineConfiner2D m_Confiner;

    private const float m_Padding = 1f;

    public void FitToBoard(int width, int height)
    {
        m_ConfinerBounds.offset = new Vector2(width * 0.5f, height * 0.5f);
        m_ConfinerBounds.size = new Vector2(width + m_Padding * 2f, height + m_Padding * 2f);

        m_Confiner.InvalidateBoundingShapeCache();
    }
}
