using System.Collections.Generic;

namespace Fovea.Renderer.Extensions;

public static class CollectionExtensions
{
    public static IList<T> SwapElements<T>(this IList<T> list, int left, int right)
    {
        (list[left], list[right]) = (list[right], list[left]);
        return list;
    }
}