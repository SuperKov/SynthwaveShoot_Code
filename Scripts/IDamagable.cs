public interface IDamagable
{
    public static event System.Action<int, UnityEngine.Transform> DamageTaked;

    public void ChangeHealth(int value);

    public static void ShowDamageTaked(int value, UnityEngine.Transform pos){ DamageTaked.Invoke(value, pos); }
}
