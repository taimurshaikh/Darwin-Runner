using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSlide : MonoBehaviour
{
    private Animator anim;
    public PlayerMove movement;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.isSliding)
        {
            anim.SetBool("PlayerSlide", true);
        }

        else
        {
            anim.SetBool("PlayerSlide", false);
        }
    }
}
