using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPAdvancedFlyByWire
{

    public class Bitset
    {

        public int[] m_Data = null;
        public int m_NumBits = 0;

        public Bitset() {}

        public Bitset(int numBits)
        {
            m_NumBits = numBits;

            if (numBits < 32)
            {
                numBits = 32;
            }

            int numInts = numBits / 32 + ((numBits % 32 == 0) ? 0 : 1);
            m_Data = new int[numInts];
            for (var i = 0; i < numInts; i++)
            {
                m_Data[i] = 0;
            }
        }

        public override bool Equals(System.Object obj)
        {
            if(obj == null)
            {
                return false;
            }

            Bitset bitset = (Bitset)obj;

            if(m_NumBits != bitset.m_NumBits)
            {
                return false;
            }

            for (int i = 0; i < m_Data.Length; i++ )
            {
                if(m_Data[i] != bitset.m_Data[i])
                {
                    return false;
                }
            }

            return true;
        }

        public Bitset Copy()
        {
            Bitset result = new Bitset(m_NumBits);

            for(int i = 0; i < m_Data.Length; i++)
            {
                result.m_Data[i] = m_Data[i];
            }

            return result;
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < m_NumBits; i++)
            {
                if(Get(i))
                {
                    result = "1" + result;
                }
                else
                {
                    result = "0" + result;
                }
            }

            return result;
        }

        public Bitset(int numBits, int initialValue)
        {
            m_NumBits = numBits;

            if (numBits < 32)
            {
                numBits = 32;
            }

            int numInts = numBits / 32 + ((numBits % 32 == 0) ? 0 : 1);

            m_Data = new int[numInts];
            for (var i = 0; i < numInts; i++)
            {
                m_Data[i] = 0;
            }

            m_Data[0] = initialValue;
        }

        public void Set(int bit)
        {
            if (bit >= m_NumBits)
            {
                return;
            }

            m_Data[bit / 32] |= 1 << (bit % 32);
        }

        public bool Get(int bit)
        {
            if(bit >= m_NumBits)
            {
                return false;
            }

            return (m_Data[bit / 32] & (1 << (bit % 32))) != 0;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < m_Data.Length; i++ )
            {
                hash ^= m_Data[i].GetHashCode();
            }

            return hash;
        }
    
    }

}
