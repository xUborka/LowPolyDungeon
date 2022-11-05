using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementScript : MonoBehaviour
{
    public Animator anim;

    private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform movePositionTransform;
    [SerializeField] private float attackTimer = 0.0f;
    [SerializeField] private bool attacking = false;


    private void Awake(){
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("Grounded", true);
        anim.SetFloat("MotionSpeed", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // if (attacking){
        //     attackTimer += Time.deltaTime;
        //     if (attackTimer > 2f && Vector3.Distance(movePositionTransform.position, gameObject.transform.position) >= navMeshAgent.stoppingDistance){
        //         attackTimer = 0f;
        //         attacking = false;
        //     }
        // }

        // if (!attacking){
        //     navMeshAgent.destination = movePositionTransform.position;
        // }

        // anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        // anim.SetBool("Attack", attacking);

        

        // // Below stopping distance = Melee range
        // if (Vector3.Distance(movePositionTransform.position, gameObject.transform.position) < navMeshAgent.stoppingDistance){
        //     Vector3 direction = (movePositionTransform.position - transform.position).normalized;
        //     Quaternion lookRotation = Quaternion.LookRotation(direction);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
        //     attacking = true;
        // }
    }
}
