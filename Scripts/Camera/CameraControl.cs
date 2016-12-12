using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;

    public void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    public void FixedUpdate()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void FindAveragePosition()
    {
        Vector3 averPos = new Vector3();
        int numTargets = 0;
        for(int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf) continue;
            averPos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averPos /= numTargets;
        averPos.y = transform.position.y;

        m_DesiredPosition = averPos;
    }
    private void Zoom()
    {
        float size = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, size, ref m_ZoomSpeed, m_DampTime);
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPosition = transform.InverseTransformPoint(m_DesiredPosition);
        float size = 0f;

        for(int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf) continue;

            Vector3 localPosition = transform.InverseTransformPoint(m_Targets[i].position);
            Vector3 desiredPosToTarget = localPosition - desiredLocalPosition;
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x / m_Camera.aspect));
        }

        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);
        return size;
    }

    public void SetStartPositionAndSize()
    {
        FindAveragePosition();
        transform.position = m_DesiredPosition;
        m_Camera.orthographicSize = FindRequiredSize();
    }

}