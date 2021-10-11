using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace ProjectNadir 
{
    public class infoUI : MonoBehaviour
    {
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerModel playerModel;
        private TMP_Text text;
        private string _textToPrint;

        private void Start()
        {
            text = this.GetComponent<TMP_Text>();
        }
        // Update is called once per frame
        void Update()
        {
            ProccessInfo();
            text.text = _textToPrint;
        }

        private void ProccessInfo()
        {
            _textToPrint =
                $"CurrentState: {playerMovement.CurrentState}\n" +
                $"IsGrounded: {playerMovement.IsGrounded}\n" +
                $"DoubleJumpPossible: {playerMovement.DoubleJumpPossible}\n" +
                $"MoveDirection: {playerMovement.moveDirection}\n" +
                $"NeutralDirection: {playerMovement.neutralDirection}\n"+
                $"isWalking: {playerMovement.IsWalking}\n" +
                $"Velocity: {playerMovement.Velocity}\n";
        }
    }
}
