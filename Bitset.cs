using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    [Serializable]
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
    
    }

}
