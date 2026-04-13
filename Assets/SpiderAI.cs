using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    public Transform player;

    public float speed = 200f;
    public float chaseSpeed = 50f;

    public float detectionDistance = 40f;
    public float attackDistance = 1.5f;

    public bool isAngry = false;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // SI está en modo furia
        if (isAngry)
        {
            ChasePlayer(chaseSpeed);
            return;
        }

        // Detectar jugador por proximidad
        if (distance < detectionDistance)
        {
            ChasePlayer(speed);
        }

        // Si está muy cerca → "matar"
        if (distance < attackDistance)
        {
            KillPlayer();
        }
    }

    void ChasePlayer(float moveSpeed)
    {
        // Mirar SOLO en horizontal (arregla lo de ir de espaldas)
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
          Quaternion rotation = Quaternion.LookRotation(direction);
          transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5f * Time.deltaTime);
        }

        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;

         transform.position = Vector3.MoveTowards(
    transform.position,
    targetPosition,
    moveSpeed * Time.deltaTime
);
    }

    void KillPlayer()
    {
        Debug.Log("💀 Has muerto.");
        // Luego aquí ponemos pantalla de muerte
    }

   void LateUpdate()
{
    float bob = Mathf.Sin(Time.time * 10f) * 0.05f;
    transform.position += new Vector3(0, bob, 0);
}
void FixedUpdate()
{
    float sway = Mathf.Sin(Time.time * 5f) * 2f;
    transform.Rotate(0, sway * Time.deltaTime, 0);
}
}