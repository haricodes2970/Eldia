using UnityEngine;
using ELDIA.Gameplay.Health;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Combat
{
    /// <summary>
    /// Compact melee enemy controller. Health remains owned by HealthComponent;
    /// this component only owns movement, attack timing, and telegraph feedback.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class EnemyCombatAI : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase,
            Telegraph,
            Attack,
            Recovery,
            Defeated
        }

        [SerializeField, Min(0f)] private float detectionRange = 7f;
        [SerializeField, Min(0f)] private float attackRange = 1.25f;
        [SerializeField, Min(0f)] private float moveSpeed = 2.1f;
        [SerializeField, Min(0)] private int attackDamage = 10;
        [SerializeField, Min(0f)] private float attackCooldown = 1.4f;
        [SerializeField, Min(0f)] private float telegraphDuration = 0.35f;
        [SerializeField, Min(0f)] private float recoveryDuration = 0.55f;
        [SerializeField] private Color telegraphColor = new Color(1f, 0.85f, 0.1f, 1f);

        private Rigidbody2D body;
        private HealthComponent health;
        private SpriteRenderer spriteRenderer;
        private Transform target;
        private IGameState gameState;
        private State state = State.Idle;
        private Color aliveColor;
        private float stateTimer;
        private float nextAttackTime;
        private bool attackApplied;

        public void Configure(Transform configuredTarget, IGameState configuredGameState)
        {
            target = configuredTarget;
            gameState = configuredGameState;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            health = GetComponent<HealthComponent>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                aliveColor = spriteRenderer.color;
            }
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.Defeated += OnDefeated;
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.Defeated -= OnDefeated;
            }
        }

        private void Update()
        {
            if (IsTerminal() || health == null || health.IsDefeated)
            {
                StopAsDefeated();
                return;
            }

            if (target == null)
            {
                state = State.Idle;
                return;
            }

            switch (state)
            {
                case State.Idle:
                    if (DistanceToTarget() <= detectionRange)
                    {
                        state = State.Chase;
                    }
                    break;
                case State.Chase:
                    if (DistanceToTarget() > detectionRange)
                    {
                        state = State.Idle;
                    }
                    else if (DistanceToTarget() <= attackRange && Time.time >= nextAttackTime)
                    {
                        BeginTelegraph();
                    }
                    break;
                case State.Telegraph:
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f)
                    {
                        state = State.Attack;
                        ApplyAttackOnce();
                        SetRecoveryVisual();
                        stateTimer = recoveryDuration;
                    }
                    break;
                case State.Attack:
                    state = State.Recovery;
                    break;
                case State.Recovery:
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f)
                    {
                        nextAttackTime = Time.time + attackCooldown;
                        state = DistanceToTarget() <= detectionRange ? State.Chase : State.Idle;
                        RestoreVisual();
                    }
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (state != State.Chase || target == null || IsTerminal())
            {
                return;
            }

            Vector2 offset = (Vector2)target.position - body.position;
            if (offset.sqrMagnitude <= attackRange * attackRange)
            {
                return;
            }

            body.MovePosition(body.position + offset.normalized * moveSpeed * Time.fixedDeltaTime);
        }

        private void BeginTelegraph()
        {
            state = State.Telegraph;
            stateTimer = telegraphDuration;
            attackApplied = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = telegraphColor;
                transform.localScale *= 1.12f;
            }
        }

        private void ApplyAttackOnce()
        {
            if (attackApplied || target == null || IsTerminal())
            {
                return;
            }

            attackApplied = true;
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && Vector2.Distance(transform.position, target.position) <= attackRange + 0.2f)
            {
                Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
                damageable.ReceiveDamage(new DamageRequest(
                    attackDamage,
                    gameObject,
                    target.position,
                    direction));
            }
        }

        private void OnDefeated(EntityDefeatedEvent payload)
        {
            StopAsDefeated();
        }

        private void StopAsDefeated()
        {
            state = State.Defeated;
            stateTimer = 0f;
            body.linearVelocity = Vector2.zero;
            if (spriteRenderer != null)
            {
                RestoreVisual();
            }
        }

        private void SetRecoveryVisual()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = aliveColor;
                transform.localScale = Vector3.one;
            }
        }

        private void RestoreVisual()
        {
            SetRecoveryVisual();
        }

        private bool IsTerminal()
        {
            if (gameState == null)
            {
                return false;
            }

            return gameState.CurrentState == GameState.PlayerDefeated ||
                   gameState.CurrentState == GameState.Victory;
        }

        private float DistanceToTarget()
        {
            return target == null ? float.MaxValue : Vector2.Distance(transform.position, target.position);
        }
    }
}
