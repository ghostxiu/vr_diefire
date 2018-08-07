using UnityEngine;
/// <summary>
/// 泡泡枪
/// </summary>
public class ViveGripExample_Bubbler : MonoBehaviour {
    //泡泡
  public GameObject bubble;
    //最大尺寸
	private float maxSize = 0.2f;
    //最小尺寸
  private float minSize = 0.1f;
    //速度
  private float speed = 5f;
    //冷却
  private float cooldown = 0f;

  void Start() {}

  void Update() {
    if (cooldown > 0) {
      cooldown -= Time.deltaTime;
    }
  }
    /// <summary>
    /// 泡泡枪如果在手里则发射泡泡
    /// </summary>
    /// <param name="grabbed">If set to<c>true</c>grabbed.</param>

  void OnViveGripInteractionHeld(bool grabbed) {
    if (!grabbed) { return; }
    if (cooldown <= 0) {
      Vector3 location = transform.position + (transform.forward*0.2f) + (transform.up*0.1f);
      GameObject instance = (GameObject)Instantiate(bubble, location, Quaternion.identity);
      float size = Random.Range(minSize, maxSize);
      instance.transform.localScale = Vector3.one * size;
      instance.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
      cooldown = 0.1f;
    }
  }
}
