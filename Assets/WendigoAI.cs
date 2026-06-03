using UnityEngine;
using UnityEngine.AI;

public class WendigoAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public GameObject screamerUI;       // Panel/imagen del screamer en Canvas
    public AudioClip screamerSound;     // Sonido del screamer
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    [Header("Patrulla")]
    public Transform[] patrolPoints;
    private int currentPoint = 0;

    [Header("Autogeneración de waypoints")]
    public bool autoGenerateWaypoints = true;
    public float waypointRadius = 15f;
    public int waypointCount = 4;

    [Header("Detección")]
    public float detectionRange = 12f;
    public float fieldOfViewAngle = 90f;  // Grados de visión frontal
    public float attackRange = 1.8f;

    [Header("Velocidades")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 6f;

    private enum State { Patrolling, Chasing, Attacking }
    private State currentState = State.Patrolling;

    private bool gameOverTriggered = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Autogenerar waypoints si es necesario
        if (autoGenerateWaypoints && (patrolPoints == null || patrolPoints.Length < 2))
        {
            GenerateRandomWaypoints();
        }

        if (screamerUI != null) screamerUI.SetActive(false);
        GoToNextPoint();
    }

    void GenerateRandomWaypoints()
    {
        patrolPoints = new Transform[waypointCount];
        Vector3 startPosition = transform.position;

        for (int i = 0; i < waypointCount; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * waypointRadius;
            randomDirection += startPosition;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, waypointRadius, NavMesh.AllAreas))
            {
                // Crear un GameObject para cada waypoint (invisible)
                GameObject waypoint = new GameObject($"AutoWaypoint_{i}");
                waypoint.transform.position = hit.position;
                waypoint.transform.SetParent(transform); // Hijo del Wendigo para organización
                patrolPoints[i] = waypoint.transform;
            }
            else
            {
                // Si no se puede encontrar un punto válido en NavMesh, usar la posición inicial
                GameObject waypoint = new GameObject($"AutoWaypoint_{i}_Fallback");
                waypoint.transform.position = startPosition + new Vector3(i * 2f, 0, i * 2f);
                waypoint.transform.SetParent(transform);
                patrolPoints[i] = waypoint.transform;
            }
        }

        Debug.Log($"Wendigo: Autogenerados {patrolPoints.Length} waypoints en radio de {waypointRadius} unidades");
    }

    void Update()
    {
        if (gameOverTriggered || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (CanSeePlayer(dist)) currentState = State.Chasing;
                break;

            case State.Chasing:
                Chase(dist);
                break;

            case State.Attacking:
                // Animación de ataque ya lanzada, esperar callback
                break;
        }
    }

    // ─── Visión en cono ────────────────────────────────────────────────
    bool CanSeePlayer(float dist)
    {
        if (dist > detectionRange) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fieldOfViewAngle / 2f) return false;

        // Raycast para que paredes lo bloqueen
        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, detectionRange))
        {
            if (hit.transform == player) return true;
        }
        return false;
    }

    // ─── Patrulla secuencial ───────────────────────────────────────────
    void Patrol()
    {
        agent.speed = walkSpeed;
        animator.SetFloat("Speed", 0.4f);
        animator.SetBool("IsChasing", false);

        if (patrolPoints.Length == 0) return;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[currentPoint].position);
        currentPoint = (currentPoint + 1) % patrolPoints.Length; // secuencial
    }

    // ─── Persecución ──────────────────────────────────────────────────
    void Chase(float dist)
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        animator.SetFloat("Speed", 1f);
        animator.SetBool("IsChasing", true);

        if (dist < attackRange)
        {
            currentState = State.Attacking;
            agent.isStopped = true;
            animator.SetTrigger("Attack");
            Invoke(nameof(TriggerGameOver), 0.8f); // esperar animación de ataque
        }

        // Si el jugador se aleja mucho → volver a patrullar
        if (dist > detectionRange * 2.5f)
        {
            currentState = State.Patrolling;
            animator.SetBool("IsChasing", false);
            agent.isStopped = false;
            GoToNextPoint();
        }
    }

    // ─── Game Over ────────────────────────────────────────────────────
    void TriggerGameOver()
    {
        if (gameOverTriggered) return;
        gameOverTriggered = true;

        if (screamerUI != null) screamerUI.SetActive(true);
        if (audioSource != null && screamerSound != null)
            audioSource.PlayOneShot(screamerSound);

        // Bloquear movimiento del jugador
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        Invoke(nameof(LoadStartScene), 2.5f);
    }

    void LoadStartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); // Escena índice 0 = menú/inicio
    }

    void OnDestroy()
    {
        // Limpiar waypoints autogenerados
        if (autoGenerateWaypoints && patrolPoints != null)
        {
            foreach (Transform waypoint in patrolPoints)
            {
                if (waypoint != null && waypoint.name.StartsWith("AutoWaypoint"))
                {
                    Destroy(waypoint.gameObject);
                }
            }
        }
    }
}
