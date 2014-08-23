using UnityEngine;

public class WorldRotator : MonoBehaviour {

	public float hintingRotationAngle;
	public float degreesPerSecond;

	[System.NonSerialized]
	private Quaternion targetRotation;

	[System.NonSerialized]
	private System.Action navigationFinished;

	void Update () {
		transform.localRotation = Quaternion.RotateTowards(
			transform.localRotation, targetRotation, degreesPerSecond * Time.deltaTime);
		if (navigationFinished != null && transform.localRotation == targetRotation) {
			navigationFinished();
			navigationFinished = null;
		}
	}

	public void Hint(Direction direction) {
		SetTargetRotation(direction, hintingRotationAngle);
	}

	public void Navigate(Direction direction, System.Action onNavigationFinished = null) {
		navigationFinished = onNavigationFinished;
		SetTargetRotation(direction, 90 - hintingRotationAngle);
	}

	public void Center() {
		targetRotation = Quaternion.identity;
	}

	public void JumpToTarget() {
		transform.localRotation = targetRotation;
	}

	public void SetTargetRotation(Direction direction, float angle) {
		switch (direction) {
		case Direction.Left:
			targetRotation = Quaternion.AngleAxis(-angle, Vector3.up);
			break;
		case Direction.Right:
			targetRotation = Quaternion.AngleAxis(angle, Vector3.up);
			break;
		case Direction.Top:
			targetRotation = Quaternion.AngleAxis(angle, Vector3.left);
			break;
		case Direction.Bottom:
			targetRotation = Quaternion.AngleAxis(-angle, Vector3.left);
			break;
		}
	}
}
