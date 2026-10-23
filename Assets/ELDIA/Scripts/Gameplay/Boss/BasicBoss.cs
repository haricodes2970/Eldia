using System;
using UnityEngine;
using ELDIA.Gameplay.Health;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Boss
{
    /// <summary>
    /// Minimal elite encounter foundation. Health and damage remain owned by
    /// HealthComponent; this component owns boss identity and defeat signaling.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class BasicBoss : MonoBehaviour, IBoss, IDamageable
    {
        [SerializeField] private string bossId = "district-warden";
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField, Min(1)] private int configuredMaxHealth = 150;
        [SerializeField] private Color aliveColor = new Color(0.75f, 0.1f, 0.95f, 1f);
        [SerializeField] private Color hitColor = new Color(1f, 0.85f, 0.3f, 1f);
        [SerializeField] private Color defeatedColor = new Color(0.2f, 0.05f, 0.25f, 1f);
        [SerializeField, Min(0f)] private float hitFeedbackDuration = 0.12f;

        private SpriteRenderer spriteRenderer;
        private bool isDefeated;
        private bool defeatEventRaised;
        private float hitFeedbackTimer;

        public string BossId => bossId;
        public bool IsDefeated => isDefeated;
        public bool CanReceiveDamage => healthComponent != null && healthComponent.CanReceiveDamage;

        public event Action<BossDefeatedEvent> Defeated;

        private void Awake()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            if (healthComponent == null)
            {
                healthComponent = gameObject.AddComponent<HealthComponent>();
                healthComponent.SetMaxHealth(configuredMaxHealth);
                healthComponent.ResetHealth();
            }

            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = CreateWhiteSprite();
            }

            spriteRenderer.color = aliveColor;
        }

        private void OnEnable()
        {
            if (healthComponent != null)
            {
                healthComponent.HealthChanged += OnHealthChanged;
                healthComponent.Defeated += OnEntityDefeated;
            }
        }

        private void OnDisable()
        {
            if (healthComponent != null)
            {
                healthComponent.HealthChanged -= OnHealthChanged;
                healthComponent.Defeated -= OnEntityDefeated;
            }
        }

        private void Update()
        {
            if (isDefeated || hitFeedbackTimer <= 0f)
            {
                return;
            }

            hitFeedbackTimer -= Time.deltaTime;
            if (hitFeedbackTimer <= 0f)
            {
                spriteRenderer.color = aliveColor;
            }
        }

        public DamageResult ReceiveDamage(DamageRequest request)
        {
            if (isDefeated || healthComponent == null)
            {
                return DamageResult.NotApplied;
            }

            return healthComponent.ReceiveDamage(request);
        }

        private void OnHealthChanged(HealthChangedEvent payload)
        {
            if (!isDefeated && payload.Delta < 0)
            {
                spriteRenderer.color = hitColor;
                hitFeedbackTimer = hitFeedbackDuration;
            }
        }

        private void OnEntityDefeated(EntityDefeatedEvent payload)
        {
            if (defeatEventRaised)
            {
                return;
            }

            isDefeated = true;
            defeatEventRaised = true;
            hitFeedbackTimer = 0f;
            spriteRenderer.color = defeatedColor;
            transform.localScale *= 0.85f;
            Defeated?.Invoke(new BossDefeatedEvent(BossId, gameObject));
        }

        private static Sprite CreateWhiteSprite()
        {
            Texture2D texture = Texture2D.whiteTexture;
            return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}
