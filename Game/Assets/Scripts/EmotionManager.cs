using UnityEngine;
using UnityEngine.Animations;
namespace Game
{
    public class EmotionManager : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimatorOverrideController _overrideController;
        [SerializeField] private AnimationClip _newIdleClip;

        void Start()
        {
            AnimatorOverrideController overrideController =
                new AnimatorOverrideController(_animator.runtimeAnimatorController);

            overrideController["idleAnimation"] = _newIdleClip;

            _animator.runtimeAnimatorController = overrideController;
        }
    }
}
