// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using System.Text;

namespace C5
{
    // ------------------------------------------------------------

    // Static helper methods for Showing collections 

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public static class Showing
    {
        /// <summary>
        /// Show  <code>Object obj</code> by appending it to <code>stringbuilder</code>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns>True if <code>obj</code> was shown completely.</returns>
        public static bool Show(Object? obj, StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
        {
            if (rest <= 0)
            {
                return false;
            }
            else if (obj is IShowable showable)
            {
                return showable.Show(stringbuilder, ref rest, formatProvider);
            }

            int oldLength = stringbuilder.Length;
            stringbuilder.AppendFormat(formatProvider, "{0}", obj);
            rest -= (stringbuilder.Length - oldLength);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="showable"></param>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static string ShowString(IShowable showable, string? format, IFormatProvider? formatProvider)
        {
            int rest = MaxLength(format);
            StringBuilder sb = new StringBuilder();
            showable.Show(sb, ref rest, formatProvider);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static int MaxLength(string? format)
        {
            //TODO: validate format string
            if (format == null)
            {
                return 80;
            }

            if (format.Length > 1 && format.StartsWith("L"))
            {
                return int.Parse(format.Substring(1));
            }
            else
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns>True if collection was shown completely</returns>
        public static bool ShowCollectionValue<T>(ICollectionValue<T>? items, StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            string startdelim = "{ ", enddelim = " }";
            bool showIndexes = false;
            bool showMultiplicities = false;
            if (items is null)
            {
                return true;
            }
            //TODO: do not test here at run time, but select code at compile time
            //      perhaps by delivering the print type to this method
            ICollection<T> coll = (items! as ICollection<T>)!;
            if (items is IList<T> list)
            {
                startdelim = "[ ";
                enddelim = " ]";
                //TODO: should have been (items as IIndexed<T>).IndexingSpeed
                showIndexes = list.IndexingSpeed == Speed.Constant;
            }
            else if (coll != null)
            {
                if (coll.AllowsDuplicates)
                {
                    startdelim = "{{ ";
                    enddelim = " }}";
                    if (coll.DuplicatesByCounting)
                    {
                        showMultiplicities = true;
                    }
                }
            }

            stringbuilder.Append(startdelim);
            rest -= 2 * startdelim.Length;
            bool first = true;
            bool complete = true;
            int index = 0;

            if (showMultiplicities)
            {
                foreach (System.Collections.Generic.KeyValuePair<T, int> p in coll!.ItemMultiplicities())
                {
                    complete = false;
                    if (rest <= 0)
                    {
                        break;
                    }

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        stringbuilder.Append(", ");
                        rest -= 2;
                    }
                    if (complete = Showing.Show(p.Key, stringbuilder, ref rest, formatProvider))
                    {
                        string multiplicityString = string.Format("(*{0})", p.Value);
                        stringbuilder.Append(multiplicityString);
                        rest -= multiplicityString.Length;
                    }
                }
            }
            else
            {
                foreach (T x in items)
                {
                    complete = false;
                    if (rest <= 0)
                    {
                        break;
                    }

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        stringbuilder.Append(", ");
                        rest -= 2;
                    }
                    if (showIndexes)
                    {
                        string indexString = string.Format("{0}:", index++);
                        stringbuilder.Append(indexString);
                        rest -= indexString.Length;
                    }
                    complete = Showing.Show(x, stringbuilder, ref rest, formatProvider);
                }
            }
            if (!complete)
            {
                stringbuilder.Append("...");
                rest -= 3;
            }
            stringbuilder.Append(enddelim);
            return complete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// 
        /// <param name="dictionary"></param>
        /// <param name="stringbuilder"></param>
        /// <param name="formatProvider"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public static bool ShowDictionary<K, V>(IDictionary<K, V> dictionary, StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
        {
            bool sorted = dictionary is ISortedDictionary<K, V>;
            stringbuilder.Append(sorted ? "[ " : "{ ");
            rest -= 4;				   // Account for "( " and " )"
            bool first = true;
            bool complete = true;

            foreach (System.Collections.Generic.KeyValuePair<K, V> p in dictionary)
            {
                complete = false;
                if (rest <= 0)
                {
                    break;
                }

                if (first)
                {
                    first = false;
                }
                else
                {
                    stringbuilder.Append(", ");
                    rest -= 2;
                }
                complete = Showing.Show(p, stringbuilder, ref rest, formatProvider);
            }
            if (!complete)
            {
                stringbuilder.Append("...");
                rest -= 3;
            }
            stringbuilder.Append(sorted ? " ]" : " }");
            return complete;
        }
    }
}