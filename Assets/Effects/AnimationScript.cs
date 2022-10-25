using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [Header("Animator Test")]
    public Animator anim;
    public List<Slash> slashes;

    private bool attacking;
    // Start is called before the first frame update
    void Start()
    {
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