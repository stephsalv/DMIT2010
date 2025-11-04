using UnityEngine;
using System.Collections;

public class Disguise : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the runner touched this disguise pickup
        if (other.CompareTag("Runner"))
        {
            AIRunner runner = other.GetComponent<AIRunner>();
            if (runner != null)
            {
                // Start disguise effect on the runner
                runner.StartCoroutine(runner.TemporaryDisguise(0.5f));

                // Optionally destroy the disguise pickup
                Destroy(gameObject);
            }
        }
    }
}
