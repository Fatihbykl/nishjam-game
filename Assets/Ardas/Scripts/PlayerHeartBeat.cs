using UnityEngine;

public class PlayerHeartBeat : MonoBehaviour
{
    public Transform EnemyTransform;

    public float MaxHearingDistance;
    public float MaxSpeed;
    public float MinSpeed;

    public AudioSource Heartbeat;

    private void Update()
    {
        float distance = Vector3.Distance(EnemyTransform.position, transform.position);
        float speed = MaxHearingDistance / distance;
        Heartbeat.pitch = Mathf.Clamp(speed, MinSpeed, MaxSpeed);

        if (distance >= MaxHearingDistance)
        {
            Heartbeat.volume = 0f;
        } else
        {
            Heartbeat.volume = 1f;
        }
    }
}
