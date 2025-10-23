using UnityEngine;
using System.Collections;

public class FauxAttractor : MonoBehaviour
{
    public float gravity = -10f;
    public void Attract(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        Rigidbody rb = body.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(gravityUp * gravity);
        }

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, Time.deltaTime * 50f);
    }
}
