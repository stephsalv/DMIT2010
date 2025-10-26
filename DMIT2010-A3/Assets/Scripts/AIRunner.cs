using UnityEngine;
using System.Collections.Generic;

public class AIRunner : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    RaycastHit hitFront, hitLeft, hitRight;
    [SerializeField] float forwardDist, sideDist, downDist;
    bool leftWall, rightWall;

    int randInt;

    [SerializeField] GameObject downCheck, jumpCheck;

    Rigidbody rbody;

    bool grounded;

    List<GameObject> targets = new List<GameObject>();

    [SerializeField] float boostPower = 0.2f;
    [SerializeField] float boostDuration = 1.0f;
    [SerializeField] float disguiseDuration = 3.0f;

    float boostTimer = 0f;
    float disguiseTimer = 0f;
    bool isBoosted = false;
    bool isDisguised = false;

    [SerializeField] Renderer bodyRenderer; // assign in inspector
    [SerializeField] Color normalColor = Color.yellow;
    [SerializeField] Color disguiseColor = Color.red;

    [SerializeField] float hunterDetectionRadius = 5f;
    [SerializeField] LayerMask hunterLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementSpeed = 3f; // Random.Range(3, 8);
        forwardDist = 1.0f;
        sideDist = 2.0f;
        downDist = 1.0f;

        rbody = GetComponent<Rigidbody>();
        grounded = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (grounded)
        {
            // Rotate the mover if an object is detected in front
            if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), transform.forward, out hitFront, Quaternion.identity, forwardDist))
            {
                transform.LookAt(transform.position - hitFront.normal);

                RotateAway();
            }
            else if (targets.Count > 0)
            {
                if (!Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), targets[0].transform.position - transform.position, Quaternion.identity, Vector3.Distance(transform.position, targets[0].transform.position)))
                {
                    transform.LookAt(targets[0].transform.position);
                }

                if (Vector3.Distance(transform.position, targets[0].transform.position) < 0.5f)
                {
                    targets[0].SetActive(false);
                }

                if (targets[0].activeSelf == false)
                {
                    targets.RemoveAt(0);
                }
            }

            // Rotate the mover if a hole is detected in front
            if (!Physics.BoxCast(downCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f), -transform.up, out hitFront, Quaternion.identity, forwardDist))
            {
                // Check if there is a floor to jump to
                if (Physics.BoxCast(jumpCheck.transform.position, new Vector3(0.3f, 0.9f, 0.3f), -transform.up, out hitFront, Quaternion.identity, forwardDist))
                {
                    // Check to make sure there is no object in the way of the jump
                    if (!Physics.CheckBox(jumpCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f)))
                    {
                        rbody.AddRelativeForce(transform.up * 300);
                        grounded = false;
                    }
                    else
                    {
                        RotateAway();
                    }
                }
                else
                {
                    RotateAway();
                }
            }

            HandleBoostAndDisguiseTimers();
        }
    }

    void HandleBoostAndDisguiseTimers()
    {
        // Boost logic
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosted = false;
            }
        }

        // Disguise logic
        if (isDisguised)
        {
            disguiseTimer -= Time.deltaTime;
            if (disguiseTimer <= 0f)
            {
                isDisguised = false;
                if (bodyRenderer != null)
                    bodyRenderer.material.color = normalColor; // restore original color
            }
        }

        // Update color while disguised
        if (isDisguised && bodyRenderer != null)
        {
            bodyRenderer.material.color = disguiseColor;
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.fixedDeltaTime);
    }

    void RotateAway()
    {
        leftWall = false;
        rightWall = false;

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f), -transform.right, out hitLeft, Quaternion.identity, sideDist))
        {
            leftWall = true;
        }

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f), transform.right, out hitRight, Quaternion.identity, sideDist))
        {
            rightWall = true;
        }

        if (leftWall && !rightWall)
        {
            transform.Rotate(Vector3.up, 90);
        }
        else if (!leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, -90);
        }
        else if (leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, 180);
        }
        else
        {
            randInt = Random.Range(0, 2);
            if (randInt == 0)
            {
                transform.Rotate(Vector3.up, 90);
            }
            else
            {
                transform.Rotate(Vector3.up, -90);
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Speed") || other.CompareTag("Disguise"))
        {
            targets.Add(other.transform.parent.gameObject);
        }

        if (other.CompareTag("Speed"))
        {
            movementSpeed += boostPower;
            boostTimer = boostDuration;
            isBoosted = true;
            //other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Disguise"))
        {
            isDisguised = true;
            disguiseTimer = disguiseDuration;
            //other.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Speed") || other.CompareTag("Disguise"))
            targets.Remove(other.transform.parent.gameObject);
    }

    Vector3 DetectHunters()
    {
        Vector3 avoidance = Vector3.zero;
        Collider[] hunters = Physics.OverlapSphere(transform.position, hunterDetectionRadius, hunterLayer);

        foreach (Collider hunter in hunters)
        {
            Vector3 dirToHunter = hunter.transform.position - transform.position;

            // Check line of sight
            if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, dirToHunter.normalized, dirToHunter.magnitude))
            {
                // Add vector away from Hunter
                avoidance -= dirToHunter.normalized; // negative to move away
            }
        }

        return avoidance.normalized;
    }
}
