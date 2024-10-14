using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Jeffijoe.MessageFormat;

internal static class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> SbPool;

    static StringBuilderPool()
    {
        var shared = new DefaultObjectPoolProvider();
        SbPool = shared.CreateStringBuilderPool();
    }

    public static StringBuilder Get()
    {
        return SbPool.Get();
    }

    public static void Return(StringBuilder sb)
    {
        SbPool.Return(sb);
    }
}