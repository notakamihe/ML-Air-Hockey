using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    public Rigidbody rb;

    float oscillationDuration;
    float serveDirection;

    public bool HasServed { get; private set; } = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        oscillationDuration = UnityEngine.Random.Range(4.0f, 11.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 2, rb.velocity.z);
        }
    }

    public void Oscillate(float speed, float scale)
    {
        serveDirection = Mathf.Cos(Time.time * speed / Mathf.PI);
        transform.Translate(transform.forward * (serveDirection * scale * Time.deltaTime));

        if (Time.time >= oscillationDuration)
        {
            Serve();
        }
    }

    void Serve()
    {
        HasServed = true;
        rb.AddForce(transform.forward * (serveDirection < 0 ? -1 : 1) * 16000 * Time.deltaTime);
    }
}
