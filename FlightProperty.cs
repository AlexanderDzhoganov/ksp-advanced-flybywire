using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class FlightProperty
    {

        private float m_Value = 0.0f;
        private float m_Velocity = 0.0f;
        private float m_Acceleration = 0.0f;
        private float m_MinValue = 0.0f;
        private float m_MaxValue = 0.0f;

        public FlightProperty(float minValue, float maxValue)
        {
            m_MinValue = minValue;
            m_MaxValue = maxValue;
        }

        public void SetAcceleration(float accel)
        {
            m_Acceleration = accel;
        }

        public void SetVelocity(float velocity)
        {
            m_Velocity = velocity;
        }

        public void SetValue(float value)
        {
            m_Value = value;
            m_Velocity = 0.0f;
            m_Acceleration = 0.0f;
        }

        public float GetValue()
        {
            return m_Value;
        }

        public void SetZero()
        {
            m_Value = 0.0f;
            m_Velocity = 0.0f;
            m_Acceleration = 0.0f;
        }

        public void SetMax()
        {
            m_Value = m_MinValue;
            m_Velocity = 0.0f;
            m_Acceleration = 0.0f;
        }

        public float Update()
        {
            m_Velocity += m_Acceleration * Time.deltaTime;
            m_Value += m_Velocity * Time.deltaTime;
            m_Value = Utility.Clamp(m_Value, m_MinValue, m_MaxValue);
            return m_Value;
        }

    }

}
