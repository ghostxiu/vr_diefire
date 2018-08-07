using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// 抓取点
/// </summary>
public class ViveGrip_GripPoint : MonoBehaviour {
  [Tooltip("你可以抓取物体的距离 The distance at which you can touch objects.")]
  public float touchRadius = 0.2f;
  [Tooltip("自动掉落的距离 The distance at which objects will automatically drop.")]
  public float holdRadius = 0.3f;
  [Tooltip("触摸的半径是否可视化?Is the touch radius visible? (Good for debugging)")]
  public bool visible = false;
  [Tooltip("是否需要抓取开关?Should the button toggle grabbing?")]
  public bool inputIsToggle = false;
	//高亮颜色
  private Color highlightTint = new Color(0.2f, 0.2f, 0.2f);
	//管理器
  private ViveGrip_ButtonManager button;
	//碰撞检测
  private ViveGrip_TouchDetection touch;
	//可配置关节
  private ConfigurableJoint joint;
	//关节对象
  private GameObject jointObject;
	//是否有锚点
  private bool anchored = false;
	//上一次触摸的对象
  private GameObject lastTouchedObject;

  void Start() {
		//获取组件
    	button = GetComponent<ViveGrip_ButtonManager>();
		//初始化触摸圆
   	 	GameObject gripSphere = InstantiateTouchSphere();
		//添加触摸检测组件
    	touch = gripSphere.AddComponent<ViveGrip_TouchDetection>();
    	touch.radius = touchRadius;
	}

  void Update() {
		//获取最近的对象
    	GameObject touchedObject = touch.NearestObject();
		//处理高亮
   	 	HandleHighlighting(touchedObject);
		//处理抓取
    	HandleGrabbing(touchedObject);
		//处理互动
    	HandleInteraction(touchedObject);
		//处理摸索
    	HandleFumbling();
		//保留触摸对象
    	lastTouchedObject = touchedObject;
  }

	/// <summary>
	/// 处理抓取
	/// </summary>
	/// <param name="touchedObject">Touched object.</param>
  void HandleGrabbing(GameObject touchedObject) {
		//没有抓到则返回
    	if (!GrabTriggered()) { return; }
		//如果已经握住什么东西了则摧毁连结
    	if (SomethingHeld()) {
      		DestroyConnection();
			//抓取对象非空,且有可抓取组件
    	}else if (touchedObject != null && touchedObject.GetComponent<ViveGrip_Grabbable>() != null) {
			//获取高亮的对象并移除高亮
      		GetHighlight(touchedObject).RemoveHighlighting();
			//添加连结
      		CreateConnectionTo(touchedObject.GetComponent<Rigidbody>());
   		}
  }

	/// <summary>
	/// 是否触发抓取,这里的Grab对应手柄输入的grip
	/// </summary>
	/// <returns><c>true</c>, if triggered was grabed, <c>false</c> otherwise.</returns>
  bool GrabTriggered() {
    if (button == null) { return false; }
    if (inputIsToggle) {
      return button.Pressed("grab");
    }
    return button.Pressed("grab") || button.Released("grab");
  }

	/// <summary>
	/// 处理互动
	/// </summary>
	/// <param name="touchedObject">Touched object.</param>
  void HandleInteraction(GameObject touchedObject) {
    if (touchedObject == null) { return; }
    if (SomethingHeld()) {
      touchedObject = joint.connectedBody.gameObject;
    }
    if (touchedObject.GetComponent<ViveGrip_Interactable>() == null) { return; }
    if (button.Pressed("interact")) {
			//这个方法在ViveGripExample_Button中调用
      touchedObject.SendMessage("OnViveGripInteraction", SomethingHeld(), SendMessageOptions.DontRequireReceiver);
    }
    if (button.Holding("interact")) {
      touchedObject.SendMessage("OnViveGripInteractionHeld", SomethingHeld(), SendMessageOptions.DontRequireReceiver);
    }
  }

	/// <summary>
	/// 高亮处理
	/// </summary>
	/// <param name="touchedObject">Touched object.</param>
  void HandleHighlighting(GameObject touchedObject) {
    ViveGrip_Highlight last = GetHighlight(lastTouchedObject);
    ViveGrip_Highlight current = GetHighlight(touchedObject);
    if (last != null && last != current) {
      last.RemoveHighlighting();
    }
    if (current != null && !SomethingHeld()) {
      current.Highlight(highlightTint);
    }
  }

	/// <summary>
	/// 获取高亮
	/// </summary>
	/// <returns>The highlight.</returns>
	/// <param name="touchedObject">Touched object.</param>
  ViveGrip_Highlight GetHighlight(GameObject touchedObject) {
    if (touchedObject == null) { return null; }
    return touchedObject.GetComponent<ViveGrip_Highlight>();
  }

