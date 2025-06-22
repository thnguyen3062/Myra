using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIKCore.Utilities
{
    public class ICallback
    {
        /// <summary>() => {...}</summary>
        public delegate void CallFunc();
        /// <summary>(T) => {...}</summary>
        public delegate void CallFunc2<T>(T t);
        /// <summary>(T1, T2) => {...}</summary>
        public delegate void CallFunc3<T1, T2>(T1 t1, T2 t2);
        /// <summary>(T1, T2, T3) => {...}</summary>
        public delegate void CallFunc4<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
        /// <summary>() => {...return T;}</summary>
        public delegate T CallFunc5<T>();
        /// <summary>(T2) => {...return T1}</summary>
        public delegate T1 CallFunc6<T1, T2>(T2 t2);
        /// <summary>(T2, T3) => {...return T1}</summary>
        public delegate T1 CallFunc7<T1, T2, T3>(T2 t2, T3 t3);
        /// <summary>(T1, T2, T3, T4) => {...}</summary>
        public delegate void CallFunc8<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
        public delegate void CallFunc9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 t1, T2 t2, T3 t3, T4 t4,T5 t5,T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12);

        public delegate void CallFunc10<T1, T2, T3, T4, T5, T6>(T1 t1,T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    }
}
