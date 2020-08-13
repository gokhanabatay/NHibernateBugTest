using NHibernate;
using NHibernate.Impl;
using NHibernate.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernateBugTest.Session
{
    public static class IQueryableExtension
    {
        private const bool IsTrue = true;

        public static EnumerableImpl Stream<T>(this IQueryable<T> source)
        {
            IQuery query = ((DefaultQueryProvider)source.Provider)
                .GetPreparedQuery(source.Expression, out var nhLinqExpression);

            return query.Enumerable() as EnumerableImpl;
        }

        /// <summary>
        /// Fetches next list of record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="items">out items</param>
        /// <param name="fetchSize">fetch size</param>
        /// <returns>If reader is not closed returns true</returns>
        public static bool Fetch<T>(this IEnumerator enumerable, out List<T> items, int fetchSize = 0)
        {
            items = new List<T>();
            bool hasNext;
            while ((hasNext = enumerable.MoveNext()) == IsTrue)
            {
                items.Add((T)enumerable.Current);

                if (fetchSize > 0 &&
                   items.Count == fetchSize)
                {
                    break;
                }
            }

            return hasNext;
        }

        /// <summary>
        /// Fetches next list of record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="items">out items</param>
        /// <param name="fetchSize">fetch size</param>
        /// <returns>If reader is not closed returns true</returns>
        public static bool Fetch<T>(this IEnumerator<T> enumerable, out List<T> items, int fetchSize = 0)
        {
            items = new List<T>();
            bool hasNext;
            while ((hasNext = enumerable.MoveNext()) == IsTrue)
            {
                items.Add(enumerable.Current);

                if (fetchSize > 0 &&
                   items.Count == fetchSize)
                {
                    break;
                }
            }

            return hasNext;
        }

        /// <summary>
        /// Fetches next record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="item">return items</param>
        /// <returns>If reader is not closed returns true</returns>
        public static bool Next<T>(this IEnumerator enumerable, out T item)
        {
            item = default;
            bool hasNext;
            if ((hasNext = enumerable.MoveNext()) == IsTrue)
            {
                item = (T)enumerable.Current;
            }

            return hasNext;
        }

        /// <summary>
        /// Fetches next record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="item">return items</param>
        /// <returns>If reader is not closed returns true</returns>
        public static bool Next<T>(this IEnumerator<T> enumerable, out T item)
        {
            item = default;
            bool hasNext;
            if ((hasNext = enumerable.MoveNext()) == IsTrue)
            {
                item = enumerable.Current;
            }

            return hasNext;
        }

        /// <summary>
        /// Conditional where criteria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">pre condition to add filter</param>
        /// <param name="predicate">filter</param>
        /// <returns></returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> source,
        Func<bool> condition,
        Expression<Func<T, bool>> predicate)
        {
            if (condition())
            {
                return source.Where(predicate);
            }

            return source;
        }

        /// <summary>
        /// Conditional where criteria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition">pre condition to add filter</param>
        /// <param name="predicate">filter</param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this IQueryable<T> source,
        Func<bool> condition,
        Expression<Func<T, bool>> predicate)
        {
            if (condition())
            {
                return source.FirstOrDefault(predicate);
            }

            return source.FirstOrDefault();
        }

        public static List<T>[] GroupBy<T>(this IEnumerable<T> items, int itemCount)
        {
            List<List<T>> pagedItems = new List<List<T>>();
            List<T> pageItem = new List<T>();
            foreach (var item in items)
            {
                if (pageItem.Count == itemCount)
                {
                    pagedItems.Add(pageItem);
                    pageItem = new List<T>();
                }

                pageItem.Add(item);
            }
            if (pageItem.Count > 0)
            {
                pagedItems.Add(pageItem);
            }

            return pagedItems.ToArray();
        }
    }
}
