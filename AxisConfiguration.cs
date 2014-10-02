using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPAdvancedFlyByWire
{

    public class AxisConfiguration
    {
        public float m_PositiveDeadZone = 0.0f;
        public float m_NegativeDeadZone = 0.0f;
        public float m_Left = 0.0f;
        public float m_Identity = 0.0f;
        public float m_Right = 0.0f;
        public bool m_Invert = false;

        public float Rescale(float value, Curve analogEvaluationCurve)
        {
            if (value < m_Left)
            {
                m_Left = value;
            }
            else if (value > m_Right)
            {
                m_Right = value;
            }

            float eps = 1e-4f;

            if (value + eps > m_Identity && value - eps < m_Identity)
            {
                value = 0.0f;
            }
            else if (value < m_Identity)
            {
                value = (value - m_Identity) / (m_Identity - m_Left);
            }
            else if (value > m_Identity)
            {
                value = (value - m_Identity) / (m_Right - m_Identity);
            }

            if (Math.Abs(value) < 1e-4)
            {
                value = 0.0f;
            }

            if (value > 0.0f)
            {
                if (value < m_PositiveDeadZone)
                {
                    m_PositiveDeadZone = value;
                }

                float deadZone = m_PositiveDeadZone;
                float t = (1.0f - deadZone);
                if (t != 0.0f)
                {
                    value = (value - deadZone) / t;
                }
            }
            else if (value < 0.0f)
            {
                if (Math.Abs(value) < m_NegativeDeadZone)
                {
                    m_NegativeDeadZone = Math.Abs(value);
                }

                float deadZone = m_NegativeDeadZone;
                float t = (1.0f - deadZone);
                if (t != 0.0f)
                {
                    value = -1.0f * (Math.Abs(value) - deadZone) / t;
                }
            }

            return (m_Invert ? -1.0f : 1.0f) * Math.Sign(value) * analogEvaluationCurve.Evaluate(Math.Abs(value));
        }

    }

}
