using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZQNB.Common.Extensions
{
    public static class Assertion
    {
        public static void NotNull(object self, string message = null)
        {
            if (self != null)
            {
                return;
            }

            if (message.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException();
            }

            throw new ArgumentNullException(message);
        }

        public static void NotNullOrWhiteSpace(string self, string message = null)
        {
            NotNull(self);

            if (self.IsNullOrWhiteSpace())
            {
                throw new ArgumentException(string.Format("Given string is either empty or contains only whitespace characters{0}", message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }

        public static void NullOrWhiteSpace(string self, string message = null)
        {
            if (!self.IsNullOrWhiteSpace())
            {
                throw new ArgumentException(string.Format("Given string is not empty{0}", message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }

        public static void Default<T>(this T self, string message = null) where T : struct
        {
            if (self.Equals(default(T)))
            {
                return;
            }

            if (message.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("Argument doesn't has a default value for type {0}. Expected : {1}. Actual : {2}".FormatSelf(typeof(T), default(T), self));
            }

            throw new ArgumentException(message);
        }

        public static void Empty(IEnumerable self, string message = null)
        {
            Empty(self != null ? self.Cast<object>() : null, message);
        }

        public static void Empty<T>(IEnumerable<T> self, string message = null)
        {
            if (self != null && self.Any())
            {
                throw new ArgumentException(string.Format("Sequence {0} is not empty{1}", self.ToListString(), message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }

        public static void Equal(object self, object other, string message = null)
        {
            if (self == null || other == null || !Equals(self, other))
            {
                throw new ArgumentException(string.Format("Objects were expected to be equal, but they were not. Expected : {0}; actual : {1}{2}", self, other, message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }

        public static void False(bool self, string message = null)
        {
            if (self)
            {
                throw new ArgumentException("Specified condition is not false{0}", message.IsNullOrWhiteSpace() ? string.Empty : " : " + message);
            }
        }

        public static void NotDefault<T>(T self, string message = null) where T : struct
        {
            if (!self.Equals(default(T)))
            {
                return;
            }

            if (message.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("Argument has a default value for type {0} : {1}".FormatSelf(typeof(T), self));
            }

            throw new ArgumentException(message);
        }

        public static void NotEmpty(IEnumerable self, string message = null)
        {
            NotEmpty(self.Cast<object>(), message);
        }

        public static void NotEmpty<T>(IEnumerable<T> self, string message = null)
        {
            NotNull(self);

            if (!self.Any())
            {
                throw new ArgumentException(string.Format("Sequence {0} is empty{1}", self.ToListString(), message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }

        public static void NotEqual(object self, object other, string message = null)
        {
            if (self != null && other != null && Equals(self, other))
            {
                throw new ArgumentException(string.Format("Objects were expected to be different, but they were equal. Expected {0}, but actual is {1}{2}", self, other, message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }


        public static void Null(object self, string message = null)
        {
            if (self != null)
            {
                throw new ArgumentException(string.Format("Argument was expected to be null, but is {0}{1}", self, message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }

        public static void True(bool self, string message = null)
        {
            if (!self)
            {
                throw new ArgumentException(string.Format("Specified condition is not true{0}", message.IsNullOrWhiteSpace() ? string.Empty : " : " + message));
            }
        }
    }
}
