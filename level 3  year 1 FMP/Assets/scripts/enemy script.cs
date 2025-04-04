
using UnityEngine;
using UnityEngine.AI;

public class enemyscript : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    // Start is called before the first frame update
    //patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile; 

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
  
    private void Awake ()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    
    }
    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && !playerInAttackRange) AttackPlayer();
    }

    // if (!playerInSightRange && !playerInAttackRange) Patrolling();
       // if (playerInSightRange && !playerInAttackRange) ChasePlayer();
       // if (playerInAttackRange && !playerInAttackRange) AttackPlayer();

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (!walkPointSet) SearchWalkPoint();
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range 
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);


        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        //Make sure enemy dosen't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);
       
       if (!alreadyAttacked)
       {
            ///Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 32f,ForceMode.Impulse);
            rb.AddForce(transform.up * 8f,ForceMode.Impulse);

            ///

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
       }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    private void onDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        //Gizmos.color = Color.red;
       // onDrawGizmosSelected.DrawWireSphere(transform.position, attackRange);
       // Gizmos.color = Color.yellow;
       // onDrawGizmosSelected.DrawWireSphere(transform.position, sightRange);
    }
}
