using UnityEngine;
using ELDIA.Gameplay.Combat;
using ELDIA.Gameplay.Health;
using ELDIA.Gameplay.Objectives;
using ELDIA.Gameplay.Boss;
using ELDIA.Gameplay.Mission;
using ELDIA.Gameplay.Presentation;
using ELDIA.Gameplay.Reset;
using ELDIA.Audio;

public sealed class PlayerTestArena : MonoBehaviour
{
    [SerializeField] private Color backgroundColor = new Color(0.025f, 0.035f, 0.08f, 1f);
    [SerializeField] private Color floorColor = new Color(0.08f, 0.12f, 0.2f, 1f);
    [SerializeField] private Color wallColor = new Color(0.15f, 0.7f, 0.85f, 1f);
    [SerializeField] private Color playerColor = new Color(0.95f, 0.2f, 0.75f, 1f);
    [SerializeField] private Color facingColor = new Color(1f, 0.85f, 0.25f, 1f);
    [SerializeField] private Color enemyColor = new Color(0.8f, 0.2f, 0.2f, 1f);

    private Transform generatedRoot;
    private GameObject player;
    private bool initialized;
    private Camera generatedCamera;
    private SimulationObjective objective;
    private BasicBoss boss;
    private MissionCoordinator missionCoordinator;

    private void Awake()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        CleanupGeneratedArena();
        generatedRoot = new GameObject("Generated Arena").transform;
        generatedRoot.SetParent(transform, false);
        generatedRoot.gameObject.AddComponent<MusicPlayer>();

