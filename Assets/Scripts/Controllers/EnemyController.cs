using UnityEngine;
using UnityEngine.AI;
using HorrorGame.Models;
using HorrorGame.Views;

namespace HorrorGame.Controllers
{
    /// <summary>
    /// Enemy AI controller. Drives EnemyModel state machine and NavMeshAgent.
    /// Attached to each Enemy GameObject.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyView))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Patrol")]
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private float       _patrolWaitTime = 2f;

        [Header("Detection")]
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private LayerMask _obstacleMask;

        [Header("Enemy Config")]
        [SerializeField] private string _enemyId   = "enemy_01";
        [SerializeField] private string _enemyName = "The Shadow";

        // --- Internal ---
        private NavMeshAgent    _agent;
        private EnemyView       _view;
        private EnemyModel      _model;
        private Transform       _playerTransform;
        private PlayerController _playerController;

        private int   _patrolIndex;
        private float _waitTimer;
        private float _searchTimer;
        private float _attackCooldown;
        private const float SearchDuration  = 5f;
        private const float AttackInterval  = 1f;

        public EnemyModel Model => _model;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _view  = GetComponent<EnemyView>();
            _model = new EnemyModel(_enemyId, _enemyName);
            _view.Initialize(_model);

            _agent.speed = _model.MovementSpeed;
        }

        private void Start()
        {
            // Cache player reference
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _playerTransform  = playerObj.transform;
                _playerController = playerObj.GetComponent<PlayerController>();
            }

            if (_patrolPoints.Length > 0)
            {
                _model.CurrentState = EnemyModel.EnemyState.Patrol;
                _agent.SetDestination(_patrolPoints[0].position);
            }
        }

        private void Update()
        {
            if (!_model.IsAlive) return;

            switch (_model.CurrentState)
            {
                case EnemyModel.EnemyState.Idle:    UpdateIdle();    break;
                case EnemyModel.EnemyState.Patrol:  UpdatePatrol();  break;
                case EnemyModel.EnemyState.Alert:   UpdateAlert();   break;
                case EnemyModel.EnemyState.Chase:   UpdateChase();   break;
                case EnemyModel.EnemyState.Attack:  UpdateAttack();  break;
                case EnemyModel.EnemyState.Search:  UpdateSearch();  break;
            }

            _view.SetMovementSpeed(_agent.velocity.magnitude);
        }

        // --- State updates ---
        private void UpdateIdle()
        {
            if (CanSeePlayer())
                _model.CurrentState = EnemyModel.EnemyState.Alert;
        }

        private void UpdatePatrol()
        {
            if (CanSeePlayer())
            {
                _model.CurrentState = EnemyModel.EnemyState.Chase;
                return;
            }

            if (_agent.remainingDistance < 0.5f)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _patrolWaitTime)
                {
                    _waitTimer   = 0f;
                    _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
                    _agent.SetDestination(_patrolPoints[_patrolIndex].position);
                }
            }
        }

        private void UpdateAlert()
        {
            _model.CurrentState = CanSeePlayer()
                ? EnemyModel.EnemyState.Chase
                : EnemyModel.EnemyState.Patrol;
        }

        private void UpdateChase()
        {
            if (_playerTransform == null) return;

            if (CanSeePlayer())
            {
                _model.LastKnownPlayerPosition = _playerTransform.position;
                _agent.speed = _model.ChaseSpeed;
                _agent.SetDestination(_playerTransform.position);

                if (IsInAttackRange())
                    _model.CurrentState = EnemyModel.EnemyState.Attack;
            }
            else
            {
                _model.CurrentState = EnemyModel.EnemyState.Search;
                _agent.speed        = _model.MovementSpeed;
                _searchTimer        = 0f;
                _agent.SetDestination(_model.LastKnownPlayerPosition);
            }
        }

        private void UpdateAttack()
        {
            if (_playerTransform == null) return;

            if (!IsInAttackRange())
            {
                _model.CurrentState = EnemyModel.EnemyState.Chase;
                return;
            }

            // Face the player
            Vector3 dir = (_playerTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));

            // Apply damage at fixed intervals, not every frame
            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown <= 0f)
            {
                _view.PlayAttackAnimation();
                _playerController?.TakeDamage(_model.AttackDamage);
                _attackCooldown = AttackInterval;
            }
        }

        private void UpdateSearch()
        {
            _searchTimer += Time.deltaTime;

            if (CanSeePlayer())
            {
                _model.CurrentState = EnemyModel.EnemyState.Chase;
                return;
            }

            if (_searchTimer >= SearchDuration)
                _model.CurrentState = EnemyModel.EnemyState.Patrol;
        }

        // --- Detection helpers ---
        private bool CanSeePlayer()
        {
            if (_playerTransform == null) return false;

            Vector3 toPlayer = _playerTransform.position - transform.position;
            if (toPlayer.magnitude > _model.DetectionRadius) return false;

            // Line-of-sight check
            if (Physics.Raycast(transform.position + Vector3.up,
                                toPlayer.normalized, toPlayer.magnitude, _obstacleMask))
                return false;

            return true;
        }

        private bool IsInAttackRange()
        {
            if (_playerTransform == null) return false;
            return Vector3.Distance(transform.position, _playerTransform.position)
                   <= _model.AttackRadius;
        }

        // --- Public API ---
        public void TakeDamage(float amount) => _model.TakeDamage(amount);
    }
}
