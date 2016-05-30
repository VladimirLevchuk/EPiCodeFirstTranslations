using System;
using JetBrains.Annotations;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class TranslationPath
    {
        public static bool IsAbsolute([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            return path.StartsWith("~") || path.StartsWith("/~");
        }

        [NotNull]
        public static string GetPathFromAbsolutePath([NotNull] string absolutePath)
        {
            if (absolutePath == null) throw new ArgumentNullException(nameof(absolutePath));
            return EnsureStartsWithSlash(absolutePath.Replace("/~", string.Empty).Replace("~", string.Empty));
        }

        [NotNull]
        public static string EnsureStartsWithSlash([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return path.StartsWith("/") ? path : "/" + path;
        }

        [NotNull]
        public static string RemoveEndSlashIfAny([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return !path.EndsWith("/") ? path : path.Remove(path.Length - 1);
        }

        [NotNull]
        public static string Combine([NotNull] string path1, [NotNull] string path2)
        {
            if (path1 == null) throw new ArgumentNullException(nameof(path1));
            if (path2 == null) throw new ArgumentNullException(nameof(path2));

            if (IsAbsolute(path2))
            {
                return GetPathFromAbsolutePath(path2);
            }

            var result = RemoveEndSlashIfAny(EnsureStartsWithSlash(path1)) + EnsureStartsWithSlash(path2);
            return result;
        }

        public static string Combine([NotNull] string path1, [NotNull] string path2, params string[] paths)
        {
            var result = Combine(path1, path2);

            foreach (var path in paths)
            {
                result = Combine(result, path);
            }

            return result;
        }
    }
}