using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace GameDevHQ.FileBase.Plugins.FPS_Character_Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class FPS_Controller : MonoBehaviour
    {
        [Header("Controller Info")]
        [SerializeField][Tooltip("How fast can the controller walk?")]
        private float _walkSpeed = 3.0f; //how fast the character is walking
        [SerializeField][Tooltip("How fast can the controller run?")]
        private float _runSpeed = 7.0f; // how fast the character is running
        [SerializeField][Tooltip("Set your gravity multiplier")]
        private float _gravity = 1.0f; //how much gravity to apply 
        [SerializeField][Tooltip("How high can the controller jump?")]
        private float _jumpHeight = 15.0f; //how high can the character jump
        [SerializeField]
        private bool _isRunning = false; //bool to display if we are running
        [SerializeField]
        private bool _crouching = false; //bool to display if we are crouched or not

        private CharacterController _controller; //reference variable to the character controller component
        private float _yVelocity = 0.0f; //cache our y velocity


        [Header("Headbob Settings")]
        [SerializeField][Tooltip("Smooth out the transition from moving to not moving")]
        private float _smooth = 20.0f; //smooth out the transition from moving to not moving
        [SerializeField][Tooltip("How quickly the player head bobs")]
        private float _walkFrequency = 4.8f; //how quickly the player head bobs when walking
        [SerializeField][Tooltip("How quickly the player head bobs")]
        private float _runFrequency = 7.8f; //how quickly the player head bobs when running
        [SerializeField][Tooltip("How dramatic the headbob is")][Range(0.0f, 0.2f)]
        private float _heightOffset = 0.05f; //how dramatic the bobbing is
        private float _timer = Mathf.PI / 2; //This is where Sin = 1 -- used to simulate walking forward. 
        private Vector3 _initialCameraPos; //local position where we reset the camera when it's not bobbing

        [Header("Camera Settings")]
        [SerializeField][Tooltip("Control the look sensitivty of the camera")]
        private float _lookSensitivity = 5.0f; //mouse sensitivity 

        private Camera _fpsCamera;

        [Header("Gun Settings")]
        private int _targetsAndBarriers;
        [SerializeField] private float _bulletDelay = 1f;
        private float _canFire;

        AudioSource _audioSource;
        [SerializeField] AudioClip _gunSound;
        [SerializeField] AudioClip _ricochetSound;
        [SerializeField] AudioClip _hitSound;
        bool _isAiming = false;
        [SerializeField] private float _aimFOV = 15f;
        [SerializeField] private float _zoomTime = 40f;
        [SerializeField] private float _regularFOV = 55f;

        private void Start()
        {
            _controller = GetComponent<CharacterController>(); //assign the reference variable to the component
            _fpsCamera = GetComponentInChildren<Camera>();
            _audioSource = GetComponent<AudioSource>();
            _initialCameraPos = _fpsCamera.transform.localPosition;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _targetsAndBarriers = LayerMask.GetMask(new string[] { "Target", "PossibleCover" });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Cursor should no longer be locked down");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            FPSController();
            CameraController();
            HeadBobbing(); 
        }

        void FPSController()
        {
            float h = Input.GetAxis("Horizontal"); //horizontal inputs (a, d, leftarrow, rightarrow)
            float v = Input.GetAxis("Vertical"); //veritical inputs (w, s, uparrow, downarrow)

            Vector3 direction = new Vector3(h, 0, v); //direction to move
            Vector3 velocity = direction * _walkSpeed; //velocity is the direction and speed we travel

            if (Input.GetKeyDown(KeyCode.C) && _isRunning == false)
            {

                if (_crouching == true)
                {
                    _crouching = false;
                    _controller.height = 2.0f;
                }
                else
                {
                    _crouching = true;
                    _controller.height = 1.0f;
                }
                
            }

            if (Input.GetKey(KeyCode.LeftShift) && _crouching == false) //check if we are holding down left shift
            {
                velocity = direction * _runSpeed; //use the run velocity 
                _isRunning = true;
            }
            else
            {
                _isRunning = false;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                _fpsCamera.DOFieldOfView(_aimFOV, _zoomTime).SetEase(Ease.OutQuad);
            }

            if (Input.GetMouseButtonUp(1) && Input.GetKeyUp(KeyCode.LeftAlt))
            {
                _fpsCamera.DOFieldOfView(_regularFOV, _zoomTime).SetEase(Ease.OutQuad);
            }

            if (Input.GetMouseButtonDown(0) && Time.time > _canFire)
            {
                _canFire = Time.time + _bulletDelay;
                _audioSource.PlayOneShot(_gunSound);
                HandleShot();
            }

            if (_controller.isGrounded == true) //check if we're grounded
            {
                if (Input.GetKeyDown(KeyCode.Space)) //check for the space key
                {
                    _yVelocity = _jumpHeight; //assign the cache velocity to our jump height
                }
            }
            else //we're not grounded
            {
                _yVelocity -= _gravity; //subtract gravity from our yVelocity 
            }

            velocity.y = _yVelocity; //assign the cached value of our yvelocity

            velocity = transform.TransformDirection(velocity);

            _controller.Move(velocity * Time.deltaTime); //move the controller x meters per second
        }

        private void HandleShot()
        {
            Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _targetsAndBarriers))
            {
                if (hit.collider.gameObject.TryGetComponent<IShootable>(out IShootable shot))
                {
                    AudioSource.PlayClipAtPoint(_hitSound, hit.point);
                    shot.HandleShot();
                } 
                else
                {
                    PlayClipAt(_ricochetSound, hit.collider.transform.position);
                }
            }
        }

        static AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float delay = 0f)
        {
            GameObject tempGO = new GameObject("TempAudio"); // create the temp object
            tempGO.transform.position = pos; // set its position
            AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
            aSource.clip = clip; // define the clip
            aSource.rolloffMode = AudioRolloffMode.Linear;
            aSource.spatialBlend = 1f;
            aSource.PlayDelayed(delay + delay); // start the sound
            Destroy(tempGO, clip.length); // destroy object after clip duration
            return aSource; // return the AudioSource reference
        }

        void CameraController()
        {
            float mouseX = Input.GetAxis("Mouse X"); //get mouse movement on the x
            float mouseY = Input.GetAxis("Mouse Y"); //get mouse movement on the y

            Vector3 rot = transform.localEulerAngles; //store current rotation
            rot.y += mouseX * _lookSensitivity; //add our mouseX movement to the y axis
            transform.localRotation = Quaternion.AngleAxis(rot.y, Vector3.up); ////rotate along the y axis by movement amount

            Vector3 camRot = _fpsCamera.transform.localEulerAngles; //store the current rotation
            camRot.x += -mouseY * _lookSensitivity; //add the mouseY movement to the x axis
            _fpsCamera.transform.localRotation = Quaternion.AngleAxis(camRot.x, Vector3.right); //rotate along the x axis by movement amount
        }

        void HeadBobbing()
        {
            float h = Input.GetAxis("Horizontal"); //horizontal inputs (a, d, leftarrow, rightarrow)
            float v = Input.GetAxis("Vertical"); //veritical inputs (w, s, uparrow, downarrow)

            if (h != 0 || v != 0) //Are we moving?
            {
               
                if (Input.GetKey(KeyCode.LeftShift)) //check if running
                {
                    _timer += _runFrequency * Time.deltaTime; //increment timer for our sin/cos waves when running
                }
                else
                {
                    _timer += _walkFrequency * Time.deltaTime; //increment timer for our sin/cos waves when walking
                }

                Vector3 headPosition = new Vector3 //calculate the head position in our walk cycle
                    (
                        _initialCameraPos.x + Mathf.Cos(_timer) * _heightOffset, //x value
                        _initialCameraPos.y + Mathf.Sin(_timer) * _heightOffset, //y value
                        0 // z value
                    );

                _fpsCamera.transform.localPosition = headPosition; //assign the head position

                if (_timer > Mathf.PI * 2) //reset the timer when we complete a full walk cycle on the unit circle
                {
                    _timer = 0; //completed walk cycle. Reset. 
                }
            }
            else
            {
                _timer = Mathf.PI / 2; //reset timer back to 1 for initial walk cycle 

                Vector3 resetHead = new Vector3 //calculate reset head position back to initial cam pos
                    (
                    Mathf.Lerp(_fpsCamera.transform.localPosition.x, _initialCameraPos.x, _smooth * Time.deltaTime), //x vlaue
                    Mathf.Lerp(_fpsCamera.transform.localPosition.y, _initialCameraPos.y, _smooth * Time.deltaTime), //y value
                    0 //z value
                    );

                _fpsCamera.transform.localPosition = resetHead; //assign the head position back to the initial cam pos
            }
        }
    }
}

