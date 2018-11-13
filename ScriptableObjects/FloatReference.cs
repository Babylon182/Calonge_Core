using System;
using UnityEngine;

[Serializable]
public class FloatReference  {

    public bool useConstant = true;
    public float constantValue;
    public FloatVariable variable;

    public float Value
    {
        get
        {
            return useConstant ? constantValue : variable.value;
        }
    }
}