        CreateBackground();
        CreateFloor();
        CreateWalls();
        CreatePlayer();
        CreateEnemy();
        CreateSimulationObjective();
        CreateBoss();
        CreateMissionCoordinator();
        CreateCamera();
        CreateHudAndReset();
        ValidateRuntimeSetup();
    }

    private void CleanupGeneratedArena()
    {
        if (generatedCamera != null)
        {
            Destroy(generatedCamera.gameObject);
            generatedCamera = null;
        }

        Transform existing = transform.Find("Generated Arena");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void CreateBackground()
    {
        GameObject background = CreateSpriteObject("Background", new Vector3(0f, 0f, 5f), new Vector2(30f, 20f), backgroundColor);
        background.GetComponent<SpriteRenderer>().sortingOrder = -10;
    }

    private void CreateFloor()
    {
        GameObject floor = CreateSpriteObject("Arena Floor", new Vector3(0f, 0f, 1f), new Vector2(18f, 10f), floorColor);
        floor.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }

    private void CreateWalls()
    {
        CreateWall("North Wall", new Vector2(0f, 5.3f), new Vector2(18f, 0.6f));
        CreateWall("South Wall", new Vector2(0f, -5.3f), new Vector2(18f, 0.6f));
        CreateWall("East Wall", new Vector2(9.3f, 0f), new Vector2(0.6f, 10f));
        CreateWall("West Wall", new Vector2(-9.3f, 0f), new Vector2(0.6f, 10f));
        CreateWall("Center Obstacle", new Vector2(2.5f, 0.7f), new Vector2(2.5f, 0.6f));
        CreateWall("South Obstacle", new Vector2(-2.5f, -1.2f), new Vector2(0.6f, 2.4f));
    }

    private void CreateWall(string wallName, Vector2 position, Vector2 size)
    {
        GameObject wall = CreateSpriteObject(wallName, new Vector3(position.x, position.y, 0f), size, wallColor);
        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
    }

    private void CreatePlayer()
    {
        player = CreateSpriteObject("Player", new Vector3(-5f, 0f, 0f), Vector2.one, playerColor);
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<BoxCollider2D>();

        GameObject facing = CreateSpriteObject("Facing Indicator", new Vector3(0f, -0.62f, -0.1f), new Vector2(0.22f, 0.3f), facingColor);
        facing.transform.SetParent(player.transform, false);

        PlayerController2D controller = player.AddComponent<PlayerController2D>();
        controller.Configure(facing.transform);

        // Add health component to player
        HealthComponent playerHealth = player.AddComponent<HealthComponent>();
        playerHealth.SetMaxHealth(100);

        // Add attack component to player
        player.AddComponent<PlayerAttack>();
        PlayerGun gun = player.AddComponent<PlayerGun>();
        gun.Configure(facing.transform);
    }

    private void CreateEnemy()
    {
        GameObject enemy = CreateSpriteObject("Enemy", new Vector3(2f, 0f, 0f), Vector2.one, enemyColor);
        enemy.AddComponent<BoxCollider2D>();

        var health = enemy.AddComponent<HealthComponent>();
        health.SetMaxHealth(50);

        enemy.AddComponent<BasicEnemy>();
    }

    private void CreateSimulationObjective()
    {
        GameObject node = CreateSpriteObject(
            "Simulation Control Node",
            new Vector3(0f, 3f, 0f),
            new Vector2(1.2f, 1.2f),
            new Color(0.1f, 0.85f, 1f, 1f));

        objective = node.AddComponent<SimulationObjective>();
        ObjectiveInteractor interactor = player.AddComponent<ObjectiveInteractor>();
        interactor.Configure(objective);
    }

    private void CreateBoss()
    {
        GameObject bossObject = CreateSpriteObject(
            "District Warden Boss",
            new Vector3(6f, 3.5f, 0f),
            new Vector2(1.5f, 1.5f),
            new Color(0.75f, 0.1f, 0.95f, 1f));

        bossObject.AddComponent<BoxCollider2D>();
        HealthComponent health = bossObject.AddComponent<HealthComponent>();
        health.SetMaxHealth(150);
        health.ResetHealth();
        boss = bossObject.AddComponent<BasicBoss>();
    }

    private void CreateMissionCoordinator()
    {
        GameObject coordinatorObject = CreateGeneratedObject("Mission Coordinator");
        missionCoordinator = coordinatorObject.AddComponent<MissionCoordinator>();
        missionCoordinator.Configure(objective, boss, player.GetComponent<HealthComponent>());
    }

    private void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(-5f, 0f, -10f);
        cameraObject.transform.SetParent(generatedRoot, true);

        generatedCamera = cameraObject.AddComponent<Camera>();
        generatedCamera.orthographic = true;
        generatedCamera.orthographicSize = 6.5f;
        generatedCamera.clearFlags = CameraClearFlags.SolidColor;
        generatedCamera.backgroundColor = backgroundColor;

        CameraFollow2D follow = cameraObject.AddComponent<CameraFollow2D>();
        follow.Configure(player.transform);
    }

    private void CreateHudAndReset()
    {
        GameObject systems = CreateGeneratedObject("Arena Systems");
        ArenaHUD hud = systems.AddComponent<ArenaHUD>();
        hud.Configure(objective, boss, missionCoordinator);
        systems.AddComponent<TestArenaReset>();
    }

    private void ValidateRuntimeSetup()
    {
        if (player == null || player.GetComponent<PlayerController2D>() == null || player.GetComponent<PlayerAttack>() == null)
        {
            Debug.LogError("PlayerTestArena: player setup is missing controller or attack component.", this);
        }

        if (objective == null || missionCoordinator == null)
        {
            Debug.LogError("PlayerTestArena: objective mission wiring is incomplete.", this);
        }

        if (boss == null || boss.GetComponent<HealthComponent>() == null)
        {
            Debug.LogError("PlayerTestArena: boss setup is missing BasicBoss or HealthComponent.", this);
        }
    }

    private GameObject CreateGeneratedObject(string objectName)
    {
        GameObject generatedObject = new GameObject(objectName);
        generatedObject.transform.SetParent(generatedRoot, false);
        return generatedObject;
    }

    private GameObject CreateSpriteObject(string objectName, Vector3 position, Vector2 size, Color color)
    {
        GameObject gameObject = new GameObject(objectName);
        gameObject.transform.SetParent(generatedRoot, false);
        gameObject.transform.position = position;
        gameObject.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateWhiteSprite();
        renderer.color = color;
        renderer.sortingOrder = 0;
        return gameObject;
    }

    private static Sprite CreateWhiteSprite()
    {
        Texture2D texture = Texture2D.whiteTexture;
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }

}