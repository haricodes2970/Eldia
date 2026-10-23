using System.Collections.Generic;
using UnityEngine;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Combat
{
    /// <summary>
    /// Simple player attack component using Legacy Input Manager.
    /// Press J to attack in the current facing direction.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlayerAttack : MonoBehaviour
    {
        [SerializeField, Min(0)] private int attackDamage = 25;
        [SerializeField, Min(0f)] private float attackRange = 1.5f;
        [SerializeField] private string attackInputName = "Fire1"; // Maps to Left Ctrl or J in Legacy Input
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField] private float attackCooldown = 0.3f;

        private PlayerController2D playerController;
        private float nextAttackTime;
        private bool gameplayEnabled = true;

        public void SetGameplayEnabled(bool enabled)
        {
            gameplayEnabled = enabled;
        }
        private readonly HashSet<IDamageable> hitTargetsThisAttack = new HashSet<IDamageable>();

        private void Awake()
        {
            playerController = GetComponent<PlayerController2D>();
        }

        private void Update()
        {
            if (!gameplayEnabled)
            {
                return;
            }

            if (Input.GetButtonDown(attackInputName) && Time.time >= nextAttackTime)
            {
                PerformAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }

        private void PerformAttack()
        {
            hitTargetsThisAttack.Clear();

            Vector2 attackDirection = playerController.FacingDirection;
            if (attackDirection == Vector2.zero)
            {
                attackDirection = Vector2.down;
            }

            Vector2 attackOrigin = (Vector2)transform.position + attackDirection * 0.5f;
            Vector2 attackSize = new Vector2(attackRange, attackRange);

            Collider2D[] hits = Physics2D.OverlapBoxAll(attackOrigin, attackSize, 0f, targetLayers);

            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject)
                {
                    continue; // Don't hit self
                }

                var damageable = hit.GetComponent<IDamageable>();
                if (damageable == null)
                {
                    continue;
                }

                if (hitTargetsThisAttack.Contains(damageable))
                {
                    continue; // Already hit this target this attack
                }

                hitTargetsThisAttack.Add(damageable);

                var request = new DamageRequest(
                    attackDamage,
                    gameObject,
                    hit.ClosestPoint(attackOrigin),
                    attackDirection);

                DamageResult result = damageable.ReceiveDamage(request);

                if (result.WasApplied)
                {
                    ShowHitEffect(hit.transform.position);
                }
            }
        }

        private void ShowHitEffect(Vector2 position)
        {
            GameObject effect = new GameObject("Hit Effect");
            effect.transform.position = position;
            var renderer = effect.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateWhiteSprite();
            renderer.color = new Color(1f, 0.9f, 0.3f, 1f);
            renderer.sortingOrder = 10;
            effect.transform.localScale = Vector2.one * 0.5f;
            Destroy(effect, 0.1f);
        }

        private void OnDrawGizmosSelected()
        {
            if (playerController == null) return;

            Vector2 attackDirection = playerController.FacingDirection;
            if (attackDirection == Vector2.zero) attackDirection = Vector2.down;

            Vector2 attackOrigin = (Vector2)transform.position + attackDirection * 0.5f;
            Vector2 attackSize = new Vector2(attackRange, attackRange);

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            Gizmos.DrawWireCube(attackOrigin, attackSize);
        }

        private static Sprite CreateWhiteSprite()
        {
            Texture2D texture = Texture2D.whiteTexture;
            return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}