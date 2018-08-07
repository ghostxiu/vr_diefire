using UnityEngine;
using System.Collections;
//用射线的方式熄灭火焰的脚本
public class RayHitFire : MonoBehaviour
{
    public GameObject wParticle;//灭火器粒子
    private LayerMask mask;//layer网格
    Ray ray;//定义射线
    RaycastHit hitInfo;//定义反馈信息类
    void Start()
    {
        mask = 1 << (LayerMask.NameToLayer("Fire"));//指定layer网格为Fire，并返回该名字所定义的层的层索引
    }
    void Update()
    {
        Debug.Log("Update is running! " );
        ray = new Ray(transform.position, transform.forward);
        Debug.Log("Ray success evaluate! ");
        Debug.Log("hitInfo : " + hitInfo.point);
        Debug.Log("wParticle：" + wParticle);
        if (Physics.Raycast(ray, out hitInfo, 15, mask.value))//ray 是射线的起点和方向，碰撞信息，射线长度，碰撞层
        {
            Debug.DrawLine(ray.origin, hitInfo.point);//在两点间绘制一条线
            GameObject gameObj = hitInfo.collider.gameObject;//游戏对象是个容器，这里返回的是碰撞的游戏对象
            Debug.Log("click object name is " + gameObj.name);//返回碰撞游戏对象的信息
            if (gameObj.tag == "Fire")
            {
                if (gameObj.gameObject.GetComponent<ParticleSystem>().startSize > 0)//逐渐减小被碰撞的光粒子插件的大小，直到为0
                {
                    gameObj.gameObject.GetComponent<ParticleSystem>().startSize -= 0.1f;
                }
                else
                {
					//销毁火焰模型
                    GameObject.Destroy(gameObj);
                }

            }
        }
    }
}
