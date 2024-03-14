using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.MonoManager
{
    public class MonoController : MonoBehaviour
    {
        // 生命周期 Update 函数监听
        private event UnityAction updateEvent;

        // 生命周期 FixedUpdate 函数监听
        private event UnityAction fixedUpdateEvent;

        // 生命周期 LateUpdate 函数监听
        private event UnityAction lateUpdateEvent;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke();
        }

        private void Update()
        {
            updateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }

        public void AddUpdateListener(UnityAction action)
        {
            updateEvent += action;
        }

        public void RemoveUpdateListener(UnityAction action)
        {
            updateEvent -= action;
        }

        public void AddFixedUpdateListener(UnityAction action)
        {
            fixedUpdateEvent += action;
        }

        public void RemoveFixedUpdateListener(UnityAction action)
        {
            fixedUpdateEvent -= action;
        }

        public void AddLateUpdateListener(UnityAction action)
        {
            lateUpdateEvent += action;
        }

        public void RemoveLateUpdateListener(UnityAction action)
        {
            lateUpdateEvent -= action;
        }
    }
}