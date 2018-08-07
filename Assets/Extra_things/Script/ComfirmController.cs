using UnityEngine;
using System.Collections;


//��������¼�
public class ConfirmController : MonoBehavior{
    //�ֱ�
    SteamVR_TrackedObject trackedObj;

    void Awake()
    {
        //��ȡ�ֱ��ű����
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    void Start()
    {

    }
    void FixedUpdate() {
        //��ȡ�ֱ�����
        var device = SteamVR_controller.Input((int)trackedObj.index);


        //�˴����Ի��������ĺ�������GetPressDown/GetPressUp GetTouchDown/GetTouchUp/GetAxis

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            //Do Something
        }
    }
}