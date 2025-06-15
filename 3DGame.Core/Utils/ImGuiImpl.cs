using OpenTK.Mathematics;
using ImGuiNET;

namespace _3DGame.Core.Utils
{
    public static class ImGuiImpl
    {
        public static bool DragFloat2(string label, ref Vector2 vector, float speed = 0.1f, float min = float.MinValue, float max = float.MaxValue)
        {
            var v = new System.Numerics.Vector2(vector.X, vector.Y);
            bool drag = ImGui.DragFloat2(label, ref v, speed, min, max);

            if(drag)
            {
                vector = new Vector2(v.X, v.Y);
            }

            return drag;
        }

        public static bool DragFloat3(string label, ref Vector3 vector, float speed = 0.1f, float min = float.MinValue, float max = float.MaxValue)
        {
            var v = new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
            bool drag = ImGui.DragFloat3(label, ref v, speed, min, max);

            if (drag)
            {
                vector = new Vector3(v.X, v.Y, v.Z);
            }

            return drag;
        }

        public static bool DragFloat4(string label, ref Vector4 vector, float speed = 0.1f, float min = float.MinValue, float max = float.MaxValue)
        {
            var v = new System.Numerics.Vector4(vector.X, vector.Y, vector.Z, vector.W);
            bool drag = ImGui.DragFloat4(label, ref v, speed, min, max);

            if (drag)
            {
                vector = new Vector4(v.X, v.Y, v.Z, v.W);
            }

            return drag;
        }

        public static bool DragLabeledFloat3(string label, ref Vector3 vec, float speed = 0.1f)
        {
            return DragLabeledFloat3(label, new[] { "X", "Y", "Z" }, new[] { new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 1, 1) }, ref vec, speed);
        }

        public static bool DragLabeledFloat3(string label, string[] names, Vector4[] colors, ref Vector3 vec, float speed = 0.1f)
        {
            ImGui.Text(label); // Название блока (например, "Position")

            float itemWidth = (ImGui.GetContentRegionAvail().X - ImGui.GetStyle().ItemSpacing.X * 2) / 3f - 16;

            ImGui.PushID(names[0] + label);
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(colors[0].X, colors[0].Y, colors[0].Z, colors[0].W)); // Красный
            ImGui.Text(names[0]);
            ImGui.PopStyleColor();
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            bool dragX = ImGui.DragFloat($"##{names[0]}", ref vec.X, speed);
            ImGui.PopID();

            ImGui.SameLine();

            ImGui.PushID(names[1] + label);
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(colors[1].X, colors[1].Y, colors[1].Z, colors[1].W)); // Зелёный
            ImGui.Text(names[1]);
            ImGui.PopStyleColor();
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            bool dragY = ImGui.DragFloat($"##{names[1]}", ref vec.Y, speed);
            ImGui.PopID();

            ImGui.SameLine();

            ImGui.PushID(names[2] + label);
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(colors[2].X, colors[2].Y, colors[2].Z, colors[2].W)); // Синий
            ImGui.Text(names[2]);
            ImGui.PopStyleColor();
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            bool dragZ = ImGui.DragFloat($"##{names[2]}", ref vec.Z, speed);
            ImGui.PopID();

            return dragX || dragY || dragZ;
        }

        public static bool ColorEdit3(string label, ref Vector3 vector)
        {
            var v = new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
            bool drag = ImGui.ColorEdit3(label, ref v);

            if(drag)
            {
                vector = new Vector3(v.X, v.Y, v.Z);
            }

            return drag;
        }

        public static bool ColorEdit4(string label, ref Vector4 vector)
        {
            var v = new System.Numerics.Vector4(vector.X, vector.Y, vector.Z, vector.W);
            bool drag = ImGui.ColorEdit4(label, ref v);

            if (drag)
            {
                vector = new Vector4(v.X, v.Y, v.Z, v.W);
            }

            return drag;
        }

        public static void Image(int textureHandle, Vector2 size, Vector2 uv0 = default, Vector2 uv1 = default)
        {
            uv0 = uv0 == default ? Vector2.UnitX : uv0;
            uv1 = uv1 == default ? Vector2.UnitY : uv1;

            ImGui.Image(textureHandle, new System.Numerics.Vector2(size.X, size.Y),
                new System.Numerics.Vector2(uv0.X, uv0.Y), new System.Numerics.Vector2(uv1.X, uv1.Y));
        }

        public static bool ImageButton(string label, int textureHandle, Vector2 size, Vector2 uv0 = default, Vector2 uv1 = default)
        {
            uv0 = uv0 == default ? Vector2.UnitX : uv0;
            uv1 = uv1 == default ? Vector2.UnitY : uv1;

            return ImGui.ImageButton(label, textureHandle, new System.Numerics.Vector2(size.X, size.Y), 
                new System.Numerics.Vector2(uv0.X, uv0.Y), new System.Numerics.Vector2(uv1.X, uv1.Y));
        }

        public static Vector2 GetContentRegionAvail()
        {
            var regionAvail = ImGui.GetContentRegionAvail();

            return new Vector2(regionAvail.X, regionAvail.Y);
        }

        public static Vector2 GetWindowSize()
        {
            var windowSize = ImGui.GetWindowSize();
            return new Vector2(windowSize.X, windowSize.Y);
        }

        public static void SetNextWindowSize(Vector2 size)
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(size.X, size.Y));
        }

        public static void SetNextWindowSize(Vector2 size, ImGuiCond cond)
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(size.X, size.Y), cond);
        }
    }
}
