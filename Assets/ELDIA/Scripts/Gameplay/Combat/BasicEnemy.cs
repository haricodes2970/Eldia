using UnityEngine;
using ELDIA.Gameplay.Health;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Combat
{
    /// <summary>
    /// Simple enemy with health that can be damaged by the player.
    /// No AI movement - stationary target for combat testing.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class BasicEnemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Color aliveColor = new Color(0.8f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color defeatedColor = new Color(0.3f, 0.1f, 0.1f, 0.5f);
        [SerializeField] private float defeatedFadeTime = 1f;

        private SpriteRenderer spriteRenderer;
        private bool isDefeated;
        private float defeatTimer;

        private void Awake()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            if (healthComponent == null)
            {
                healthComponent = gameObject.AddComponent<HealthComponent>();
            }

            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = CreateWhiteSprite();
                spriteRenderer.color = aliveColor;
            }

            healthComponent.Defeated += OnDefeated;
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
            {
                healthComponent.Defeated -= OnDefeated;
            }
        }

        private void Update()
        {
            if (isDefeated && defeatTimer > 0f)
            {
                defeatTimer -= Time.deltaTime;
                float alpha = Mathf.Lerp(0f, defeatedColor.a, defeatTimer / defeatedFadeTime);
                Color c = defeatedColor;
                c.a = alpha;
                spriteRenderer.color = c;

                if (defeatTimer <= 0f)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnDefeated(EntityDefeatedEvent evt)
        {
            isDefeated = true;
            defeatTimer = defeatedFadeTime;
        }

        public bool CanReceiveDamage => healthComponent != null && healthComponent.CanReceiveDamage;

        public DamageResult ReceiveDamage(DamageRequest request)
        {
            if (healthComponent == null)
            {
                return DamageResult.NotApplied;
            }

            return healthComponent.ReceiveDamage(request);
        }

        private static Sprite CreateWhiteSprite()
        {
            Texture2D texture = Texture2D.whiteTexture;
            return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}