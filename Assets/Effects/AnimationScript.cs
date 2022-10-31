using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class AnimationScript : MonoBehaviour
{
    [Header("Animator Test")]
    public Animator anim;
    public List<Slash> slashes;

    private bool attacking;
    public bool rolling;
    
    private ThirdPersonController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        DisableSlashes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack") && !attacking)
        {
            attacking = true;
            anim.SetBool("Attack", true);
            StartCoroutine(SlashAttack());
        }

        if (Input.GetButtonDown("Roll") && !attacking && !rolling)
        {
            rolling = true;
            anim.SetBool("Roll", true);
            controller.MoveSpeed = 7f;
            StartCoroutine(Rolling());
        }
        
    }

    IEnumerator Rolling(){
        yield return new WaitForSeconds(0.55f);
        controller.MoveSpeed = 3f;
        rolling = false;
        anim.SetBool("Roll", false);
    }

    IEnumerator SlashAttack(){
        for (int i=0; i<slashes.Count; i++){
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObj.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);
        DisableSlashes();
        attacking = false;
        anim.SetBool("Attack", false);
    }

    void DisableSlashes(){
        for (int i = 0; i < slashes.Count; i++){
            slashes[i].slashObj.SetActive(false);
        }
    }
}

[System.Serializable]
public class Slash
{
    public GameObject slashObj;
    public float delay;
}