﻿using System;
using System.Numerics;

namespace SimdSpike {
    public static class IntSimdProcessor {
        public static void NaiveMinMax(int[] input, out int minimum, out int maximum) {
            int min = int.MaxValue, max = int.MinValue;
            foreach (var value in input) {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
            minimum = min;
            maximum = max;
        }

        public static void HWAcceleratedMinMax(int[] input, out int min, out int max) {
            var simdLength = Vector<int>.Count;
            var vmin = new Vector<int>(int.MaxValue);
            var vmax = new Vector<int>(int.MinValue);
            var i = 0;
            var lastSafeVectorIndex = input.Length - simdLength;
            for (i = 0; i < lastSafeVectorIndex; i += simdLength) {
                var va = new Vector<int>(input, i);
                vmin = Vector.Min(va, vmin);
                vmax = Vector.Max(va, vmax);
            }
            min = int.MaxValue;
            max = int.MinValue;
            for (var j = 0; j < simdLength; ++j) {
                min = Math.Min(min, vmin[j]);
                max = Math.Max(max, vmax[j]);
            }
            for (; i < input.Length; ++i) {
                min = Math.Min(min, input[i]);
                max = Math.Max(max, input[i]);
            }
        }

        public static int[] NaiveSumFunc(int[] lhs, int[] rhs) {
            var length = lhs.Length;
            var result = new int[length];
            for (var i = 0; i < length; ++i) {
                result[i] = lhs[i] + rhs[i];
            }
            return result;
        }

        public static int[] HWAcceleratedSumFunc(int[] lhs, int[] rhs) {
            var simdLength = Vector<int>.Count;
            var result = new int[lhs.Length];
            var i = 0;
            for (i = 0; i < lhs.Length - simdLength; i += simdLength) {
                var va = new Vector<int>(lhs, i);
                var vb = new Vector<int>(rhs, i);
                (va + vb).CopyTo(result, i);
            }
            for (; i < lhs.Length; ++i) {
                result[i] = lhs[i] + rhs[i];
            }

            return result;
        }

        public static void NaiveSumInPlace(int[] lhs, int[] rhs) {
            var length = lhs.Length;
            for (var i = 0; i < length; ++i) {
                lhs[i] += rhs[i];
            }
        }

        public static void HwAcceleratedSumInPlace(int[] lhs, int[] rhs) {
            var simdLength = Vector<int>.Count;
            var i = 0;
            for (i = 0; i < lhs.Length - simdLength; i += simdLength) {
                var va = new Vector<int>(lhs, i);
                var vb = new Vector<int>(rhs, i);
                va += vb;
                va.CopyTo(lhs, i);
            }
            for (; i < lhs.Length; ++i) {
                lhs[i] += rhs[i];
            }
        }
    }
}