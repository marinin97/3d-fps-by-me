using UnityEngine;

public class HealthContainer : LootContainer
{
    [SerializeField]
    private float _healthAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player) && player.Health < player.MaxHealth)
        {
            float healthToAdd = _healthAmount;
            healthToAdd = healthToAdd + player.Health < player.MaxHealth ? healthToAdd : player.MaxHealth - player.Health;
            player.TakeDamage(-healthToAdd); //Небольшой костылик
            Destroy(gameObject);
        }
    }
}
