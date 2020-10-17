using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();

    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;

    public float tiltSmooth = 5;

    public Vector3 startPos;

    private Rigidbody2D rigidbody;

    private Quaternion downRotation;

    private Quaternion forwardRotation;

    public AudioSource chirpAudio;
    public AudioSource loseAudio;
    public AudioSource scoreAudio;

    GameManager manager;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        manager = GameManager.Instance;
        rigidbody.simulated = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (manager.GameOver)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0)) // Left button
        {
            chirpAudio.Play();
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
            Console.WriteLine($"X: {Mathf.Clamp(transform.position.x, -2f, 2f)}");
            Console.WriteLine($"Y: {Mathf.Clamp(transform.position.y, -5f, 5f)}");
            Console.WriteLine($"Z: {transform.position.z}");
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -2f, 2f), Mathf.Clamp(transform.position.y, -5f, 5f), transform.position.z);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    private void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameStarted()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true; 
    }

    private void OnGameOverConfirmed()
    {
        // because we are inside of a parent object
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    private  void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "ScoreZone"){
            // register a score event
            OnPlayerScored();  // Event sent to GameManager
            scoreAudio.Play();
        }
        if (other.gameObject.tag == "DeadZone"){
            rigidbody.simulated = false;
            // register a dead event
            OnPlayerDied();  // Event sent to GameManger
            loseAudio.Play();
        }
    }
}
