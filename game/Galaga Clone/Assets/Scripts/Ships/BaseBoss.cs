using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseBoss : BaseEnemy
{
    public string displayName;
    public Color healthBarColor = new Color(0, 0, 0, 1);
    protected Slider bossHealthBar;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        bossHealthBar = gameManager.bossHealthBar;
        bossHealthBar.transform.GetChild(2).GetComponent<Text>().text = displayName;
        bossHealthBar.transform.GetChild(1).GetComponent<Image>().color = healthBarColor;
        bossHealthBar.gameObject.SetActive(true);
        bossHealthBar.maxValue = health;
        bossHealthBar.value = currentHealth;
    }

    protected override void OnHealthRemoved()
    {
        bossHealthBar.value = currentHealth;
    }

    protected override void OnHealthAdded()
    {
        bossHealthBar.value = currentHealth;
    }

    protected override void OnDeath()
    {
        bossHealthBar.gameObject.SetActive(false);
    }
}
