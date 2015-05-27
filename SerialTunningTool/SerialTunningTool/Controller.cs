using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialTunningTool
{
    class Controller
    {
        enum CMD
        {
            WATCHDOG,
            PRINT_MODE,
            HIGH,
            LOW,
            RESET_ALL,
            START,
            STOP,
            POWER,
            INTIAL_POWER,
            ROLL_OFFSET,
            PITCH_OFFSET,
            YAW_OFFSET,
            ROLL_KP,
            ROLL_KI,
            ROLL_KD,
            PITCH_KP,
            PITCH_KI,
            PITCH_KD,
            YAW_KP,
            YAW_KI,
            YAW_KD,
            OFFSET0,
            OFFSET1,
            OFFSET2,
            OFFSET3,
            MOTOR_KP,
            MOTOR_KD,
            Q,
            R1,
            R2,
            DRIFT_KP,
            DRIFT_KI,
            SWITCH_LIGHT,
            HIGHT_KP,
            HIGHT_KI,
            HIGHT_KD,
            X_KP,
            X_KI,
            X_KD,
            Y_KP,
            Y_KI,
            Y_KD,
            MAX_LIFT_VALUE,
            MIN_LIFT_VALUE
        };


        public void MotorContorl(float rpm) {
            Communication.SendCmd((int)CMD.POWER, rpm);
        }

        public void MotorStop()
        {
            Communication.SendCmd((int)CMD.STOP, 0);
        }
        
        public void MotorStart()
        {
            Communication.SendCmd((int)CMD.START, 0);
        }

        public void ClearWatchDog()
        {
            Communication.SendCmd((int)CMD.WATCHDOG, 0);
        }


        public void MotorSetInitPower(float value)
        {
            Communication.SendCmd((int)CMD.INTIAL_POWER, value);
        }

        public void RollKp(float value)
        {
            Communication.SendCmd((int)CMD.ROLL_KP, value);
        }

        public void PitchKp(float value)
        {
            Communication.SendCmd((int)CMD.PITCH_KP, value);
        }
        
        public void RollKd(float value)
        {
            Communication.SendCmd((int)CMD.ROLL_KD, value);
        }

        public void YawKp(float value)
        {
            Communication.SendCmd((int)CMD.YAW_KP, value);
        }

        public void YawKi(float value)
        {
            Communication.SendCmd((int)CMD.YAW_KI, value);
        }

        public void YawKd(float value)
        {
            Communication.SendCmd((int)CMD.YAW_KD, value);
        }

        public void PitchKd(float value)
        {
            Communication.SendCmd((int)CMD.PITCH_KD, value);
        }

        public void Reset()
        {
            Communication.SendCmd((int)CMD.RESET_ALL, 0);
        }

        public void TogglePrint()
        {
            Communication.SendCmd((int)CMD.PRINT_MODE, 0);
        }

        public void MotorKp(float value)
        {
            Communication.SendCmd((int)CMD.MOTOR_KP, value);
        }

        public void MotorKd(float value)
        {
            Communication.SendCmd((int)CMD.MOTOR_KD, value);
        }
        
        public void RollKi(float value){
            Communication.SendCmd((int)CMD.ROLL_KI, value);
        
        }

        public void PitchKi(float value){
            Communication.SendCmd((int)CMD.PITCH_KI, value);
        
        }

        public void KalmanQ(float value)
        {
            Communication.SendCmd((int)CMD.Q, value);

        }

        public void KalmanR1(float value)
        {
            Communication.SendCmd((int)CMD.R1, value);

        }

        public void KalmanR2(float value)
        {
            Communication.SendCmd((int)CMD.R2, value);
        }

        public void RollOffset(float value)
        {
            Communication.SendCmd((int)CMD.ROLL_OFFSET, value);
        }

        public void PitchOffset(float value)
        {
            Communication.SendCmd((int)CMD.PITCH_OFFSET, value);
        }

        public void YawOffset(float value)
        {
            Communication.SendCmd((int)CMD.YAW_OFFSET, value);
        }

        public void DriftKp(float value)
        {
            Communication.SendCmd((int)CMD.DRIFT_KP, value);
        }

        public void High()
        {
            Communication.SendCmd((int)CMD.HIGH, 0);
        }
        public void Low()
        {
            Communication.SendCmd((int)CMD.LOW, 0);
        }
        public void SwitchLight()
        {
            Communication.SendCmd((int)CMD.SWITCH_LIGHT, 0);
        }

        public void HightKp(float value)
        {
            Communication.SendCmd((int)CMD.HIGHT_KP, value);

        }
        public void HightKi(float value)
        {
            Communication.SendCmd((int)CMD.HIGHT_KI, value);

        }
        public void HightKd(float value)
        {
            Communication.SendCmd((int)CMD.HIGHT_KD, value);

        }

        public void XKp(float value)
        {
            Communication.SendCmd((int)CMD.X_KP, value);

        }
        public void XKi(float value)
        {
            Communication.SendCmd((int)CMD.X_KI, value);

        }
        public void XKd(float value)
        {
            Communication.SendCmd((int)CMD.X_KD, value);

        }

        public void YKp(float value)
        {
            Communication.SendCmd((int)CMD.Y_KP, value);

        }
        public void YKi(float value)
        {
            Communication.SendCmd((int)CMD.Y_KI, value);

        }
        public void YKd(float value)
        {
            Communication.SendCmd((int)CMD.Y_KD, value);

        }
        public void MaxLift(float value)
        {
            Communication.SendCmd((int)CMD.MAX_LIFT_VALUE, value);

        }
        public void MinLift(float value)
        {
            Communication.SendCmd((int)CMD.MIN_LIFT_VALUE, value);

        }

    }
}
