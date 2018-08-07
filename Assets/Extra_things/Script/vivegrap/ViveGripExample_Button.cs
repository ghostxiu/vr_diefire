using UnityEngine;
using System.Collections;

public class ViveGripExample_Button : MonoBehaviour {
  private const float SPEED = 0.1f;
  private float distance;
  private int direction = 1;

  void Start () {
    ResetDistance();
  }
    //触发抓取互动后移除组件

  void OnViveGripInteraction() {
    Destroy(GetComponent<ViveGrip_Interactable>());
    StartCoroutine("Move");
  }
    //移动

  IEnumerator Move() {
    while (distance > 0) {
      Increment();
      yield return null;
    }
    yield return StartCoroutine("MoveBack");
  }
    //后移 重置后重新添加互动组件

  IEnumerator MoveBack() {
    direction *= -1;
    ResetDistance();
    while (distance > 0) {
      Increment();
      yield return null;
    }
    direction *= -1;
    ResetDistance();
    gameObject.AddComponent<ViveGrip_Interactable>();
  }
    //增加距离

  void Increment() {
    float increment = Time.deltaTime * SPEED;
    increment = Mathf.Min(increment, distance);
    transform.Translate(0, 0, increment * direction);
    distance -= increment;
  }
    //重置距离

  void ResetDistance() {
    distance = 0.03f;
  }
}
