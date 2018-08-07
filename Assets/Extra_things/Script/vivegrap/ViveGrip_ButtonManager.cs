using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// 按钮管理
/// </summary>
public class ViveGrip_ButtonManager : MonoBehaviour {
  public enum ViveInput {
    Grip,
    Trigger
  }; // TODO: add more buttons
	//待办:添加更多按钮
  [Tooltip("输入的设备The device that will be giving the input.")]
  public SteamVR_TrackedObject trackedObject;
  [Tooltip("用来抓取的按钮The button used for gripping.")]
  public ViveInput grab = ViveInput.Grip;
  [Tooltip("用来互动的按钮The button used for interacting.")]
  public ViveInput interact = ViveInput.Trigger;

  void Start() {}

	/// <summary>
	/// 是否按下
	/// </summary>
	/// <param name="action">Action.动作名称</param>
  public bool Pressed(string action) {
    ulong rawInput = ConvertString(action);
    return Device().GetTouchDown(rawInput);
  }

	/// <summary>
	/// 是否放开
	/// </summary>
	/// <param name="action">Action.</param>
  public bool Released(string action) {
    ulong rawInput = ConvertString(action);
    return Device().GetTouchUp(rawInput);
  }

	/// <summary>
	/// 是否握住
	/// </summary>
	/// <param name="action">Action.</param>
  public bool Holding(string action) {
    ulong rawInput = ConvertString(action);
    return Device().GetTouch(rawInput);
  }

	/// <summary>
	/// 转换字符串
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="action">Action.</param>
  ulong ConvertString(string action) {
    ViveInput input = GetInputFor(action);
    return Decode(input);
  }

	/// <summary>
	/// 获取设备输入
	/// </summary>
  SteamVR_Controller.Device Device() {
    return SteamVR_Controller.Input((int)trackedObject.index);
  }

	/// <summary>
	/// 获取输入的动作
	/// </summary>
	/// <returns>The input for.</returns>
	/// <param name="action">Action.</param>
  ViveInput GetInputFor(string action) {
    switch (action.ToLower()) {
      default:
      case "grab":
        return grab;
      case "interact":
        return interact;
    }
  }

	/// <summary>
	/// 解码,把字符串动作转换成对应的index索引
	/// </summary>
	/// <param name="input">Input.</param>
  ulong Decode(ViveInput input) {
    switch ((int)input) {
      default:
      case 0:
        return SteamVR_Controller.ButtonMask.Grip;
      case 1:
        return SteamVR_Controller.ButtonMask.Trigger;
    }
  }
}
