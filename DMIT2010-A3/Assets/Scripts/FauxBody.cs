using UnityEngine;

public class FauxGravityBody : MonoBehaviour
{
    public FauxAttractor attractor;
    private Transform myTransform;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        myTransform = transform;
    }

    void Update()
    {
        attractor.Attract(myTransform);
    }
}
