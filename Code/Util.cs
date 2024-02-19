using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Util
{
    public static Vector2 Rot90CW(this Vector2 v) => new(v.Y, -v.X);
    public static Vector2 Rot90CCW(this Vector2 v) => new(-v.Y, v.X);

    public static IEnumerable<T> GetChildren<T>(this Node n) where T : Node => n.GetChildren().Select(x => x as T).NotNull();
    public static T GetChild<T>(this Node n) where T : Node => n.GetChildren<T>().FirstOrDefault();
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T> e) => e.Where(x => x != null);
 
    public static void AssignPathTo(this RemoteTransform2D rtrs, Node2D node) => rtrs.RemotePath = rtrs.GetPathTo(node);
}