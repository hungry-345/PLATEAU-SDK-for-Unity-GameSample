using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackHandler : MonoBehaviour
{
    private InputAction attack;   

    [Header("attackParameter")]
    [SerializeField] private LayerMask attackable;
    [SerializeField] private LayerMask cityMaterials;
    [SerializeField] private Material lineColor;
    [SerializeField] private Transform tipPosition;
    [SerializeField] private float distance = 50f;
    private bool isAttack;
    private Vector3 attackPosition;

    //敵のコントローラー取得
    private EnemyController enemyController;

    //電撃
    private LineRenderer lineRenderer;
    [SerializeField] private float fps = 30f;
    private float fpsCounter;
    private float electroRelease = 0.1f;

    //経過時間管理
    private float elapsedTime = 0f;

    //SE
    [SerializeField] private AudioClip spark;
    private AudioSource sparkSound;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        attack = playerInput.actions["attack"];
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        sparkSound = this.AddComponent<AudioSource>();
        sparkSound.clip = spark;
        sparkSound.loop = false;
    }

    private void Update()
    {
        if(isAttack)
        {
            elapsedTime += Time.deltaTime;
            fpsCounter += Time.deltaTime;
            if(elapsedTime >electroRelease)
            {
                RemoveElectro();
                elapsedTime = 0f;
            }
        }
        attack.performed += OnAttackAction;
        DrawElectro();
    }
    private void OnAttackAction(InputAction.CallbackContext context)
    {
        isAttack = true;
        Attack();
    }

    public void Attack()
    {
        if (isAttack)
        {
            lineRenderer.enabled = true;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitAttack, distance, attackable))
            {

                //敵の情報取得
                hitAttack.collider.gameObject.layer = 0;
                Transform geometry = hitAttack.transform.Find("Geometry");
                Transform mesh = geometry.Find("Armature_Mesh");
                attackPosition = new Vector3(mesh.position.x, mesh.position.y, mesh.position.z);
                enemyController = hitAttack.collider.GetComponent<EnemyController>();
                if (enemyController == null)
                {
                    Transform parent = hitAttack.transform.parent;
                    enemyController = parent.GetComponent<EnemyController>();
                }

                if (!enemyController.GetIsBiribiri())
                {
                    enemyController.SetState(EnemyController.EnemyState.Hit);
                    enemyController.EnemyColorYellow(hitAttack);
                    enemyController.ChangeBiribiri();
                }
            }
            else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 50f,cityMaterials))
            {

                attackPosition = hit.point;

            }
            else
            {
                Vector3 forwardDirectiion = Camera.main.transform.forward;
                Vector3 targetPosition = Camera.main.transform.position + forwardDirectiion * distance;
                attackPosition = targetPosition;
            }
            //se再生
            sparkSound.Play();
        }
    }

    public bool CheckAttack()
    {
        return isAttack;
    }

    public void DrawElectro()
    {
        lineRenderer.SetPosition(0, tipPosition.position);
        lineRenderer.SetPosition(1, attackPosition);
    }

    public void RemoveElectro()
    {
        isAttack = false;
        if(lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
    private void OnDestroy()
    {
        attack.performed -= OnAttackAction;
    }
}
