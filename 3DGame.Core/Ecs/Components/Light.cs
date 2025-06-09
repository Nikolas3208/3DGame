
using OpenTK.Mathematics;

namespace _3DGame.Core.Ecs.Components
{
    public enum LightType
    {
        Directional = 0,
        Point = 1,
        Spot = 2
    }

    public class Light : Component
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }

        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }
        public Vector3 Ambient { get; set; }

        public Vector4 Color { get; set; } = new Vector4(1,1,1,1);

        public float Linear { get; set; }
        public float Constant { get; set; }
        public float Quadratic { get; set; }

        public float CutOff { get; set; }
        public float OuterCutOff { get; set; }

        public LightType Type { get; set; } = LightType.Directional;

        public int Index { get; set; }

        public Light(LightType type)
        {
            Type = type;
        }

        public Light(Vector3 diffuse, Vector3 specular, Vector3 ambient,
            Vector4 color, float linear, float constant, float quadratic, float cutOff, float outerCutOff, LightType type)
        {
            Diffuse = diffuse;
            Specular = specular;
            Ambient = ambient;
            Color = color;
            Linear = linear;
            Constant = constant;
            Quadratic = quadratic;
            CutOff = cutOff;
            OuterCutOff = outerCutOff;
            Type = type;
        }

        public override void Start()
        {
            base.Start();

            if(Scene != null)
            {
                Scene.AddLight(this);
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if(GameObject != null)
            {
                var transform = GameObject.GetComponent<Transformable>();

                Position = transform!.Position;
                Direction = transform!.Rotation;
            }
        }

        public static Light Directional => new Light(new Vector3(0.5f), new Vector3(0.2f), new Vector3(0.5f), new Vector4(1), 0, 0, 0, 0, 0, LightType.Directional);
        public static Light Point => new Light(new Vector3(0.5f), new Vector3(0.2f), new Vector3(0.5f), new Vector4(1), 0.09f, 1f, 0.32f, 0, 0, LightType.Point);
        public static Light Spot => new Light(new Vector3(0.5f), new Vector3(0.2f), new Vector3(0.5f), new Vector4(1), 0.09f, 1f, 0.32f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(17.5f)), LightType.Point);
    }
}
