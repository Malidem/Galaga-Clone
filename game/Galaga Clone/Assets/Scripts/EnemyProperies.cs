using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperies : MonoBehaviour
{
    public int speed;
    public GameObject bullet;
    public GameObject hasTurret;
    public GameObject turretCount;
    public GameObject turretBullet;

    void Start()
    {
        GetComponent<BaseEnemy>();
    }
}
