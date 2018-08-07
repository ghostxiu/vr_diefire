using UnityEngine;
using System.Collections;


//扳机触发事件
public class ConfirmController : MonoBehavior{
    //手柄
    SteamVR_TrackedObject trackedObj;

    void Awake()
    {
        //获取手柄脚本组件
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    void Start()
    {

    }
    void FixedUpdate() {
        //获取手柄输入
        var device = SteamVR_controller.Input((int)trackedObj.index);


        //此处可以换成其它的函数触发GetPressDown/GetPressUp GetTouchDown/GetTouchUp/GetAxis

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            //Do Something
        }
    }
}