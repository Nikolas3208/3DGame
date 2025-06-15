
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
        private float cutOff;
        private float outerCutOff;

        public Vector3 Position { get => transform.Position; set => transform.Position = value; }
        public Vector3 Direction { get => transform.Rotation; set => transform.Rotation = value; }

        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }
        public Vector3 Ambient { get; set; }

        public Vector4 Color { get; set; } = new Vector4(1,1,1,1);

        public float Linear { get; set; }
        public float Constant { get; set; }
        public float Quadratic { get; set; }

        public float CutOff { get => cutOff; set { cutOff = value; CosCutOff = MathF.Cos(MathHelper.DegreesToRadians(value)); } }
        public float OuterCutOff { get => outerCutOff; set { outerCutOff = value; CosOterCutOff = MathF.Cos(MathHelper.DegreesToRadians(value)); } }

        public float CosCutOff { get; set; }
        public float CosOterCutOff { get; set; }

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
        }

        public static Light Directional => new Light(new Vector3(1f), new Vector3(1f), new Vector3(0.5f), new Vector4(1), 0, 0, 0, 0, 0, LightType.Directional);
        public static Light Point => new Light(new Vector3(1f), new Vector3(1f), new Vector3(0.5f), new Vector4(1), 0.09f, 1f, 0.32f, 0, 0, LightType.Point);
        public static Light Spot => new Light(new Vector3(1f), new Vector3(1f), new Vector3(0.5f), new Vector4(1), 0.09f, 1f, 0.32f, 12.5f, 17.5f, LightType.Spot);
    }
}
