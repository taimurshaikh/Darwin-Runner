using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider col;
    public LayerMask groundLayers;

    [Header("Move controls")]
    public float forwardSpeed;
    public float sideSpeed;
    public float moveSpeed;
    private float tempSpeed;

    public float horizontal;

    [Header("Speed Increase")]
    public float speedMultiplier;
    public float speedIncreaseMilestone;
    public float maxSpeed;

    private float speedMilestoneCount;

    public bool isJumpPressed = false;
    private float forwardSpeedWhileInAir;
    
    [Header("Jump controls")]
    public float jumpSpeed;
    public float fallMultiplier;
    public float lowJumpMultiplier;

    public bool isSliding = false;
    public bool slideStopped = true;

    private Vector3 moveHorizontal;
    private Vector3 moveForward;
    private Vector3 movement;

    private Animator anim;

    void Start()
    {
        forwardSpeedWhileInAir = forwardSpeed;
        tempSpeed = forwardSpeed;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // isJumpPressed = Input.GetButtonDown("Jump");

        // isSliding = Input.GetKey("s");
        // slideStopped = Input.GetKeyUp("s");

        moveHorizontal = new Vector3(horizontal, 0 , 0) * sideSpeed;
        moveForward = new Vector3(0, 0 , forwardSpeed);
        movement = moveHorizontal + moveForward;

        if (rb.velocity.y < 0) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !isJumpPressed) {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (IsGrounded()) {
            if (isSliding) {
                if (!isJumpPressed) {
                    anim.SetBool("Slide", true);
                    anim.SetBool("Run", false);
                } else {
                    anim.SetBool("Slide", false);
                }
                Slide();
            } else if (slideStopped) {
                resetSlide();
            }
        }

        if (transform.position.z > speedMilestoneCount && forwardSpeed < maxSpeed && IsGrounded()) {
            speedMilestoneCount += speedIncreaseMilestone;
            speedIncreaseMilestone *= speedMultiplier;

            forwardSpeed *= speedMultiplier;
            tempSpeed = forwardSpeed;
        }
    }

    void FixedUpdate() 
    {
        if (IsGrounded()) {
            forwardSpeed = tempSpeed;
            tempSpeed = forwardSpeed;
        } else {
            forwardSpeed = forwardSpeedWhileInAir;
        }
        if (IsGrounded() && !isSliding) {
            anim.SetBool("Run", true);
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", false);
            anim.SetBool("Slide", false);
            anim.SetBool("Flip", false);
        } else if (IsGrounded() && isSliding) {
            anim.SetBool("Run", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", false);
            anim.SetBool("Slide", true);
            anim.SetBool("Flip", false);
        }

        //JUMPING
        if (IsGrounded() && isJumpPressed) { 
            if (isSliding) {
                anim.SetBool("Run", false);
                anim.SetBool("Slide", false);
                anim.SetBool("Fall", false);
                anim.SetBool("Flip", true); 
            } else {
                anim.SetBool("Run", false);
                anim.SetBool("Slide", false);
                anim.SetBool("Jump", true);
            }


            rb.velocity = Vector3.up * jumpSpeed;
            
            FindObjectOfType<AudioManager>().Play("Jump");
        
        } 

 
        else if (!IsGrounded() && !anim.GetBool("Jump")) {

            anim.SetBool("Run", false);
            anim.SetBool("Slide", false);
            anim.SetBool("Fall", true);
        }
       moveCharacter(movement);             
    }

    void moveCharacter(Vector3 desired)
    {
        rb.MovePosition(transform.position + (desired * moveSpeed * Time.fixedDeltaTime));      
    }

    private bool IsGrounded()
    {
        return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.size.x * .9f, groundLayers);
    }

    void Slide()
    {   
        col.center = new Vector3(0f ,0.4f, 0f);
        col.size = new Vector3(1f, 0.8f, 1f);     
    }

    void resetSlide()
    {
        col.center = new Vector3(0f, 0.9f, 0);
        col.size = new Vector3(1f, 1.8f, 1f);  
        anim.SetBool("Slide", false);
        anim.SetBool("Run", true);  
    }
}
