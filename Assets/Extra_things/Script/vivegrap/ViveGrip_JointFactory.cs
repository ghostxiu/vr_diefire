using UnityEngine;
//关节工厂
public static class ViveGrip_JointFactory {
  
    /// <summary>
    /// 连接关节
    /// </summary>
    /// <param name="jointObject">可配置关节</param>
    /// <param name="desiredObject">关节对象</param>
    /// <param name="offset">偏移</param>
    /// <param name="desiredRotation">想要旋转的角度</param>
    /// <returns>可配置关节</returns>
  public static ConfigurableJoint JointToConnect(GameObject jointObject, Rigidbody desiredObject, Vector3 offset, Quaternion desiredRotation) {
    ViveGrip_Grabbable grabbable = desiredObject.gameObject.GetComponent<ViveGrip_Grabbable>();
    ConfigurableJoint joint = jointObject.AddComponent<ConfigurableJoint>();
    ViveGrip_JointFactory.SetLinearDrive(joint, desiredObject.mass);
    ViveGrip_JointFactory.ConfigureAnchor(joint, offset, grabbable.applyGripRotation);
    if (grabbable.applyGripRotation) {
      ViveGrip_JointFactory.SetAngularDrive(joint, desiredObject.mass);
    }
    joint.targetRotation = desiredRotation;
    joint.connectedBody = desiredObject;
    return joint;
  }
    /// <summary>
    /// 配置锚点
    /// </summary>
    /// <param name="joint">Joint.关节</param>
    /// <param name="offset">Offset.偏移</param>
    /// <param name="applyGripRotation">If set to <c> ture</c> apply grip rotation. 应用抓取偏移</param>

  private static void ConfigureAnchor(ConfigurableJoint joint, Vector3 offset, bool applyGripRotation) {
    if (applyGripRotation) { // TODO: Why is this important when we rotate? We pass in a local offset...
            //待办：为什么当我们旋转的时候这个是最重要的？我们传递进去一个本地的偏移...
      joint.autoConfigureConnectedAnchor = false;
      joint.connectedAnchor = offset;
    }
    else {
      joint.anchor = offset;
    }
  }
/// <summary>
/// 设置线性驱动
/// </summary>
/// <param name="joint">Joint.关节</param>
/// <param name="mass">Mass.质量</param>
  private static void SetLinearDrive(ConfigurableJoint joint, float mass) {
        //抓取力量是质量的3000倍
    float gripStrength = 3000f * mass;
        //抓取速度是质量的10倍
    float gripSpeed = 10f * mass;
        //最大能量是质量的70倍
    float maxPower = 70f * mass;
        //x轴方向驱动
    JointDrive jointDrive = joint.xDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    jointDrive.maximumForce = maxPower;
    joint.xDrive = jointDrive;
        //y轴方向驱动
    jointDrive = joint.yDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    jointDrive.maximumForce = maxPower;
    joint.yDrive = jointDrive;
        //z轴方向驱动
    jointDrive = joint.zDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    jointDrive.maximumForce = maxPower;
    joint.zDrive = jointDrive;
  }
    /// <summary>
    /// 设置角度驱动
    /// </summary>
    /// <param name="joint">Joint.关节</param>
    /// <param name="mass">Mass.质量</param>

  private static void SetAngularDrive(ConfigurableJoint joint, float mass) {
    float gripStrength = 30f * mass;
    float gripSpeed = 1f * mass;
    joint.rotationDriveMode = RotationDriveMode.XYAndZ;
    JointDrive jointDrive = joint.angularYZDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    joint.angularYZDrive = jointDrive;
    jointDrive = joint.angularXDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    joint.angularXDrive = jointDrive;
  }
}
