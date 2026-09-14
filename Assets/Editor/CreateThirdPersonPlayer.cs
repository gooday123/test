using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class CreateThirdPersonPlayer
{
    private const string PlayerName = "박진영";

    static CreateThirdPersonPlayer()
    {
        EditorApplication.delayCall += AutoCreateIfMissing;
    }

    [MenuItem("Tools/Create Third Person Player")]
    public static void Create()
    {
        GameObject existing = GameObject.Find(PlayerName);
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = PlayerName;
        player.transform.position = new Vector3(0f, 1f, 0f);
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());

        CharacterController characterController = player.AddComponent<CharacterController>();
        characterController.center = new Vector3(0f, 1f, 0f);
        characterController.height = 2f;
        characterController.radius = 0.38f;

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(player.transform);
        head.transform.localPosition = new Vector3(0f, 1.35f, 0f);
        head.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        Object.DestroyImmediate(head.GetComponent<SphereCollider>());

        GameObject nameTag = new GameObject("Name Tag");
        nameTag.transform.SetParent(player.transform);
        nameTag.transform.localPosition = new Vector3(0f, 2.35f, 0f);
        TextMesh textMesh = nameTag.AddComponent<TextMesh>();
        textMesh.text = PlayerName;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.characterSize = 0.22f;
        textMesh.fontSize = 64;
        textMesh.color = Color.white;
        nameTag.AddComponent<PlayerNameBillboard>();

        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        camera.transform.position = new Vector3(0f, 3.5f, -6.5f);
        camera.transform.rotation = Quaternion.Euler(18f, 0f, 0f);

        ThirdPersonCameraFollow cameraFollow = camera.GetComponent<ThirdPersonCameraFollow>();
        if (cameraFollow == null)
        {
            cameraFollow = camera.gameObject.AddComponent<ThirdPersonCameraFollow>();
        }

        ThirdPersonPlayerController playerController = player.AddComponent<ThirdPersonPlayerController>();
        playerController.SetCameraTransform(camera.transform);
        cameraFollow.SetTarget(player.transform);

        EnsureGround();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
    }

    private static void AutoCreateIfMissing()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().path == string.Empty)
        {
            return;
        }

        if (GameObject.Find(PlayerName) != null)
        {
            return;
        }

        Create();
    }

    private static void EnsureGround()
    {
        if (GameObject.Find("Ground") != null)
        {
            return;
        }

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(8f, 1f, 8f);
    }
}
