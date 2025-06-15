using _3DGame.Core;
using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Physics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class PlayerController : Component
{
    private GameObject camera;
    private Transform cameraTransform;
    private Camera cameraComponent;
    private RigidBody rb;

    private bool isGrounded;

    private float speed = 10;
    private float jumpPower = 20;

    public override void Start()
    {
        rb = GetComponent<RigidBody>()!;

        camera = GameObject!.Scene.GetGameObjectAt(0)!;

        cameraTransform = camera?.GetComponent<Transform>()!;
        cameraComponent = camera?.GetComponent<Camera>()!;
    }

    public override void Update(float deltaTime)
    {
        var cameraFront = cameraComponent.Front;
        var cameraRight = cameraComponent.Right;

        if (Keyboard.IsKeyDown(Keys.W))
        {
            transform.Position += new Vector3(cameraFront.X, 0, cameraFront.Z) * speed * deltaTime;
        }
        else if (Keyboard.IsKeyDown(Keys.S))
        {
            transform.Position -= new Vector3(cameraFront.X, 0, cameraFront.Z) * speed * deltaTime;
        }
        if (Keyboard.IsKeyDown(Keys.A))
        {
            transform.Position -= new Vector3(cameraRight.X, 0, cameraRight.Z) * speed * deltaTime;
        }
        else if (Keyboard.IsKeyDown(Keys.D))
        {
            transform.Position += new Vector3(cameraRight.X, 0, cameraRight.Z) * speed * deltaTime;
        }

        if (Keyboard.IsKeyPressed(Keys.Space) && isGrounded)
        {
            rb.AddImpuls(Vector3.UnitY * jumpPower);
            isGrounded = false;
        }

        cameraTransform.Position = transform.Position + new Vector3(0, 2, 0);
        transform.Rotation = new Vector3(0, -cameraTransform.Rotation.X, 0);
        cameraComponent.Pitch = -35;
    }

    public override void OnCollided(CollidedEventArgs args)
    {
        if (args.Normal.Y >= 0.5f)
        {
            isGrounded = true;
        }
    }
}
