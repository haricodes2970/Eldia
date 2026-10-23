using UnityEngine;
using ELDIA.Gameplay.Shared;

namespace ELDIA.Gameplay.Combat
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class PlayerGun : MonoBehaviour
    {
        [SerializeField] private Transform facingTransform;
        private PlayerController2D controller;
        private AudioSource audioSource;
        private float nextShotTime;
        private float nextRechargeTime;
        private int currentAmmo = 12;

        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => 12;

        public void Configure(Transform configuredFacingTransform)
        {
            facingTransform = configuredFacingTransform;
        }

        private void Awake()
        {
            controller = GetComponent<PlayerController2D>();
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            nextRechargeTime = Time.time + 1.5f;
        }

        private void Update()
        {
            if (!Input.GetMouseButton(1) && currentAmmo < MaxAmmo && Time.time >= nextRechargeTime)
            {
                currentAmmo++;
                nextRechargeTime = Time.time + 1.5f;
            }

            if (Input.GetMouseButtonDown(1) && Time.time >= nextShotTime && currentAmmo > 0)
            {
                Fire();
                currentAmmo--;
                nextShotTime = Time.time + 0.25f;
                nextRechargeTime = Time.time + 1.5f;
            }
        }

        private void Fire()
        {
            Vector2 direction = controller != null ? controller.FacingDirection : Vector2.right;
            if (direction == Vector2.zero && facingTransform != null)
            {
                direction = facingTransform.position - transform.position;
            }
            direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

            GameObject projectile = new GameObject("Player Projectile");
            projectile.transform.position = transform.position + (Vector3)direction * 0.5f;
            SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateWhiteSprite();
            renderer.color = Color.yellow;
            renderer.sortingOrder = 10;
            projectile.transform.localScale = Vector3.one * 0.2f;
            Projectile projectileLogic = projectile.AddComponent<Projectile>();
            projectileLogic.Configure(direction, gameObject);
            audioSource.PlayOneShot(CreatePew());
        }

        private static AudioClip CreatePew()
        {
            const int rate = 44100;
            int count = rate / 20;
            AudioClip clip = AudioClip.Create("Pew", count, 1, rate, false);
            float[] data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)rate;
                float frequency = Mathf.Lerp(440f, 220f, i / (float)count);
                data[i] = Mathf.Sin(t * Mathf.PI * 2f * frequency) * 0.2f;
            }
            clip.SetData(data, 0);
            return clip;
        }

        private static Sprite CreateWhiteSprite()
        {
            return Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f), 1f);
        }

        private sealed class Projectile : MonoBehaviour
        {
            private Vector2 direction;
            private GameObject source;
            private float expiresAt;

            public void Configure(Vector2 configuredDirection, GameObject configuredSource)
            {
                direction = configuredDirection;
                source = configuredSource;
                expiresAt = Time.time + 1.5f;
            }

            private void Update()
            {
                transform.position += (Vector3)(direction * 12f * Time.deltaTime);
                Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f);
                if (hit != null && hit.gameObject != source)
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.ReceiveDamage(new DamageRequest(8, source, transform.position, direction));
                        Destroy(gameObject);
                        return;
                    }
                }
                if (Time.time >= expiresAt) Destroy(gameObject);
            }
        }
    }
}
