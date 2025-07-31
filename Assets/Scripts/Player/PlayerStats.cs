using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHP = 100;
    public int maxTP = 100;
    
    [SerializeField] private int currentHP;
    [SerializeField] private int currentTP;
    
    // Properties for external access
    public int CurrentHP 
    { 
        get => currentHP; 
        private set => currentHP = Mathf.Clamp(value, 0, maxHP); 
    }
    
    public int CurrentTP 
    { 
        get => currentTP; 
        private set => currentTP = Mathf.Clamp(value, 0, maxTP); 
    }
    
    void Start()
    {
        // Initialize stats
        CurrentHP = maxHP;
        CurrentTP = maxTP;
        
        // Subscribe to events
        GameEvents.OnHPChange += HandleHPChange;
        GameEvents.OnTPChange += HandleTPChange;
        
        Debug.Log($"Player Stats Initialized - HP: {CurrentHP}/{maxHP}, TP: {CurrentTP}/{maxTP}");
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        GameEvents.OnHPChange -= HandleHPChange;
        GameEvents.OnTPChange -= HandleTPChange;
    }
    
    private void HandleHPChange(int amount)
    {
        int oldHP = CurrentHP;
        CurrentHP += amount;
        
        Debug.Log($"HP Changed: {oldHP} -> {CurrentHP} (Change: {amount})");
        
        // Check for death
        if (CurrentHP <= 0)
        {
            Debug.Log("Player has died!");
            OnPlayerDeath();
        }
    }
    
    private void HandleTPChange(int amount)
    {
        int oldTP = CurrentTP;
        CurrentTP += amount;
        
        Debug.Log($"TP Changed: {oldTP} -> {CurrentTP} (Change: {amount})");
        
        // Check for hypothermia
        if (CurrentTP <= 0)
        {
            Debug.Log("Player is suffering from hypothermia!");
            OnHypothermia();
        }
    }
    
    private void OnPlayerDeath()
    {
        // Handle player death logic here
        // Could trigger game over screen, respawn, etc.
    }
    
    private void OnHypothermia()
    {
        // Handle hypothermia effects here
        // Could cause gradual HP loss, movement penalties, etc.
    }
    
    // Public methods for manual stat changes if needed
    public void SetHP(int newHP)
    {
        CurrentHP = newHP;
    }
    
    public void SetTP(int newTP)
    {
        CurrentTP = newTP;
    }
    
    // Get current stats as percentages
    public float GetHPPercentage()
    {
        return (float)CurrentHP / maxHP;
    }
    
    public float GetTPPercentage()
    {
        return (float)CurrentTP / maxTP;
    }
}