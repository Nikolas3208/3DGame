using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Graphics;
using _3DGame.Core.Physics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _3DGame.Core
{
    public abstract class Scene
    {
        protected Game game;
        protected List<GameObject> gameObjects;
        protected List<Light> lights;
        protected Camera? activeCamera;
        protected PhysicsWorld physicsWorld;

        protected float accumulator = 0f;
        protected float fixedDeltaTime = 1f / 60f; // 60 шагов в секунду
        protected float maxDelta = 0.25f; // максимум 0.25 секунды за раз


        public string Name { get; set; } = nameof(Scene);

        public bool IsActive { get; set; }
        public bool IsStarted { get; set; }

        public Vector2i Size { get; set; }

        public Scene(Vector2i size, Game game)
        {
            Size = size;
            this.game = game;

            gameObjects = new List<GameObject>();
            lights = new List<Light>();
            physicsWorld = new PhysicsWorld(this);

            GL.Enable(EnableCap.DepthTest);
        }

        public virtual void Start()
        {
            foreach (GameObject obj in gameObjects)
            {
                obj.Start();
            }

            IsStarted = true;
        }

        public virtual void Update(float deltaTime)
        {
            foreach (GameObject obj in gameObjects)
            {
                obj.Update(deltaTime);
            }

            accumulator += deltaTime;
            accumulator = MathF.Min(accumulator, maxDelta);

            while (accumulator >= fixedDeltaTime)
            {
                FixedUpdate(fixedDeltaTime);
                accumulator -= fixedDeltaTime;
            }
        }

        public virtual void FixedUpdate(float fixedDeltaTime)
        {
            physicsWorld.FixedUpdate(fixedDeltaTime);

            gameObjects.ForEach(g => g.FixedUpdate(fixedDeltaTime));
        }

        public virtual void Draw(Renderer renderer)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            renderer.Shader.SetMatrix4("projection", activeCamera!.GetProjectionMatrix());
            renderer.Shader.SetMatrix4("view", activeCamera!.GetViewMatrix());
            renderer.Shader.SetVector3("viewPos", activeCamera.Position);

            renderer.Shader.SetInt("lightCount", lights.Count);

            for (int i = 0; i < lights.Count; i++)
            {
                Light light = lights[i];

                renderer.Shader.SetVector3($"lights[{i}].position", light.Position);
                renderer.Shader.SetVector3($"lights[{i}].direction", light.Direction);

                renderer.Shader.SetVector3($"lights[{i}].diffuse", light.Diffuse);
                renderer.Shader.SetVector3($"lights[{i}].specular", light.Specular);
                renderer.Shader.SetVector3($"lights[{i}].ambient", light.Ambient);

                renderer.Shader.SetVector4($"lights[{i}].color", light.Color);

                renderer.Shader.SetFloat($"lights[{i}].linear", light.Linear);
                renderer.Shader.SetFloat($"lights[{i}].constant", light.Constant);
                renderer.Shader.SetFloat($"lights[{i}].quadratic", light.Quadratic);

                renderer.Shader.SetFloat($"lights[{i}].cutOff", light.CutOff);
                renderer.Shader.SetFloat($"lights[{i}].outerCutOff", light.OuterCutOff);

                renderer.Shader.SetInt($"lights[{i}].type", (int)light.Type);
            }

            foreach (GameObject obj in gameObjects)
            {
                obj.Draw(renderer);
            }
        }

        public virtual void Resize(Vector2i size)
        {
            Size = size;

            activeCamera!.AspectRatio = size.X / (float)size.Y;

            GL.Viewport(0, 0, size.X, size.Y);
        }

        public virtual void MouseWheel(Vector2 offset)
        {

        }

        public virtual bool AddGameObject(GameObject obj)
        {
            if (!gameObjects.Contains(obj))
            {
                gameObjects.Add(obj);
                obj.Scene = this;
                return true;
            }

            return false;
        }

        public virtual GameObject? GetGameObjectAt(int index)
        {
            if (index < 0 || index >= gameObjects.Count)
                return null;

            return gameObjects[index];
        }

        public virtual bool RemoveGameObject(GameObject obj)
        {
            return gameObjects.Remove(obj);
        }

        public List<T> GetComponents<T>() where T : Component
        {
            var list = new List<T>();

            var objsContsinsComponet = gameObjects.Where(g => g.ContainsComponent<T>());

            foreach(var obj in objsContsinsComponet)
            {
                list.Add(obj.GetComponent<T>()!);
            }

            return list;
        }
        public virtual bool AddLight(Light light)
        {
            if (!lights.Contains(light))
            {
                lights.Add(light);
                light.Index = lights.Count - 1;

                return true;
            }

            return false;
        }

        public virtual Light? GetLightAt(int index)
        {
            if (index < 0 || index >= lights.Count)
                return null;

            return lights[index];
        }

        public virtual bool RemoveLight(Light light)
        {
            return lights.Remove(light);
        }

        public virtual void SetCamera(Camera camera)
        {
            if (camera == null)
                return;

            if (activeCamera != null)
                activeCamera.IsActive = false;

            activeCamera = camera;
            activeCamera.IsActive = true;
        }

        public virtual Camera? GetCamera() => activeCamera;

        public Game GetGame() => game; 
    }
}
