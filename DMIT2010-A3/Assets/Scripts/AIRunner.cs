using UnityEngine;
using System.Collections;
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

    [SerializeField] LayerMask hunterLayer;

    void Start()
    {
        movementSpeed = 5.0f;  //Random.Range(3, 5);
        forwardDist = 1.0f;
        sideDist = 2.0f;
        downDist = 1.0f;

        rbody = GetComponent<Rigidbody>();
        grounded = true;
    }

    private void Update()
    {
        if (grounded)
        {
            RaycastHit hunterHit;
            if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f),
                transform.forward, out hunterHit, Quaternion.identity, forwardDist, hunterLayer))
            {
                RotateAwayFromHunter();
                return;
            }

            if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f),
                transform.forward, out hitFront, Quaternion.identity, forwardDist))
            {
                transform.LookAt(transform.position - hitFront.normal);
                RotateAway();
            }
            else if (targets.Count > 0)
            {
                if (!Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f),
                    targets[0].transform.position - transform.position, Quaternion.identity,
                    Vector3.Distance(transform.position, targets[0].transform.position)))
                {
                    transform.LookAt(targets[0].transform.position);
                }

                if (Vector3.Distance(transform.position, targets[0].transform.position) < 0.5f)
                {
                    targets[0].SetActive(false);
                }

                if (!targets[0].activeSelf)
                {
                    targets.RemoveAt(0);
                }
            }

            if (!Physics.BoxCast(downCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f),
                -transform.up, out hitFront, Quaternion.identity, forwardDist))
            {
                if (Physics.BoxCast(jumpCheck.transform.position, new Vector3(0.3f, 0.9f, 0.3f),
                    -transform.up, out hitFront, Quaternion.identity, forwardDist))
                {
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

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f),
            -transform.right, out hitLeft, Quaternion.identity, sideDist))
        {
            leftWall = true;
        }

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f),
            transform.right, out hitRight, Quaternion.identity, sideDist))
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
            transform.Rotate(Vector3.up, randInt == 0 ? 90 : -90);
        }
    }

    void RotateAwayFromHunter()
    {
        leftWall = false;
        rightWall = false;

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f),
            -transform.right, out hitLeft, Quaternion.identity, sideDist, hunterLayer))
        {
            leftWall = true;
        }

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f),
            transform.right, out hitRight, Quaternion.identity, sideDist, hunterLayer))
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
        else
        {
            transform.Rotate(Vector3.up, 180);
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
            StartCoroutine(TemporarySpeedBoost(1f, 2f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Speed") || other.CompareTag("Disguise"))
        {
            targets.Remove(other.transform.parent.gameObject);
        }
    }
    private IEnumerator TemporarySpeedBoost(float boostAmount, float duration)
    {
        movementSpeed += boostAmount;
        yield return new WaitForSeconds(duration);
        movementSpeed -= boostAmount;
    }
}

