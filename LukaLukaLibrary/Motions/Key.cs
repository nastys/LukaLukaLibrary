﻿namespace LukaLukaLibrary.Motions
{
    public class Key
    {
        public float Value { get; set; }
        public float Interpolation { get; set; }
        public int Frame { get; set; }

        public override string ToString() =>
            $"{Frame}, {Value}, {Interpolation}";
    }
}