using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using StarterAssets;
using PLATEAU.Samples;

public class Zombie : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    // [SerializeField] float searchAngle = 180f;
    private Contact ContactScript;
    private ThirdPersonController ThirdPersonControllerScript;
    public float playerDistance;
    public bool search;
    
    private float moveDistance;
    private Vector3 direction;
    private Vector3 previousPos;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        ContactScript = GameObject.Find("PlayerArmature").GetComponent<Contact>();
        ThirdPersonControllerScript = GameObject.Find("PlayerArmature").GetComponent<ThirdPersonController>();
        previousPos = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerDistance = Vector3.Distance(this.transform.position, player.transform.position);
        moveDistance = Vector3.Distance(this.transform.position,previousPos);
        elapsedTime += Time.deltaTime;

        if(playerDistance < 2f)
        {
            ContactScript.GameOverFunc();
        }
        else if(playerDistance < 30 && !ThirdPersonControllerScript.isDied)
        {
            Chase();
        }
        else
        {
            Lost();
        }
        
        if(elapsedTime > 2f && moveDistance < 1f)
        {
            direction += new Vector3(Random.Range(0f,360f),0f,Random.Range(0f,360f));
            elapsedTime = 0;
        }

    }

    private void Lost()
    {
        animator.SetBool("Walk",true);
        this.transform.LookAt(direction);
    }

    private void Chase()
    {
        animator.SetBool("Run",true);
        direction = new Vector3(player.transform.position.x,this.transform.position.y,player.transform.position.z);
        this.transform.LookAt(direction);
        transform.position -= transform.forward * 0.01f;
    }
}
