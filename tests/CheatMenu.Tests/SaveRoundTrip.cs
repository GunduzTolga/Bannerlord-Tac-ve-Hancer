using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CheatMenu.Intrigue.Core;

namespace CheatMenu.Tests
{
    /// <summary>
    /// Kayıt güvenliği: IntrigueState'in her alanı (her listenin bir öğesinin her alanı dahil) yansımayla doldurulur, yazılıp geri okunur
    /// ve karşılaştırılır. Yeni bir alan Write'a ya da Parse'a eklenmeyi unutulursa burada kırmızı yanar; yoksa kayıtta sessizce kaybolurdu.
    /// Sayılar 1-3 arası, yaşlar 30: Parse bazı alanları kurallarının sınırına kırpıyor (seviye ve aşama en çok 3, ajan en çok 5, gerilim
    /// en çok 100, çalışan yaşı en az 19).
    /// </summary>
    internal static partial class Program
    {
        private const BindingFlags Members = BindingFlags.Public | BindingFlags.Instance;
        private static int _sample;

        private static void SaveRoundTripTests()
        {
            _sample = 0;
            var state = new IntrigueState();
            FillMembers(state);
            var empty = new List<string>();
            foreach (FieldInfo field in typeof(IntrigueState).GetFields(Members))
                if (IsCollection(field.FieldType) && !((IEnumerable)field.GetValue(state)).Cast<object>().Any()) empty.Add(field.Name);
            Check("save: test fills every list (" + string.Join(", ", empty) + ")", empty.Count == 0);

            var diffs = new List<string>();
            CompareMembers("state", state, IntrigueState.Parse(state.Write()), diffs);
            foreach (string diff in diffs.Take(10)) Console.WriteLine("     kayıtta değişti: " + diff);
            Check("save: every field survives write and read (" + diffs.Count + " differences)", diffs.Count == 0);
        }

        private static bool IsScalar(Type type) =>
            type == typeof(string) || type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double) || type == typeof(bool) || type.IsEnum;

        private static bool IsCollection(Type type) =>
            type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                || type.GetGenericTypeDefinition() == typeof(HashSet<>));

        private static object Sample(Type type, string name)
        {
            int n = _sample++;
            if (type == typeof(int) && name == "Age") return 30;
            if (type == typeof(string)) return "d" + n;
            if (type == typeof(int)) return 1 + n % 3;
            if (type == typeof(long)) return (long)(1 + n % 3);
            if (type == typeof(float)) return 1.5f + n % 3;
            if (type == typeof(double)) return 1.5 + n % 3;
            if (type == typeof(bool)) return true;
            Array values = Enum.GetValues(type);
            return values.GetValue(values.Length - 1);
        }

        private static void FillMembers(object target)
        {
            foreach (FieldInfo field in target.GetType().GetFields(Members))
                if (!field.IsInitOnly || IsCollection(field.FieldType))
                    FillMember(field.Name, field.FieldType, () => field.GetValue(target), field.IsInitOnly ? null : (Action<object>)(v => field.SetValue(target, v)));
            foreach (PropertyInfo property in target.GetType().GetProperties(Members))
                if (property.CanRead && property.GetIndexParameters().Length == 0 && (property.CanWrite || IsCollection(property.PropertyType)))
                    FillMember(property.Name, property.PropertyType, () => property.GetValue(target), property.CanWrite ? (Action<object>)(v => property.SetValue(target, v)) : null);
        }

        private static void FillMember(string name, Type type, Func<object> get, Action<object> set)
        {
            if (IsScalar(type))
            {
                set?.Invoke(Sample(type, name));
                return;
            }
            if (!IsCollection(type)) return;
            object collection = get();
            if (collection == null && set != null) set(collection = Activator.CreateInstance(type));
            if (collection == null) return;
            Type[] args = type.GetGenericArguments();
            if (collection is IDictionary dictionary)
            {
                object key = NewItem(args[0]), value = NewItem(args[1]);
                if (key != null && value != null) dictionary[key] = value;
                return;
            }
            object item = NewItem(args[0]);
            if (item != null) type.GetMethod("Add").Invoke(collection, new[] { item });
        }

        private static object NewItem(Type type)
        {
            if (IsScalar(type)) return Sample(type, null);
            if (!type.IsClass || type.GetConstructor(Type.EmptyTypes) == null) return null;
            object item = Activator.CreateInstance(type);
            FillMembers(item);
            return item;
        }

        private static void CompareMembers(string path, object before, object after, List<string> diffs)
        {
            if (before == null && after == null) return;
            if (before == null || after == null)
            {
                diffs.Add(path + ": " + (before ?? "null") + " -> " + (after ?? "null"));
                return;
            }
            Type type = before.GetType();
            if (IsScalar(type))
            {
                if (!before.Equals(after)) diffs.Add(path + ": " + before + " -> " + after);
                return;
            }
            if (before is IDictionary map)
            {
                var other = (IDictionary)after;
                if (map.Count != other.Count) diffs.Add(path + ": " + map.Count + " -> " + other.Count + " öğe");
                else
                    foreach (object key in map.Keys)
                        if (!other.Contains(key)) diffs.Add(path + "[" + key + "]: yok");
                        else CompareMembers(path + "[" + key + "]", map[key], other[key], diffs);
                return;
            }
            if (before is IEnumerable list)
            {
                List<object> a = list.Cast<object>().ToList(), b = ((IEnumerable)after).Cast<object>().ToList();
                if (a.Count != b.Count) diffs.Add(path + ": " + a.Count + " -> " + b.Count + " öğe");
                else
                    for (int i = 0; i < a.Count; i++) CompareMembers(path + "[" + i + "]", a[i], b[i], diffs);
                return;
            }
            foreach (FieldInfo field in type.GetFields(Members)) CompareMembers(path + "." + field.Name, field.GetValue(before), field.GetValue(after), diffs);
            foreach (PropertyInfo property in type.GetProperties(Members))
                if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
                    CompareMembers(path + "." + property.Name, property.GetValue(before), property.GetValue(after), diffs);
        }
    }
}
