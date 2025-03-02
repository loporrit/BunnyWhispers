using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
    internal struct FieldElement
    {
        internal int x0;
        internal int x1;
        internal int x2;
        internal int x3;
        internal int x4;
        internal int x5;
        internal int x6;
        internal int x7;
        internal int x8;
        internal int x9;

        //public static readonly FieldElement Zero = new FieldElement();
        //public static readonly FieldElement One = new FieldElement() { x0 = 1 };

        internal FieldElement(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9)
        {
            x0 = a0;
            x1 = a1;
            x2 = a2;
            x3 = a3;
            x4 = a4;
            x5 = a5;
            x6 = a6;
            x7 = a7;
            x8 = a8;
            x9 = a9;
        }
    }
}
