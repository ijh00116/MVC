using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;

namespace BlackTree.Bundles
{
    public class ViewUnit : Poolable
    {
        [HideInInspector]public int hash;
        [SerializeField] public Animator animator;
        public Transform raycastTr;

        [SerializeField] AnimatorEvent animEventer;

        public SpriteRenderer weapon_L;
        public SpriteRenderer weapon_R;

        public SpriteRenderer leftShoulder;
        public SpriteRenderer rightShoulder;
        public SpriteRenderer bodysprite;

        public SpriteRenderer helmetsprite;

        public Transform skillPos;

        public Transform BuffTransform;
        public Transform PetTransform;
        ControllerUnitInGame _unit;

        public LayerMask targetLayer;
        public void Init(ControllerUnitInGame unit)
        {
            _unit = unit;
            hash = GetHashCode();
            animEventer.SetAttackCallback(AnimAttackEvent);
            animEventer.SetEndCallback(AnimEndEvent);
        }

        void AnimAttackEvent()
        {
            Model.Player.Unit.CallbackAtAttack?.Invoke();
        }
        void AnimEndEvent()
        {
            Model.Player.Unit.CallbackAtAnimEnd?.Invoke();
        }
      
    }
}
