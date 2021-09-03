using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    public PlayerMove movement;
    private Animator anim;
    private Rigidbody rb;
    private float currFitness;
    public AgentNN agentNN;
    AudioManager audioManager;
    GAManager gm;

    private bool dead = false;
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        gm = FindObjectOfType<GAManager>();
        audioManager = FindObjectOfType<AudioManager>();    
    }
    
    void Update()
    {
        if ((transform.position.y < -1) || (System.Math.Abs(transform.position.x) > 7))
        {
            dead = true;
        }

        if (dead)
        {
            currFitness = gm.Fitness(transform, agentNN.JumpCount, agentNN.SlideCount);
            gm.AgentDeath(agentNN.nn, currFitness, gameObject);
            audioManager.Play("Die");
        }
    }

    void OnCollisionEnter (Collision collided)
    {
        if (collided.collider.tag == "Spike" || 
            collided.collider.tag == "RedSpikes" ||
            collided.collider.tag == "Boombox" || 
            collided.collider.tag == "Enemy")
        {
            anim.SetBool("Alive", false);
            anim.SetBool("Fall", false);
            anim.SetBool("Run", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Slide", false);
            anim.SetBool("Flip", false);
            movement.enabled = false;
            
            dead = true;
            
        }
        // Agents must not collide with other agents
        else if (collided.collider.tag == "Agent")
        {
            Physics.IgnoreCollision(collided.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }

    }
}
