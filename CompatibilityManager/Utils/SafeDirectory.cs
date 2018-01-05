using System.Collections.Generic;
using System.Linq;

namespace System.IO
{
    public class SafeDirectory
    {
        /// <summary>
        /// Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.
        /// Exactly like the regular EnumerateFiles, except this one skips over unaccessible folders/files (for whatever reason).
        /// </summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories. The default value is TopDirectoryOnly.</param>
        /// <returns>An enumerable collection of the full names (including paths) for the files in the directory specified by path and that match the specified search pattern and option.</returns>
        public static IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                var files = Enumerable.Empty<string>();
                if (searchOption == SearchOption.AllDirectories)
                {
                    files = Directory.EnumerateDirectories(path).SelectMany(subdir => SafeDirectory.SafeEnumerateFiles(subdir, searchPattern, searchOption));
                }
                return files.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (Exception e) when (e is UnauthorizedAccessException || e is PathTooLongException || e is IOException)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}
