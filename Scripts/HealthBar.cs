using Godot;

public partial class HealthBar : Node2D
{
	[Export] public float MaxHealth = 100f;
	[Export] public float CurrentHealth = 100f;
	[Export] public Vector2 BarSize = new Vector2(50, 5);
	[Export] public Color HealthColor = new Color(0, 1, 0);
	[Export] public Color BackgroundColor = new Color(0.2f, 0.2f, 0.2f);

	public override void _Draw()
	{
		DrawRect(new Rect2(-BarSize.X / 2, -20, BarSize.X, BarSize.Y), BackgroundColor);

		float healthPercent = CurrentHealth / MaxHealth;
		float healthWidth = BarSize.X * healthPercent;

		Color currentColor = HealthColor;
		if (healthPercent < 0.3f)
			currentColor = new Color(1, 0, 0);
		else if (healthPercent < 0.6f)
			currentColor = new Color(1, 1, 0);

		DrawRect(new Rect2(-BarSize.X / 2, -20, healthWidth, BarSize.Y), currentColor);
	}

	public void UpdateHealth(float current, float max)
	{
		CurrentHealth = current;
		MaxHealth = max;
		QueueRedraw();
	}
}
