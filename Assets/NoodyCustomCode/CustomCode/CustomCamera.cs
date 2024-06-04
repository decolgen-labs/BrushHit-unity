using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOOD.NoodCamera 
{ 
    public class CustomCamera : MonoBehaviorInstance<CustomCamera>
    {
        #region Components
        #endregion

        #region Stats
        [SerializeField] float duration = 0.2f, magnitude = 0.02f;
        [SerializeField] float explodeMagnitude = 0.1f;
        [SerializeField] float smoothTime = 2;
        [SerializeField] string targetTag = "Player";
        Transform targetTransform;

        [SerializeField] Vector3 offset;

        [SerializeField] bool isFollow;
        [SerializeField] bool isShake;
        [SerializeField] bool isHeavyShake;
        #endregion

        public static CustomCamera InsCustomCamera;

        protected override void ChildAwake()
        {
            if(InsCustomCamera == null) InsCustomCamera = this;
        }

        private void Update()
        {
            if (isShake) Shake();
            if (isHeavyShake) HeaveShake();
            if(Input.GetKeyDown(KeyCode.U))
            {
                Shake();
	        }
        }

        private void LateUpdate()
        {
            if (isFollow) FollowPlayer();
        }

        void FollowPlayer()
        {
            if (!targetTransform && GameObject.FindGameObjectWithTag(targetTag)) targetTransform = GameObject.FindGameObjectWithTag(targetTag).transform;
            if(targetTransform)
                NOOD.NoodyCustomCode.LerpSmoothCameraFollow(Camera.main.gameObject, smoothTime, targetTransform, offset);
            //this.transform.LookAt(targetTransform);
        }
    
        public void Shake(){
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, magnitude));
            isShake = false;
        }

        public void HeaveShake(){
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, explodeMagnitude));
            isHeavyShake = false;
        }
    }

}
