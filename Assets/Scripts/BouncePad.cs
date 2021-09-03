
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceForce = 10f;
    private GameObject player;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
    }

    void OnCollisionEnter(Collision collided)
    {

        collided.gameObject.GetComponent<Rigidbody>().velocity = Vector3.up * bounceForce; // Basically a B I G jump
        FindObjectOfType<AudioManager>().Play("BouncePad");
        
    }
}
