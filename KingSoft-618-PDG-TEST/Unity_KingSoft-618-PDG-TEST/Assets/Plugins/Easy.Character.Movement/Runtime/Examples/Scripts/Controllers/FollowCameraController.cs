using UnityEngine;

namespace ECM.Examples
{
    public sealed class FollowCameraController : MonoBehaviour
    {
        #region PUBLIC FIELDS

        //摄像机朝向的目标模型
        public Transform target;
        //摄像机与模型保持的距离
        public float distance = 10.0f;
        //射线机与模型保持的高度
        public float height = 5.0f;
        //旋转阻尼
        public float rotationDamping = 5.0f;
        //主角对象
        // public GameObject controller;
        public float lookHeight = 1.5f;

        private float xVelocity = 5f;
        private float yVelocity = 5f;

        float wantedRotationAngle;
        float wantedRotationAngleX;
        float targetDistance;
        void Start()
        {
            wantedRotationAngle = transform.eulerAngles.y;
            wantedRotationAngleX = transform.eulerAngles.x;
            targetDistance = distance;
        }

        void Update()
        {

        }
        void LateUpdate()
        {
            // Early out if we don't have a target
            if (!target)
                return;

            //计算相机与主角Y轴旋转角度的差。
            //float abs = Mathf.Abs(transform.rotation.eulerAngles.y - target.transform.rotation.eulerAngles.y);
            //if (abs > 130 && abs < 230) {
            //    follow = false;
            //}
            //else {
            //    follow = true;
            //}

            Vector3 lookPoint = target.transform.position + new Vector3(0, lookHeight, 0);

            float yMouse = Input.GetAxis("Mouse X");
            float xMouse = Input.GetAxis("Mouse Y");

            // UpdateTouch(ref xMouse, ref yMouse);

            if (Mathf.Abs(yMouse) > Mathf.Abs(xMouse))
                xMouse = 0;
            else
                yMouse = 0;


            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                targetDistance += 0.8f;
            }
            //Zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                targetDistance -= 0.8f;
                if (targetDistance < 2) targetDistance = 2;
            }

            distance = Mathf.Lerp(distance, targetDistance, 6 * Time.deltaTime);

            RaycastHit hitInfo;

            var hit = Physics.Raycast(lookPoint - transform.forward, transform.position - new Vector3(0, 0.2f, 0) - lookPoint, out hitInfo, distance + 4, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
            var dis = distance;
            if (hit)
            {
                if (hitInfo.distance < 0.8f) hitInfo.distance = 0.8f;
                if (hitInfo.distance - 0.5f <= dis)
                {
                    dis = hitInfo.distance - 0.5f;
                }
            }


            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                wantedRotationAngle += yMouse * 5;
                wantedRotationAngleX -= xMouse * 5;
            }

            //float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            //float currentHeight = transform.position.y;

            float currentRotationAngleX = transform.eulerAngles.x;
            currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle, ref yVelocity, rotationDamping * Time.deltaTime);
            currentRotationAngleX = Mathf.SmoothDampAngle(currentRotationAngleX, wantedRotationAngleX, ref xVelocity, rotationDamping * Time.deltaTime);

            //currentRotationAngleX = ClampAngle(currentRotationAngleX, -20, 360);
            //if (currentRotationAngleX >= 90) currentRotationAngleX = 89.99f;
            //if (currentRotationAngleX <= 0) {
            //    float x = 0;
            //}
            if (currentRotationAngleX >= 90 && currentRotationAngleX < 270)
            {
                currentRotationAngleX = 90;
            }
            if (currentRotationAngleX >= 270 && currentRotationAngleX <= 300)
            {
                currentRotationAngleX = 300;
            }

            //currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
            //currentRotationAngleX = ClampAngle(currentRotationAngleX, -40, 90);
            Quaternion currentRotation = Quaternion.Euler(currentRotationAngleX, currentRotationAngle, 0);
            Vector3 positon = lookPoint;

            positon += currentRotation * new Vector3(0.0f, 0.0f, -dis);
            //positon = new Vector3(positon.x, currentHeight, positon.z);
            transform.position = Vector3.Lerp(transform.position, positon, 0.4f * Time.time);
            //transform.rotation = currentRotation;
            var x = transform.localEulerAngles.x;
            x = ClampAngle(transform.localEulerAngles.x, -40, 89.99f);
            transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);

            Vector3 aim = lookPoint;
            aim.y += 0.5f;
            Vector3 ve = (lookPoint - transform.position).normalized;
            float an = transform.eulerAngles.y;
            aim -= an * ve;
            Debug.DrawLine(lookPoint, aim, Color.red);

            transform.forward = lookPoint - transform.position;
            //transform.LookAt(target);
        }
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        #endregion
    }
}