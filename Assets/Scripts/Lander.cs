using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour {


    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler onBeforeForce;


    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount = 10f;

    private void Awake() {
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        onBeforeForce?.Invoke(this, EventArgs.Empty);

        Debug.Log(fuelAmount);
        if (fuelAmount <= 0f) {
            //no fuel
            return;
        }

        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed) {
            ConsumeFuel();
        }

        if (Keyboard.current.upArrowKey.isPressed) {
            float force = 700f;
            landerRigidbody2D.AddForce(force * Time.deltaTime * transform.up);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.leftArrowKey.isPressed) {
            float turnSpeed = +100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.rightArrowKey.isPressed) {
            float turnSpeed = -100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D) {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)) {
            Debug.Log("Crashed on the Terrain!");
            return;
        }
        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude) {
            Debug.Log("Landed too hard!");
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .9f;
        if (dotVector < minDotVector) {
            Debug.Log("Landed on a too steep angle!");
            return;
        }

        Debug.Log("Successful Landing");

        float scoreDotVectorMultiplier = 10f;
        float maxScoreAmountLandingAngle = 100;
        float landingAngleScore = Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        Debug.Log("landingAngleScore: " + landingAngleScore);
        Debug.Log("landingSpeedScore: " + landingSpeedScore);

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

        Debug.Log("score: " + score);
    }

    private void OnTriggerEnter2D(Collider2D collider2D) {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup)) {
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;
            fuelPickup.DestroySelf();
        }
    }

    private void ConsumeFuel() {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    private void Update() {

    }
}
