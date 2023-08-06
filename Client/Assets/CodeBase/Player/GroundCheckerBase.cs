using System;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public abstract class GroundCheckerBase : MonoBehaviour
    {
        public abstract bool IsGrounded { get; protected set; }
    }
}
