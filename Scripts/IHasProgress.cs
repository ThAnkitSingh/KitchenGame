using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress
{
    public event EventHandler<OnProgressChangeEvenArgs> OnProgressChange;
    public class OnProgressChangeEvenArgs : EventArgs
    {
        public float progressNormalized;
    }
}
