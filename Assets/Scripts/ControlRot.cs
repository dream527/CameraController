using UnityEngine;

public class ControlRot : MonoBehaviour
{
    //相机目标点
    public Transform target; 
    public LockAix mLockAix;
    public float MinDist = 0.3f;
    public float MaxDist = 10.0f; 
    public float distance = 10.0f;
    public float xSpeed = 0.5f;
    public float ySpeed = 0.5f;
    public float TargetMove = 1f;
    public float yMinLimit = -20;
    public float yMaxLimit = 80;


    void Start()
    {
        a = transform.eulerAngles;
    }

    public bool isInit;
    public void Init()
    {
        if (!isInit)
        {
            isInit = true;
        }

        transform.position = target.position - transform.forward * distance;
    }

    public bool isRotInit = true;
    bool isState;



    void LateUpdate()
    {
        //UpdateMove();
        UpdataRot();
        transform.position = target.position - transform.forward * distance;
    }

    Vector3 a;
    bool downMouse;
    public float scrollSpeed = 5;
    void UpdataRot()
    {
        if (!isRotInit)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            downMouse = true;
            a = transform.eulerAngles;
        }
        if (Input.GetMouseButtonUp(0))
        {
            downMouse = false;
        }
        if (Input.GetMouseButton(0) && downMouse)
        {
            a.y += Input.GetAxis("Mouse X") * xSpeed;
            a.x -= Input.GetAxis("Mouse Y") * ySpeed;

            a.x = Mathf.Clamp(TiaoZhengJiaoDu(a.x), yMinLimit, yMaxLimit);


        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(a.x, a.y, 0),
                 5 * Time.deltaTime);
        target.transform.rotation = Quaternion.Lerp(target.transform.rotation, Quaternion.Euler(a.x, a.y, 0),
                5 * Time.deltaTime);

        distance1 = distance - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed / 2;
        distance1 = Mathf.Clamp(distance1, MinDist, MaxDist);

        distance = Mathf.Lerp(distance, distance1, Time.deltaTime * suoFangSpeed);


    }

   float distance1;
   public  float suoFangSpeed;
    float TiaoZhengJiaoDu(float angle)
    {
        if(angle>180)
        {
            return angle - 360;
        }
        if (angle < -180)
        {
            return angle + 360;
        }
            
        return angle;
    }

    public enum LockAix {x,y,z ,Null}
    /// <summary>    /// 锁死最大值    /// </summary>
    public float MaxLockAix = Mathf.Infinity;
    /// <summary>    /// 锁死的最小值    /// </summary>
    public float MinLockAix = -Mathf.Infinity;
    /// <summary>    /// 是否锁住滚轮位移    /// </summary>
    public bool isLockMouseButton = true;
    void UpdateMove()
    {
        if (!isLockMouseButton)
            return;
        if (Input.GetMouseButton(1))
        {
            Vector3 pos=target.localPosition;
            target.Translate(-Input.GetAxis("Mouse X") * TargetMove * 0.1f,
                  -Input.GetAxis("Mouse Y") * TargetMove * 0.1f,
                0, Space.Self);
            switch (mLockAix)
            {
                case LockAix.x:
                    target.localPosition = new Vector3(Mathf.Clamp(target.localPosition.x,MinLockAix,MaxLockAix), pos.y, pos.z);
                    break;
                case LockAix.y:
                    target.localPosition = new Vector3(pos.x, Mathf.Clamp(target.localPosition.y, MinLockAix, MaxLockAix), pos.z);
                    break;
                case LockAix.z:
                    target.localPosition = new Vector3(pos.x, pos.y, Mathf.Clamp(target.localPosition.z, MinLockAix, MaxLockAix));
                    break;
                case LockAix.Null:
                    
                    break;
                default:
                    break;
            }
        }
    }
}
















