using UnityEngine;

public class SpeedBoostDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Runner") || other.CompareTag("Hunter"))
        {
            Destroy(gameObject);
        }
    }
}
