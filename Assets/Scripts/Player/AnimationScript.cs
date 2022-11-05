using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class AnimationScript : NetworkBehaviour
{
    public GameObject PlayerModel;


    [Header("Animator Test")]
    public Animator anim;
    public List<Slash> slashes;

    private bool attacking;
    public bool rolling;

    private ThirdPersonController controller;

    [Header("Character Input Values")]
    public Vector2 move;
    public bool sprint;

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        PlayerModel.SetActive(false);
        DisableSlashes();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (PlayerModel.activeSelf == false)
            {
                SetPosition();
                PlayerModel.SetActive(true);
            }
        }

        if (hasAuthority)
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
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-2, 2), 1f, Random.Range(-8, -3));
    }

    IEnumerator Rolling()
    {
        yield return new WaitForSeconds(0.55f);
        controller.MoveSpeed = 3f;
        rolling = false;
        anim.SetBool("Roll", false);
    }

    IEnumerator SlashAttack()
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObj.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);
        DisableSlashes();
        attacking = false;
        anim.SetBool("Attack", false);
    }

    void DisableSlashes()
    {
        for (int i = 0; i < slashes.Count; i++)
        {
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