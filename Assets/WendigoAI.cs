using UnityEngine;
using UnityEngine.AI;

public class WendigoAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Patrulla")]
    public Transform[] patrolPoints;
    private int currentPoint;

    [Header("Rangos")]
    public float detectionRange = 12f;
    public float attackRange = 2f;

    [Header("Velocidades")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    private bool isChasing = false;
    private bool isAngry = false;

    private float attackCooldown = 2f;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GoToNextPoint();
    }

    void Update()
    {
        // SI NO HAY PLAYER → SOLO PATRULLA
        if (player == null)
        {
            Patrol();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // DETECTAR
        if (distance < detectionRange)
        {
            isChasing = true;
        }

        // PERSEGUIR
        if (isChasing)
        {
            agent.SetDestination(player.position);

            // CAMINAR O CORRER
            if (isAngry)
            {
                agent.speed = runSpeed;
                animator.SetFloat("Speed", 1f);
            }
            else
            {
                agent.speed = walkSpeed;
                animator.SetFloat("Speed", 0.5f);
            }

            // ATAQUE
            if (distance < attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }

            // PERDER AL JUGADOR
            if (distance > detectionRange * 2f)
            {
                isChasing = false;
                GoToNextPoint();
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            GoToNextPoint();
        }

        agent.speed = walkSpeed;

        if (agent.velocity.magnitude > 0.1f)
            animator.SetFloat("Speed", 0.5f); // caminar
        else
            animator.SetFloat("Speed", 0f); // idle
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPoint = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    public void MakeAngry()
    {
        isAngry = true;
    }
}