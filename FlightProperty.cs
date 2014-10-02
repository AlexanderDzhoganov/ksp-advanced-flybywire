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
        private int m_Increment = 0;
        private float m_IncrementStep = 0.0f;

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

        public void SetIncrement(int increment, float incrementStep = 0.0f)
        {
            m_Increment = increment;
            m_IncrementStep = incrementStep;
        }

        public bool HasIncrement()
        {
            return m_Increment != 0;
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

        public void SetMinMaxValues(float min, float max)
        {
            m_MinValue = min;
            m_MaxValue = max;
        }

        public void Increment(float value)
        {
            m_Value += value;
            m_Velocity = 0.0f;
            m_Acceleration = 0.0f;
        }

        public float Update()
        {
            if (m_Increment != 0)
            {
                SetAcceleration(m_Increment * m_IncrementStep);
            }

            m_Velocity += m_Acceleration * Time.deltaTime;
            m_Value += m_Velocity * Time.deltaTime;
            m_Value = Utility.Clamp(m_Value, m_MinValue, m_MaxValue);
            return m_Value;
        }

    }

}
