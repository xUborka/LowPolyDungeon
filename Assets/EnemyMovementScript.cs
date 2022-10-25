using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("Grounded", true);
        anim.SetFloat("MotionSpeed", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
