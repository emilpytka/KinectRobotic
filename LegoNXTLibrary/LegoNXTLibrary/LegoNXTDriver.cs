using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Robotics.Lego;

namespace LegoNXTLibrary
{
    public class LegoNXTDriver
    {
        private int _armsAngle = 0;
        private string _connectionPort;
        private NXTBrick nxt = new NXTBrick();

        public LegoNXTDriver(int armsAngle, string connectionPort)
        {
            _armsAngle = armsAngle;
            _connectionPort = connectionPort;
            ConnectToNXT();
        }

        private void ConnectToNXT()
        {
            nxt.Connect(_connectionPort);
        }

        //Moves the smallest step forwards
        public void MoveOneStepForward()
        {
            LeftForward();
            RightForward();
        }

        private void LeftForward()
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState();
            motorState.Power = 70;
            motorState.TurnRatio = 50;
            motorState.Mode = NXTBrick.MotorMode.On;
            motorState.Regulation = NXTBrick.MotorRegulationMode.Speed;
            motorState.RunState = NXTBrick.MotorRunState.Running;
            motorState.TachoLimit = 360;
            nxt.SetMotorState(NXTBrick.Motor.C, motorState);
        }

        private void RightForward()
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState();
            motorState.Power = 70;
            motorState.TurnRatio = 50;
            motorState.Mode = NXTBrick.MotorMode.On;
            motorState.Regulation = NXTBrick.MotorRegulationMode.Speed;
            motorState.RunState = NXTBrick.MotorRunState.Running;
            motorState.TachoLimit = 360;
            nxt.SetMotorState(NXTBrick.Motor.B, motorState);
        }

        //Moves the smallest step forwards
        public void MoveOneStepBackwards()
        {
            RightBackwards();
            LeftBackwards();
        }

        private void RightBackwards()
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState();

            motorState.Power = -70;
            motorState.TurnRatio = -50;
            motorState.Mode = NXTBrick.MotorMode.On;
            motorState.Regulation = NXTBrick.MotorRegulationMode.Speed;
            motorState.RunState = NXTBrick.MotorRunState.Running;
            motorState.TachoLimit = 360;

            nxt.SetMotorState(NXTBrick.Motor.B, motorState);
        }

        private void LeftBackwards()
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState();

            motorState.Power = -70;
            motorState.TurnRatio = -50;
            motorState.Mode = NXTBrick.MotorMode.On;
            motorState.Regulation = NXTBrick.MotorRegulationMode.Speed;
            motorState.RunState = NXTBrick.MotorRunState.Running;
            motorState.TachoLimit = 360;

            nxt.SetMotorState(NXTBrick.Motor.C, motorState);
        }

        private bool isFirstTurn = false;

        //Turns one smallest step right
        public void TurnRight()
        {
//            OneStepRight(180);
//            if (isFirstTurn)
//            {
//                OneStepLeft(90);
//                isFirstTurn = false;
//            }
            //tutaj skreca, po tej petli nalezy ustawic isFirstTurn = true;
            for (int i = 0; i < 5; i++)
                OneStepRight(360);
            isFirstTurn = true;
        }

        //Turns one smallest step left
        public void TurnLeft()
        {
//            OneStepLeft(180);
//            if (isFirstTurn)
//            {
//                OneStepRight(90);
//                isFirstTurn = false;
//            }
            //w tej petli skreca po niej nalezy ustawic isFirstTurn na true;
            for (int i = 0; i < 5; i++)
                OneStepLeft(360);
            isFirstTurn = true;
        }

        private void OneStepLeft(int tacho)
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState();

            motorState.Power = 75;
            motorState.TurnRatio = -100;
            motorState.Mode = NXTBrick.MotorMode.On;
            motorState.Regulation = NXTBrick.MotorRegulationMode.Speed;
            motorState.RunState = NXTBrick.MotorRunState.Running;
            motorState.TachoLimit = tacho;

            nxt.SetMotorState(NXTBrick.Motor.C, motorState);
        }

        private void OneStepRight(int tacho)
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState();

            motorState.Power = 75;
            motorState.TurnRatio = 100;
            motorState.Mode = NXTBrick.MotorMode.On;
            motorState.Regulation = NXTBrick.MotorRegulationMode.Speed;
            motorState.RunState = NXTBrick.MotorRunState.Running;
            motorState.TachoLimit = tacho;

            nxt.SetMotorState(NXTBrick.Motor.B, motorState);
        }

        public void MoveArms(Direction direction, int angle)
        {
            if (_armsAngle < 90 && direction == Direction.Right)
            {
                MoveArm(80, 80, 10);
                _armsAngle+=45;
            }
            else if (_armsAngle > -90 && direction == Direction.Left)
            {
                MoveArm(80, 80, -10);
                _armsAngle-=45;
            }
        }

        private void MoveArm(int angle, int power, int turnRatio)
        {
            NXTBrick.MotorState motorState = new NXTBrick.MotorState();

            motorState.Power = power;
            motorState.TurnRatio = turnRatio;
            motorState.Mode = NXTBrick.MotorMode.On;
            motorState.Regulation = NXTBrick.MotorRegulationMode.Speed;
            motorState.RunState = NXTBrick.MotorRunState.Running;
            motorState.TachoLimit = angle;

            nxt.SetMotorState(NXTBrick.Motor.A, motorState);
        }

        public void DisconnectNXT()
        {
            nxt.Disconnect();
        }
    }
}
