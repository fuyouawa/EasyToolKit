using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Core
{
    public class EasyPathConfig
    {
        public char SplitChar { get; set; } = '/';
    }

    public class EasyPath : IEquatable<EasyPath>
    {
        private static readonly HashSet<char> IllegalFileNameCharacters =
            new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());

        private readonly EasyPathConfig _config;
        private string _path;

        public static EasyPath FromPath([NotNull] string path, EasyPathConfig config = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return new EasyPath(config).AddPath(path);
        }

        public static EasyPath FromName([NotNull] string name, EasyPathConfig config = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new EasyPath(config).AddName(name);
        }

        public EasyPath(EasyPathConfig config = null)
        {
            _config = config ?? new EasyPathConfig();
        }

        public EasyPath AddPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return this;

            var formattedPath = FormatPath(path);
            if (string.IsNullOrEmpty(_path))
            {
                _path = formattedPath;
            }
            else
            {
                _path += _config.SplitChar + formattedPath;
            }
            return this;
        }

        public EasyPath AddPath(EasyPath path)
        {
            if (path is null || string.IsNullOrEmpty(path._path))
                return this;

            return AddPath(path._path);
        }

        public EasyPath AddName(string name, bool replaceIllegalCharacters = true, char replacement = '_')
        {
            if (string.IsNullOrEmpty(name))
                return this;

            var processedName = replaceIllegalCharacters ? ReplaceIllegalFileNameCharacters(name, replacement) : name;

            if (string.IsNullOrEmpty(_path))
            {
                _path = processedName;
            }
            else
            {
                _path += _config.SplitChar + processedName;
            }
            return this;
        }

        public static bool operator ==(EasyPath left, EasyPath right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(EasyPath left, EasyPath right)
        {
            return !(left == right);
        }

        public static bool operator ==(EasyPath left, string right)
        {
            if (left is null) return right is null;
            if (right is null) return false;
            return left._path == right;
        }

        public static bool operator !=(EasyPath left, string right)
        {
            return !(left == right);
        }

        public static bool operator ==(string left, EasyPath right)
        {
            if (right is null) return left is null;
            if (left is null) return false;
            return left == right._path;
        }

        public static bool operator !=(string left, EasyPath right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return _path;
        }

        private string FormatPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // 替换所有常见的路径分隔符为配置的分隔符
            var formatted = path.Replace('\\', _config.SplitChar)
                               .Replace('/', _config.SplitChar);

            // 移除开头和结尾的分隔符
            formatted = formatted.Trim(_config.SplitChar);

            // 合并多个连续的分隔符
            var doubleSplitChar = new string(_config.SplitChar, 2);
            var singleSplitChar = _config.SplitChar.ToString();
            while (formatted.Contains(doubleSplitChar))
            {
                formatted = formatted.Replace(doubleSplitChar, singleSplitChar);
            }

            return formatted;
        }

        private string ReplaceIllegalFileNameCharacters(string fileName, char replacement)
        {
            var charArray = fileName.ToCharArray();
            for (var i = 0; i < charArray.Length; ++i)
            {
                if (IllegalFileNameCharacters.Contains(charArray[i]))
                    charArray[i] = replacement;
            }
            return new string(charArray);
        }

        public bool Equals(EasyPath other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _path == other._path;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EasyPath)obj);
        }

        public override int GetHashCode()
        {
            return (_path != null ? _path.GetHashCode() : 0);
        }
    }
}
