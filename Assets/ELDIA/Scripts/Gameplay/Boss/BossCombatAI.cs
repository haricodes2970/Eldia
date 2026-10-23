using UnityEngine;
using ELDIA.Gameplay.Health;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Boss
{
    /// <summary>
    /// Compact District Warden controller. BasicBoss remains responsible for
    /// health, identity, and the IBoss defeat event.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class BossCombatAI : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase,
            Telegraph,
            Recovery,
            Defeated
        }

        [SerializeField, Min(0f)] private float detectionRange = 10f;
        [SerializeField, Min(0f)] private float attackRange = 1.5f;
        [SerializeField, Min(0f)] private float moveSpeed = 1.4f;
        [SerializeField, Min(0)] private int closeAttackDamage = 18;
        [SerializeField, Min(0)] private int dashAttackDamage = 24;
        [SerializeField, Min(0f)] private float attackCooldown = 1.8f;
        [SerializeField, Min(0f)] private float telegraphDuration = 0.45f;
        [SerializeField, Min(0f)] private float recoveryDuration = 0.7f;
        [SerializeField, Min(0f)] private float dashDistance = 2.2f;
        [SerializeField] private Color telegraphColor = new Color(1f, 0.35f, 0.1f, 1f);

        private Rigidbody2D body;
        private BasicBoss boss;
        private HealthComponent health;
        private SpriteRenderer spriteRenderer;
        private Transform target;
        private IGameState gameState;
        private State state = State.Idle;
        private Color aliveColor;
        private float stateTimer;
        private float nextAttackTime;
        private bool attackApplied;
        private bool useDashAttack;

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
            boss = GetComponent<BasicBoss>();
            health = GetComponent<HealthComponent>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                aliveColor = spriteRenderer.color;
            }
        }

        private void OnEnable()
        {
            if (boss != null)
            {
                boss.Defeated += OnBossDefeated;
            }
        }

        private void OnDisable()
        {
            if (boss != null)
            {
                boss.Defeated -= OnBossDefeated;
            }
        }

        private void Update()
        {
            if (IsTerminal() || boss == null || boss.IsDefeated || health == null || health.IsDefeated)
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
                        ApplyAttackOnce();
                        SetRecoveryVisual();
                        state = State.Recovery;
                        stateTimer = recoveryDuration;
                    }
                    break;
                case State.Recovery:
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f)
                    {
                        nextAttackTime = Time.time + attackCooldown;
                        state = State.Chase;
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
            useDashAttack = !useDashAttack;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = telegraphColor;
                transform.localScale = Vector3.one * 1.12f;
            }
        }

        private void ApplyAttackOnce()
        {
            if (attackApplied || target == null || IsTerminal())
            {
                return;
            }

            attackApplied = true;
            Vector2 direction = ((Vector2)target.position - body.position).normalized;
            Vector2 attackPosition = body.position;
            if (useDashAttack)
            {
                attackPosition += direction * dashDistance;
                body.MovePosition(attackPosition);
            }

            float hitDistance = useDashAttack ? attackRange + 0.2f : attackRange + 0.2f;
            if (Vector2.Distance(attackPosition, target.position) > hitDistance)
            {
                return;
            }

            IDamageable damageable = target.GetComponent<IDamageable>();
            damageable?.ReceiveDamage(new DamageRequest(
                useDashAttack ? dashAttackDamage : closeAttackDamage,
                gameObject,
                target.position,
                direction));
        }

        private void OnBossDefeated(BossDefeatedEvent payload)
        {
            StopAsDefeated();
        }

        private void StopAsDefeated()
        {
            state = State.Defeated;
            stateTimer = 0f;
            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
            }
            RestoreVisual();
        }

        private void SetRecoveryVisual()
        {
            if (spriteRenderer != null && state != State.Defeated)
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
