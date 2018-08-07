using UnityEngine;
//�ؽڹ���
public static class ViveGrip_JointFactory {
  
    /// <summary>
    /// ���ӹؽ�
    /// </summary>
    /// <param name="jointObject">�����ùؽ�</param>
    /// <param name="desiredObject">�ؽڶ���</param>
    /// <param name="offset">ƫ��</param>
    /// <param name="desiredRotation">��Ҫ��ת�ĽǶ�</param>
    /// <returns>�����ùؽ�</returns>
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
    /// ����ê��
    /// </summary>
    /// <param name="joint">Joint.�ؽ�</param>
    /// <param name="offset">Offset.ƫ��</param>
    /// <param name="applyGripRotation">If set to <c> ture</c> apply grip rotation. Ӧ��ץȡƫ��</param>

  private static void ConfigureAnchor(ConfigurableJoint joint, Vector3 offset, bool applyGripRotation) {
    if (applyGripRotation) { // TODO: Why is this important when we rotate? We pass in a local offset...
            //���죺Ϊʲô��������ת��ʱ�����������Ҫ�ģ����Ǵ��ݽ�ȥһ�����ص�ƫ��...
      joint.autoConfigureConnectedAnchor = false;
      joint.connectedAnchor = offset;
    }
    else {
      joint.anchor = offset;
    }
  }
/// <summary>
/// ������������
/// </summary>
/// <param name="joint">Joint.�ؽ�</param>
/// <param name="mass">Mass.����</param>
  private static void SetLinearDrive(ConfigurableJoint joint, float mass) {
        //ץȡ������������3000��
    float gripStrength = 3000f * mass;
        //ץȡ�ٶ���������10��
    float gripSpeed = 10f * mass;
        //���������������70��
    float maxPower = 70f * mass;
        //x�᷽������
    JointDrive jointDrive = joint.xDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    jointDrive.maximumForce = maxPower;
    joint.xDrive = jointDrive;
        //y�᷽������
    jointDrive = joint.yDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    jointDrive.maximumForce = maxPower;
    joint.yDrive = jointDrive;
        //z�᷽������
    jointDrive = joint.zDrive;
    jointDrive.positionSpring = gripStrength;
    jointDrive.positionDamper = gripSpeed;
    jointDrive.maximumForce = maxPower;
    joint.zDrive = jointDrive;
  }
    /// <summary>
    /// ���ýǶ�����
    /// </summary>
    /// <param name="joint">Joint.�ؽ�</param>
    /// <param name="mass">Mass.����</param>

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
