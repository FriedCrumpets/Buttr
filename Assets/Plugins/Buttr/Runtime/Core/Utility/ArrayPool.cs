using System;
using System.Collections.Generic;

namespace Buttr.Core {
    public static class ArrayPool<T> {
        private static readonly Stack<T[]> s_Pool = new();

        public static T[] Get(int size) {
            if (s_Pool.TryPop(out var array) == false)
                return new T[size];
            
            if(array.Length == size) return array;
            
            Array.Resize(ref array, size);
            return array;
        }

        public static void Release(T[] array) {
            Array.Clear(array, 0, array.Length);
            s_Pool.Push(array);
        }
    }
}