	/// <summary>
	/// 处理摸索
	/// </summary>
  void HandleFumbling() {
    if (SomethingHeld()) {
			//获取锚点的世界坐标
      Vector3 grabbableAnchorPosition = AnchorWorldPositionOf(joint.connectedBody.gameObject);
			//获取抓取的距离
      float grabDistance = Vector3.Distance(transform.position, grabbableAnchorPosition);
			//是否在可握住的半径内
      bool pulledToMiddle = grabDistance < holdRadius;
			//或判断
      anchored = anchored || pulledToMiddle;
      if (anchored && grabDistance > holdRadius) {
				//有锚点 且 抓取的距离超出握住距离则摧毁连结
        DestroyConnection();
      }
    }
  }

	/// <summary>
	/// 创建连接
	/// </summary>
	/// <param name="desiredBody">Desired body.</param>
  void CreateConnectionTo(Rigidbody desiredBody) {
		//实例化关节
    jointObject = InstantiateJointParent();
		//获取朝向 偏移 
    Quaternion desiredRotation = OrientationChangeFor(desiredBody.gameObject);
    Vector3 offset = desiredBody.gameObject.GetComponent<ViveGrip_Grabbable>().anchor;
		//通过关节组件连结起来
    joint = ViveGrip_JointFactory.JointToConnect(jointObject, desiredBody, offset, desiredRotation);
  }

	/// <summary>
	/// 改变朝向
	/// </summary>
	/// <returns>The change for.</returns>
	/// <param name="target">Target.</param>
  Quaternion OrientationChangeFor(GameObject target) {
		//获取可抓取组件
    ViveGrip_Grabbable grabbable = target.GetComponent<ViveGrip_Grabbable>();
    if (grabbable.snapToOrientation) {
		// Undo current rotation, apply the orientation, and translate that to controller space
		// ...but in reverse order because thats how Quaternions work
		//撤消当前的旋转,应用朝向,并转换到控制器空间
		//...但是以倒置的顺序,因为Quaternions就是这样工作的
      Quaternion localToController = transform.rotation;
      localToController *= Quaternion.Euler(grabbable.localOrientation);
      return localToController * Quaternion.Inverse(target.transform.rotation);
    }
    return Quaternion.identity;
  }

	/// <summary>
	/// 摧毁连结
	/// </summary>
  void DestroyConnection() {
		//摧毁关节
    Destroy(jointObject);
    anchored = false;
  }

	/// <summary>
	/// 锚点的世界坐标
	/// </summary>
	/// <returns>The world position of.</returns>
	/// <param name="grabbableObject">Grabbable object.可抓取的对象</param>
  Vector3 AnchorWorldPositionOf(GameObject grabbableObject) {
		//锚点即可抓取对象的锚点
    Vector3 anchor = grabbableObject.GetComponent<ViveGrip_Grabbable>().anchor;
    Transform grabbableTransform = grabbableObject.transform;
    return grabbableTransform.position + grabbableTransform.TransformVector(anchor);
  }

	/// <summary>
	/// 实例化关节父类
	/// </summary>
	/// <returns>The joint parent.</returns>
  GameObject InstantiateJointParent() {
		//实例化关节
    GameObject newJointObject = new GameObject("ViveGrip Joint");
		//把当前变换赋值为其父类 初始化位置/大小/旋转
    newJointObject.transform.parent = transform;
    newJointObject.transform.localPosition = Vector3.zero;
    newJointObject.transform.localScale = Vector3.one;
    newJointObject.transform.rotation = Quaternion.identity;
		//添加刚体 
    Rigidbody jointRigidbody = newJointObject.AddComponent<Rigidbody>();
    jointRigidbody.useGravity = false;
    jointRigidbody.isKinematic = true;
    return newJointObject;
  }

	/// <summary>
	/// 实例化触摸圆
	/// </summary>
	/// <returns>The touch sphere.</returns>
  GameObject InstantiateTouchSphere() {
		//创建原始球 获取渲染  是否可视化
    GameObject gripSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    Renderer sphereRenderer = gripSphere.GetComponent<Renderer>();
    sphereRenderer.enabled = visible;
    if (visible) {
      sphereRenderer.material = new Material(Shader.Find("ViveGrip/TouchSphere"));
      sphereRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      sphereRenderer.receiveShadows = false;
    }
    gripSphere.transform.localScale = Vector3.one * touchRadius;
    gripSphere.transform.position = transform.position;
    gripSphere.transform.SetParent(transform);
    gripSphere.AddComponent<Rigidbody>().isKinematic = true;
    gripSphere.name = "ViveGrip Touch Sphere";
    return gripSphere;
  }

	/// <summary>
	/// 握住什么东西
	/// </summary>
	/// <returns><c>true</c>, if held was somethinged, <c>false</c> otherwise.</returns>
  bool SomethingHeld() {
    return jointObject != null;
  }
}
