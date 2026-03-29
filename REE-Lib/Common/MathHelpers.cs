using System.Numerics;
using ReeLib.via;

namespace ReeLib.Common;

public static class MathHelpers
{
    private static readonly Vector3[] Axes = [
        new Vector3(1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1)
    ];

    public static bool ContainsTriangle(this AABB bounds, in Vector3 a, in Vector3 b, in Vector3 c)
    {
        // ref: https://stackoverflow.com/a/17503268
        Span<Vector3> triVerts = stackalloc Vector3[3];
        triVerts[0] = a;
        triVerts[1] = b;
        triVerts[2] = c;

        float boxMin, boxMax, triMin, triMax;
        for (int i = 0; i < 3; i++)
        {
            Project(triVerts, Axes[i], out triMin, out triMax);
            if (triMax < bounds.minpos[i] || triMin > bounds.maxpos[i])
                return false;
        }

        Span<Vector3> boundVerts = stackalloc Vector3[2];
        boundVerts[0] = bounds.minpos;
        boundVerts[1] = bounds.maxpos;

        var triNormal = Vector3.Cross(a - b, c - b);
        var triangleOffset = Vector3.Dot(triNormal, a);
        Project(boundVerts, triNormal, out boxMin, out boxMax);
        if (boxMax < triangleOffset || boxMin > triangleOffset)
            return false; // No intersection possible.

        Span<Vector3> edges = stackalloc Vector3[3];
        edges[0] = a - b;
        edges[1] = b - c;
        edges[2] = c - a;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                var axis = Vector3.Cross(edges[i], Axes[j]);
                Project(boundVerts, axis, out boxMin, out boxMax);
                Project(triVerts, axis, out triMin, out triMax);
                if (boxMax < triMin || boxMin > triMax)
                    return false;
            }
        }

        static void Project(ReadOnlySpan<Vector3> points, Vector3 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;
            foreach (var p in points)
            {
                float val = Vector3.Dot(axis, p);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        return true;
    }

}
