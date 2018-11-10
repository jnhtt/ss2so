using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master
{
    [Serializable]
    public abstract class MasterElement<TKey>
    {
        public TKey id;

        public MasterElement(TKey id)
        {
            this.id = id;
        }
    }
}
